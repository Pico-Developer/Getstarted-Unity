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
using System.Diagnostics;
using System.IO;
using System.Threading;
using UnityEditor;
using UnityEngine;

public class PXR_BuildAndRunEW : EditorWindow
{
#if UNITY_EDITOR && UNITY_ANDROID

    static string gradleExport;

    [MenuItem("PXR_SDK/Build Tool/Build And Run")]
    static void BuildAndRun()
    {
        if (!PXR_ADBTool.GetInstance().PicoDeviceReady())
        {
            return;
        }

        string tempPath = Path.Combine(Path.Combine(Application.dataPath, "../Temp"), "PXRGradleTempExport");
        gradleExport = Path.Combine(Path.Combine(Application.dataPath, "../Temp"), "PXRGradleExport");
        if (!Directory.Exists(gradleExport))
        {
            Directory.CreateDirectory(gradleExport);
        }

#if UNITY_2020_1_OR_NEWER || UNITY_2019_4_OR_NEWER
        EditorUserBuildSettings.exportAsGoogleAndroidProject = true;
#endif
        PXR_BuildToolManager.GetScenesEnabled();

        var buildOptions = BuildOptions.None;
        if (EditorUserBuildSettings.development)
            buildOptions |= (BuildOptions.Development | BuildOptions.AllowDebugging);
#if !UNITY_2020_1_OR_NEWER && !UNITY_2019_4_OR_NEWER
        buildOptions |= BuildOptions.AcceptExternalModificationsToPlayer;
#endif

        var options = new BuildPlayerOptions
        {
            scenes = PXR_BuildToolManager.sceneNameList.ToArray(),
            locationPathName = tempPath,
            target = BuildTarget.Android,
            options = buildOptions
        };

        UnityEditor.Build.Reporting.BuildReport buildResult = BuildPipeline.BuildPlayer(options);

        if (UnityEditor.Build.Reporting.BuildResult.Succeeded == buildResult.summary.result)
        {
            try
            {
                var ps = System.Text.RegularExpressions.Regex.Escape("" + Path.DirectorySeparatorChar);
                var syncer = new PXR_DirectorySyncer(tempPath, gradleExport, string.Format("^([^{0}]+{0})?(\\.gradle|build){0}", ps));
                PXR_DirectorySyncer.CancellationTokenSource cancel = new PXR_DirectorySyncer.CancellationTokenSource();
                if (cancel.isCancellationRequested)
                {
                    return;
                }

                syncer.Synchronize(cancel.token);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.Log("PXRLog Processing gradle project failed with exception: " + e.Message);
                return;
            }

            if (!BuildProject())
            {
                return;
            }

            InstallApp();
        }
    }

    private static bool BuildProject()
    {
        bool? success = null;

        string[] pushCmd = { "-Xmx4096m -classpath \"" + PXR_ADBTool.GetInstance().GetGradlePath() +
            "\" org.gradle.launcher.GradleMain assembleDebug -x validateSigningDebug --profile", "" };
        PXR_ADBTool.GetInstance().RunCmd(pushCmd, PXR_ADBTool.GetInstance().GetJDKPath(), gradleExport, (s, e) =>
         {
             if (e != null && e.Data != null && e.Data.Length != 0 && (e.Data.Contains("BUILD") || e.Data.StartsWith("See the profiling report at:")))
             {
                 UnityEngine.Debug.Log("PXRLog Gradle: " + e.Data);
                 if (e.Data.Contains("SUCCESSFUL"))
                 {
                     UnityEngine.Debug.Log("PXRLog APK Build Completed: " +
                         Path.Combine(Path.Combine(gradleExport, "build\\outputs\\apk\\debug"), "launcher-debug.apk").Replace("/", "\\"));
                     if (!success.HasValue)
                     {
                         success = true;
                     }
                 }
                 else if (e.Data.Contains("FAILED"))
                 {
                     success = false;
                 }
             }
         }, (s, e) =>
         {
             if (e != null && e.Data != null && e.Data.Length != 0)
             {
                 UnityEngine.Debug.LogError("Gradle: " + e.Data);
             }
             success = false;
         });

        Stopwatch timeout = new Stopwatch();
        timeout.Start();
        while (null == success)
        {
            if (timeout.ElapsedMilliseconds > 4900)
            {
                UnityEngine.Debug.LogError("PXRLog Gradle has exited unexpectedly.");
                success = false;
            }
            Thread.Sleep(100);
        }

        return success.HasValue && success.Value;
    }

    public static void InstallApp()
    {
        if (!PXR_ADBTool.GetInstance().AdbToolReady())
        {
            return;
        }

        string gradleFolder = Path.Combine(Path.Combine(gradleExport, "launcher"), "build\\outputs\\apk\\debug");
        gradleFolder = gradleFolder.Replace("/", "\\");
        if (!Directory.Exists(gradleFolder))
        {
            return;
        }

        var apkPathLocal = Path.Combine(gradleFolder, "launcher-debug.apk");
        if (!File.Exists(apkPathLocal))
        {
            return;
        }

        string output, error;

        string[] mkdirCmd = { "-d shell", "mkdir -p", "/data/local/tmp" };
        if (!PXR_ADBTool.GetInstance().RunProcess(mkdirCmd, out output, out error)) return;

        string[] pushCmd = { "-d push", "\"" + apkPathLocal + "\"", "/data/local/tmp" };
        if (!PXR_ADBTool.GetInstance().RunProcess(pushCmd, out output, out error)) return;

        string apkPath = "/data/local/tmp/launcher-debug.apk";
        apkPath = apkPath.Replace(" ", "\\ ");
        string[] installCmd = { "-d shell", "pm install -r", apkPath };
        if (!PXR_ADBTool.GetInstance().RunProcess(installCmd, out output, out error)) return;

        string[] appStartCmd = { "-d shell", "am start -a android.intent.action.MAIN -c android.intent.category.LAUNCHER -S -f 0x10200000 -n", "\"" + PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.Android) + "/com.unity3d.player.UnityPlayerActivity\"" };
        if (!PXR_ADBTool.GetInstance().RunProcess(appStartCmd, out output, out error)) return;

        UnityEngine.Debug.Log("PXRLog Application Start Success");
    }

#endif
}
#endif