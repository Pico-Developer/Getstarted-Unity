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

#if UNITY_EDITOR_WIN && UNITY_ANDROID

using UnityEngine;
using UnityEditor;

public class PXR_SceneQuickPreviewEW : EditorWindow
{
    private static GUIStyle logStyle;
    private static string log;
    private bool forceRestart = false;

    public enum LogType
    {
        Normal,
        Success,
        Error
    }

    [MenuItem("PXR_SDK/Build Tool/Scene Quick Preview", false, 10)]
    static void OpenSceneQuickPreviewUI()
    {
        GetWindow<PXR_SceneQuickPreviewEW>();
        PXR_ADBTool.GetInstance().PicoDeviceReady();
        EditorBuildSettings.sceneListChanged += PXR_BuildToolManager.GetScenesEnabled;
    }

    private void OnEnable()
    {
        PXR_BuildToolManager.GetScenesEnabled();
        EditorBuildSettings.sceneListChanged += PXR_BuildToolManager.GetScenesEnabled;
    } 

    private void OnDestroy()
    {
        PXR_BuildToolManager.DeleteCachePreviewIndexFile();
        log = null;
    }

    private void OnGUI()
    {
        this.titleContent.text = "Scene Quick Preview";
        InitLogUI();
        GUILayout.Space(10.0f);
        GUILayout.Label("Scenes", EditorStyles.boldLabel);
        GUIContent selectScenesBtn = new GUIContent("Select Scenes");

        foreach (PXR_BuildToolManager.SceneData scene in PXR_BuildToolManager.sceneDataList)
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField(scene.sceneName, GUILayout.ExpandWidth(true));
                GUILayout.FlexibleSpace();
            }
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.BeginHorizontal();
        {
            GUIContent sceneBtnTxt = new GUIContent("Build Scene(s)");
            var sceneBtnRt = GUILayoutUtility.GetRect(sceneBtnTxt, GUI.skin.button, GUILayout.Width(200));
            if (GUI.Button(sceneBtnRt, sceneBtnTxt))
            {
                PXR_BuildToolManager.BuildScenes(forceRestart);
                GUIUtility.ExitGUI();
            }
            GUIContent forceRestartLabel = new GUIContent("Force Restart [?]", "Restart the app after scene bundles are finished deploying.");
            forceRestart = GUILayout.Toggle(forceRestart, forceRestartLabel, GUILayout.Width(200));

            var buildSettingBtnRt = GUILayoutUtility.GetRect(selectScenesBtn, GUI.skin.button, GUILayout.Width(200));
            if (GUI.Button(buildSettingBtnRt, selectScenesBtn))
            {
                GetWindow(System.Type.GetType("UnityEditor.BuildPlayerWindow,UnityEditor"));
            }
        }
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(10.0f);
        GUILayout.Label("Utilities", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        {
            GUIContent restartBtnTxt = new GUIContent("Restart App");
            var restartBtnRt = GUILayoutUtility.GetRect(restartBtnTxt, GUI.skin.button, GUILayout.Width(200));
            if (GUI.Button(restartBtnRt, restartBtnTxt))
            {
                if (!PXR_BuildToolManager.IsInstalledApp())
                {
                    PrintLog("App is not install.", LogType.Error);
                    return;
                }

                PXR_BuildToolManager.RestartApp();
            }

            GUIContent uninstallTxt = new GUIContent("Uninstall APP");
            var uninstallBtnRt = GUILayoutUtility.GetRect(uninstallTxt, GUI.skin.button, GUILayout.Width(200));
            if (GUI.Button(uninstallBtnRt, uninstallTxt))
            {
                PXR_BuildToolManager.UninstallApp();
                PXR_BuildToolManager.IsInstalledApp();
            }

            GUIContent deleteCacheBundlesTxt = new GUIContent("Delete Cache Bundles");
            var deleteCacheBundlesBtnRt = GUILayoutUtility.GetRect(deleteCacheBundlesTxt, GUI.skin.button, GUILayout.Width(200));
            if (GUI.Button(deleteCacheBundlesBtnRt, deleteCacheBundlesTxt))
            {
                PXR_BuildToolManager.DeleteCacheBundles();
            }
        }
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(10.0f);
        GUILayout.Label("Log", EditorStyles.boldLabel);

        if (!string.IsNullOrEmpty(log))
        {
            GUILayout.Label(log, logStyle, GUILayout.Height(30.0f));
        }
    }

    private static void InitLogUI()
    {
        if (null != logStyle)
        {
            return;
        }

        logStyle = new GUIStyle();
        logStyle.margin.left = 5;
        logStyle.wordWrap = true;
        logStyle.richText = true;
    }

    public static void PrintLog(string message, LogType type)
    {
        switch (type)
        {
            case LogType.Normal:
                {
                    log = message;
                    break;
                }
            case LogType.Success:
                {
                    log = "<color=green>Success!   " + message + "</color>\n";
                    break;
                }
            case LogType.Error:
                {
                    log = "<color=red>Failed!   " + message + "</color>\n";
                    break;
                }
        }
    }
}
#endif