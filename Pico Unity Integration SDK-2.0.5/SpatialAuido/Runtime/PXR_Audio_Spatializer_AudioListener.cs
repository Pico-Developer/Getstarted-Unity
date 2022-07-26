//  Copyright © 2015-2022 Pico Technology Co., Ltd. All Rights Reserved.

using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioListener))]
public class PXR_Audio_Spatializer_AudioListener : MonoBehaviour
{
    private bool isActive;

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

    private float[] positionArray = new float[3] {0.0f, 0.0f, 0.0f};
    private float[] frontArray = new float[3] {0.0f, 0.0f, 0.0f};
    private float[] upArray = new float[3] {0.0f, 0.0f, 0.0f};

    private bool isAudioDSPInProgress = false;

    public bool IsAudioDSPInProgress
    {
        get
        {
            return isAudioDSPInProgress;
        }
    }

    IEnumerator Start()
    {
        //  Wait for context to be initialized
        yield return new WaitUntil(() =>
        {
            return Context != null && Context.Initialized;
        });

        //  Initialize listener pose
        UpdatePose();

        isActive = true;
    }

    private void OnEnable()
    {
        isActive = true;
    }

    void Update()
    {
        if (isActive && context != null && context.Initialized && transform.hasChanged)
        {
            UpdatePose();
        }
    }

    private void OnDestroy()
    {
        isActive = false;
        isAudioDSPInProgress = false;
    }

    private void OnDisable()
    {
        isActive = false;
    }

    void UpdatePose()
    {
        positionArray[0] = transform.position.x;
        positionArray[1] = transform.position.y;
        positionArray[2] = -transform.position.z;
        frontArray[0] = transform.forward.x;
        frontArray[1] = transform.forward.y;
        frontArray[2] = -transform.forward.z;
        upArray[0] = transform.up.x;
        upArray[1] = transform.up.y;
        upArray[2] = -transform.up.z;
        Context.SetListenerPose(positionArray, frontArray, upArray);
    }

    private void OnAudioFilterRead(float[] data, int channels)
    {
        if (!isActive || context == null || !context.Initialized)
            return;

        isAudioDSPInProgress = true;
        context.GetInterleavedBinauralBuffer(data, (uint) (data.Length / channels), true);
        isAudioDSPInProgress = false;
    }
}
