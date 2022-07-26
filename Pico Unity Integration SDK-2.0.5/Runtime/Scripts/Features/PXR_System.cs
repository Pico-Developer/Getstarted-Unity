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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.XR.PXR
{
    public class PXR_System
    {
        /// <summary>
        /// Turn on power service.
        /// </summary>
        /// <param name="objName"></param>
        /// <returns></returns>
        public static bool StartBatteryReceiver(string objName)
        {
            return PXR_Plugin.System.UPxr_StartBatteryReceiver(objName);
        }

        /// <summary>
        /// Turn off power service.
        /// </summary>
        /// <returns></returns>
        public static bool StopBatteryReceiver()
        {
            return PXR_Plugin.System.UPxr_StopBatteryReceiver();
        }

        /// <summary>
        /// Set the brightness value of the current general device.
        /// </summary>
        /// <param name="brightness">brightness value range is 0-255</param>
        /// <returns></returns>
        public static bool SetCommonBrightness(int brightness)
        {
            return PXR_Plugin.System.UPxr_SetBrightness(brightness);
        }

        /// <summary>
        /// Get the brightness value of the current general device.
        /// </summary>
        /// <returns>brightness value range: 0-255</returns>
        public static int GetCommonBrightness()
        {
            return PXR_Plugin.System.UPxr_GetCurrentBrightness();
        }

        /// <summary>
        /// Gets the brightness level of the current screen.
        /// </summary>
        /// <returns>int array. The first bit is the total brightness level supported, the second bit is the current brightness level, and it is the interval value of the brightness level from the third bit to the end bit.</returns>
        public static int[] GetScreenBrightnessLevel()
        {
            int[] currentLight = { 0 };
            currentLight = PXR_Plugin.System.UPxr_GetScreenBrightnessLevel();
            return currentLight;
        }

        /// <summary>
        /// Set the brightness of the screen.
        /// </summary>
        /// <param name="brightness">Brightness mode</param>
        /// <param name="level">Brightness value (brightness level value). If brightness passes in 1, level passes in brightness level; if brightness passes in 0, it means that the system default brightness setting mode is adopted. Level can be set to a value between 1 and 255.</param>
        public static void SetScreenBrightnessLevel(int brightness, int level)
        {
            PXR_Plugin.System.UPxr_SetScreenBrightnessLevel(brightness, level);
        }

        /// <summary>
        /// Init volume device.
        /// </summary>
        /// <returns></returns>
        public static bool InitAudioDevice()
        {
            return PXR_Plugin.System.UPxr_InitAudioDevice();
        }

        /// <summary>
        /// Turn on volume service.
        /// </summary>
        /// <param name="objName"></param>
        /// <returns></returns>
        public static bool StartAudioReceiver(string objName)
        {
            return PXR_Plugin.System.UPxr_StartAudioReceiver(objName);
        }

        /// <summary>
        /// Turn off volume service.
        /// </summary>
        /// <returns></returns>
        public static bool StopAudioReceiver()
        {
            return PXR_Plugin.System.UPxr_StopAudioReceiver();
        }

        /// <summary>
        /// Get maximum volume.
        /// </summary>
        /// <returns></returns>
        public static int GetMaxVolumeNumber()
        {
            return PXR_Plugin.System.UPxr_GetMaxVolumeNumber();
        }

        /// <summary>
        /// Get the current volume.
        /// </summary>
        /// <returns></returns>
        public static int GetCurrentVolumeNumber()
        {
            return PXR_Plugin.System.UPxr_GetCurrentVolumeNumber();
        }

        /// <summary>
        /// Increase volume.
        /// </summary>
        /// <returns></returns>
        public static bool VolumeUp()
        {
            return PXR_Plugin.System.UPxr_VolumeUp();
        }

        /// <summary>
        /// Decrease volume.
        /// </summary>
        /// <returns></returns>
        public static bool VolumeDown()
        {
            return PXR_Plugin.System.UPxr_VolumeDown();
        }

        /// <summary>
        /// Set volume.
        /// </summary>
        /// <param name="volume"></param>
        /// <returns></returns>
        public static bool SetVolumeNum(int volume)
        {
            return PXR_Plugin.System.UPxr_SetVolumeNum(volume);
        }

        /// <summary>
        /// Judging whether current device’s permission is valid.
        /// </summary>
        /// <returns></returns>
        public static PXR_PlatformSetting.simulationType IsCurrentDeviceValid()
        {
            return PXR_Plugin.PlatformSetting.UPxr_IsCurrentDeviceValid();
        }

        /// <summary>
        /// Use appid to get result whether entitlement required by app is present.
        /// </summary>
        /// <param name="appid"></param>
        /// <returns>value: True: Success; False: Fail</returns>
        public static bool AppEntitlementCheck(string appid)
        {
            return PXR_Plugin.PlatformSetting.UPxr_AppEntitlementCheck(appid);
        }

        /// <summary>
        /// Use publicKey to get error code of entitlement check result.
        /// </summary>
        /// <param name="publicKey"></param>
        /// <returns>value: True: Success; False: Fail</returns>
        public static bool KeyEntitlementCheck(string publicKey)
        {
            return PXR_Plugin.PlatformSetting.UPxr_KeyEntitlementCheck(publicKey);
        }

        /// <summary>
        /// Use appid to get error code of entitlement check result.
        /// </summary>
        /// <param name="appId"></param>
        /// <returns>value: 0:success -1:invalid params -2:service not exist (old versions of ROM have no Service. If the application needs to be limited to operating in old versions, this state needs processing) -3:time out</returns>
        public static int AppEntitlementCheckExtra(string appId)
        {
            return PXR_Plugin.PlatformSetting.UPxr_AppEntitlementCheckExtra(appId);
        }

        /// <summary>
        /// Use publicKey to get error code of entitlement check result.
        /// </summary>
        /// <param name="publicKey"></param>
        /// <returns>value: 0:success -1:invalid params -2:service not exist (old versions of ROM have no Service. If the application needs to be limited to operating in old versions, this state needs processing) -3:time out</returns>
        public static int KeyEntitlementCheckExtra(string publicKey)
        {
            return PXR_Plugin.PlatformSetting.UPxr_KeyEntitlementCheckExtra(publicKey);
        }

        /// <summary>
        /// Get SDK version number.
        /// </summary>
        /// <returns></returns>
        public static string GetSDKVersion()
        {
            return PXR_Plugin.System.UPxr_GetSDKVersion();
        }

        /// <summary>
        /// Get Predicted DisplayTime.
        /// </summary>
        /// <returns></returns>
        public static double GetPredictedDisplayTime()
        {
            return PXR_Plugin.System.UPxr_GetPredictedDisplayTime();
        }

        /// <summary>
        /// Set extra latency mode.
        /// </summary>
        /// <returns></returns>
        public static bool SetExtraLatencyMode(int mode)
        {
            return PXR_Plugin.System.UPxr_SetExtraLatencyMode(mode);
        }

        /// <summary>
        /// Init System Service.
        /// </summary>
        /// <param name="objectName">Receive callback object name</param>
        public static void InitSystemService(string objectName)
        {
            PXR_Plugin.System.UPxr_InitToBService();
            PXR_Plugin.System.UPxr_SetUnityObjectName(objectName);
            PXR_Plugin.System.UPxr_InitAudioDevice();
        }

        /// <summary>
        /// Bind System Service.
        /// </summary>
        public static void BindSystemService()
        {
            PXR_Plugin.System.UPxr_BindSystemService();
        }

        /// <summary>
        /// UnBind System Service.
        /// </summary>
        public static void UnBindSystemService()
        {
            PXR_Plugin.System.UPxr_UnBindSystemService();
        }

        /// <summary>
        /// Get Device's Info.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string StateGetDeviceInfo(SystemInfoEnum type)
        {
            return PXR_Plugin.System.UPxr_StateGetDeviceInfo(type);
        }

        /// <summary>
        /// Set Device's Action.
        /// </summary>
        /// <param name="deviceControl"></param>
        /// <param name="callback"></param>
        public static void ControlSetDeviceAction(DeviceControlEnum deviceControl, Action<int> callback)
        {
            PXR_Plugin.System.UPxr_ControlSetDeviceAction(deviceControl, callback);
        }

        /// <summary>
        /// APP Manager.
        /// </summary>
        /// <param name="packageControl"></param>
        /// <param name="path"></param>
        /// <param name="callback"></param>
        public static void ControlAPPManager(PackageControlEnum packageControl, string path, Action<int> callback)
        {
            PXR_Plugin.System.UPxr_ControlAPPManager(packageControl, path, callback);
        }

        /// <summary>
        /// Set Auto Connect WIFI.
        /// </summary>
        /// <param name="ssid"></param>
        /// <param name="pwd"></param>
        /// <param name="callback"></param>
        public static void ControlSetAutoConnectWIFI(string ssid, string pwd, Action<bool> callback)
        {
            PXR_Plugin.System.UPxr_ControlSetAutoConnectWIFI(ssid, pwd, callback);
        }

        /// <summary>
        /// Clear Auto Connect WIFI.
        /// </summary>
        /// <param name="callback"></param>
        public static void ControlClearAutoConnectWIFI(Action<bool> callback)
        {
            PXR_Plugin.System.UPxr_ControlClearAutoConnectWIFI(callback);
        }

        /// <summary>
        /// Set Home Key Event.
        /// </summary>
        /// <param name="eventEnum"></param>
        /// <param name="function"></param>
        /// <param name="callback"></param>
        public static void PropertySetHomeKey(HomeEventEnum eventEnum, HomeFunctionEnum function, Action<bool> callback)
        {
            PXR_Plugin.System.UPxr_PropertySetHomeKey(eventEnum, function, callback);
        }

        /// <summary>
        /// Set Home Key All Event.
        /// </summary>
        /// <param name="eventEnum"></param>
        /// <param name="function"></param>
        /// <param name="timesetup"></param>
        /// <param name="pkg"></param>
        /// <param name="className"></param>
        /// <param name="callback"></param>
        public static void PropertySetHomeKeyAll(HomeEventEnum eventEnum, HomeFunctionEnum function, int timesetup, string pkg, string className, Action<bool> callback)
        {
            PXR_Plugin.System.UPxr_PropertySetHomeKeyAll(eventEnum, function, timesetup, pkg, className, callback);
        }

        /// <summary>
        /// Disable Power Key.
        /// </summary>
        /// <param name="isSingleTap"></param>
        /// <param name="enable"></param>
        /// <param name="callback"></param>
        public static void PropertyDisablePowerKey(bool isSingleTap, bool enable, Action<int> callback)
        {
            PXR_Plugin.System.UPxr_PropertyDisablePowerKey(isSingleTap, enable, callback);
        }

        /// <summary>
        /// Set ScreenOff Delay Time.
        /// </summary>
        /// <param name="timeEnum"></param>
        /// <param name="callback"></param>
        public static void PropertySetScreenOffDelay(ScreenOffDelayTimeEnum timeEnum, Action<int> callback)
        {
            PXR_Plugin.System.UPxr_PropertySetScreenOffDelay(timeEnum, callback);
        }

        /// <summary>
        /// Set SleepDelay Time.
        /// </summary>
        /// <param name="timeEnum"></param>
        public static void PropertySetSleepDelay(SleepDelayTimeEnum timeEnum)
        {
            PXR_Plugin.System.UPxr_PropertySetSleepDelay(timeEnum);
        }

        /// <summary>
        /// Switch System Function.
        /// </summary>
        /// <param name="systemFunction"></param>
        /// <param name="switchEnum"></param>
        public static void SwitchSystemFunction(SystemFunctionSwitchEnum systemFunction, SwitchEnum switchEnum)
        {
            PXR_Plugin.System.UPxr_SwitchSystemFunction(systemFunction, switchEnum);
        }

        /// <summary>
        /// Set UsbConfiguration Option.
        /// </summary>
        /// <param name="uSBConfigModeEnum"></param>
        public static void SwitchSetUsbConfigurationOption(USBConfigModeEnum uSBConfigModeEnum)
        {
            PXR_Plugin.System.UPxr_SwitchSetUsbConfigurationOption(uSBConfigModeEnum);
        }

        /// <summary>
        /// Screen On.
        /// </summary>
        public static void ScreenOn()
        {
            PXR_Plugin.System.UPxr_ScreenOn();
        }

        /// <summary>
        /// Screen Off.
        /// </summary>
        public static void ScreenOff()
        {
            PXR_Plugin.System.UPxr_ScreenOff();
        }

        /// <summary>
        /// Acquire WakeLock.
        /// </summary>
        public static void AcquireWakeLock()
        {
            PXR_Plugin.System.UPxr_AcquireWakeLock();
        }

        /// <summary>
        /// Release WakeLock.
        /// </summary>
        public static void ReleaseWakeLock()
        {
            PXR_Plugin.System.UPxr_ReleaseWakeLock();
        }

        /// <summary>
        /// Enable Enter Key.
        /// </summary>
        public static void EnableEnterKey()
        {
            PXR_Plugin.System.UPxr_EnableEnterKey();
        }

        /// <summary>
        /// Disable Enter Key.
        /// </summary>
        public static void DisableEnterKey()
        {
            PXR_Plugin.System.UPxr_DisableEnterKey();
        }

        /// <summary>
        /// Enable Volume Key.
        /// </summary>
        public static void EnableVolumeKey()
        {
            PXR_Plugin.System.UPxr_EnableVolumeKey();
        }

        /// <summary>
        /// Disable Volume Key.
        /// </summary>
        public static void DisableVolumeKey()
        {
            PXR_Plugin.System.UPxr_DisableVolumeKey();
        }

        /// <summary>
        /// Enable Back Key.
        /// </summary>
        public static void EnableBackKey()
        {
            PXR_Plugin.System.UPxr_EnableBackKey();
        }

        /// <summary>
        /// Disable Back Key.
        /// </summary>
        public static void DisableBackKey()
        {
            PXR_Plugin.System.UPxr_DisableBackKey();
        }

        /// <summary>
        /// Write ConfigFile To DataLocal.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="content"></param>
        /// <param name="callback"></param>
        public static void WriteConfigFileToDataLocal(string path, string content, Action<bool> callback)
        {
            PXR_Plugin.System.UPxr_WriteConfigFileToDataLocal(path, content, callback);
        }

        /// <summary>
        /// Reset All Key To Default.
        /// </summary>
        /// <param name="callback"></param>
        public static void ResetAllKeyToDefault(Action<bool> callback)
        {
            PXR_Plugin.System.UPxr_ResetAllKeyToDefault(callback);
        }

        /// <summary>
        /// Set APP As Launcher.
        /// </summary>
        /// <param name="switchEnum"></param>
        /// <param name="packageName"></param>
        public static void SetAPPAsHome(SwitchEnum switchEnum, string packageName)
        {
            PXR_Plugin.System.UPxr_SetAPPAsHome(switchEnum, packageName);
        }

        /// <summary>
        /// Kill Apps By Pid Or PackageName.
        /// </summary>
        /// <param name="pids"></param>
        /// <param name="packageNames"></param>
        public static void KillAppsByPidOrPackageName(int[] pids, string[] packageNames)
        {
            PXR_Plugin.System.UPxr_KillAppsByPidOrPackageName(pids, packageNames);
        }

        /// <summary>
        /// Kill Apps By PackageName With White List.
        /// </summary>
        /// <param name="packageNames"></param>
        public static void KillBackgroundAppsWithWhiteList(string[] packageNames)
        {
            PXR_Plugin.System.UPxr_KillBackgroundAppsWithWhiteList(packageNames);
        }

        /// <summary>
        /// Freeze Screen.
        /// </summary>
        /// <param name="freeze"></param>
        public static void FreezeScreen(bool freeze)
        {
            PXR_Plugin.System.UPxr_FreezeScreen(freeze);
        }

        /// <summary>
        /// Open Miracast.
        /// </summary>
        public static void OpenMiracast()
        {
            PXR_Plugin.System.UPxr_OpenMiracast();
        }

        /// <summary>
        /// Get Miracast On-Off State.
        /// </summary>
        /// <returns></returns>
        public static bool IsMiracastOn()
        {
            return PXR_Plugin.System.UPxr_IsMiracastOn();
        }

        /// <summary>
        /// Close Miracast.
        /// </summary>
        public static void CloseMiracast()
        {
            PXR_Plugin.System.UPxr_CloseMiracast();
        }

        /// <summary>
        /// Start Scan.
        /// </summary>
        public static void StartScan()
        {
            PXR_Plugin.System.UPxr_StartScan();
        }

        /// <summary>
        /// Stop Scan.
        /// </summary>
        public static void StopScan()
        {
            PXR_Plugin.System.UPxr_StopScan();
        }

        /// <summary>
        /// Connect to Wifi Display.
        /// </summary>
        /// <param name="modelJson"></param>
        public static void ConnectWifiDisplay(string modelJson)
        {
            PXR_Plugin.System.UPxr_ConnectWifiDisplay(modelJson);
        }

        /// <summary>
        /// DisConnect Wifi Display.
        /// </summary>
        public static void DisConnectWifiDisplay()
        {
            PXR_Plugin.System.UPxr_DisConnectWifiDisplay();
        }

        /// <summary>
        /// Forget Wifi Display.
        /// </summary>
        /// <param name="address"></param>
        public static void ForgetWifiDisplay(string address)
        {
            PXR_Plugin.System.UPxr_ForgetWifiDisplay(address);
        }

        /// <summary>
        /// Rename Wifi Display.
        /// </summary>
        /// <param name="address"></param>
        /// <param name="newName"></param>
        public static void RenameWifiDisplay(string address, string newName)
        {
            PXR_Plugin.System.UPxr_RenameWifiDisplay(address, newName);
        }

        /// <summary>
        /// Set Wifi Display Models Callback.
        /// </summary>
        public static void SetWDModelsCallback()
        {
            PXR_Plugin.System.UPxr_SetWDModelsCallback();
        }

        /// <summary>
        /// Set Wifi Display Json Callback.
        /// </summary>
        public static void SetWDJsonCallback()
        {
            PXR_Plugin.System.UPxr_SetWDJsonCallback();
        }

        /// <summary>
        /// Update Wifi Displays.
        /// </summary>
        /// <param name="callback"></param>
        public static void UpdateWifiDisplays(Action<string> callback)
        {
            PXR_Plugin.System.UPxr_UpdateWifiDisplays(callback);
        }

        /// <summary>
        /// Get Connected Wifi Display.
        /// </summary>
        /// <returns></returns>
        public static string GetConnectedWD()
        {
            return PXR_Plugin.System.UPxr_GetConnectedWD();
        }

        /// <summary>
        /// Switch Large Space Scene.
        /// </summary>
        /// <param name="open"></param>
        /// <param name="callback"></param>
        public static void SwitchLargeSpaceScene(bool open, Action<bool> callback)
        {
            PXR_Plugin.System.UPxr_SwitchLargeSpaceScene(open, callback);
        }

        /// <summary>
        ///  Get Switch Large Space Status.
        /// </summary>
        /// <param name="callback"></param>
        public static void GetSwitchLargeSpaceStatus(Action<string> callback)
        {
            PXR_Plugin.System.UPxr_GetSwitchLargeSpaceStatus(callback);
        }

        /// <summary>
        /// Save Large Space Maps.
        /// </summary>
        /// <returns>value: True: Success; False: Fail</returns>
        public static bool SaveLargeSpaceMaps()
        {
            return PXR_Plugin.System.UPxr_SaveLargeSpaceMaps();
        }

        /// <summary>
        /// Export Maps.
        /// </summary>
        /// <param name="callback"></param>
        public static void ExportMaps(Action<bool> callback)
        {
            PXR_Plugin.System.UPxr_ExportMaps(callback);
        }

        /// <summary>
        /// Import Maps.
        /// </summary>
        /// <param name="callback"></param>
        public static void ImportMaps(Action<bool> callback)
        {
            PXR_Plugin.System.UPxr_ImportMaps(callback);
        }

        /// <summary>
        /// Get Sensor Status.
        /// </summary>
        /// <returns>status : 0\1\3，null\3dof\6dof</returns>
        public static int GetSensorStatus()
        {
            return PXR_Plugin.System.UPxr_GetSensorStatus();
        }

        /// <summary>
        /// Set system display frequency.
        /// </summary>
        /// <returns></returns>
        public static void SetSystemDisplayFrequency(float rate)
        {
            PXR_Plugin.System.UPxr_SetSystemDisplayFrequency(rate);
        }

        /// <summary>
        /// Get system display frequency.
        /// </summary>
        /// <returns>display rate</returns>
        public static float GetSystemDisplayFrequency()
        {
            return PXR_Plugin.System.UPxr_GetSystemDisplayFrequency();
        }

        /// <summary>
        /// Whether the current application supports large space
        /// </summary>
        /// <param name="value"></param>
        public static void SetLargeSpaceEnable(bool value) {
            PXR_Plugin.System.UPxr_SetLargeSpaceEnable(value);
        }

        /// <summary>
        /// Get attitude data of the device
        /// </summary>
        /// <param name="sensorState"></param>
        /// <param name="sensorFrameIndex"></param>
        /// <returns></returns>
        public static int GetPredictedMainSensorStateNew(ref PxrSensorState2 sensorState, ref int sensorFrameIndex) {
            return PXR_Plugin.System.UPxr_GetPredictedMainSensorStateNew(ref sensorState, ref sensorFrameIndex);
        }

        public static int ContentProtect(int data) {
            return PXR_Plugin.System.UPxr_ContentProtect(data);
        }

        /// <summary>
        /// Get CPU utility
        /// </summary>
        /// <returns></returns>
        public static float[] GetCpuUsages() {
            return PXR_Plugin.System.UPxr_GetCpuUsages();
        }

        /// <summary>
        /// Get device temperature
        /// </summary>
        /// <param name="type"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static float[] GetDeviceTemperatures(int type, int source) {
            return PXR_Plugin.System.UPxr_GetDeviceTemperatures(type, source);
        }

        /// <summary>
        /// Snappy for Mac
        /// </summary>
        public static void Capture() {
            PXR_Plugin.System.UPxr_Capture();
        }

        /// <summary>
        /// Quick record the screen
        /// </summary>
        public static void Record() {
            PXR_Plugin.System.UPxr_Record();
        }

        /// <summary>
        /// Connect to specified Wifi  
        /// </summary>
        /// <param name="ssid"></param>
        /// <param name="pwd"></param>
        /// <param name="ext"></param>
        /// <param name="callback"></param>
        public static void ControlSetAutoConnectWIFIWithErrorCodeCallback(String ssid, String pwd, int ext, Action<int> callback) {
            PXR_Plugin.System.UPxr_ControlSetAutoConnectWIFIWithErrorCodeCallback(ssid, pwd, ext, callback);
        }

        public static void AppKeepAlive(String appPackageName, bool keepAlive, int ext) {
            PXR_Plugin.System.UPxr_AppKeepAlive(appPackageName, keepAlive, ext);
        }

    }
}

