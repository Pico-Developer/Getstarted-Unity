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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
// using CodiceApp.EventTracking;
using UnityEditor;
using UnityEditor.Android;
using Debug = UnityEngine.Debug;

public class PXR_ADBTool
{
    private static bool adbToolReady;
    private string androidSdkRootPath;
    private string adbToolPath;

    private static PXR_ADBTool instance = null;
    private static readonly object obj = new object();

    public static PXR_ADBTool GetInstance()
    {
        if (instance != null) return instance;
        lock (obj)
        {
            if (instance == null)
            {
                instance = new PXR_ADBTool();
                return instance;
            }
            return instance;
        }

    }

    public PXR_ADBTool()
    {
        androidSdkRootPath = AndroidExternalToolsSettings.sdkRootPath;
        if (androidSdkRootPath.EndsWith("\\") || androidSdkRootPath.EndsWith("/"))
        {
            androidSdkRootPath = androidSdkRootPath.Remove(androidSdkRootPath.Length - 1);
        }
        adbToolPath = Path.Combine(androidSdkRootPath, "platform-tools\\adb.exe");
        adbToolReady = File.Exists(adbToolPath);
    }

    public bool AdbToolReady()
    {
        if (!adbToolReady)
        {
            Debug.LogError("PXRLog Failed to initialize ADB Tool");
        }
        return adbToolReady;
    }

    public bool PicoDeviceReady()
    {
        if (!GetInstance().AdbToolReady())
        {
            return false;
        }

        string outputStr = "";
        string errorStr = "";
        RunProcess(new string[] { "devices" }, out outputStr, out errorStr);
        string[] devices = outputStr.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

        List<string> deviceList = new List<string>(devices);
        deviceList.RemoveAt(0);

        switch (deviceList.Count)
        {
            case 0:
                Debug.LogError("PXRLog Device not connected.");
                return false;
            case 1:
                return true;
            default:
                Debug.LogError("PXRLog Multiple devices connected.");
                return false;
        }
    }

    public bool RunProcess(string[] arguments, out string outputString, out string errorString)
    {
        if (!adbToolReady)
        {
            outputString = string.Empty;
            errorString = "PXR adb Tool not ready.";
            return false;
        }

        string args = string.Join(" ", arguments);

        Process process = new Process();
        process.StartInfo.FileName = adbToolPath;
        process.StartInfo.Arguments = args;
        process.StartInfo.WorkingDirectory = androidSdkRootPath;
        process.StartInfo.CreateNoWindow = true;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
        process.StartInfo.RedirectStandardInput = true;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;
        process.Start();

        StringBuilder outputStr = new StringBuilder("");
        StringBuilder errorStr = new StringBuilder("");

        process.OutputDataReceived += (_, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                outputStr.Append(e.Data);
                outputStr.Append(Environment.NewLine);
            }
        };
        process.ErrorDataReceived += (_, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                errorStr.Append(e.Data);
                errorStr.Append(Environment.NewLine);
            }
        };

        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        process.WaitForExit();
        int exitCode = process.ExitCode;
        process.Close();

        outputString = outputStr.ToString();
        errorString = errorStr.ToString();

        outputStr = null;
        errorStr = null;

        if (!string.IsNullOrEmpty(errorString))
        {
            if (errorString.Contains("Warning"))
            {
                Debug.LogWarning("PXRLog Adb tool " + errorString);
            }
            else
            {
                Debug.LogError("PXRLog Adb tool " + errorString);
            }
        }

        return exitCode == 0;
    }

    public bool RunCmd(string[] arguments, string fileName, string workingDirectory, DataReceivedEventHandler outputEvent, DataReceivedEventHandler errorEvent)
    {
        if (!adbToolReady)
        {
            Debug.LogWarning("PXRLog Adb tool not ready.");
            return false;
        }

        Process process = new Process();
        string args = string.Join(" ", arguments);
        process.StartInfo.FileName = fileName;
        process.StartInfo.Arguments = args;
        process.StartInfo.WorkingDirectory = workingDirectory;
        process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
        process.StartInfo.CreateNoWindow = true;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardError = true;
        process.StartInfo.RedirectStandardOutput = true;
        process.EnableRaisingEvents = true;
        process.OutputDataReceived += outputEvent;
        process.ErrorDataReceived += errorEvent;
        process.Start();
        process.BeginOutputReadLine();
        process.WaitForExit();

        return process.ExitCode == 0;
    }

    public string GetGradlePath()
    {
        string libPath = Path.Combine(AndroidExternalToolsSettings.gradlePath, "lib");
        if (!string.IsNullOrEmpty(libPath) && Directory.Exists(libPath))
        {
            string[] gradleJar = Directory.GetFiles(libPath, "gradle-launcher-*.jar");
            if (gradleJar.Length == 1)
            {
                if (File.Exists(gradleJar[0]))
                {
                    return gradleJar[0];
                }
            }
        }

        EditorUtility.DisplayDialog("Gradle not Found",
                "Please ensure that the path is set correctly in External Tools.",
                "Ok");
        return string.Empty;
    }

    public string GetJDKPath()
    {
        string jdkPath = Path.Combine(AndroidExternalToolsSettings.jdkRootPath, "bin");
        jdkPath = Path.Combine(jdkPath, "java.exe");

        if (File.Exists(jdkPath))
        {
            return jdkPath;
        }

        string javaHome = Environment.GetEnvironmentVariable("JAVA_HOME");
        if (!string.IsNullOrEmpty(javaHome))
        {
            jdkPath = Path.Combine(javaHome, "bin\\java.exe");
            if (File.Exists(jdkPath))
            {
                return jdkPath;
            }
        }

        EditorUtility.DisplayDialog("JDK not Found",
            "Please ensure that the path is set correctly in External Tools.",
            "Ok");

        return string.Empty;
    }

}
