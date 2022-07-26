/*******************************************************************************
Copyright © 2015-2022 Pico Technology Co., Ltd.All rights reserved.

NOTICE：All information contained herein is, and remains the property of
Pico Technology Co., Ltd. The intellectual and technical concepts
contained herein are proprietary to Pico Technology Co., Ltd. and may be
covered by patents, patents in process, and are protected by trade secret or
copyright law. Dissemination of this information or reproduction of this
material is strictly forbidden unless prior written permission is obtained from
Pico Technology Co., Ltd.
*******************************************************************************/

using System;
using System.Collections.Generic;
using Pico.Platform.Models;
using UnityEngine;
using UnityEngine.Android;

namespace Pico.Platform
{
    /**
     * \ingroup Platform
     */
    public static class RtcService
    {
        /// <summary>
        /// Initializes the RTC engine. You should call this method before using the RTC service.
        /// </summary>
        /// <returns>The status that indicates whether the initialization is successful.</returns>
        public static RtcEngineInitResult InitRtcEngine()
        {
            if (Application.platform == RuntimePlatform.Android && !Permission.HasUserAuthorizedPermission(Permission.Microphone))
            {
                Permission.RequestUserPermission(Permission.Microphone);
            }

            return CLIB.ppf_Rtc_InitRtcEngine();
        }

        /// <summary>
        /// Gets the token required by `JoinRoom`.
        /// </summary>
        /// <param name="roomId">The ID of the room that the token is for.</param>
        /// <param name="userId">The ID of the user that the token is for.</param>
        /// <param name="ttl">The time-to-live (ttl) of the token.</param>
        /// <param name="privileges">The dictionary that maps privilege to ttl.</param>
        public static Task<string> GetToken(string roomId, string userId, int ttl, Dictionary<RtcPrivilege, int> privileges)
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.UninitializedError);
                return null;
            }

            var tokenOption = new RtcGetTokenOptions();
            tokenOption.SetRoomId(roomId);
            tokenOption.SetUserId(userId);
            tokenOption.SetTtl(ttl);
            if (privileges != null)
            {
                foreach (var i in privileges)
                {
                    tokenOption.SetPrivileges(i.Key, i.Value);
                }
            }
            return new Task<string>(CLIB.ppf_Rtc_GetToken(tokenOption));
        }

        /// <summary>
        /// Joins a user to a specified room.
        /// </summary>
        /// <param name="roomId">The ID of the room to join.</param>
        /// <param name="userId">The ID of user.</param>
        /// <param name="token">The token required for joining the room. You can get the token by calling `GetToken`.</param>
        /// <param name="roomProfileType">Room type: `0`-communication room; `1`-live broadcasting room; `2`-game room; `3`-cloud game room; `4`-low-latency room.</param>
        /// <param name="isAutoSubscribeAudio">Whether to automatically subscribe to the audio in the room: `true`-yes; `false`-no.</param>
        /// <returns>`0` indicates success, and other values indicate failure.
        /// | Error Code| Description |
        /// |---|---|
        /// |0|Success.|
        /// |-1|Invalid `roomID` or `userId`.|
        /// |-2|The user is already in this room.|
        /// |-3|The RTC engine is null. You should initialize the RTC engine before joining a room.|
        /// |-4|Creating the room failed.|
        /// </returns>
        public static int JoinRoom(string roomId, string userId, string token, RtcRoomProfileType rtcRoomProfileType, bool isAutoSubscribeAudio)
        {
            var roomOption = new RtcRoomOptions();
            roomOption.SetRoomId(roomId);
            roomOption.SetUserId(userId);
            roomOption.SetToken(token);
            roomOption.SetRoomProfileType(rtcRoomProfileType);
            roomOption.SetIsAutoSubscribeAudio(isAutoSubscribeAudio);
            return CLIB.ppf_Rtc_JoinRoom(roomOption);
        }

        /// <summary>
        /// Leaves a specified room.
        /// </summary>
        /// <param name="roomId">The ID of the room to leave.</param>
        /// <returns>`0` indicates success, and other values indicate failure.
        /// | Error Code| Description |
        /// |---|---|
        /// |0|Success.|
        /// |-1|The RTC engine is not initialized.|
        /// |-2|The user is not in the room.|
        /// </returns>
        public static int LeaveRoom(string roomId)
        {
            return CLIB.ppf_Rtc_LeaveRoom(roomId);
        }

        /// <summary>
        /// Sets the audio playback device.
        /// </summary>
        /// <param name="device">The device ID.</param>
        public static void SetAudioPlaybackDevice(RtcAudioPlaybackDevice device)
        {
            CLIB.ppf_Rtc_SetAudioPlaybackDevice(device);
        }

        /// <summary>
        /// Pauses all subscribed streams of a room. Once paused, the voice of users in the room is blocked so nothing can be heard from this room.
        /// </summary>
        /// <param name="roomId">The ID of the room to pause subscribed streams for.</param>
        public static void RoomPauseAllSubscribedStream(string roomId)
        {
            CLIB.ppf_Rtc_RoomPauseAllSubscribedStream(roomId, RtcPauseResumeMediaType.Audio);
        }

        /// <summary>
        /// Resumes all subscribed streams of a room. Once resumed, the voice of users in the room can be heard again. 
        /// </summary>
        /// <param name="roomId">The ID of the room to resume subscribed streams for.</param>
        public static void RoomResumeAllSubscribedStream(string roomId)
        {
            CLIB.ppf_Rtc_RoomResumeAllSubscribedStream(roomId, RtcPauseResumeMediaType.Audio);
        }

        /// <summary>
        /// Enables audio properties report. Once enabled, you will receive audio report data regularly.
        /// </summary>
        /// <param name="interval">
        /// The interval (in milliseconds) between one report and the next. You can set this parameter to `0` or any negative integer to stop receiving audio properties report.
        /// For any integer between (0, 100), the SDK will regard it as invalid and automatically set this parameter to `100`; any integer equal to or greater than `100` is valid.
        /// </param>
        public static void EnableAudioPropertiesReport(int interval)
        {
            var conf = new RtcAudioPropertyOptions();
            conf.SetInterval(interval);
            CLIB.ppf_Rtc_EnableAudioPropertiesReport((IntPtr) conf);
        }

        /// <summary>
        /// Publishes the local audio stream to a room, thereby making the voice heard be others in the same room.
        /// </summary>
        /// <param name="roomId">The ID of the room that the local audio stream is published to.</param>
        public static void PublishRoom(string roomId)
        {
            CLIB.ppf_Rtc_PublishRoom(roomId);
        }

        /// <summary>
        /// Stops publishing the local audio stream to a room, so others in the same room cannot hear the voice.
        /// </summary>
        /// <param name="roomId">The ID of the room to stop publishing the local audio stream to.</param>
        public static void UnPublishRoom(string roomId)
        {
            CLIB.ppf_Rtc_UnPublishRoom(roomId);
        }

        /// <summary>
        /// Destroys a specified room. The resources occupied by the room will be released after destruction.
        /// </summary>
        /// <param name="roomId">The ID of the room to destroy.</param>
        public static void DestroyRoom(string roomId)
        {
            CLIB.ppf_Rtc_DestroyRoom(roomId);
        }

        /// <summary>
        /// Starts audio capture via the microphone.
        /// </summary>
        public static void StartAudioCapture()
        {
            CLIB.ppf_Rtc_StartAudioCapture();
        }

        /// <summary>
        /// Stops audio capture.
        /// </summary>
        public static void StopAudioCapture()
        {
            CLIB.ppf_Rtc_StopAudioCapture();
        }

        /// <summary>
        /// Sets the volume of the captured audio.
        /// </summary>
        /// <param name="volume">The target volume. The valid value ranges from `0` to `400`. `100` indicates keeping the original volume.</param>
        public static void SetCaptureVolume(int volume)
        {
            CLIB.ppf_Rtc_SetCaptureVolume(RtcStreamIndex.Main, volume);
        }

        /// <summary>
        /// Sets the playback volume.
        /// </summary>
        /// <param name="volume">The target volume. The valid value ranges from `0` to `400`. `100` indicates keeping the original volume.</param>
        public static void SetPlaybackVolume(int volume)
        {
            CLIB.ppf_Rtc_SetPlaybackVolume(volume);
        }

        /// <summary>
        /// Switches the in-ear monitoring mode on/off. Once the in-ear monitoring mode is enabled, one can hear their own voice.
        /// </summary>
        /// <param name="mode">Whether to switch the in-ear monitoring mode on/off: `0`-off; `1`-on.</param>
        public static void SetEarMonitorMode(RtcEarMonitorMode mode)
        {
            CLIB.ppf_Rtc_SetEarMonitorMode(mode);
        }

        /// <summary>
        /// Sets the volume for in-ear monitoring.
        /// </summary>
        /// <param name="volume">The target volume. The valid value range from `0` to `400`.</param>
        public static void SetEarMonitorVolume(int volume)
        {
            CLIB.ppf_Rtc_SetEarMonitorVolume(volume);
        }

        /// <summary>
        /// Mutes local audio to make one's voice unable to be heard be others in the same room.
        /// </summary>
        /// <param name="rtcMuteState">The state of local audio: `0`-off; `1`-on.</param>
        public static void MuteLocalAudio(RtcMuteState rtcMuteState)
        {
            CLIB.ppf_Rtc_MuteLocalAudio(rtcMuteState);
        }

        /// <summary>
        /// Updates the token. Once a token's ttl is about to expire, you should update the token if you still want to stay in the room.
        /// </summary>
        /// <param name="roomId">The ID of the room you are in.</param>
        /// <param name="token">The token to update.</param>
        public static void UpdateToken(string roomId, string token)
        {
            CLIB.ppf_Rtc_UpdateToken(roomId, token);
        }

        /// <summary>
        /// Sets the audio scenario. Different audio scenarios can impact the voice quality and how the earphones work.
        /// </summary>
        /// <param name="scenarioType">The audio scenario type: `0`-Music; `1`-HighQualityCommunication; `2`-Communication; `3`-Media; `4`-GameStreaming.</param>
        public static void SetAudioScenario(RtcAudioScenarioType scenarioType)
        {
            CLIB.ppf_Rtc_SetAudioScenario(scenarioType);
        }

        /// <summary>
        /// Sets the callback of `JoinRoom` to get `RtcJoinRoomResult`.
        /// </summary>
        /// <param name="handler">The callback handler.</param>
        public static void SetOnJoinRoomResultCallback(Message<RtcJoinRoomResult>.Handler handler)
        {
            Looper.RegisterNotifyHandler(
                MessageType.Notification_Rtc_OnJoinRoom,
                handler
            );
        }

        /// <summary>
        /// Sets the callback of `LeaveRoom` to get `RtcLeaveRoomResult`.
        /// </summary>
        /// <param name="handler">The callback handler.</param>
        public static void SetOnLeaveRoomResultCallback(Message<RtcLeaveRoomResult>.Handler handler)
        {
            Looper.RegisterNotifyHandler(
                MessageType.Notification_Rtc_OnLeaveRoom,
                handler
            );
        }

        /// <summary>
        /// Sets the callback to get notified when someone has joined the room.
        /// </summary>
        /// <param name="handler">The callback handler.</param>
        public static void SetOnUserJoinRoomResultCallback(Message<RtcUserJoinInfo>.Handler handler)
        {
            Looper.RegisterNotifyHandler(
                MessageType.Notification_Rtc_OnUserJoinRoom,
                handler
            );
        }

        /// <summary>
        /// Sets the callback to get notified when someone has left the room.
        /// </summary>
        /// <param name="handler">The callback handler.</param>
        public static void SetOnUserLeaveRoomResultCallback(Message<RtcUserLeaveInfo>.Handler handler)
        {
            Looper.RegisterNotifyHandler(
                MessageType.Notification_Rtc_OnUserLeaveRoom,
                handler
            );
        }

        /// <summary>
        /// Sets the callback to regularly get room statistics after joining a room.
        /// </summary>
        /// <param name="handler">The callback handler.</param>
        public static void SetOnRoomStatsCallback(Message<RtcRoomStats>.Handler handler)
        {
            Looper.RegisterNotifyHandler(
                MessageType.Notification_Rtc_OnRoomStats,
                handler
            );
        }

        /// <summary>
        /// Sets the callback to get warning messages from the RTC engine.
        /// The warning codes and descriptions are given below.
        ///
        /// |Warning Code|Description|
        /// |---|---|
        /// |-2001|Joining the room failed.|
        /// |-2002|Publishing audio stream failed.|
        /// |-2003|Subscribing to the audio stream failed because the stream cannot be found.|
        /// |-2004|Subscribing to the audio stream failed due to server error.|
        /// |-2013|When the people count in the room exceeds 500, the client will not be informed of user join and leave info anymore.|
        /// |-5001|The camera permission is missing.|
        /// |-5002|The microphone permission is missing.|
        /// |-5003|Starting the audio capture device failed.|
        /// |-5004|Starting the audio playback device failed.|
        /// |-5005|No available audio capture device.|
        /// |-5006|No available audio playback device.|
        /// |-5007|The audio capture device failed to capture valid audio data.|
        /// |-5008|Invalid media device operation.|
        /// </summary>
        /// <param name="handler">The callback handler.</param>
        public static void SetOnWarnCallback(Message<int>.Handler handler)
        {
            Looper.RegisterNotifyHandler(MessageType.Notification_Rtc_OnWarn, handler);
        }

        /// <summary>
        /// Sets the callback to get error messages from the RTC engine.
        /// The error codes and descriptions are given below.
        ///
        /// |Error Code|Description|
        /// |---|---|
        /// |-1000|Invalid token.|
        /// |-1001|Unknown error.|
        /// |-1002|No permission to publish audio stream.|
        /// |-1004|A user with the same user Id joined this room. You are kicked out of the room.|
        /// |-1005|Incorrect configuration on the Developer Platform.|
        /// |-1070|Subscribing to audio stream failed. Perhaps the number of subscribed audio streams has exceeded the limit.|
        /// </summary>
        /// <param name="handler">The callback handler.</param>
        public static void SetOnErrorCallback(Message<int>.Handler handler)
        {
            Looper.RegisterNotifyHandler(MessageType.Notification_Rtc_OnError, handler);
        }

        /// <summary>
        /// Sets the callback to get warning messages from the room.
        /// </summary>
        /// <param name="handler">The callback handler.</param>
        public static void SetOnRoomWarnCallback(Message<RtcRoomWarn>.Handler handler)
        {
            Looper.RegisterNotifyHandler(MessageType.Notification_Rtc_OnRoomWarn, handler);
        }

        /// <summary>
        /// Sets the callback to get error messages from the room.
        /// </summary>
        /// <param name="handler">The callback handler.</param>
        public static void SetOnRoomErrorCallback(Message<RtcRoomError>.Handler handler)
        {
            Looper.RegisterNotifyHandler(MessageType.Notification_Rtc_OnRoomError, handler);
        }

        /// <summary>
        /// Sets the callback to get notified when the state of the connection to the RTC server has changed.
        /// </summary>
        /// <param name="handler">The callback handler.</param>
        public static void SetOnConnectionStateChangeCallback(Message<RtcConnectionState>.Handler handler)
        {
            Looper.RegisterNotifyHandler(MessageType.Notification_Rtc_OnConnectionStateChange, handler);
        }

        /// <summary>
        /// Sets the callback to get notified when the user has muted local audio.
        /// </summary>
        /// <param name="handler">The callback handler.</param>
        public static void SetOnUserMuteAudio(Message<RtcMuteInfo>.Handler handler)
        {
            Looper.RegisterNotifyHandler(MessageType.Notification_Rtc_OnUserMuteAudio, handler);
        }

        /// <summary>
        /// Sets the callback to get notified when the user has started audio capture.
        /// </summary>
        /// <param name="handler">The callback handler.</param>
        public static void SetOnUserStartAudioCapture(Message<string>.Handler handler)
        {
            Looper.RegisterNotifyHandler(MessageType.Notification_Rtc_OnUserStartAudioCapture, handler);
        }

        /// <summary>
        /// Sets the callback to get notified when the user has stopped audio capture.
        /// </summary>
        /// <param name="handler">The callback handler.</param>
        public static void SetOnUserStopAudioCapture(Message<string>.Handler handler)
        {
            Looper.RegisterNotifyHandler(MessageType.Notification_Rtc_OnUserStopAudioCapture, handler);
        }

        /// <summary>
        /// Sets the callback to get notified when the audio playback device has been changed.
        /// </summary>
        /// <param name="handler">The callback handler.</param>
        public static void SetOnAudioPlaybackDeviceChange(Message<RtcAudioPlaybackDevice>.Handler handler)
        {
            Looper.RegisterNotifyHandler(MessageType.Notification_Rtc_OnAudioPlaybackDeviceChanged, handler);
        }

        /// <summary>
        /// Sets the callback to receive local audio report.
        /// </summary>
        /// <param name="handler">The callback handler.</param>
        public static void SetOnLocalAudioPropertiesReport(Message<RtcLocalAudioPropertiesReport>.Handler handler)
        {
            Looper.RegisterNotifyHandler(MessageType.Notification_Rtc_OnLocalAudioPropertiesReport, handler);
        }

        /// <summary>
        /// Sets the callback to receive remote audio report.
        /// </summary>
        /// <param name="handler">The callback handler.</param>
        public static void SetOnRemoteAudioPropertiesReport(Message<RtcRemoteAudioPropertiesReport>.Handler handler)
        {
            Looper.RegisterNotifyHandler(MessageType.Notification_Rtc_OnRemoteAudioPropertiesReport, handler);
        }
    }

    public enum RtcAudioPlaybackDevice
    {
        Headset = 1,
        EarPiece = 2,
        SpeakerPhone = 3,
        HeadsetBlueTooth = 4,
        HeadsetUsb = 5,
    }

    public enum RtcAudioScenarioType
    {
        Music = 0,
        HighQualityCommunication = 1,
        Communication = 2,
        Media = 3,
        GameStreaming = 4,
    }

    public enum RtcConnectionState
    {
        Disconnected = 1,
        Connecting = 2,
        Connected = 3,
        Reconnecting = 4,
        Reconnected = 5,
        Lost = 6,
    }

    public enum RtcEarMonitorMode
    {
        Off = 0,
        On = 1,
    }

    public enum RtcEngineInitResult
    {
        Unknown = -999,
        AlreadyInitialized = -1,
        InvalidConfig = -2,
        Success = 0,
    }

    public enum RtcJoinRoomType
    {
        First = 0,
        Reconnected = 1,
    }

    public enum RtcMediaDeviceError
    {
        Ok = 0,
        NoPermission = 1,
        DeviceBusy = 2,
        DeviceFailure = 3,
        DeviceNotFound = 4,
        DeviceDisconnected = 5,
        DeviceNoCallback = 6,
        UnSupportedFormat = 7,
    }

    public enum RtcMediaDeviceState
    {
        Started = 1,
        Stopped = 2,
        RuntimeError = 3,
        Added = 4,
        Removed = 5,
    }

    public enum RtcMediaDeviceType
    {
        AudioUnknown = -1,
        AudioRenderDevice = 0,
        AudioCaptureDevice = 1,
    }

    public enum RtcMuteState
    {
        Off = 0,
        On = 1,
    }

    public enum RtcPauseResumeMediaType
    {
        Audio = 0,
        Video = 1,
        AudioAndVideo = 2,
    }

    public enum RtcPrivilege
    {
        PublishStream = 0,
        PublishAudioStream = 1,
        PublishVideoStream = 2,
        SubscribeStream = 3,
    }

    public enum RtcRoomProfileType
    {
        Communication = 0,
        LiveBroadcasting = 1,
        Game = 2,
        CloudGame = 3,
        LowLatency = 4,
    }

    public enum RtcStreamIndex
    {
        Main = 0,
        Screen = 1,
    }

    public enum RtcUserLeaveReasonType
    {
        Quit = 0,
        Dropped = 1,
    }

    public class RtcAudioPropertyInfo
    {
        public readonly int Volume;

        public RtcAudioPropertyInfo(IntPtr o)
        {
            Volume = CLIB.ppf_RtcAudioPropertyInfo_GetVolume(o);
        }
    }

    public class RtcRoomOptions
    {
        public RtcRoomOptions()
        {
            Handle = CLIB.ppf_RtcRoomOptions_Create();
        }


        public void SetRoomProfileType(RtcRoomProfileType value)
        {
            CLIB.ppf_RtcRoomOptions_SetRoomProfileType(Handle, value);
        }


        public void SetIsAutoSubscribeAudio(bool value)
        {
            CLIB.ppf_RtcRoomOptions_SetIsAutoSubscribeAudio(Handle, value);
        }

        public void SetRoomId(string value)
        {
            CLIB.ppf_RtcRoomOptions_SetRoomId(Handle, value);
        }


        public void SetUserId(string value)
        {
            CLIB.ppf_RtcRoomOptions_SetUserId(Handle, value);
        }


        public void SetUserExtra(string value)
        {
            CLIB.ppf_RtcRoomOptions_SetUserExtra(Handle, value);
        }


        public void SetToken(string value)
        {
            CLIB.ppf_RtcRoomOptions_SetToken(Handle, value);
        }

        /// For passing to native C
        public static explicit operator IntPtr(RtcRoomOptions options)
        {
            return options != null ? options.Handle : IntPtr.Zero;
        }

        ~RtcRoomOptions()
        {
            CLIB.ppf_RtcRoomOptions_Destroy(Handle);
        }

        IntPtr Handle;
    }

    public class RtcGetTokenOptions
    {
        public RtcGetTokenOptions()
        {
            Handle = CLIB.ppf_RtcGetTokenOptions_Create();
        }


        public void SetUserId(string value)
        {
            CLIB.ppf_RtcGetTokenOptions_SetUserId(Handle, value);
        }


        public void SetRoomId(string value)
        {
            CLIB.ppf_RtcGetTokenOptions_SetRoomId(Handle, value);
        }

        public void SetTtl(int value)
        {
            CLIB.ppf_RtcGetTokenOptions_SetTtl(Handle, value);
        }

        public void SetPrivileges(RtcPrivilege key, int value)
        {
            CLIB.ppf_RtcGetTokenOptions_SetPrivileges(Handle, key, value);
        }

        public void ClearPrivileges()
        {
            CLIB.ppf_RtcGetTokenOptions_ClearPrivileges(Handle);
        }


        /// For passing to native C
        public static explicit operator IntPtr(RtcGetTokenOptions options)
        {
            return options != null ? options.Handle : IntPtr.Zero;
        }

        ~RtcGetTokenOptions()
        {
            CLIB.ppf_RtcGetTokenOptions_Destroy(Handle);
        }

        IntPtr Handle;
    }

    public class RtcAudioPropertyOptions
    {
        public RtcAudioPropertyOptions()
        {
            Handle = CLIB.ppf_RtcAudioPropertyOptions_Create();
        }


        public void SetInterval(int value)
        {
            CLIB.ppf_RtcAudioPropertyOptions_SetInterval(Handle, value);
        }

        /// For passing to native C
        public static explicit operator IntPtr(RtcAudioPropertyOptions options)
        {
            return options != null ? options.Handle : IntPtr.Zero;
        }

        ~RtcAudioPropertyOptions()
        {
            CLIB.ppf_RtcAudioPropertyOptions_Destroy(Handle);
        }

        IntPtr Handle;
    }
}