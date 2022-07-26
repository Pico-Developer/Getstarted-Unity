//  Copyright © 2015-2022 Pico Technology Co., Ltd. All Rights Reserved.

using System.Collections;
using PXR_Audio.Spatializer;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PXR_Audio_Spatializer_AudioSource : MonoBehaviour
{
    [SerializeField] [Range(0.0f, 24.0f)] private float sourceGainDB = 0.0f;
    private bool sourceGainChanged = false;

    [SerializeField] [Range(0.0f, 100000.0f)] private float sourceSize = 0.0f;
    private bool sourceSizeChanged = false;
    
    [SerializeField] private bool enableDoppler = true;
    private bool enableDopplerChanged = false;

    [SerializeField] public SourceAttenuationMode sourceAttenuationMode = SourceAttenuationMode.InverseSquare;
    [SerializeField] public float minAttenuationDistance = 1.0f;
    [SerializeField] public float maxAttenuationDistance = 100.0f;
    private bool attenuationDistanceChanged = false;

    private bool isActive;
    private bool isAudioDSPInProgress = false;

    public bool IsAudioDSPInProgress
    {
        get { return isAudioDSPInProgress; }
    }

    private PXR_Audio_Spatializer_Context context;

    private PXR_Audio_Spatializer_Context Context
    {
        get
        {
            if (context == null)
                context = FindObjectOfType<PXR_Audio_Spatializer_Context>();
            return context;
        }
    }

    private AudioSource nativeSource;

    private int sourceId = -1;

    private float[] positionArray = new float[3] { 0.0f, 0.0f, 0.0f };

    private float playheadPosition = 0.0f;
    private bool wasPlaying = false;

    IEnumerator Start()
    {
        yield return new WaitUntil(() =>
        {
            return Context != null && Context.Initialized;
        });

        RegisterSourceInternal();
    }
    
    private void OnEnable()
    {
        isActive = true;
    }
    
    /// <summary>
    /// Register this audio source in spatializer
    /// </summary>
    internal void RegisterSourceInternal()
    {
        nativeSource = GetComponent<AudioSource>();
        
        positionArray[0] = transform.position.x;
        positionArray[1] = transform.position.y;
        positionArray[2] = -transform.position.z;

        SourceConfig sourceConfig = new SourceConfig();

        sourceConfig.mode = PXR_Audio.Spatializer.SourceMode.Spatialize;
        sourceConfig.position.x = positionArray[0];
        sourceConfig.position.y = positionArray[1];
        sourceConfig.position.z = positionArray[2];
        sourceConfig.front.x = transform.forward.x;
        sourceConfig.front.y = transform.forward.y;
        sourceConfig.front.z = -transform.forward.z;
        sourceConfig.up.x = transform.up.x;
        sourceConfig.up.y = transform.up.y;
        sourceConfig.up.z = -transform.up.z;
        sourceConfig.radius = 0.1f;
        sourceConfig.enableDoppler = enableDoppler;
        
        PXR_Audio.Spatializer.Result ret = Context.AddSourceWithConfig(
            ref sourceConfig,
            ref sourceId,
            true);
        if (ret != PXR_Audio.Spatializer.Result.Success)
        {
            Debug.LogError("Failed to add source.");
            return;
        }

        SetGainDB(sourceGainDB);

        ret = Context.SetSourceSize(sourceId, sourceSize);
        if (ret != PXR_Audio.Spatializer.Result.Success)
        {
            Debug.LogError("Failed to recover source size.");
        }
        
        ret = Context.SetDopplerEffect(sourceId, enableDoppler);
        if (ret != PXR_Audio.Spatializer.Result.Success)
        {
            Debug.LogError("Failed to initialize source #" + sourceId + " doppler effect.");
        }

        ret = Context.SetSourceAttenuationMode(sourceId, sourceAttenuationMode, null, null);
        if (ret != PXR_Audio.Spatializer.Result.Success)
        {
            Debug.LogError("Failed to initialize source #" + sourceId + " attenuation mode.");
        }
        
        ret = Context.SetSourceRange(sourceId, minAttenuationDistance, maxAttenuationDistance);
        if (ret != PXR_Audio.Spatializer.Result.Success)
        {
            Debug.LogError("Failed to initialize source #" + sourceId + " attenuation range.");
        }

        isActive = true;

        Debug.Log("Source #" + sourceId + " is added.");
    }

    /// <summary>
    /// Resume playing status of this source
    /// </summary>
    public void Resume()
    {
        nativeSource.time = playheadPosition;
        if (wasPlaying)
        {
            nativeSource.Play();
        }
    }

    /// <summary>
    /// Setup source gain in dB
    /// </summary>
    /// <param name="gainDB">Gain in dB</param>
    public void SetGainDB(float gainDB)
    {
        sourceGainDB = gainDB;
        sourceGainChanged = true;
    }

    /// <summary>
    /// Setup source radius in meters
    /// </summary>
    /// <param name="radius">source radius in meter</param>
    public void SetSize(float radius)
    {
        sourceSize = radius;
        sourceSizeChanged = true;
    }

    /// <summary>
    /// Turn on/off in-engine doppler effect
    /// </summary>
    /// <param name="on">Turn doppler effect on/off </param>
    public void SetDopplerStatus(bool on)
    {
        enableDoppler = on;
        enableDopplerChanged = true;
    }
    
    /// <summary>
    /// Setup min attenuation range
    /// </summary>
    /// <param name="min"> Minimum attenuation range. Source loudness would stop increasing when source-listener
    /// distance is shorter than this </param>
    public void SetMinAttenuationRange(float min)
    {
        minAttenuationDistance = min;
        attenuationDistanceChanged = true;
    }
    
    /// <summary>
    /// Setup max attenuation range
    /// </summary>
    /// <param name="max"> Maximum attenuation range. Source loudness would stop decreasing when source-listener
    /// distance is further than this </param>
    public void SetMaxAttenuationRange(float max)
    {
        maxAttenuationDistance = max;
        attenuationDistanceChanged = true;
    }
    
    void Update()
    {
        if (isActive && sourceId >= 0 && context != null && context.Initialized)
        {
            if (transform.hasChanged)
            {
                positionArray[0] = transform.position.x;
                positionArray[1] = transform.position.y;
                positionArray[2] = -transform.position.z;

                PXR_Audio.Spatializer.Result ret = Context.SetSourcePosition(sourceId, positionArray);
            }

            if (nativeSource.isPlaying)
                playheadPosition = nativeSource.time;
            wasPlaying = nativeSource.isPlaying;

            if (sourceGainChanged)
            {
                PXR_Audio.Spatializer.Result ret = Context.SetSourceGain(sourceId, DB2Mag(sourceGainDB));
                if (ret == PXR_Audio.Spatializer.Result.Success)
                    sourceGainChanged = false;
            }

            if (sourceSizeChanged)
            {
                PXR_Audio.Spatializer.Result ret = Context.SetSourceSize(sourceId, sourceSize);
                if (ret == PXR_Audio.Spatializer.Result.Success)
                    sourceSizeChanged = false;
            }

            if (enableDopplerChanged)
            {
                PXR_Audio.Spatializer.Result ret = Context.SetDopplerEffect(sourceId, enableDoppler);
                if (ret == PXR_Audio.Spatializer.Result.Success)
                    enableDopplerChanged = false;
            }

            if (attenuationDistanceChanged)
            {
                PXR_Audio.Spatializer.Result ret = 
                    Context.SetSourceRange(sourceId, minAttenuationDistance, maxAttenuationDistance);
                if (ret == PXR_Audio.Spatializer.Result.Success)
                    attenuationDistanceChanged = false;
            }
        }

    }
    
    private void OnDestroy()
    {
        DestroySourceInternal();
    }
    
    private void OnDisable()
    {
        isActive = false;
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        if (EditorApplication.isPlaying)
        {
            SetGainDB(sourceGainDB);
            SetSize(sourceSize);
            SetDopplerStatus(enableDoppler);
        }
    }
#endif
    private void DestroySourceInternal()
    {
        isActive = false;
        if (context != null && context.Initialized)
        {
            var ret = context.RemoveSource(sourceId);
            if (ret != PXR_Audio.Spatializer.Result.Success)
            {
                Debug.LogError("Failed to delete source #" + sourceId + ", error code is: " + ret);
            }
            else
            {
                Debug.Log("Source #" + sourceId + " is deleted.");
            }
        }

        isAudioDSPInProgress = false;
    }
    
    private void OnAudioFilterRead(float[] data, int channels)
    {
        if (!isActive || sourceId < 0 || context == null || !context.Initialized)
        {
            //  Mute Original signal
            for (int i = 0; i < data.Length; ++i)
                data[i] = 0.0f;
            return;
        }

        isAudioDSPInProgress = true;
        int numFrames = data.Length / channels;
        float oneOverChannelsF = 1.0f / ((float) channels);
        
        //  force to mono
        if (channels > 1)
        {
            for (int frame = 0; frame < numFrames; ++frame)
            {
                float sample = 0.0f;
                for (int channel = 0; channel < channels; ++channel)
                {
                    sample += data[frame * channels + channel];
                }
                data[frame] = sample * oneOverChannelsF;
            }
        }
        Context.SubmitSourceBuffer(sourceId, data, (uint) numFrames);
        
        //  Mute Original signal
        for (int i = 0; i < data.Length; ++i)
            data[i] = 0.0f;
        isAudioDSPInProgress = false;
    }

    private float DB2Mag(float db)
    {
        return Mathf.Pow(10.0f, db / 20.0f);
    }

    void OnDrawGizmos()
    {
        Color c;
        const float colorSolidAlpha = 0.1f;

        // VolumetricRadius (purple)
        c.r = 1.0f;
        c.g = 0.0f;
        c.b = 1.0f;
        c.a = 1.0f;
        Gizmos.color = c;
        Gizmos.DrawWireSphere(transform.position, sourceSize);
        c.a = colorSolidAlpha;
        Gizmos.color = c;
        Gizmos.DrawSphere(transform.position, sourceSize);
        
        //  Attenuation distance (min && max)
        if (sourceAttenuationMode == SourceAttenuationMode.InverseSquare)
        {
            //  min 
            c.r = 1.0f;
            c.g = 0.35f;
            c.b = 0.0f;
            c.a = 1.0f;
            Gizmos.color = c;
            Gizmos.DrawWireSphere(transform.position, minAttenuationDistance);
            c.a = colorSolidAlpha;
            Gizmos.color = c;
            Gizmos.DrawSphere(transform.position, minAttenuationDistance);
            
            //  max
            c.r = 0.0f;
            c.g = 1.0f;
            c.b = 1.0f;
            c.a = 1.0f;
            Gizmos.color = c;
            Gizmos.DrawWireSphere(transform.position, maxAttenuationDistance);
            c.a = colorSolidAlpha;
            Gizmos.color = c;
            Gizmos.DrawSphere(transform.position, maxAttenuationDistance);
        }
    }
}
