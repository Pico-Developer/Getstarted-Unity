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
using UnityEngine.XR;

namespace Unity.XR.PXR
{
    public static class PXR_Input
    {
        public enum ControllerDevice
        {
            G2 = 3,
            Neo2,
            Neo3,
            NewController = 10
        }

        public enum Controller
        {
            LeftController,
            RightController,
        }

        /// <summary>
        /// Get the current master control controller.
        /// </summary>
        public static Controller GetDominantHand()
        {
            return (Controller)PXR_Plugin.Controller.UPxr_GetControllerMainInputHandle();
        }

        /// <summary>
        /// Set the current master control controller.
        /// </summary>
        public static void SetDominantHand(Controller controller)
        {
            PXR_Plugin.Controller.UPxr_SetControllerMainInputHandle((UInt32)controller);
        }

        /// <summary>
        /// Set the controller vibrate.
        /// </summary>
        public static void SetControllerVibration(float strength, int time, Controller controller)
        {
            PXR_Plugin.Controller.UPxr_SetControllerVibration((UInt32)controller, strength, time);
        }

        /// <summary>
        /// Get the controller device.
        /// </summary>
        public static ControllerDevice GetControllerDeviceType()
        {
            return (ControllerDevice)PXR_Plugin.Controller.UPxr_GetControllerType();
        }

        /// <summary>
        /// Get the connection status of Controller.
        /// </summary>
        public static bool IsControllerConnected(Controller controller)
        {
            var state = false;
            switch (controller)
            {
                case Controller.LeftController:
                    InputDevices.GetDeviceAtXRNode(XRNode.LeftHand).TryGetFeatureValue(PXR_Usages.controllerStatus, out state);
                    return state;
                case Controller.RightController:
                    InputDevices.GetDeviceAtXRNode(XRNode.RightHand).TryGetFeatureValue(PXR_Usages.controllerStatus, out state);
                    return state;
            }
            return state;
        }

        /// <summary>
        /// Set the controller origin offset data.
        /// </summary>
        /// <param name="hand">0,1</param>
        /// <param name="offset">m</param>
        public static void SetControllerOriginOffset(Controller controller, Vector3 offset)
        {
            PXR_Plugin.Controller.UPxr_SetControllerOriginOffset((int)controller, offset);
        }

        /// <summary>
        /// Get the controller predict rotation data.
        /// </summary>
        /// <param name="hand">0,1</param>
        /// <param name="predictTime">ms</param>
        public static Quaternion GetControllerPredictRotation(Controller controller, double predictTime)
        {
            PxrControllerTracking pxrControllerTracking = new PxrControllerTracking();
            float[] headData = new float[7] { 0, 0, 0, 0, 0, 0, 0 };

            PXR_Plugin.Controller.UPxr_GetControllerTrackingState((uint)controller, predictTime,headData, ref pxrControllerTracking);

            return new Quaternion(
                pxrControllerTracking.localControllerPose.pose.orientation.x,
                pxrControllerTracking.localControllerPose.pose.orientation.y, 
                pxrControllerTracking.localControllerPose.pose.orientation.z,
                pxrControllerTracking.localControllerPose.pose.orientation.w);
        }

        /// <summary>
        /// Get the controller predict position data.
        /// </summary>
        /// <param name="hand">0,1</param>
        /// <param name="predictTime">ms</param>
        public static Vector3 GetControllerPredictPosition(Controller controller, double predictTime)
        {
            PxrControllerTracking pxrControllerTracking = new PxrControllerTracking();
            float[] headData = new float[7] { 0, 0, 0, 0, 0, 0, 0 };

            PXR_Plugin.Controller.UPxr_GetControllerTrackingState((uint)controller, predictTime, headData, ref pxrControllerTracking);

            return new Vector3(
                pxrControllerTracking.localControllerPose.pose.position.x, 
                pxrControllerTracking.localControllerPose.pose.position.y,
                pxrControllerTracking.localControllerPose.pose.position.z);
        }

        public static int SetControllerEnableKey(bool isEnable, PxrControllerKeyMap Key) {
            return PXR_Plugin.Controller.UPxr_SetControllerEnableKey(isEnable, Key);
        }
    }
}

