/*******************************************************************************
Copyright © 2015-2022 Pico Technology Co., Ltd.All rights reserved.  

NOTICE：All information contained herein is, and remains the property of 
Pico Technology Co., Ltd. The intellectual and technical concepts 
contained hererin are proprietary to Pico Technology Co., Ltd. and may be 
covered by patents, patents in process, and are protected by trade secret or 
copyright law. Dissemination of this information or reproduction of this 
material is strictly forbidden unless prior written permission is obtained from
Pico Technology Co., Ltd. 
*******************************************************************************/

using System;
using UnityEngine;
using UnityEngine.XR.Management;
using UnityEngine.UI;


namespace Unity.XR.PXR
{
    public enum EyesEnum
    {
        LeftEye,
        RightEye
    }
    public class PXR_PassThrough : MonoBehaviour
    {
        public RawImage viewImageLeft, viewImageRight;

        private PXR_PassThroughSystem passThroughSystem;
        private PXR_Loader loader;
        private Texture2D cameraTexLeft, cameraTexRight;
        private int width, height;

        private void Start()
        {
            width = 600;
            height = 600;
        }

        void OnEnable()
        {
            loader = XRGeneralSettings.Instance.Manager.activeLoader as PXR_Loader;
            if (loader == null)
            {
                Debug.LogError("PXRLog Has no XR loader in the project!");
                return;
            }
            passThroughSystem = loader.GetLoadedSubsystem<PXR_PassThroughSystem>();
            if (passThroughSystem == null)
            {
                Debug.LogError("PXRLog Has no XR Camera subsystem !");
                return;
            }
            passThroughSystem.Start();
        }

        private void OnDisable()
        {
            if (passThroughSystem != null)
            {
                passThroughSystem.Stop();
            }
            loader = null;
            passThroughSystem = null;
            cameraTexLeft = null;
            cameraTexRight = null;
        }

        void Update()
        {
            if (UnityEngine.Input.GetKey(KeyCode.JoystickButton0))
            {
                DrawTexture();
            }
        }

        private void DrawTexture()
        {
            if (passThroughSystem == null) return;

            passThroughSystem.UpdateTextures();
            int eye = (int)EyesEnum.LeftEye;
            //left eye
            IntPtr textureId = (IntPtr)passThroughSystem.UpdateCameraTextureID(eye);
            if (cameraTexLeft == null)
                cameraTexLeft = Texture2D.CreateExternalTexture(width, height, TextureFormat.RGBA32, false, QualitySettings.activeColorSpace == ColorSpace.Linear, textureId);
            else
            {
                cameraTexLeft.UpdateExternalTexture(textureId);
            }

            //right eye
            eye = (int)EyesEnum.RightEye;
            textureId = (IntPtr)passThroughSystem.UpdateCameraTextureID(eye);
            if (cameraTexRight == null)
                cameraTexRight = Texture2D.CreateExternalTexture(width, height, TextureFormat.RGBA32, false, QualitySettings.activeColorSpace == ColorSpace.Linear, textureId);
            else
            {
                cameraTexRight.UpdateExternalTexture(textureId);
            }

            viewImageLeft.texture = cameraTexLeft;
            viewImageRight.texture = cameraTexRight;
        }
    }
}