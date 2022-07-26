//  Copyright © 2015-2022 Pico Technology Co., Ltd. All Rights Reserved.

using System;
using System.Collections;
using UnityEngine;

public partial class PXR_Audio_Spatializer_Context : MonoBehaviour
{
    private IntPtr context = IntPtr.Zero;

    private bool initialized = false;

    public bool Initialized
    {
        get => initialized;
    }

    [SerializeField]
    private PXR_Audio.Spatializer.RenderingMode renderingQuality = PXR_Audio.Spatializer.RenderingMode.MediumQuality;

    public PXR_Audio.Spatializer.RenderingMode RenderingQuality
    {
        get => renderingQuality;
    }

    private AudioConfiguration audioConfig;

    public AudioConfiguration AudioConfig
    {
        get => audioConfig;
    }

    private bool bypass = true;

    private bool Bypass
    {
        get => bypass;
    }

    public PXR_Audio.Spatializer.Result SubmitMesh(
        float[] vertices,
        int verticesCount,
        int[] indices,
        int indicesCount,
        PXR_Audio.Spatializer.AcousticsMaterial material,
        ref int geometryId)
    {
        return PXR_Audio.Spatializer.Api.SubmitMesh(
            context,
            vertices,
            verticesCount,
            indices,
            indicesCount,
            material,
            ref geometryId);
    }

    public PXR_Audio.Spatializer.Result SubmitMeshAndMaterialFactor(
        float[] vertices,
        int verticesCount,
        int[] indices,
        int indicesCount,
        float[] absorptionFactor,
        float scatteringFactor,
        float transmissionFactor,
        ref int geometryId)
    {
        return PXR_Audio.Spatializer.Api.SubmitMeshAndMaterialFactor(
            context,
            vertices,
            verticesCount,
            indices,
            indicesCount,
            absorptionFactor,
            scatteringFactor,
            transmissionFactor,
            ref geometryId);
    }

    public PXR_Audio.Spatializer.Result AddSource(
        PXR_Audio.Spatializer.SourceMode sourceMode,
        float[] position,
        ref int sourceId,
        bool isAsync = false)
    {
        return PXR_Audio.Spatializer.Api.AddSource(
            context,
            sourceMode,
            position,
            ref sourceId,
            isAsync);
    }

    public PXR_Audio.Spatializer.Result AddSourceWithOrientation(
        PXR_Audio.Spatializer.SourceMode mode,
        float[] position,
        float[] front,
        float[] up,
        float radius,
        ref int sourceId,
        bool isAsync)
    {
        return PXR_Audio.Spatializer.Api.AddSourceWithOrientation(
            context,
            mode,
            position,
            front,
            up,
            radius,
            ref sourceId,
            isAsync);
    }

    public PXR_Audio.Spatializer.Result AddSourceWithConfig(
        ref PXR_Audio.Spatializer.SourceConfig sourceConfig,
        ref int sourceId,
        bool isAsync)
    {
        return PXR_Audio.Spatializer.Api.AddSourceWithConfig(context, ref sourceConfig, ref sourceId, isAsync);
    }

    public PXR_Audio.Spatializer.Result SetSourceAttenuationMode(int sourceId,
        PXR_Audio.Spatializer.SourceAttenuationMode mode,
        PXR_Audio.Spatializer.DistanceAttenuationCallback directDistanceAttenuationCallback = null,
        PXR_Audio.Spatializer.DistanceAttenuationCallback indirectDistanceAttenuationCallback = null)
    {
        return PXR_Audio.Spatializer.Api.SetSourceAttenuationMode(context, sourceId, mode,
            directDistanceAttenuationCallback, indirectDistanceAttenuationCallback);
    }

    public PXR_Audio.Spatializer.Result SetSourceRange(int sourceId, float rangeMin, float rangeMax)
    {
        return PXR_Audio.Spatializer.Api.SetSourceRange(context, sourceId, rangeMin, rangeMax);
    }

    public PXR_Audio.Spatializer.Result RemoveSource(int sourceId)
    {
        return PXR_Audio.Spatializer.Api.RemoveSource(context, sourceId);
    }

    public PXR_Audio.Spatializer.Result SubmitSourceBuffer(
        int sourceId,
        float[] inputBufferPtr,
        uint numFrames)
    {
        return PXR_Audio.Spatializer.Api.SubmitSourceBuffer(
            context,
            sourceId,
            inputBufferPtr,
            numFrames);
    }

    public PXR_Audio.Spatializer.Result SubmitAmbisonicChannelBuffer(
        float[] ambisonicChannelBuffer,
        int order,
        int degree,
        PXR_Audio.Spatializer.AmbisonicNormalizationType normType,
        float gain)
    {
        return PXR_Audio.Spatializer.Api.SubmitAmbisonicChannelBuffer(
            context,
            ambisonicChannelBuffer,
            order,
            degree,
            normType,
            gain);
    }

    public PXR_Audio.Spatializer.Result SubmitInterleavedAmbisonicBuffer(
        float[] ambisonicBuffer,
        int ambisonicOrder,
        PXR_Audio.Spatializer.AmbisonicNormalizationType normType,
        float gain)
    {
        return PXR_Audio.Spatializer.Api.SubmitInterleavedAmbisonicBuffer(
            context,
            ambisonicBuffer,
            ambisonicOrder,
            normType,
            gain);
    }

    public PXR_Audio.Spatializer.Result SubmitMatrixInputBuffer(
        float[] inputBuffer,
        int inputChannelIndex)
    {
        return PXR_Audio.Spatializer.Api.SubmitMatrixInputBuffer(
            context,
            inputBuffer,
            inputChannelIndex);
    }

    public PXR_Audio.Spatializer.Result GetInterleavedBinauralBuffer(
        float[] outputBufferPtr,
        uint numFrames,
        bool isAccumulative)
    {
        return PXR_Audio.Spatializer.Api.GetInterleavedBinauralBuffer(
            context,
            outputBufferPtr,
            numFrames,
            isAccumulative);
    }

    public PXR_Audio.Spatializer.Result GetPlanarBinauralBuffer(
        float[][] outputBufferPtr,
        uint numFrames,
        bool isAccumulative)
    {
        return PXR_Audio.Spatializer.Api.GetPlanarBinauralBuffer(
            context,
            outputBufferPtr,
            numFrames,
            isAccumulative);
    }

    public PXR_Audio.Spatializer.Result GetInterleavedLoudspeakersBuffer(
        float[] outputBufferPtr,
        uint numFrames)
    {
        return PXR_Audio.Spatializer.Api.GetInterleavedLoudspeakersBuffer(
            context,
            outputBufferPtr,
            numFrames);
    }

    public PXR_Audio.Spatializer.Result GetPlanarLoudspeakersBuffer(
        float[][] outputBufferPtr,
        uint numFrames)
    {
        return PXR_Audio.Spatializer.Api.GetPlanarLoudspeakersBuffer(
            context,
            outputBufferPtr,
            numFrames);
    }

    public PXR_Audio.Spatializer.Result SetPlaybackMode(
        PXR_Audio.Spatializer.PlaybackMode playbackMode)
    {
        return PXR_Audio.Spatializer.Api.SetPlaybackMode(
            context,
            playbackMode);
    }

    public PXR_Audio.Spatializer.Result SetLoudspeakerArray(
        float[] positions,
        int numLoudspeakers)
    {
        return PXR_Audio.Spatializer.Api.SetLoudspeakerArray(
            context,
            positions,
            numLoudspeakers);
    }

    public PXR_Audio.Spatializer.Result SetMappingMatrix(
        float[] matrix,
        int numInputChannels,
        int numOutputChannels)
    {
        return PXR_Audio.Spatializer.Api.SetMappingMatrix(
            context,
            matrix,
            numInputChannels,
            numOutputChannels);
    }

    public PXR_Audio.Spatializer.Result SetListenerPosition(
        float[] position)
    {
        return PXR_Audio.Spatializer.Api.SetListenerPosition(
            context,
            position);
    }

    public PXR_Audio.Spatializer.Result SetListenerOrientation(
        float[] front,
        float[] up)
    {
        return PXR_Audio.Spatializer.Api.SetListenerOrientation(
            context,
            front,
            up);
    }

    public PXR_Audio.Spatializer.Result SetListenerPose(
        float[] position,
        float[] front,
        float[] up)
    {
        return PXR_Audio.Spatializer.Api.SetListenerPose(
            context,
            position,
            front,
            up);
    }

    public PXR_Audio.Spatializer.Result SetSourcePosition(
        int sourceId,
        float[] position)
    {
        return PXR_Audio.Spatializer.Api.SetSourcePosition(
            context,
            sourceId,
            position);
    }

    public PXR_Audio.Spatializer.Result SetSourceGain(
        int sourceId,
        float gain)
    {
        return PXR_Audio.Spatializer.Api.SetSourceGain(
            context,
            sourceId,
            gain);
    }

    public PXR_Audio.Spatializer.Result SetSourceSize(
        int sourceId,
        float volumetricSize)
    {
        return PXR_Audio.Spatializer.Api.SetSourceSize(
            context,
            sourceId,
            volumetricSize);
    }

    public PXR_Audio.Spatializer.Result UpdateSourceMode(
        int sourceId,
        PXR_Audio.Spatializer.SourceMode mode)
    {
        return PXR_Audio.Spatializer.Api.UpdateSourceMode(
            context,
            sourceId,
            mode);
    }

    public PXR_Audio.Spatializer.Result SetDopplerEffect(int sourceId, bool on)
    {
        return PXR_Audio.Spatializer.Api.SetDopplerEffect(context, sourceId, on);
    }

    void OnAudioConfigurationChangedEventHandler(bool deviceWasChanged)
    {
        audioConfig = AudioSettings.GetConfiguration();
        ResetContext(renderingQuality);
    }

    /// <summary>
    /// Setup Spatializer rendering quality.
    /// </summary>
    /// <param name="quality">Rendering quality preset.</param>
    public void SetRenderingQuality(PXR_Audio.Spatializer.RenderingMode quality)
    {
        renderingQuality = quality;
        AudioSettings.Reset(AudioSettings.GetConfiguration());
        Debug.Log("Pico Spatializer has set rendering quality to: " + renderingQuality);
    }

    private void Start()
    {
        AudioSettings.OnAudioConfigurationChanged += OnAudioConfigurationChangedEventHandler;

        //  Create context
        audioConfig = AudioSettings.GetConfiguration();
        PXR_Audio.Spatializer.Result ret = PXR_Audio.Spatializer.Api.CreateContext(
            ref context,
            renderingQuality,
            (uint)audioConfig.dspBufferSize,
            (uint)audioConfig.sampleRate);
        if (ret != PXR_Audio.Spatializer.Result.Success)
        {
            Debug.LogError("Failed to create context, Error code is: " + ret);
            return;
        }

        ret = PXR_Audio.Spatializer.Api.InitializeContext(context);
        if (ret != PXR_Audio.Spatializer.Result.Success)
        {
            Debug.LogError("Failed to initialize context, Error code is: " + ret);
        }

        //  Find all game objects in the scene that have Pico scene geometry and Pico scene material components
        PXR_Audio_Spatializer_SceneGeometry[] geometries = FindObjectsOfType<PXR_Audio_Spatializer_SceneGeometry>();
        for (int geoId = 0; geoId < geometries.Length; ++geoId)
        {
            //  For all found geometry and material pair, submit them into Pico spatializer
            ret = geometries[geoId].SubmitToContext(context);
            if (ret != PXR_Audio.Spatializer.Result.Success)
            {
                Debug.LogError("Failed to submit geometry #" + geoId + ", error code: " + ret);
            }
        }

        ret = PXR_Audio.Spatializer.Api.CommitScene(context);
        if (ret != PXR_Audio.Spatializer.Result.Success)
        {
            Debug.LogError("Failed to commit scene, error code: " + ret);
        }

        initialized = true;
        Debug.Log("Pico Spatializer Initialized.");
    }

    private void DestroyContextInternal()
    {
        initialized = false;

        //  Wait until all sources and listener's on-going audio DSP process had finished
        bool canContinue = true;
        do
        {
            canContinue = true;
            PXR_Audio_Spatializer_AudioListener[] listeners = FindObjectsOfType<PXR_Audio_Spatializer_AudioListener>();
            foreach (var listener in listeners)
            {
                if (listener != null && listener.IsAudioDSPInProgress)
                {
                    canContinue = false;
                    break;
                }
            }

            PXR_Audio_Spatializer_AudioSource[] sources = FindObjectsOfType<PXR_Audio_Spatializer_AudioSource>();
            foreach (var source in sources)
            {
                if (source != null && source.IsAudioDSPInProgress)
                {
                    canContinue = false;
                    break;
                }
            }
        } while (!canContinue);

        PXR_Audio.Spatializer.Api.Destroy(context);
        context = IntPtr.Zero;
    }

    private void OnDestroy()
    {
        //  Remove context reset handler when destructing context
        //  https://docs.microsoft.com/en-us/dotnet/desktop/winforms/controls/how-to-add-an-event-handler?view=netdesktop-6.0
        AudioSettings.OnAudioConfigurationChanged -= OnAudioConfigurationChangedEventHandler;
        DestroyContextInternal();
    }

    void Update()
    {
        PXR_Audio.Spatializer.Api.UpdateScene(context);
    }

    void ResetContext(PXR_Audio.Spatializer.RenderingMode quality)
    {
        DestroyContextInternal();

        audioConfig = AudioSettings.GetConfiguration();
        PXR_Audio.Spatializer.Result ret = PXR_Audio.Spatializer.Api.CreateContext(
            ref context,
            quality,
            (uint)audioConfig.dspBufferSize,
            (uint)audioConfig.sampleRate);
        if (ret != PXR_Audio.Spatializer.Result.Success)
        {
            Debug.LogError("Failed to create context, error code: " + ret);
        }

        ret = PXR_Audio.Spatializer.Api.InitializeContext(context);
        if (ret != PXR_Audio.Spatializer.Result.Success)
        {
            Debug.LogError("Failed to initialize context, error code: " + ret);
        }

        //  Add all the geometries back
        PXR_Audio_Spatializer_SceneGeometry[] geometries = FindObjectsOfType<PXR_Audio_Spatializer_SceneGeometry>();
        for (int geoId = 0; geoId < geometries.Length; ++geoId)
        {
            //  For all found geometry and material pair, submit them into Pico spatializer
            ret = geometries[geoId].SubmitToContext(context);
            if (ret != PXR_Audio.Spatializer.Result.Success)
            {
                Debug.LogError("Failed to submit geometry #" + geoId + ", error code: " + ret);
            }
        }

        ret = PXR_Audio.Spatializer.Api.CommitScene(context);
        if (ret != PXR_Audio.Spatializer.Result.Success)
        {
            Debug.LogError("Failed to commit scene, error code: " + ret);
        }

        initialized = true;

        //  Add all the sources back
        PXR_Audio_Spatializer_AudioSource[] sources = FindObjectsOfType<PXR_Audio_Spatializer_AudioSource>();
        for (int i = 0; i < sources.Length; ++i)
        {
            sources[i].RegisterSourceInternal();
            sources[i].Resume();
        }

        PXR_Audio_Spatializer_AmbisonicSource[] ambisonicSources =
            FindObjectsOfType<PXR_Audio_Spatializer_AmbisonicSource>();
        for (int i = 0; i < ambisonicSources.Length; ++i)
        {
            ambisonicSources[i].Resume();
        }

        Debug.Log("Pico Spatializer Context restarted.");
    }
}