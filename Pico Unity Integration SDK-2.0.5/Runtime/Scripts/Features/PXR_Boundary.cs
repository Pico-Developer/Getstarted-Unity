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

namespace Unity.XR.PXR
{
    public class PXR_Boundary
    {
        /// <summary>
        /// Set boundary system visibility to be the specified value.
        /// Note:
        ///     The actual visibility of boundary can be overridden by the system(e.g. proximity trigger) or user setting(e.g. disable boundary system).
        /// </summary>
        /// <param name="value"></param>
        public static void SetVisible(bool value)
        {
            PXR_Plugin.Boundary.UPxr_SetBoundaryVisiable(value);
        }

        /// <summary>
        /// Get boundary system visibility to be the specified value.
        /// Note:
        ///     The actual visibility of boundary can be overridden by the system(e.g. proximity trigger) or user setting(e.g. disable boundary system).
        /// </summary>
        /// <param name="value"></param>
        public static bool GetVisible()
        {
            return PXR_Plugin.Boundary.UPxr_GetBoundaryVisiable();
        }

        /// <summary>
        /// If true is returned.Indicates that the system is configured with valid data.
        /// </summary>
        /// <returns></returns>
        public static bool GetConfigured()
        {
            return PXR_Plugin.Boundary.UPxr_GetBoundaryConfigured();
        }

        /// <summary>
        /// Return result of whether Safety Boundary is enabled.
        /// </summary>
        /// <returns></returns>
        public static bool GetEnabled()
        {
            return PXR_Plugin.Boundary.UPxr_GetBoundaryEnabled();
        }

        /// <summary>
        ///  Returns the result of the traced node based on the specified boundary type.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="boundaryType"></param>
        /// <returns></returns>
        public static PxrBoundaryTriggerInfo TestNode(BoundaryTrackingNode node, BoundaryType boundaryType)
        {
            return PXR_Plugin.Boundary.UPxr_TestNodeIsInBoundary(node, boundaryType);
        }

        /// <summary>
        /// Returns the result of testing a 3d point against the specified boundary type.
        /// </summary>
        /// <param name="point">the coordinate of the point</param>
        /// <param name="boundaryType">OuterBoundary or PlayArea</param>
        /// <returns></returns>
        public static PxrBoundaryTriggerInfo TestPoint(PxrVector3f point, BoundaryType boundaryType)
        {
            return PXR_Plugin.Boundary.UPxr_TestPointIsInBoundary(point, boundaryType);
        }

        /// <summary>
        /// Return the boundary geometry points.
        /// </summary>
        /// <param name="boundaryType">OuterBoundary or PlayArea</param>
        /// <returns>Boundary geometry point-collection num</returns>
        public static Vector3[] GetGeometry(BoundaryType boundaryType)
        {
            return PXR_Plugin.Boundary.UPxr_GetBoundaryGeometry(boundaryType);
        }

        /// <summary>
        /// Get the size of self-defined safety boundary PlayArea.
        /// </summary>
        /// <param name="boundaryType"></param>
        /// <returns></returns>
        public static Vector3 GetDimensions(BoundaryType boundaryType)
        {
            return PXR_Plugin.Boundary.UPxr_GetBoundaryDimensions(boundaryType);
        }

        /// <summary>
        /// Get the camera image of Neo and set it as the environmental background.
        /// </summary>
        /// <param name="value">whether SeeThrough is enabled or not, true enabled, false disabled</param>
        public static void EnableSeeThroughManual(bool value)
        {
            PXR_Plugin.Boundary.UPxr_SetSeeThroughBackground(value);
        }

        /// <summary>
        /// Get Boundary Dialog State.
        /// </summary>
        /// <returns>NothingDialog = -1,Go-backDialog = 0,Too-farDialog = 1,LostDialog = 2,LostNoReason = 3,LostCamera = 4,LostHighLight = 5,LostLowLight = 6,LostLowFeatureCount = 7,LostReLocation = 8</returns>
        public static int GetDialogState()
        {
            return PXR_Plugin.Boundary.UPxr_GetDialogState();
        }
    }
}


