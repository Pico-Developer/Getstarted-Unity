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

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace Unity.XR.PXR
{
    public class PXR_EyeTracking
    {
        /// <summary>
        /// Get the PosMatrix of the head (This interface is only available for Neo 2 Eye and Neo3 Pro).
        /// </summary>
        /// <param name="matrix">Receive the Matrix4x4 returned by the result</param>
        /// <returns>True - succeed, false - failed</returns>
        public static bool GetHeadPosMatrix(out Matrix4x4 matrix)
        {
            matrix = Matrix4x4.identity;

            if (!PXR_Manager.Instance.eyeTracking)
                return false;

            InputDevice device = default(InputDevice);
            if (!GetEyeTrackingDevice(out device))
                return false;

            Vector3 headPos = Vector3.zero;
            if (!device.TryGetFeatureValue(CommonUsages.devicePosition, out headPos))
            {
                Debug.LogError("PXRLog Failed at GetHeadPosMatrix Pos");
                return false;
            }

            Quaternion headRot = Quaternion.identity;
            if (!device.TryGetFeatureValue(CommonUsages.deviceRotation, out headRot))
            {
                Debug.LogError("PXRLog Failed at GetHeadPosMatrix Rot");
                return false;
            }

            matrix = Matrix4x4.TRS(headPos, headRot, Vector3.one);
            return true;
        }

        /// <summary>
        /// Get Eye Tracking data (This interface is only available for Neo 2 Eye and Neo3 Pro).
        /// </summary>
        /// <param name="device">Receive the InputDevice returned by the result</param>
        /// <returns>True - succeed, false - failed</returns>
        static bool GetEyeTrackingDevice(out InputDevice device)
        {
            device = default(InputDevice);

            if (!PXR_Manager.Instance.eyeTracking)
                return false;

            List<InputDevice> devices = new List<InputDevice>();
            InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.EyeTracking | InputDeviceCharacteristics.HeadMounted, devices);
            if (devices.Count == 0)
            {
                Debug.LogError("PXRLog Failed at GetEyeTrackingDevice devices.Count");
                return false;
            }
            device = devices[0];
            if (!device.isValid)
                Debug.LogError("PXRLog Failed at GetEyeTrackingDevice device.isValid");
            return device.isValid;
        }

        /// <summary>
        /// Get the position of the combined gaze point (This interface is only available for Neo 2 Eye and Neo3 Pro).
        /// </summary>
        /// <param name="point">Receive the vector 3 returned by the result</param>
        /// <returns>True - succeed, false - failed</returns>
        public static bool GetCombineEyeGazePoint(out Vector3 point)
        {
            point = Vector3.zero;

            if (!PXR_Manager.Instance.eyeTracking)
                return false;

            InputDevice device = default(InputDevice);
            if (!GetEyeTrackingDevice(out device))
                return false;

            if (!device.TryGetFeatureValue(PXR_Usages.combineEyePoint, out point))
            {
                Debug.Log("PXRLog Failed at GetCombineEyeGazePoint point");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Get the direction of the combined gaze point (This interface is only available for Neo 2 Eye and Neo3 Pro).
        /// </summary>
        /// <param name="vector">Receive the vector 3 returned by the result</param>
        /// <returns>True - succeed, false - failed</returns>
        public static bool GetCombineEyeGazeVector(out Vector3 vector)
        {
            vector = Vector3.zero;

            if (!PXR_Manager.Instance.eyeTracking)
                return false;

            InputDevice device = default(InputDevice);
            if (!GetEyeTrackingDevice(out device))
                return false;

            if (!device.TryGetFeatureValue(PXR_Usages.combineEyeVector, out vector))
            {
                Debug.LogError("PXRLog Failed at GetCombineEyeGazeVector vector");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Get the openness/closeness of left eye (This interface is only available for Neo 2 Eye and Neo3 Pro).
        /// </summary>
        /// <param name="openness">Receive the float returned by the result; range: 0.0 - 1.0, 0.0 - complete closeness, 1.0 - complete openness</param>
        /// <returns>True - succeed, false - failed</returns>
        public static bool GetLeftEyeGazeOpenness(out float openness)
        {
            openness = 0;

            if (!PXR_Manager.Instance.eyeTracking)
                return false;

            InputDevice device = default(InputDevice);
            if (!GetEyeTrackingDevice(out device))
                return false;

            if (!device.TryGetFeatureValue(PXR_Usages.leftEyeOpenness, out openness))
            {
                Debug.LogError("PXRLog Failed at GetLeftEyeGazeOpenness openness");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Get the openness/closeness of right eye (This interface is only available for Neo 2 Eye and Neo3 Pro).
        /// </summary>
        /// <param name="openness">Receive the float returned by the result; range: 0.0 - 1.0, 0.0 - complete closeness, 1.0 - complete openness</param>
        /// <returns>True - succeed, false - failed</returns>
        public static bool GetRightEyeGazeOpenness(out float openness)
        {
            openness = 0;

            if (!PXR_Manager.Instance.eyeTracking)
                return false;

            InputDevice device = default(InputDevice);
            if (!GetEyeTrackingDevice(out device))
                return false;

            if (!device.TryGetFeatureValue(PXR_Usages.rightEyeOpenness, out openness))
            {
                Debug.LogError("PXRLog Failed at GetRightEyeGazeOpenness openness");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Get the pose status of left eye(This interface is only available for Neo 2 Eye and Neo3 Pro).
        /// </summary>
        /// <param name="status">Receive the int returned by the result; 0- not good, 1-good</param>
        /// <returns>True - succeed, false - failed</returns>
        public static bool GetLeftEyePoseStatus(out uint status)
        {
            status = 0;
            if (!PXR_Manager.Instance.eyeTracking)
                return false;

            InputDevice device = default(InputDevice);
            if (!GetEyeTrackingDevice(out device))
                return false;

            if (!device.TryGetFeatureValue(PXR_Usages.leftEyePoseStatus, out status))
            {
                Debug.LogError("PXRLog Failed at GetLeftEyePoseStatus status");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Get the pose status of right eye(This interface is only available for Neo 2 Eye and Neo3 Pro).
        /// </summary>
        /// <param name="status">Receive the int returned by the result; 0- not good, 1-good</param>
        /// <returns>True - succeed, false - failed</returns>
        public static bool GetRightEyePoseStatus(out uint status)
        {
            status = 0;
            if (!PXR_Manager.Instance.eyeTracking)
                return false;

            InputDevice device = default(InputDevice);
            if (!GetEyeTrackingDevice(out device))
                return false;

            if (!device.TryGetFeatureValue(PXR_Usages.rightEyePoseStatus, out status))
            {
                Debug.LogError("PXRLog Failed at GetRightEyePoseStatus status");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Get the combined pose status (This interface is only available for Neo 2 Eye and Neo3 Pro).
        /// </summary>
        /// <param name="status">Receive the int returned by the result; 0- not good, 1-good</param>
        /// <returns>True - succeed, false - failed</returns>
        public static bool GetCombinedEyePoseStatus(out uint status)
        {
            status = 0;
            if (!PXR_Manager.Instance.eyeTracking)
                return false;

            InputDevice device = default(InputDevice);
            if (!GetEyeTrackingDevice(out device))
                return false;

            if (!device.TryGetFeatureValue(PXR_Usages.combinedEyePoseStatus, out status))
            {
                Debug.LogError("PXRLog Failed at GetCombinedEyePoseStatus status");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Get the position guide of left eye (This interface is only available for Neo 2 Eye and Neo3 Pro).
        /// </summary>
        /// <param name="position">Receive the vector 3 returned by the result</param>
        /// <returns>True - succeed, false - failed</returns>
        public static bool GetLeftEyePositionGuide(out Vector3 position)
        {
            position = Vector3.zero;
            if (!PXR_Manager.Instance.eyeTracking)
                return false;

            InputDevice device = default(InputDevice);
            if (!GetEyeTrackingDevice(out device))
                return false;

            if (!device.TryGetFeatureValue(PXR_Usages.leftEyePositionGuide, out position))
            {
                Debug.LogError("PXRLog Failed at GetLeftEyePositionGuide pos");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Get the position guide of right eye (This interface is only available for Neo 2 Eye and Neo3 Pro).
        /// </summary>
        /// <param name="position">Receive the vector 3 returned by the result</param>
        /// <returns>True - succeed, false - failed</returns>
        public static bool GetRightEyePositionGuide(out Vector3 position)
        {
            position = Vector3.zero;
            if (!PXR_Manager.Instance.eyeTracking)
                return false;

            InputDevice device = default(InputDevice);
            if (!GetEyeTrackingDevice(out device))
                return false;

            if (!device.TryGetFeatureValue(PXR_Usages.rightEyePositionGuide, out position))
            {
                Debug.LogError("PXRLog Failed at GetRightEyePositionGuide pos");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Get the foveated gaze direction (This interface is only available for Neo 2 Eye and Neo3 Pro).
        /// </summary>
        /// <param name="direction">Receive the vector 3 returned by the result</param>
        /// <returns></returns>
        public static bool GetFoveatedGazeDirection(out Vector3 direction)
        {
            direction = Vector3.zero;
            if (!PXR_Manager.Instance.eyeTracking)
                return false;

            InputDevice device = default(InputDevice);
            if (!GetEyeTrackingDevice(out device))
                return false;

            if (!device.TryGetFeatureValue(PXR_Usages.foveatedGazeDirection, out direction))
            {
                Debug.LogError("PXRLog Failed at GetFoveatedGazeDirection direction");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Get the current state of the foveated Gaze Direction signal. (This interface is only available for Neo 2 Eye and Neo3 Pro).
        /// </summary>
        /// <param name="status">Receive the int returned by the result; 0- not good, 1-good</param>
        /// <returns>True - succeed, false - failed</returns>
        public static bool GetFoveatedGazeTrackingState(out uint state)
        {
            state = 0;
            if (!PXR_Manager.Instance.eyeTracking)
                return false;

            InputDevice device = default(InputDevice);
            if (!GetEyeTrackingDevice(out device))
                return false;

            if (!device.TryGetFeatureValue(PXR_Usages.foveatedGazeTrackingState, out state))
            {
                Debug.LogError("PXRLog Failed at GetFoveatedGazeTrackingState state");
                return false;
            }
            return true;
        }

    }
}