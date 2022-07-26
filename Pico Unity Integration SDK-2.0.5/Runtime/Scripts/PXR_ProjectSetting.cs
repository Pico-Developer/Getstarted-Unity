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

using System.IO;
using UnityEngine;

namespace Unity.XR.PXR
{
    [System.Serializable]
    public class PXR_ProjectSetting: ScriptableObject
    {
        public bool useContentProtect;
        public static PXR_ProjectSetting GetProjectConfig()
        {
            PXR_ProjectSetting projectConfig = Resources.Load<PXR_ProjectSetting>("PXR_ProjectSetting");
#if UNITY_EDITOR
            if (projectConfig == null)
            {
                projectConfig = CreateInstance<PXR_ProjectSetting>();
                projectConfig.useContentProtect = false;
                string path = Application.dataPath + "/Resources";
                if (!Directory.Exists(path))
                {
                    UnityEditor.AssetDatabase.CreateFolder("Assets", "Resources");
                    UnityEditor.AssetDatabase.CreateAsset(projectConfig, "Assets/Resources/PXR_ProjectSetting.asset");
                }
                else
                {
                    UnityEditor.AssetDatabase.CreateAsset(projectConfig, "Assets/Resources/PXR_ProjectSetting.asset");
                }
            }
#endif
            return projectConfig;
        }
    }
}
