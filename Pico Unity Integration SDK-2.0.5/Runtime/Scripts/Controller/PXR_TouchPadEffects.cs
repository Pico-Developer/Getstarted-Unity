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

using UnityEngine;
using UnityEngine.XR;

namespace Unity.XR.PXR
{
    public class PXR_TouchPadEffects : MonoBehaviour
    {
        private MeshRenderer touchRenderer;
        [HideInInspector]
        public PXR_Input.ControllerDevice currentDevice;

        private Vector2 touchPos = new Vector2();

        void Start()
        {
            touchRenderer = GetComponent<MeshRenderer>();
        }

        private void Update()
        {
            InputDevices.GetDeviceAtXRNode(XRNode.LeftHand).TryGetFeatureValue(CommonUsages.primary2DAxis, out touchPos);

            if (touchPos != Vector2.zero)
            {
                touchRenderer.enabled = true;
                transform.localPosition = new Vector3(-touchPos.x * 1.28f, 1.6f, -2.98f - touchPos.y * 1.28f);
            }
            else
            {
                touchRenderer.enabled = false;
            }
        }
    }
}
