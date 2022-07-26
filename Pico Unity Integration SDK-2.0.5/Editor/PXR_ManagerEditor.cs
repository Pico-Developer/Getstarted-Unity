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
using System.IO;
using System.Linq;
using System.Xml;
using Unity.XR.PXR;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Unity.XR.PXR.Editor
{
    [CustomEditor(typeof(PXR_Manager))]
    public class PXR_ManagerEditor : UnityEditor.Editor
    {
        private GameObject fpsObject = null;

        public override void OnInspectorGUI()
        {
            GUI.changed = false;
            DrawDefaultInspector();

            PXR_Manager manager = (PXR_Manager)target;
            PXR_ProjectSetting projectConfig = PXR_ProjectSetting.GetProjectConfig();

            if (Camera.main != null)
            {
                if (!Camera.main.transform.Find("FPS"))
                {
                    fpsObject = Instantiate(Resources.Load<GameObject>("Prefabs/FPS"), Camera.main.transform, false);
                    fpsObject.name = "FPS";
                    fpsObject.SetActive(false);
                }
                else
                {
                    fpsObject = Camera.main.transform.Find("FPS").gameObject;
                }
            }

            //Screen Fade
            manager.screenFade = EditorGUILayout.Toggle("Open Screen Fade", manager.screenFade);
            if (Camera.main != null)
            {
                var head = Camera.main.transform;
                if (head)
                {
                    var fade = head.GetComponent<PXR_ScreenFade>();
                    if (manager.screenFade)
                    {
                        if (!fade)
                        {
                            head.gameObject.AddComponent<PXR_ScreenFade>();
                            Selection.activeObject = head;
                        }
                    }
                    else
                    {
                        if (fade) DestroyImmediate(fade);
                    }
                }
            }
            //ffr
            manager.foveationLevel = (FoveationLevel)EditorGUILayout.EnumPopup("Foveation Level", manager.foveationLevel);

            //eye tracking
            GUIStyle firstLevelStyle = new GUIStyle(GUI.skin.label);
            firstLevelStyle.alignment = TextAnchor.UpperLeft;
            firstLevelStyle.fontStyle = FontStyle.Bold;
            firstLevelStyle.fontSize = 12;
            firstLevelStyle.wordWrap = true;
            var guiContent = new GUIContent();
            guiContent.text = "Eye Tracking";
            guiContent.tooltip = "Before calling EyeTracking API, enable this option first, only for Neo3 Pro device.";
            manager.eyeTracking = EditorGUILayout.Toggle(guiContent, manager.eyeTracking);
            if (manager.eyeTracking)
            {
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.LabelField("Note:", firstLevelStyle);
                EditorGUILayout.LabelField("EyeTracking is supported only on the Neo3 Pro");
                EditorGUILayout.EndVertical();
            }

            // content protect
            projectConfig.useContentProtect = EditorGUILayout.Toggle("Use Content Protect", projectConfig.useContentProtect);

            // msaa
            if (QualitySettings.renderPipeline != null)
            {
                EditorGUI.BeginDisabledGroup(true);
                manager.useRecommendedAntiAliasingLevel = EditorGUILayout.Toggle("Use Recommended MSAA", manager.useRecommendedAntiAliasingLevel);
                EditorGUI.EndDisabledGroup();
                EditorGUILayout.HelpBox("A Scriptable Render Pipeline is in use,the 'Use Recommended MSAA' will not be used. ", MessageType.Info,true);
            }
            else
            {
                manager.useRecommendedAntiAliasingLevel = EditorGUILayout.Toggle("Use Recommended MSAA", manager.useRecommendedAntiAliasingLevel);
            }

            if (GUI.changed)
            {
                EditorUtility.SetDirty(projectConfig);
                EditorUtility.SetDirty(manager);
            }
            serializedObject.ApplyModifiedProperties();
        }
        
    }
}


