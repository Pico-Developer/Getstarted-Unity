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
using System.Collections;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Management;

namespace Unity.XR.PXR
{
    public class PXR_Manager : MonoBehaviour
    {
        private const string TAG = "[PXR_Manager]";
        private static PXR_Manager instance = null;
        public static PXR_Manager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<PXR_Manager>();
                    if (instance == null)
                    {
                        Debug.LogError("PXRLog instance is not initialized!");
                    }
                }
                return instance;
            }
        }

        private int lastBoundaryState = 0;
        private int currentBoundaryState;
        private float refreshRate = -1.0f;
        
        private Camera eyeCamera;
        private int eyeCameraOriginCullingMask;
        private CameraClearFlags eyeCameraOriginClearFlag;
        private Color eyeCameraOriginBackgroundColor;

        [HideInInspector]
        public bool showFps;
        [HideInInspector]
        public bool useDefaultFps = true;
        [HideInInspector]
        public int customFps;
        [HideInInspector]
        public bool screenFade;
        [HideInInspector]
        public bool eyeTracking;
        [HideInInspector]
        public FoveationLevel foveationLevel = FoveationLevel.None;
        private bool isNeedResume = false;

        //Entitlement Check Result
        [HideInInspector]
        public int appCheckResult = 100;
        public delegate void EntitlementCheckResult(int ReturnValue);
        public static event EntitlementCheckResult EntitlementCheckResultEvent;

        public Action<float> DisplayRefreshRateChanged;

        [HideInInspector]
        public bool useRecommendedAntiAliasingLevel = true;
        
        void Awake()
        {
            PXR_Plugin.PlatformSetting.UPxr_BindVerifyService(gameObject.name);
            eyeCamera = Camera.main;
            Camera.main.depthTextureMode = DepthTextureMode.Depth;
#if UNITY_ANDROID && !UNITY_EDITOR
            SetFrameRate();
#endif
            if (foveationLevel != FoveationLevel.None)
            {
                PXR_Plugin.Render.UPxr_SetFoveationLevel(foveationLevel);
            }

            PXR_Plugin.System.UPxr_EnableEyeTracking(eyeTracking);

            //version log
            Debug.Log("PXRLog XR Platform----SDK Version:" + PXR_Plugin.System.UPxr_GetSDKVersion());

            //log level
            int logLevel = PXR_Plugin.System.UPxr_GetConfigInt(ConfigType.UnityLogLevel);
            Debug.Log("PXRLog XR Platform----SDK logLevel:" + logLevel);
            PLog.logLevel = (PLog.LogLevel)logLevel;

            int recommendedAntiAliasingLevel = 0;
            recommendedAntiAliasingLevel = PXR_Plugin.System.UPxr_GetConfigInt(ConfigType.AntiAliasingLevelRecommended);
            if (useRecommendedAntiAliasingLevel && QualitySettings.antiAliasing != recommendedAntiAliasingLevel)
            {
                QualitySettings.antiAliasing = recommendedAntiAliasingLevel;
            }
        }

        void OnEnable()
        {
            if (PXR_OverLay.Instances.Count > 0)
            {
                if (Camera.main.gameObject.GetComponent<PXR_OverlayManager>() == null)
                {
                    Camera.main.gameObject.AddComponent<PXR_OverlayManager>();
                }

                foreach (var layer in PXR_OverLay.Instances)
                {
                    layer.RefreshCamera(eyeCamera);
                }
            }
        }

        void OnApplicationPause(bool pause)
        {
            if (!pause)
            {
                if (isNeedResume)
                {
                    StartCoroutine("StartXR");
                    isNeedResume = false;
                }
            }
        }

        public IEnumerator StartXR()
        {
            yield return XRGeneralSettings.Instance.Manager.InitializeLoader();

            if (XRGeneralSettings.Instance.Manager.activeLoader == null)
            {
                Debug.LogError("PXRLog Initializing XR Failed. Check log for details.");
            }
            else
            {
                XRGeneralSettings.Instance.Manager.StartSubsystems();
            }
        }

        void StopXR()
        {
            XRGeneralSettings.Instance.Manager.StopSubsystems();
            XRGeneralSettings.Instance.Manager.DeinitializeLoader();
        }

        void Start()
        {
            bool systemFps = false;
#if UNITY_ANDROID && !UNITY_EDITOR
            PXR_Plugin.System.UPxr_GetTextSize("");//load res & get permission of external storage
            systemFps = Convert.ToBoolean(Convert.ToInt16(PXR_Plugin.System.UPxr_GetConfigInt(ConfigType.ShowFps)));
#endif
            if (systemFps || showFps)
            {
                Camera.main.transform.Find("FPS").gameObject.SetActive(true);
            }

            if (PXR_PlatformSetting.Instance.startTimeEntitlementCheck)
            {
                if (PXR_Plugin.PlatformSetting.UPxr_IsCurrentDeviceValid() != PXR_PlatformSetting.simulationType.Valid)
                {
                    Debug.Log("PXRLog Entitlement Check Simulation DO NOT PASS");
                    string appID = PXR_PlatformSetting.Instance.appID;
                    Debug.Log("PXRLog Entitlement Check Enable");
                    // 0:success -1:invalid params -2:service not exist -3:time out
                    PXR_Plugin.PlatformSetting.UPxr_AppEntitlementCheckExtra(appID);
                }
                else
                {
                    Debug.Log("PXRLog Entitlement Check Simulation PASS");
                }
            }
#if UNITY_EDITOR
            Application.targetFrameRate = 72;
#endif
        }
        
        void Update()
        {
            // boundary
            if (eyeCamera != null && eyeCamera.enabled)
            {
                currentBoundaryState = PXR_Plugin.Boundary.UPxr_GetSeeThroughState();

                if (currentBoundaryState != lastBoundaryState)
                {
                    if (currentBoundaryState == 2) // close camera render
                    {
                        // record
                        eyeCameraOriginCullingMask = eyeCamera.cullingMask;
                        eyeCameraOriginClearFlag = eyeCamera.clearFlags;
                        eyeCameraOriginBackgroundColor = eyeCamera.backgroundColor;

                        // close render
                        eyeCamera.cullingMask = 0;
                        eyeCamera.clearFlags = CameraClearFlags.SolidColor;
                        eyeCamera.backgroundColor = Color.black;
                    }
                    else if (currentBoundaryState == 1) // open camera render
                    {
                        if (lastBoundaryState == 2)
                        {
                            if (eyeCamera.cullingMask == 0)
                            {
                                eyeCamera.cullingMask = eyeCameraOriginCullingMask;
                            }
                            if (eyeCamera.clearFlags == CameraClearFlags.SolidColor)
                            {
                                eyeCamera.clearFlags = eyeCameraOriginClearFlag;
                            }
                            if (eyeCamera.backgroundColor == Color.black)
                            {
                                eyeCamera.backgroundColor = eyeCameraOriginBackgroundColor;
                            }
                        }
                    }
                    else // open camera render(recover)
                    {
                        if ((lastBoundaryState == 2 || lastBoundaryState == 1))
                        {
                            if (eyeCamera.cullingMask == 0)
                            {
                                eyeCamera.cullingMask = eyeCameraOriginCullingMask;
                            }
                            if (eyeCamera.clearFlags == CameraClearFlags.SolidColor)
                            {
                                eyeCamera.clearFlags = eyeCameraOriginClearFlag;
                            }
                            if (eyeCamera.backgroundColor == Color.black)
                            {
                                eyeCamera.backgroundColor = eyeCameraOriginBackgroundColor;
                            }
                        }
                    }
                    lastBoundaryState = currentBoundaryState;
                }
            }

            if (Math.Abs(refreshRate - PXR_Plugin.System.UPxr_RefreshRateChanged()) > 0.1f)
            {
                refreshRate = PXR_Plugin.System.UPxr_RefreshRateChanged();
                if (DisplayRefreshRateChanged != null)
                    DisplayRefreshRateChanged(refreshRate);
            }
            //recenter callback
            if (PXR_Plugin.System.UPxr_GetHomeKey())
            {
                if (PXR_Plugin.System.RecenterSuccess != null)
                {
                    PXR_Plugin.System.RecenterSuccess();
                }
                PXR_Plugin.System.UPxr_InitHomeKey();
            }
        }
        void OnDisable()
        {
            StopAllCoroutines();
        }

        private void SetFrameRate()
        {
            int targetFrameRate = (int)PXR_Plugin.System.UPxr_GetConfigInt(ConfigType.TargetFrameRate);
            int displayRefreshRate = (int)PXR_Plugin.System.UPxr_GetConfigFloat(ConfigType.DisplayRefreshRate);
            Application.targetFrameRate = targetFrameRate > 0 ? targetFrameRate : displayRefreshRate;
            if (!useDefaultFps)
            {
                if (customFps <= displayRefreshRate)
                {
                    Application.targetFrameRate = customFps;
                }
                else
                {
                    Application.targetFrameRate = displayRefreshRate;
                }
            }
            PLog.i(TAG, string.Format("Customize FPS : {0}", Application.targetFrameRate));
        }
    }
}

