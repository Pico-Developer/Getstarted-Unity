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

namespace Unity.XR.PXR
{
    public class PXR_ControllerKeyTips : MonoBehaviour
    {
        private float tipsAlpha;
        public static PXR_ControllerKeyTips toolTips;

        private void Awake()
        {
            toolTips = transform.GetComponent<PXR_ControllerKeyTips>();
        }

        void Update()
        {
            tipsAlpha = (330 - transform.parent.parent.parent.parent.localRotation.eulerAngles.x) / 45.0f;
            if (transform.parent.parent.parent.parent.localRotation.eulerAngles.x >= 270 &&
                transform.parent.parent.parent.parent.localRotation.eulerAngles.x <= 330)
            {
                tipsAlpha = Mathf.Max(0.0f, tipsAlpha);
                tipsAlpha = tipsAlpha > 1.0f ? 1.0f : tipsAlpha;
            }
            else
            {
                tipsAlpha = 0.0f;
            }
            GetComponent<CanvasGroup>().alpha = tipsAlpha;
        }

        private void LoadTextFromJson()
        {
            transform.Find("app/btn/Text").GetComponent<Text>().text = PXR_Plugin.System.UPxr_GetLangString("apptip");
            transform.Find("touch/btn/Text").GetComponent<Text>().text = PXR_Plugin.System.UPxr_GetLangString("touchtip");
            transform.Find("home/btn/Text").GetComponent<Text>().text = PXR_Plugin.System.UPxr_GetLangString("hometip");

            var volUp = transform.Find("volup/btn/Text");
            if (volUp != null)
                volUp.GetComponent<Text>().text = PXR_Plugin.System.UPxr_GetLangString("voluptip");
            var volDown = transform.Find("voldown/btn/Text");
            if (volDown != null)
                volDown.GetComponent<Text>().text = PXR_Plugin.System.UPxr_GetLangString("voldowntip");
            var trigTip = transform.Find("trigger/btn/Text");
            if (trigTip != null)
                trigTip.GetComponent<Text>().text = PXR_Plugin.System.UPxr_GetLangString("triggertip");
            var gripTip = transform.Find("grip/btn/Text");
            if (gripTip != null)
                gripTip.GetComponent<Text>().text = PXR_Plugin.System.UPxr_GetLangString("griptip");
        }

        public static void RefreshTips()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if(toolTips != null)
                toolTips.LoadTextFromJson();
#endif
        }
    }
}

