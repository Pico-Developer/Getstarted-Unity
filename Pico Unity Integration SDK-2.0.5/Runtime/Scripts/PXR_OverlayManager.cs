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
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Rendering;

namespace Unity.XR.PXR
{
    public class PXR_OverlayManager : MonoBehaviour
    {
        private void OnEnable()
        {
#if UNITY_2019_1_OR_NEWER
            if (GraphicsSettings.renderPipelineAsset != null)
            {
                RenderPipelineManager.beginCameraRendering += BeginRendering;
                RenderPipelineManager.endCameraRendering += EndRendering;
            }
            else
            {
                Camera.onPreRender += OnPreRenderCallBack;
                Camera.onPostRender += OnPostRenderCallBack;
            }
#endif
        }

        private void OnDisable()
        {
#if UNITY_2019_1_OR_NEWER
            if (GraphicsSettings.renderPipelineAsset != null)
            {
                RenderPipelineManager.beginCameraRendering -= BeginRendering;
                RenderPipelineManager.endCameraRendering -= EndRendering;
            }
            else
            {
                Camera.onPreRender -= OnPreRenderCallBack;
                Camera.onPostRender -= OnPostRenderCallBack;
            }
#endif
        }

        private void Start()
        {
            // external surface
            if (PXR_OverLay.Instances.Count > 0)
            {
                foreach (var overlay in PXR_OverLay.Instances)
                {
                    if (overlay.isExternalAndroidSurface)
                    {
                        overlay.CreateExternalSurface(overlay);
                    }
                }
            }
        }

        private void BeginRendering(ScriptableRenderContext arg1, Camera arg2)
        {
            OnPreRenderCallBack(arg2);
        }

        private void EndRendering(ScriptableRenderContext arg1, Camera arg2)
        {
            OnPostRenderCallBack(arg2);
        }

        private void OnPreRenderCallBack(Camera cam)
        {
            if (cam.tag != Camera.main.tag || cam.stereoActiveEye == Camera.MonoOrStereoscopicEye.Right) return;

            //CompositeLayers
            if (PXR_OverLay.Instances.Count > 0)
            {
                foreach (var overlay in PXR_OverLay.Instances)
                {
                    if (!overlay.isActiveAndEnabled) continue;
                    overlay.CreateTexture();
                    PXR_Plugin.Render.UPxr_GetLayerNextImageIndex(overlay.layerIndex, ref overlay.imageIndex);
                }
            }
        }

        private void OnPostRenderCallBack(Camera cam)
        {
            if (cam.tag != Camera.main.tag || cam.stereoActiveEye == Camera.MonoOrStereoscopicEye.Right) return;

            int boundaryState = PXR_Plugin.Boundary.UPxr_GetSeeThroughState();
            if (PXR_OverLay.Instances.Count > 0 && boundaryState != 2)
            {
                PXR_OverLay.Instances.Sort();
                foreach (var compositeLayer in PXR_OverLay.Instances)
                {
                    compositeLayer.UpdateCoords();
                    if (!compositeLayer.isActiveAndEnabled) continue;
                    if (compositeLayer.layerTextures[0] == null && compositeLayer.layerTextures[1] == null && !compositeLayer.isExternalAndroidSurface) continue;
                    if (compositeLayer.layerTransform != null && !compositeLayer.layerTransform.gameObject.activeSelf) continue;


                    Vector4 colorScale = compositeLayer.GetLayerColorScale();
                    Vector4 colorBias = compositeLayer.GetLayerColorOffset();
                    bool isHeadLocked = false;
                    if (compositeLayer.layerTransform != null && compositeLayer.layerTransform.parent == transform)
                    {
                        isHeadLocked = true;
                    }
                    if (!compositeLayer.isExternalAndroidSurface && !compositeLayer.CopyRT())
                    {
                        return;
                    }

                    if (compositeLayer.overlayShape == PXR_OverLay.OverlayShape.Quad)
                    {
                        PxrLayerQuad layerSubmit = new PxrLayerQuad();
                        layerSubmit.header.layerId = compositeLayer.layerIndex;
                        layerSubmit.header.colorScaleX = colorScale.x;
                        layerSubmit.header.colorScaleY = colorScale.y;
                        layerSubmit.header.colorScaleZ = colorScale.z;
                        layerSubmit.header.colorScaleW = colorScale.w;
                        layerSubmit.header.colorBiasX = colorBias.x;
                        layerSubmit.header.colorBiasY = colorBias.y;
                        layerSubmit.header.colorBiasZ = colorBias.z;
                        layerSubmit.header.colorBiasW = colorBias.w;
                        layerSubmit.header.compositionDepth = compositeLayer.layerDepth;
                        layerSubmit.header.headPose.orientation.x = compositeLayer.cameraRotations[0].x;
                        layerSubmit.header.headPose.orientation.y = compositeLayer.cameraRotations[0].y;
                        layerSubmit.header.headPose.orientation.z = -compositeLayer.cameraRotations[0].z;
                        layerSubmit.header.headPose.orientation.w = -compositeLayer.cameraRotations[0].w;
                        layerSubmit.header.headPose.position.x = (compositeLayer.cameraTranslations[0].x + compositeLayer.cameraTranslations[1].x) / 2;
                        layerSubmit.header.headPose.position.y = (compositeLayer.cameraTranslations[0].y + compositeLayer.cameraTranslations[1].y) / 2;
                        layerSubmit.header.headPose.position.z = -(compositeLayer.cameraTranslations[0].z + compositeLayer.cameraTranslations[1].z) / 2;

                        if (isHeadLocked)
                        {
                            layerSubmit.pose.orientation.x = compositeLayer.layerTransform.localRotation.x;
                            layerSubmit.pose.orientation.y = compositeLayer.layerTransform.localRotation.y;
                            layerSubmit.pose.orientation.z = -compositeLayer.layerTransform.localRotation.z;
                            layerSubmit.pose.orientation.w = -compositeLayer.layerTransform.localRotation.w;
                            layerSubmit.pose.position.x = compositeLayer.layerTransform.localPosition.x;
                            layerSubmit.pose.position.y = compositeLayer.layerTransform.localPosition.y;
                            layerSubmit.pose.position.z = -compositeLayer.layerTransform.localPosition.z;

                            layerSubmit.header.layerFlags = (UInt32)(
                                PxrLayerSubmitFlags.PxrLayerFlagLayerPoseNotInTrackingSpace |
                                PxrLayerSubmitFlags.PxrLayerFlagHeadLocked);
                        }
                        else
                        {
                            layerSubmit.pose.orientation.x = compositeLayer.modelRotations[0].x;
                            layerSubmit.pose.orientation.y = compositeLayer.modelRotations[0].y;
                            layerSubmit.pose.orientation.z = -compositeLayer.modelRotations[0].z;
                            layerSubmit.pose.orientation.w = -compositeLayer.modelRotations[0].w;
                            layerSubmit.pose.position.x = compositeLayer.modelTranslations[0].x;
                            layerSubmit.pose.position.y = compositeLayer.modelTranslations[0].y;
                            layerSubmit.pose.position.z = -compositeLayer.modelTranslations[0].z;

                            layerSubmit.header.layerFlags = (UInt32)(
                                PxrLayerSubmitFlags.PxrLayerFlagUseExternalHeadPose |
                                PxrLayerSubmitFlags.PxrLayerFlagLayerPoseNotInTrackingSpace);
                        }

                        layerSubmit.width = compositeLayer.modelScales[0].x;
                        layerSubmit.height = compositeLayer.modelScales[0].y;

                        PXR_Plugin.Render.UPxr_SubmitLayerQuad(layerSubmit);
                    }
                    else if (compositeLayer.overlayShape == PXR_OverLay.OverlayShape.Cylinder)
                    {
                        PxrLayerCylinder layerSubmit = new PxrLayerCylinder();
                        layerSubmit.header.layerId = compositeLayer.layerIndex;
                        layerSubmit.header.colorScaleX = colorScale.x;
                        layerSubmit.header.colorScaleY = colorScale.y;
                        layerSubmit.header.colorScaleZ = colorScale.z;
                        layerSubmit.header.colorScaleW = colorScale.w;
                        layerSubmit.header.colorBiasX = colorBias.x;
                        layerSubmit.header.colorBiasY = colorBias.y;
                        layerSubmit.header.colorBiasZ = colorBias.z;
                        layerSubmit.header.colorBiasW = colorBias.w;
                        layerSubmit.header.compositionDepth = compositeLayer.layerDepth;
                        layerSubmit.header.headPose.orientation.x = compositeLayer.cameraRotations[0].x;
                        layerSubmit.header.headPose.orientation.y = compositeLayer.cameraRotations[0].y;
                        layerSubmit.header.headPose.orientation.z = -compositeLayer.cameraRotations[0].z;
                        layerSubmit.header.headPose.orientation.w = -compositeLayer.cameraRotations[0].w;
                        layerSubmit.header.headPose.position.x = (compositeLayer.cameraTranslations[0].x + compositeLayer.cameraTranslations[1].x) / 2;
                        layerSubmit.header.headPose.position.y = (compositeLayer.cameraTranslations[0].y + compositeLayer.cameraTranslations[1].y) / 2;
                        layerSubmit.header.headPose.position.z = -(compositeLayer.cameraTranslations[0].z + compositeLayer.cameraTranslations[1].z) / 2;

                        if (isHeadLocked)
                        {
                            layerSubmit.pose.orientation.x = compositeLayer.layerTransform.localRotation.x;
                            layerSubmit.pose.orientation.y = compositeLayer.layerTransform.localRotation.y;
                            layerSubmit.pose.orientation.z = -compositeLayer.layerTransform.localRotation.z;
                            layerSubmit.pose.orientation.w = -compositeLayer.layerTransform.localRotation.w;
                            layerSubmit.pose.position.x = compositeLayer.layerTransform.localPosition.x;
                            layerSubmit.pose.position.y = compositeLayer.layerTransform.localPosition.y;
                            layerSubmit.pose.position.z = -compositeLayer.layerTransform.localPosition.z;

                            layerSubmit.header.layerFlags = (UInt32)(
                                PxrLayerSubmitFlags.PxrLayerFlagLayerPoseNotInTrackingSpace |
                                PxrLayerSubmitFlags.PxrLayerFlagHeadLocked);
                        }
                        else
                        {
                            layerSubmit.pose.orientation.x = compositeLayer.modelRotations[0].x;
                            layerSubmit.pose.orientation.y = compositeLayer.modelRotations[0].y;
                            layerSubmit.pose.orientation.z = -compositeLayer.modelRotations[0].z;
                            layerSubmit.pose.orientation.w = -compositeLayer.modelRotations[0].w;
                            layerSubmit.pose.position.x = compositeLayer.modelTranslations[0].x;
                            layerSubmit.pose.position.y = compositeLayer.modelTranslations[0].y;
                            layerSubmit.pose.position.z = -compositeLayer.modelTranslations[0].z;

                            layerSubmit.header.layerFlags = (UInt32)(
                                PxrLayerSubmitFlags.PxrLayerFlagUseExternalHeadPose |
                                PxrLayerSubmitFlags.PxrLayerFlagLayerPoseNotInTrackingSpace);
                        }

                        if (compositeLayer.modelScales[0].z != 0)
                        {
                            layerSubmit.centralAngle = compositeLayer.modelScales[0].x / compositeLayer.modelScales[0].z;
                        }
                        else
                        {
                            Debug.LogError("PXRLog scale.z is 0");
                        }
                        layerSubmit.height = compositeLayer.modelScales[0].y;
                        layerSubmit.radius = compositeLayer.modelScales[0].z;

                        PXR_Plugin.Render.UPxr_SubmitLayerCylinder(layerSubmit);
                    }
                    else if (compositeLayer.overlayShape == PXR_OverLay.OverlayShape.Equirect)
                    {
                        PxrLayerEquirect layerSubmit = new PxrLayerEquirect();
                        layerSubmit.header.layerId = compositeLayer.layerIndex;
                        layerSubmit.header.layerFlags = (UInt32)(
                                PxrLayerSubmitFlags.PxrLayerFlagUseExternalHeadPose |
                                PxrLayerSubmitFlags.PxrLayerFlagLayerPoseNotInTrackingSpace);
                        layerSubmit.header.colorScaleX = colorScale.x;
                        layerSubmit.header.colorScaleY = colorScale.y;
                        layerSubmit.header.colorScaleZ = colorScale.z;
                        layerSubmit.header.colorScaleW = colorScale.w;
                        layerSubmit.header.colorBiasX = colorBias.x;
                        layerSubmit.header.colorBiasY = colorBias.y;
                        layerSubmit.header.colorBiasZ = colorBias.z;
                        layerSubmit.header.colorBiasW = colorBias.w;
                        layerSubmit.header.compositionDepth = compositeLayer.layerDepth;
                        layerSubmit.header.headPose.orientation.x = compositeLayer.cameraRotations[0].x;
                        layerSubmit.header.headPose.orientation.y = compositeLayer.cameraRotations[0].y;
                        layerSubmit.header.headPose.orientation.z = -compositeLayer.cameraRotations[0].z;
                        layerSubmit.header.headPose.orientation.w = -compositeLayer.cameraRotations[0].w;
                        layerSubmit.header.headPose.position.x = (compositeLayer.cameraTranslations[0].x + compositeLayer.cameraTranslations[1].x) / 2;
                        layerSubmit.header.headPose.position.y = (compositeLayer.cameraTranslations[0].y + compositeLayer.cameraTranslations[1].y) / 2;
                        layerSubmit.header.headPose.position.z = -(compositeLayer.cameraTranslations[0].z + compositeLayer.cameraTranslations[1].z) / 2;

                        layerSubmit.pose.orientation.x = compositeLayer.modelRotations[0].x;
                        layerSubmit.pose.orientation.y = compositeLayer.modelRotations[0].y;
                        layerSubmit.pose.orientation.z = -compositeLayer.modelRotations[0].z;
                        layerSubmit.pose.orientation.w = -compositeLayer.modelRotations[0].w;
                        layerSubmit.pose.position.x = compositeLayer.modelTranslations[0].x;
                        layerSubmit.pose.position.y = compositeLayer.modelTranslations[0].y;
                        layerSubmit.pose.position.z = -compositeLayer.modelTranslations[0].z;

                        layerSubmit.centralHorizontalAngle = compositeLayer.modelScales[0].x;
                        layerSubmit.upperVerticalAngle = compositeLayer.modelScales[0].y / 2;
                        layerSubmit.lowerVerticalAngle = -compositeLayer.modelScales[0].y / 2;
                        layerSubmit.radius = compositeLayer.modelScales[0].z;

                        PXR_Plugin.Render.UPxr_SubmitLayerEquirect(layerSubmit);
                    }
                }
            }
        }
    }
}

