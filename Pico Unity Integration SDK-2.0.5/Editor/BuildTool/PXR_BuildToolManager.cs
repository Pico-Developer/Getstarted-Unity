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
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PXR_BuildToolManager
{
    // Scene Quick Preview
    private const string SQP_APK_VERSION = "PxrSQPAPKVersion";
    private const string SQP_BUNDLE_PATH = "PxrSQPBundles";
    private const string SQP_APK_NAME = "PxrSQP.apk";

    public static List<string> sceneNameList = new List<string>();
    public static List<SceneData> sceneDataList = new List<SceneData>();

    private static string previewIP;

    public class SceneData
    {
        public string scenePath;
        public string sceneName;

        public SceneData(string path, string name)
        {
            scenePath = path;
            sceneName = name;
        }
    }

    public static void BuildScenes(bool forceRestart)
    {
        if (!PXR_ADBTool.GetInstance().AdbToolReady())
        {
            return;
        }

        GetScenesEnabled();

        string localPath = Path.Combine(Application.dataPath, "..", SQP_BUNDLE_PATH);
        string androidPath = Path.Combine(PXR_SQPLoader.ANDROID_DATA_PATH, PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.Android), PXR_SQPLoader.CACHE_SCENES_PATH).Replace("\\", "/");

        Dictionary<string, string> assetSB = new Dictionary<string, string>();
        List<AssetBundleBuild> assetList = new List<AssetBundleBuild>();
        Dictionary<string, List<string>> toAssetList = new Dictionary<string, List<string>>();

        foreach (var scene in sceneDataList)
        {
            foreach (string asset in AssetDatabase.GetDependencies(scene.scenePath))
            {
                string ext = Path.GetExtension(asset);
                if (string.IsNullOrEmpty(ext))
                {
                    continue;
                }
                ext = ext.Substring(1);
                if (ext.Equals("cs") || ext.Equals("unity"))
                {
                    continue;
                }
                if (assetSB.ContainsKey(asset))
                {
                    continue;
                }

                var assetObj = AssetDatabase.LoadAssetAtPath(asset, typeof(UnityEngine.Object));
                if (null != assetObj && 0 != (assetObj.hideFlags & HideFlags.DontSaveInBuild))
                {
                    continue;
                }

                assetSB[asset] = scene.sceneName;
                if ("resources" == scene.sceneName)
                {
                    continue;
                }

                if (!toAssetList.ContainsKey(ext))
                {
                    toAssetList[ext] = new List<string>();
                }
                toAssetList[ext].Add(asset);
            }

            assetList.Add(new AssetBundleBuild()
            {
                assetBundleName = PXR_SQPLoader.SCENE_BUNDLE_NAME + scene.sceneName,
                assetNames = new string[1] { scene.scenePath }
            });
        }

        BuildPipeline.BuildAssetBundles(PXR_DirectorySyncer.CreateDirectory(SQP_BUNDLE_PATH), assetList.ToArray(), BuildAssetBundleOptions.UncompressedAssetBundle, BuildTarget.Android);

        string output, error;
        string[] mkdirCmd = { "-d shell", "mkdir -p", "\"" + androidPath + "\"" };
        if (PXR_ADBTool.GetInstance().RunProcess(mkdirCmd, out output, out error))
        {
            string[] assetBundlePaths = Directory.GetFiles(localPath);
            if (assetBundlePaths.Length == 0)
            {
                PXR_SceneQuickPreviewEW.PrintLog("Failed to locate scene bundles to transfer.", PXR_SceneQuickPreviewEW.LogType.Error);
                return;
            }

            foreach (string path in assetBundlePaths)
            {
                if (!path.Contains(".manifest"))
                {
                    string[] bundleCmd = { "-d push", "\"" + Path.Combine(Application.dataPath, "..", path) + "\"", "\"" + androidPath + "\"" };
                    PXR_ADBTool.GetInstance().RunProcess(bundleCmd, out output, out error);
                }
            }
        }

        string loadDataPath = Path.Combine(localPath, PXR_SQPLoader.SCENE_LOAD_DATA_NAME);

        if (File.Exists(loadDataPath))
        {
            File.Delete(loadDataPath);
        }

        StreamWriter writer = new StreamWriter(loadDataPath, true);
        writer.WriteLine(DateTime.Now.Ticks.ToString());
        for (int i = 0; i < sceneDataList.Count; i++)
        {
            writer.WriteLine(Path.GetFileNameWithoutExtension(sceneDataList[i].scenePath));
        }
        writer.Close();

        string[] pushCmd = { "-d push", "\"" + loadDataPath + "\"", "\"" + androidPath + "\"" };
        if (!PXR_ADBTool.GetInstance().RunProcess(pushCmd, out output, out error))
        {
            PXR_SceneQuickPreviewEW.PrintLog(pushCmd[1], PXR_SceneQuickPreviewEW.LogType.Error);
            return;
        }

        if (!IsInstalledApp())
        {
            InstallApp();
        }


        if (forceRestart)
        {
            RestartApp();
            return;
        }

        PXR_SceneQuickPreviewEW.PrintLog("Build Scenes Success!", PXR_SceneQuickPreviewEW.LogType.Success);
    }

    public static bool IsInstalledApp()
    {
        if (!PXR_ADBTool.GetInstance().AdbToolReady())
        {
            return false;
        }

        string output, error;
        var packageName = PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.Android);

        string[] packageCheckCmd = new string[] { "-d shell pm list package", packageName };
        if (PXR_ADBTool.GetInstance().RunProcess(packageCheckCmd, out output, out error))
        {
            if (string.IsNullOrEmpty(output))
            {
                return false;
            }

            if (!output.Contains("package:" + packageName + "\r\n"))
            {
                return false;
            }

            string[] packageCmd = new string[] { "-d shell dumpsys package", packageName };
            string packageInfo;
            if (PXR_ADBTool.GetInstance().RunProcess(packageCmd, out packageInfo, out error) &&
                    !string.IsNullOrEmpty(packageInfo) &&
                    packageInfo.Contains(SQP_APK_VERSION))
            {
                return true;
            }
        }
        return false;
    }

    public static bool RestartApp()
    {
        if (!PXR_ADBTool.GetInstance().AdbToolReady())
        {
            return false;
        }

        string output, error;
        string[] restartCmd = { "-d shell", "am start -a android.intent.action.MAIN -c android.intent.category.LAUNCHER -S -f 0x10200000 -n", "\"" + PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.Android) + "/com.unity3d.player.UnityPlayerActivity\"" };
        if (PXR_ADBTool.GetInstance().RunProcess(restartCmd, out output, out error))
        {
            PXR_SceneQuickPreviewEW.PrintLog("App Restart Success!", PXR_SceneQuickPreviewEW.LogType.Success);
            return true;
        }

        PXR_SceneQuickPreviewEW.PrintLog("App Restart Failed! " + (string.IsNullOrEmpty(error) ? output : error), PXR_SceneQuickPreviewEW.LogType.Error);

        return false;
    }

    public static bool UninstallApp()
    {
        if (!PXR_ADBTool.GetInstance().AdbToolReady())
        {
            return false;
        }

        string output, error;
        string[] appStartCmd = { "-d shell", "pm uninstall", PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.Android) };
        if (PXR_ADBTool.GetInstance().RunProcess(appStartCmd, out output, out error))
        {
            PXR_SceneQuickPreviewEW.PrintLog("App uninstall Success!  ---" + PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.Android), PXR_SceneQuickPreviewEW.LogType.Success);
            return true;
        }

        PXR_SceneQuickPreviewEW.PrintLog("App uninstall Failed!  ---" + PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.Android), PXR_SceneQuickPreviewEW.LogType.Error);
        return false;
    }

    public static void DeleteCacheBundles()
    {
        try
        {
            if (Directory.Exists(SQP_BUNDLE_PATH))
            {
                Directory.Delete(SQP_BUNDLE_PATH, true);
            }
            DeleteCachePreviewIndexFile();
        }
        catch (Exception e)
        {
            PXR_SceneQuickPreviewEW.PrintLog(e.Message, PXR_SceneQuickPreviewEW.LogType.Error);
        }
        PXR_SceneQuickPreviewEW.PrintLog("Deleted Cache Bundles Success!", PXR_SceneQuickPreviewEW.LogType.Success);
    }

    public static void DeleteCachePreviewIndexFile()
    {
        try
        {
            if (File.Exists(previewIP))
            {
                File.Delete(previewIP);
                previewIP = null;
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    public static void GetScenesEnabled()
    {
        sceneNameList.Clear();
        sceneDataList.Clear();
        foreach (var scene in EditorBuildSettings.scenes)
        {
            if (scene.enabled)
            {
                if (PXR_SQPLoader.SQP_INDEX_NAME != Path.GetFileName(scene.path))
                {
                    sceneNameList.Add(scene.path);
                    SceneData sceneData = new SceneData(scene.path, Path.GetFileNameWithoutExtension(scene.path));
                    sceneDataList.Add(sceneData);
                }
            }
        }

        if (sceneDataList.Count == 0)
        {
            Scene scene = SceneManager.GetActiveScene();
            SceneData sceneInfo = new SceneData(scene.path, Path.GetFileNameWithoutExtension(scene.path));
            sceneDataList.Add(sceneInfo);
        }
    }

    private static void InstallApp()
    {
        PXR_DirectorySyncer.CreateDirectory(SQP_BUNDLE_PATH);

        AndroidArchitecture targetArc = PlayerSettings.Android.targetArchitectures;
        ScriptingImplementation backend = PlayerSettings.GetScriptingBackend(EditorUserBuildSettings.selectedBuildTargetGroup);
        ManagedStrippingLevel level = PlayerSettings.GetManagedStrippingLevel(BuildTargetGroup.Android);
        bool stripEngineCode = PlayerSettings.stripEngineCode;

        PlayerSettings.bundleVersion = SQP_APK_VERSION;

        if (targetArc != AndroidArchitecture.ARMv7)
        {
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARMv7;
        }

        if (backend != ScriptingImplementation.Mono2x)
        {
            PlayerSettings.SetScriptingBackend(EditorUserBuildSettings.selectedBuildTargetGroup, ScriptingImplementation.Mono2x);
        }

        if (level != ManagedStrippingLevel.Disabled)
        {
            PlayerSettings.SetManagedStrippingLevel(BuildTargetGroup.Android, ManagedStrippingLevel.Disabled);
        }

        if (stripEngineCode)
        {
            PlayerSettings.stripEngineCode = false;
        }

        if (string.IsNullOrEmpty(previewIP) || !File.Exists(previewIP))
        {
            string[] editorSP = Directory.GetFiles(Path.GetFullPath("Packages/com.unity.xr.picoxr/"), PXR_SQPLoader.SQP_INDEX_NAME, SearchOption.AllDirectories);

            if (editorSP.Length == 0 || editorSP.Length > 1)
            {
                PXR_SceneQuickPreviewEW.PrintLog(editorSP.Length + " " + PXR_SQPLoader.SQP_INDEX_NAME + " has been found, please double check your PicoXR Plugin import.", PXR_SceneQuickPreviewEW.LogType.Error);
                return;
            }
            previewIP = Path.Combine(Application.dataPath, PXR_SQPLoader.SQP_INDEX_NAME);
            if (File.Exists(previewIP))
            {
                File.Delete(previewIP);
            }
            File.Copy(editorSP[0], previewIP);
        }

        string apkPN = Path.Combine(SQP_BUNDLE_PATH, SQP_APK_NAME);

        if (File.Exists(apkPN))
        {
            File.Delete(apkPN);
        }

        BuildReport report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
        {
            scenes = new string[1] { previewIP },
            locationPathName = apkPN,
            target = BuildTarget.Android,
            options = BuildOptions.Development |
                BuildOptions.AutoRunPlayer
        });
        if (report.summary.result == BuildResult.Succeeded)
        {
            PXR_SceneQuickPreviewEW.PrintLog("App install Success!", PXR_SceneQuickPreviewEW.LogType.Success);
        }
        else if (report.summary.result == BuildResult.Failed)
        {
            PXR_SceneQuickPreviewEW.PrintLog("App install Failed!", PXR_SceneQuickPreviewEW.LogType.Error);
        }

        if (PlayerSettings.GetScriptingBackend(EditorUserBuildSettings.selectedBuildTargetGroup) != backend)
        {
            PlayerSettings.SetScriptingBackend(EditorUserBuildSettings.selectedBuildTargetGroup, backend);
        }

        if (PlayerSettings.GetManagedStrippingLevel(BuildTargetGroup.Android) != level)
        {
            PlayerSettings.SetManagedStrippingLevel(BuildTargetGroup.Android, level);
        }

        if (PlayerSettings.stripEngineCode != stripEngineCode)
        {
            PlayerSettings.stripEngineCode = stripEngineCode;
        }

        if (PlayerSettings.Android.targetArchitectures != targetArc)
        {
            PlayerSettings.Android.targetArchitectures = targetArc;
        }
    }
}
#endif