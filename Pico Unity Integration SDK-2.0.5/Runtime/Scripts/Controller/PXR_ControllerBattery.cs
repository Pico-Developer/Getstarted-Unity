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
using UnityEngine.UI;
using UnityEngine.XR;

namespace Unity.XR.PXR
{
    public class PXR_ControllerBattery : MonoBehaviour
    {
        [SerializeField]
        private Sprite power1;
        [SerializeField]
        private Sprite power2;
        [SerializeField]
        private Sprite power3;
        [SerializeField]
        private Sprite power4;
        [SerializeField]
        private Sprite power5;

        public PXR_Input.Controller hand;
        private Image powerImage;
        private int battery;

        public PXR_ControllerBattery(Sprite power1Sprite, Sprite power2Sprite, Sprite power3Sprite, Sprite power4Sprite, Sprite power5Sprite)
        {
            power1 = power1Sprite;
            power2 = power2Sprite;
            power3 = power3Sprite;
            power4 = power4Sprite;
            power5 = power5Sprite;
        }

        void Start()
        {
            powerImage = transform.GetComponent<Image>();
            battery = -1;
        }

        void Update()
        {
            RefreshPower(hand);
        }

        private void RefreshPower(PXR_Input.Controller hand)
        {
            battery = RefreshControllerBattery(hand);
            if (battery == 0)
            {
                powerImage.enabled = true;
            }
            switch (battery)
            {
                case 1:
                    powerImage.sprite = power1;
                    powerImage.color = Color.red;
                    break;
                case 2:
                    powerImage.sprite = power2;
                    powerImage.color = Color.white;
                    break;
                case 3:
                    powerImage.sprite = power3;
                    powerImage.color = Color.white;
                    break;
                case 4:
                    powerImage.sprite = power4;
                    powerImage.color = Color.white;
                    break;
                case 5:
                    powerImage.sprite = power5;
                    powerImage.color = Color.white;
                    break;
                default:
                    powerImage.sprite = power1;
                    powerImage.color = Color.white;
                    break;
            }
        }

        private int RefreshControllerBattery(PXR_Input.Controller hand)
        {
            var curBattery = 0f;
            switch (hand)
            {
                case PXR_Input.Controller.LeftController:
                {
                    InputDevices.GetDeviceAtXRNode(XRNode.LeftHand).TryGetFeatureValue(CommonUsages.batteryLevel, out curBattery);
                }
                    break;
                case PXR_Input.Controller.RightController:
                {
                    InputDevices.GetDeviceAtXRNode(XRNode.RightHand).TryGetFeatureValue(CommonUsages.batteryLevel, out curBattery);
                }
                    break;
            }
            return (int)curBattery;
        }
    }
}

