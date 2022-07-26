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
using System.Runtime.InteropServices;
using System.Text;

#pragma warning disable 414
namespace Pico.Platform
{
    public partial class CLIB
    {
        const string LoaderName = "pxrplatformloader";
        private static readonly Encoding DefaultEncoding = Encoding.UTF8;

        public static string NativeToString(IntPtr pointer)
        {
            if (pointer == IntPtr.Zero)
            {
                return null;
            }

            var l = GetNativeStringLength(pointer);
            var data = new byte[l];
            Marshal.Copy(pointer, data, 0, l);
            return DefaultEncoding.GetString(data);
        }

        public static IntPtr StringToNative(string s)
        {
            if (s == null)
            {
                throw new Exception("StringToNative: input argument is null.");
            }

            var l = DefaultEncoding.GetByteCount(s);
            var data = new byte[l + 1];
            DefaultEncoding.GetBytes(s, 0, s.Length, data, 0);
            var pointer = Marshal.AllocCoTaskMem(l + 1);
            Marshal.Copy(data, 0, pointer, l + 1);
            return pointer;
        }

        public static int GetNativeStringLength(IntPtr pointer)
        {
            var length = 0;
            while (true)
            {
                if (Marshal.ReadByte(pointer, length) == 0)
                {
                    return length;
                }

                length++;
            }
        }


        // Initialization
        [DllImport(LoaderName)]
        public static extern PlatformInitializeResult ppf_UnityInitWrapper(string appId);

        [DllImport(LoaderName)]
        public static extern ulong ppf_UnityInitAsynchronousWrapper(string appId);

        [DllImport(LoaderName)]
        public static extern ulong ppf_Message_GetRequestID(IntPtr obj);

        [DllImport(LoaderName)]
        public static extern IntPtr ppf_Message_GetError(IntPtr obj);

        [DllImport(LoaderName)]
        public static extern ulong ppf_User_GetAccessToken();

        [DllImport(LoaderName)]
        public static extern ulong ppf_User_GetLoggedInUser();

        // Message queue access
        [DllImport(LoaderName)]
        public static extern IntPtr ppf_PopMessage();

        [DllImport(LoaderName)]
        public static extern void ppf_FreeMessage(IntPtr message);

        [DllImport(LoaderName)]
        public static extern int ppf_Error_GetCode(IntPtr obj);


        public static string ppf_Error_GetMessage(IntPtr obj)
        {
            var result = NativeToString(ppf_Error_GetMessage_Native(obj));
            return result;
        }

        [DllImport(LoaderName, EntryPoint = "ppf_Error_GetMessage")]
        private static extern IntPtr ppf_Error_GetMessage_Native(IntPtr obj);


        public static string ppf_Message_GetString(IntPtr obj)
        {
            var result = NativeToString(ppf_Message_GetString_Native(obj));
            return result;
        }

        [DllImport(LoaderName, EntryPoint = "ppf_Message_GetString")]
        private static extern IntPtr ppf_Message_GetString_Native(IntPtr obj);

        [DllImport(LoaderName, EntryPoint = "ppf_Message_GetInt32")]
        public static extern Int32 ppf_Message_GetInt32(IntPtr obj);

        [DllImport(LoaderName)]
        public static extern MessageType ppf_Message_GetType(IntPtr obj);

        [DllImport(LoaderName)]
        public static extern IntPtr ppf_Message_GetUser(IntPtr obj);

        [DllImport(LoaderName)]
        public static extern bool ppf_Message_IsError(IntPtr obj);

        public static string ppf_User_GetDisplayName(IntPtr obj)
        {
            var result = NativeToString(ppf_User_GetDisplayName_Native(obj));
            return result;
        }

        [DllImport(LoaderName)]
        public static extern UserPresenceStatus ppf_User_GetPresenceStatus(IntPtr obj);

        public static string ppf_User_GetID(IntPtr obj)
        {
            var result = NativeToString(ppf_User_GetID_Native(obj));
            return result;
        }
        [DllImport(LoaderName, EntryPoint = "ppf_User_GetDisplayName")]
        private static extern IntPtr ppf_User_GetDisplayName_Native(IntPtr obj);

        [DllImport(LoaderName, EntryPoint = "ppf_User_GetID")]
        public static extern IntPtr ppf_User_GetID_Native(IntPtr obj);

        [DllImport(LoaderName, EntryPoint = "ppf_User_GetImageUrl")]
        private static extern IntPtr ppf_User_GetImageUrl_Native(IntPtr obj);

        [DllImport(LoaderName)]
        public static extern IntPtr ppf_Message_GetUserArray(IntPtr obj);

        [DllImport(LoaderName)]
        public static extern IntPtr ppf_UserArray_GetElement(IntPtr obj, UIntPtr index);

        [DllImport(LoaderName,
            EntryPoint = "ppf_UserArray_GetNextPageParam")]
        private static extern IntPtr ppf_UserArray_GetNextPageParam_Native(IntPtr obj);

        public static string ppf_UserArray_GetNextPageParam(IntPtr obj)
        {
            var result = NativeToString(ppf_UserArray_GetNextPageParam_Native(obj));
            return result;
        }

        [DllImport(LoaderName)]
        public static extern UIntPtr ppf_UserArray_GetSize(IntPtr obj);

        [DllImport(LoaderName)]
        public static extern ulong ppf_User_GetLoggedInUserFriends();

        [DllImport(LoaderName)]
        public static extern Gender ppf_User_GetGender(IntPtr handle);


        [DllImport(LoaderName)]
        public static extern void ppf_RtcGetTokenOptions_Destroy(IntPtr handle);

        [DllImport(LoaderName, EntryPoint = "ppf_RtcGetTokenOptions_SetRoomId")]
        public static extern void ppf_RtcGetTokenOptions_SetRoomId_Native(IntPtr handle, IntPtr value);


        public static void ppf_RtcGetTokenOptions_SetRoomId(IntPtr handle, string value)
        {
            var valueNative = StringToNative(value);
            ppf_RtcGetTokenOptions_SetRoomId_Native(handle, valueNative);
            Marshal.FreeCoTaskMem(valueNative);
        }

        [DllImport(LoaderName, EntryPoint = "ppf_RtcRoomOptions_SetToken")]
        public static extern void ppf_RtcRoomOptions_SetToken_Native(IntPtr handle, IntPtr value);


        public static void ppf_RtcRoomOptions_SetToken(IntPtr handle, string value)
        {
            var valueNative = StringToNative(value);
            ppf_RtcRoomOptions_SetToken_Native(handle, valueNative);
            Marshal.FreeCoTaskMem(valueNative);
        }

        [DllImport(LoaderName, EntryPoint = "ppf_Rtc_GetToken")]
        public static extern ulong ppf_Rtc_GetToken_Native(IntPtr options);


        public static ulong ppf_Rtc_GetToken(RtcGetTokenOptions options)
        {
            var result = ppf_Rtc_GetToken_Native((IntPtr) options);
            return result;
        }

        [DllImport(LoaderName, EntryPoint = "ppf_RtcRoomStats_GetRoomId")]
        public static extern IntPtr ppf_RtcRoomStats_GetRoomId_Native(IntPtr handle);


        public static string ppf_RtcRoomStats_GetRoomId(IntPtr handle)
        {
            var result = NativeToString(ppf_RtcRoomStats_GetRoomId_Native(handle));
            return result;
        }

        [DllImport(LoaderName, EntryPoint = "ppf_RtcUserJoinInfo_GetRoomId")]
        public static extern IntPtr ppf_RtcUserJoinInfo_GetRoomId_Native(IntPtr handle);


        public static string ppf_RtcUserJoinInfo_GetRoomId(IntPtr handle)
        {
            var result = NativeToString(ppf_RtcUserJoinInfo_GetRoomId_Native(handle));
            return result;
        }

        [DllImport(LoaderName, EntryPoint = "ppf_RtcUserLeaveInfo_GetRoomId")]
        public static extern IntPtr ppf_RtcUserLeaveInfo_GetRoomId_Native(IntPtr handle);


        public static string ppf_RtcUserLeaveInfo_GetRoomId(IntPtr handle)
        {
            var result = NativeToString(ppf_RtcUserLeaveInfo_GetRoomId_Native(handle));
            return result;
        }

        public static ulong ppf_User_LaunchFriendRequestFlow(string userID)
        {
            IntPtr userIdNative = StringToNative(userID);
            var result = ppf_User_LaunchFriendRequestFlow_Native(userIdNative);
            Marshal.FreeCoTaskMem(userIdNative);
            return result;
        }


        [DllImport(LoaderName, EntryPoint = "ppf_User_LaunchFriendRequestFlow")]
        public static extern ulong ppf_User_LaunchFriendRequestFlow_Native(IntPtr userID);

        [DllImport(LoaderName)]
        public static extern IntPtr ppf_RtcGetTokenOptions_Create();

        [DllImport(LoaderName, EntryPoint = "ppf_RtcGetTokenOptions_SetUserId")]
        public static extern void ppf_RtcGetTokenOptions_SetUserId_Native(IntPtr obj, IntPtr value);


        public static void ppf_RtcGetTokenOptions_SetUserId(IntPtr handle, string value)
        {
            var valueNative = StringToNative(value);
            ppf_RtcGetTokenOptions_SetUserId_Native(handle, valueNative);
            Marshal.FreeCoTaskMem(valueNative);
        }

        [DllImport(LoaderName, EntryPoint = "ppf_RtcGetTokenOptions_SetTtl")]
        public static extern void ppf_RtcGetTokenOptions_SetTtl(IntPtr obj, int value);

        [DllImport(LoaderName, EntryPoint = "ppf_RtcGetTokenOptions_ClearPrivileges")]
        public static extern void ppf_RtcGetTokenOptions_ClearPrivileges(IntPtr obj);

        [DllImport(LoaderName, EntryPoint = "ppf_Rtc_InitRtcEngine")]
        public static extern RtcEngineInitResult ppf_Rtc_InitRtcEngine();


        [DllImport(LoaderName, EntryPoint = "ppf_RtcGetTokenOptions_SetPrivileges")]
        public static extern void ppf_RtcGetTokenOptions_SetPrivileges(IntPtr obj, RtcPrivilege k, int v);

        public static string ppf_User_GetImageUrl(IntPtr obj)
        {
            var result = NativeToString(ppf_User_GetImageUrl_Native(obj));
            return result;
        }

        public static ulong ppf_User_Get(string userId)
        {
            IntPtr userIdNative = StringToNative(userId);
            var result = ppf_User_Get_Native(userIdNative);
            Marshal.FreeCoTaskMem(userIdNative);
            return result;
        }

        [DllImport(LoaderName, EntryPoint = "ppf_User_GetNextUserArrayPage")]
        public static extern ulong ppf_User_GetNextUserArrayPage_Native(IntPtr userID);


        public static ulong ppf_User_GetNextUserArrayPage(string bodyParams)
        {
            IntPtr bodyParamsNative = StringToNative(bodyParams);
            var result = ppf_User_GetNextUserArrayPage_Native(bodyParamsNative);
            Marshal.FreeCoTaskMem(bodyParamsNative);
            return result;
        }

        [DllImport(LoaderName, EntryPoint = "ppf_User_Get")]
        public static extern ulong ppf_User_Get_Native(IntPtr userID);

        [DllImport(LoaderName)]
        public static extern bool ppf_LaunchFriendRequestFlowResult_GetDidCancel(IntPtr obj);

        [DllImport(LoaderName)]
        public static extern bool ppf_LaunchFriendRequestFlowResult_GetDidSendRequest(IntPtr obj);

        [DllImport(LoaderName)]
        public static extern IntPtr ppf_Message_GetLaunchFriendRequestFlowResult(IntPtr obj);

        [DllImport(LoaderName, EntryPoint = "ppf_RtcUserLeaveInfo_GetUserId")]
        public static extern IntPtr ppf_RtcUserLeaveInfo_GetUserId_Native(IntPtr handle);


        public static string ppf_RtcUserLeaveInfo_GetUserId(IntPtr handle)
        {
            var result = NativeToString(ppf_RtcUserLeaveInfo_GetUserId_Native(handle));
            return result;
        }

        [DllImport(LoaderName)]
        public static extern RtcUserLeaveReasonType ppf_RtcUserLeaveInfo_GetOfflineReason(IntPtr handle);


        [DllImport(LoaderName, EntryPoint = "ppf_RtcUserJoinInfo_GetUserId")]
        public static extern IntPtr ppf_RtcUserJoinInfo_GetUserId_Native(IntPtr handle);


        public static string ppf_RtcUserJoinInfo_GetUserId(IntPtr handle)
        {
            var result = NativeToString(ppf_RtcUserJoinInfo_GetUserId_Native(handle));
            return result;
        }

        [DllImport(LoaderName, EntryPoint = "ppf_RtcUserJoinInfo_GetUserExtra")]
        public static extern IntPtr ppf_RtcUserJoinInfo_GetUserExtra_Native(IntPtr handle);


        public static string ppf_RtcUserJoinInfo_GetUserExtra(IntPtr handle)
        {
            var result = NativeToString(ppf_RtcUserJoinInfo_GetUserExtra_Native(handle));
            return result;
        }

        [DllImport(LoaderName)]
        public static extern int ppf_RtcUserJoinInfo_GetElapsed(IntPtr handle);


        [DllImport(LoaderName, EntryPoint = "ppf_RtcLeaveRoomResult_GetRoomId")]
        public static extern IntPtr ppf_RtcLeaveRoomResult_GetRoomId_Native(IntPtr handle);


        public static string ppf_RtcLeaveRoomResult_GetRoomId(IntPtr handle)
        {
            var result = NativeToString(ppf_RtcLeaveRoomResult_GetRoomId_Native(handle));
            return result;
        }

        [DllImport(LoaderName)]
        public static extern int ppf_RtcRoomStats_GetTotalDuration(IntPtr handle);


        [DllImport(LoaderName)]
        public static extern int ppf_RtcRoomStats_GetUserCount(IntPtr handle);


        [DllImport(LoaderName, EntryPoint = "ppf_RtcJoinRoomResult_GetRoomId")]
        public static extern IntPtr ppf_RtcJoinRoomResult_GetRoomId_Native(IntPtr handle);


        public static string ppf_RtcJoinRoomResult_GetRoomId(IntPtr handle)
        {
            var result = NativeToString(ppf_RtcJoinRoomResult_GetRoomId_Native(handle));
            return result;
        }

        [DllImport(LoaderName, EntryPoint = "ppf_RtcJoinRoomResult_GetUserId")]
        public static extern IntPtr ppf_RtcJoinRoomResult_GetUserId_Native(IntPtr handle);


        public static string ppf_RtcJoinRoomResult_GetUserId(IntPtr handle)
        {
            var result = NativeToString(ppf_RtcJoinRoomResult_GetUserId_Native(handle));
            return result;
        }

        [DllImport(LoaderName)]
        public static extern int ppf_RtcJoinRoomResult_GetErrorCode(IntPtr handle);


        [DllImport(LoaderName)]
        public static extern int ppf_RtcJoinRoomResult_GetElapsed(IntPtr handle);


        [DllImport(LoaderName)]
        public static extern RtcJoinRoomType ppf_RtcJoinRoomResult_GetJoinType(IntPtr handle);


        [DllImport(LoaderName)]
        public static extern IntPtr ppf_Message_GetRtcJoinRoomResult(IntPtr handle);


        [DllImport(LoaderName)]
        public static extern IntPtr ppf_Message_GetRtcLeaveRoomResult(IntPtr handle);


        [DllImport(LoaderName)]
        public static extern IntPtr ppf_Message_GetRtcRoomStats(IntPtr handle);


        [DllImport(LoaderName)]
        public static extern IntPtr ppf_Message_GetRtcUserJoinInfo(IntPtr handle);


        [DllImport(LoaderName)]
        public static extern IntPtr ppf_Message_GetRtcUserLeaveInfo(IntPtr handle);


        [DllImport(LoaderName)]
        public static extern IntPtr ppf_RtcRoomOptions_Create();

        [DllImport(LoaderName)]
        public static extern void ppf_RtcRoomOptions_Destroy(IntPtr handle);

        [DllImport(LoaderName)]
        public static extern void ppf_RtcRoomOptions_SetRoomProfileType(IntPtr handle, RtcRoomProfileType value);


        [DllImport(LoaderName)]
        public static extern void ppf_RtcRoomOptions_SetIsAutoSubscribeAudio(IntPtr handle, bool value);

        [DllImport(LoaderName, EntryPoint = "ppf_RtcRoomOptions_SetRoomId")]
        public static extern void ppf_RtcRoomOptions_SetRoomId_Native(IntPtr handle, IntPtr value);


        public static void ppf_RtcRoomOptions_SetRoomId(IntPtr handle, string value)
        {
            var valueNative = StringToNative(value);
            ppf_RtcRoomOptions_SetRoomId_Native(handle, valueNative);
            Marshal.FreeCoTaskMem(valueNative);
        }

        [DllImport(LoaderName, EntryPoint = "ppf_RtcRoomOptions_SetUserId")]
        public static extern void ppf_RtcRoomOptions_SetUserId_Native(IntPtr handle, IntPtr value);


        public static void ppf_RtcRoomOptions_SetUserId(IntPtr handle, string value)
        {
            var valueNative = StringToNative(value);
            ppf_RtcRoomOptions_SetUserId_Native(handle, valueNative);
            Marshal.FreeCoTaskMem(valueNative);
        }

        [DllImport(LoaderName, EntryPoint = "ppf_RtcRoomOptions_SetUserExtra")]
        public static extern void ppf_RtcRoomOptions_SetUserExtra_Native(IntPtr handle, IntPtr value);


        public static void ppf_RtcRoomOptions_SetUserExtra(IntPtr handle, string value)
        {
            var valueNative = StringToNative(value);
            ppf_RtcRoomOptions_SetUserExtra_Native(handle, valueNative);
            Marshal.FreeCoTaskMem(valueNative);
        }

        [DllImport(LoaderName, EntryPoint = "ppf_Rtc_LeaveRoom")]
        public static extern int ppf_Rtc_LeaveRoom_Native(IntPtr roomId);


        public static int ppf_Rtc_LeaveRoom(string roomId)
        {
            var roomIdNative = StringToNative(roomId);
            var result = ppf_Rtc_LeaveRoom_Native(roomIdNative);
            Marshal.FreeCoTaskMem(roomIdNative);
            return result;
        }

        [DllImport(LoaderName, EntryPoint = "ppf_Rtc_PublishRoom")]
        public static extern void ppf_Rtc_PublishRoom_Native(IntPtr roomId);


        public static void ppf_Rtc_PublishRoom(string roomId)
        {
            var roomIdNative = StringToNative(roomId);
            ppf_Rtc_PublishRoom_Native(roomIdNative);
            Marshal.FreeCoTaskMem(roomIdNative);
        }

        [DllImport(LoaderName)]
        public static extern void ppf_Rtc_SetEarMonitorMode(RtcEarMonitorMode rtcEarMonitorMode);


        [DllImport(LoaderName)]
        public static extern void ppf_Rtc_SetEarMonitorVolume(int volume);


        [DllImport(LoaderName)]
        public static extern void ppf_Rtc_StartAudioCapture();


        [DllImport(LoaderName)]
        public static extern void ppf_Rtc_StopAudioCapture();


        [DllImport(LoaderName)]
        public static extern void ppf_Rtc_SetAudioScenario(RtcAudioScenarioType scenario);


        [DllImport(LoaderName, EntryPoint = "ppf_Rtc_JoinRoom")]
        public static extern int ppf_Rtc_JoinRoom_Native(IntPtr roomOptions);


        public static int ppf_Rtc_JoinRoom(RtcRoomOptions rtcRoomOptions)
        {
            var result = ppf_Rtc_JoinRoom_Native((IntPtr) rtcRoomOptions);
            return result;
        }

        [DllImport(LoaderName, EntryPoint = "ppf_Rtc_UnPublishRoom")]
        public static extern void ppf_Rtc_UnPublishRoom_Native(IntPtr roomId);


        public static void ppf_Rtc_UnPublishRoom(string roomId)
        {
            var roomIdNative = StringToNative(roomId);
            ppf_Rtc_UnPublishRoom_Native(roomIdNative);
            Marshal.FreeCoTaskMem(roomIdNative);
        }


        [DllImport(LoaderName, EntryPoint = "ppf_Rtc_DestroyRoom")]
        public static extern void ppf_Rtc_DestroyRoom_Native(IntPtr roomId);


        public static void ppf_Rtc_DestroyRoom(string roomId)
        {
            var roomIdNative = StringToNative(roomId);
            ppf_Rtc_DestroyRoom_Native(roomIdNative);
            Marshal.FreeCoTaskMem(roomIdNative);
        }

        [DllImport(LoaderName)]
        public static extern void ppf_Rtc_SetCaptureVolume(RtcStreamIndex index, int volume);


        [DllImport(LoaderName)]
        public static extern void ppf_Rtc_SetPlaybackVolume(int volume);


        [DllImport(LoaderName)]
        public static extern void ppf_Rtc_MuteLocalAudio(RtcMuteState rtcMuteState);

        [DllImport(LoaderName, EntryPoint = "ppf_Rtc_UpdateToken")]
        public static extern void ppf_Rtc_UpdateToken_Native(IntPtr roomId, IntPtr token);


        public static void ppf_Rtc_UpdateToken(string roomId, string token)
        {
            var roomIdNative = StringToNative(roomId);
            var tokenNative = StringToNative(token);
            ppf_Rtc_UpdateToken_Native(roomIdNative, tokenNative);
            Marshal.FreeCoTaskMem(tokenNative);
            Marshal.FreeCoTaskMem(roomIdNative);
        }


        [DllImport(LoaderName, EntryPoint = "ppf_RtcRoomError_GetCode")]
        public static extern int ppf_RtcRoomError_GetCode(IntPtr obj);

        [DllImport(LoaderName, EntryPoint = "ppf_RtcRoomWarn_GetCode")]
        public static extern int ppf_RtcRoomWarn_GetCode(IntPtr obj);


        [DllImport(LoaderName, EntryPoint = "ppf_RtcRoomError_GetRoomId")]
        public static extern IntPtr ppf_RtcRoomError_GetRoomId_Native(IntPtr obj);


        public static string ppf_RtcRoomError_GetRoomId(IntPtr obj)
        {
            var result = NativeToString(ppf_RtcRoomError_GetRoomId_Native(obj));
            return result;
        }

        [DllImport(LoaderName, EntryPoint = "ppf_RtcRoomWarn_GetRoomId")]
        public static extern IntPtr ppf_RtcRoomWarn_GetRoomId_Native(IntPtr obj);


        public static string ppf_RtcRoomWarn_GetRoomId(IntPtr obj)
        {
            var result = NativeToString(ppf_RtcRoomWarn_GetRoomId_Native(obj));
            return result;
        }

        [DllImport(LoaderName, EntryPoint = "ppf_Message_GetRtcRoomWarn")]
        public static extern IntPtr ppf_Message_GetRtcRoomWarn(IntPtr obj);


        [DllImport(LoaderName, EntryPoint = "ppf_Message_GetRtcRoomError")]
        public static extern IntPtr ppf_Message_GetRtcRoomError(IntPtr obj);

        [DllImport(LoaderName, EntryPoint = "ppf_RtcRemoteStreamKey_GetRoomId")]
        public static extern IntPtr ppf_RtcRemoteStreamKey_GetRoomId_Native(IntPtr obj);


        public static string ppf_RtcRemoteStreamKey_GetRoomId(IntPtr obj)
        {
            var result = NativeToString(ppf_RtcRemoteStreamKey_GetRoomId_Native(obj));
            return result;
        }

        [DllImport(LoaderName, EntryPoint = "ppf_RtcRemoteStreamKey_GetUserId")]
        public static extern IntPtr ppf_RtcRemoteStreamKey_GetUserId_Native(IntPtr obj);


        public static string ppf_RtcRemoteStreamKey_GetUserId(IntPtr obj)
        {
            var result = NativeToString(ppf_RtcRemoteStreamKey_GetUserId_Native(obj));
            return result;
        }

        [DllImport(LoaderName, EntryPoint = "ppf_RtcRemoteStreamKey_GetStreamIndex")]
        public static extern RtcStreamIndex ppf_RtcRemoteStreamKey_GetStreamIndex_Native(IntPtr obj);


        public static RtcStreamIndex ppf_RtcRemoteStreamKey_GetStreamIndex(IntPtr obj)
        {
            var result = ppf_RtcRemoteStreamKey_GetStreamIndex_Native(obj);
            return result;
        }

        [DllImport(LoaderName, EntryPoint = "ppf_RtcLocalAudioPropertiesInfo_GetStreamIndex")]
        public static extern RtcStreamIndex ppf_RtcLocalAudioPropertiesInfo_GetStreamIndex(IntPtr obj);

        [DllImport(LoaderName, EntryPoint = "ppf_RtcLocalAudioPropertiesInfo_GetAudioPropertyInfo")]
        public static extern IntPtr ppf_RtcLocalAudioPropertiesInfo_GetAudioPropertyInfo_Native(IntPtr obj);


        public static IntPtr ppf_RtcLocalAudioPropertiesInfo_GetAudioPropertyInfo(IntPtr obj)
        {
            var result = ppf_RtcLocalAudioPropertiesInfo_GetAudioPropertyInfo_Native(obj);
            return result;
        }

        [DllImport(LoaderName, EntryPoint = "ppf_RtcRemoteAudioPropertiesReport_GetAudioPropertiesInfos")]
        public static extern IntPtr ppf_RtcRemoteAudioPropertiesReport_GetAudioPropertiesInfos_Native(IntPtr obj, UInt32 index);


        public static IntPtr ppf_RtcRemoteAudioPropertiesReport_GetAudioPropertiesInfos(IntPtr obj, UInt32 index)
        {
            var result = ppf_RtcRemoteAudioPropertiesReport_GetAudioPropertiesInfos_Native(obj, index);
            return result;
        }

        [DllImport(LoaderName, EntryPoint = "ppf_RtcRemoteAudioPropertiesReport_GetAudioPropertiesInfosSize")]
        public static extern UInt32 ppf_RtcRemoteAudioPropertiesReport_GetAudioPropertiesInfosSize(IntPtr obj);

        [DllImport(LoaderName, EntryPoint = "ppf_RtcAudioPropertyInfo_GetVolume")]
        public static extern int ppf_RtcAudioPropertyInfo_GetVolume(IntPtr obj);

        [DllImport(LoaderName, EntryPoint = "ppf_RtcLocalAudioPropertiesReport_GetAudioPropertiesInfos")]
        public static extern IntPtr ppf_RtcLocalAudioPropertiesReport_GetAudioPropertiesInfos(IntPtr obj, UInt32 index);


        [DllImport(LoaderName)]
        public static extern UInt32 ppf_RtcLocalAudioPropertiesReport_GetAudioPropertiesInfosSize(IntPtr obj);

        [DllImport(LoaderName, EntryPoint = "ppf_RtcMediaDeviceChangeInfo_GetDeviceId")]
        public static extern IntPtr ppf_RtcMediaDeviceChangeInfo_GetDeviceId_Native(IntPtr obj);


        public static string ppf_RtcMediaDeviceChangeInfo_GetDeviceId(IntPtr obj)
        {
            var result = NativeToString(ppf_RtcMediaDeviceChangeInfo_GetDeviceId_Native(obj));
            return result;
        }

        [DllImport(LoaderName)]
        public static extern RtcMediaDeviceType ppf_RtcMediaDeviceChangeInfo_GetDeviceType(IntPtr obj);

        [DllImport(LoaderName)]
        public static extern RtcMediaDeviceState ppf_RtcMediaDeviceChangeInfo_GetDeviceState(IntPtr obj);

        [DllImport(LoaderName)]
        public static extern RtcMediaDeviceError ppf_RtcMediaDeviceChangeInfo_GetDeviceError(IntPtr obj);


        [DllImport(LoaderName, EntryPoint = "ppf_RtcMuteInfo_GetUserId")]
        public static extern IntPtr ppf_RtcMuteInfo_GetUserId_Native(IntPtr obj);


        public static string ppf_RtcMuteInfo_GetUserId(IntPtr obj)
        {
            var result = NativeToString(ppf_RtcMuteInfo_GetUserId_Native(obj));
            return result;
        }

        [DllImport(LoaderName)]
        public static extern RtcMuteState ppf_RtcMuteInfo_GetMuteState(IntPtr obj);

        [DllImport(LoaderName)]
        public static extern IntPtr ppf_RtcRemoteAudioPropertiesInfo_GetStreamKey(IntPtr obj);

        [DllImport(LoaderName)]
        public static extern IntPtr ppf_RtcRemoteAudioPropertiesInfo_GetAudioPropertiesInfo(IntPtr obj);

        [DllImport(LoaderName, EntryPoint = "ppf_RtcRemoteAudioPropertiesReport_GetTotalRemoteVolume")]
        public static extern int ppf_RtcRemoteAudioPropertiesReport_GetTotalRemoteVolume(IntPtr obj);

        [DllImport(LoaderName, EntryPoint = "ppf_Message_GetRtcMediaDeviceChangeInfo")]
        public static extern IntPtr ppf_Message_GetRtcMediaDeviceChangeInfo(IntPtr obj);

        [DllImport(LoaderName, EntryPoint = "ppf_Message_GetRtcMuteInfo")]
        public static extern IntPtr ppf_Message_GetRtcMuteInfo(IntPtr obj);

        [DllImport(LoaderName, EntryPoint = "ppf_Message_GetRtcLocalAudioPropertiesReport")]
        public static extern IntPtr ppf_Message_GetRtcLocalAudioPropertiesReport(IntPtr obj);

        [DllImport(LoaderName, EntryPoint = "ppf_Message_GetRtcRemoteAudioPropertiesReport")]
        public static extern IntPtr ppf_Message_GetRtcRemoteAudioPropertiesReport(IntPtr obj);


        [DllImport(LoaderName, EntryPoint = "ppf_Rtc_EnableAudioPropertiesReport")]
        public static extern void ppf_Rtc_EnableAudioPropertiesReport(IntPtr config);

        [DllImport(LoaderName)]
        public static extern void ppf_Rtc_SetAudioPlaybackDevice(RtcAudioPlaybackDevice device);

        [DllImport(LoaderName)]
        public static extern IntPtr ppf_RtcAudioPropertyOptions_Create();


        [DllImport(LoaderName, EntryPoint = "ppf_RtcAudioPropertyOptions_SetInterval")]
        public static extern void ppf_RtcAudioPropertyOptions_SetInterval(IntPtr obj, int value);


        [DllImport(LoaderName, EntryPoint = "ppf_RtcAudioPropertyOptions_Destroy")]
        public static extern void ppf_RtcAudioPropertyOptions_Destroy(IntPtr obj);


        [DllImport(LoaderName, EntryPoint = "ppf_Rtc_RoomPauseAllSubscribedStream")]
        public static extern void ppf_Rtc_RoomPauseAllSubscribedStream_Native(IntPtr roomId, RtcPauseResumeMediaType mediaType);


        public static void ppf_Rtc_RoomPauseAllSubscribedStream(string roomId, RtcPauseResumeMediaType mediaType)
        {
            var roomIdNative = StringToNative(roomId);
            ppf_Rtc_RoomPauseAllSubscribedStream_Native(roomIdNative, mediaType);
            Marshal.FreeCoTaskMem(roomIdNative);
        }

        [DllImport(LoaderName, EntryPoint = "ppf_Rtc_RoomResumeAllSubscribedStream")]
        public static extern void ppf_Rtc_RoomResumeAllSubscribedStream_Native(IntPtr roomId, RtcPauseResumeMediaType mediaType);


        public static void ppf_Rtc_RoomResumeAllSubscribedStream(string roomId, RtcPauseResumeMediaType mediaType)
        {
            var roomIdNative = StringToNative(roomId);
            ppf_Rtc_RoomResumeAllSubscribedStream_Native(roomIdNative, mediaType);
            Marshal.FreeCoTaskMem(roomIdNative);
        }
    }

    public partial class CLIB
    {
        public static string ppf_DataStore_GetKey_String(IntPtr jarg1, int jarg2)
        {
            var ptr = ppf_DataStore_GetKey(jarg1, jarg2);
            return Marshal.PtrToStringAuto(ptr);
        }

        public static string ppf_DataStore_GetValue_String(IntPtr jarg1, string jarg2)
        {
            var ptr = ppf_DataStore_GetValue(jarg1, jarg2);
            return Marshal.PtrToStringAuto(ptr);
        }

        public static string ppf_MatchmakingEnqueueResult_GetPool_String(IntPtr jarg1)
        {
            var ptr = ppf_MatchmakingEnqueueResult_GetPool(jarg1);
            return Marshal.PtrToStringAuto(ptr);
        }

        public static string ppf_Room_GetDescription_String(IntPtr jarg1)
        {
            var ptr = ppf_Room_GetDescription(jarg1);
            return Marshal.PtrToStringAuto(ptr);
        }

        public static string ppf_Packet_GetSenderID_String(IntPtr jarg1)
        {
            var ptr = ppf_Packet_GetSenderID(jarg1);
            return Marshal.PtrToStringAuto(ptr);
        }

        public static Dictionary<string, string> DataStoreFromNative(IntPtr ppfDataStore)
        {
            var map = new Dictionary<string, string>();
            var size = (int) CLIB.ppf_DataStore_GetNumKeys(ppfDataStore);
            for (var i = 0; i < size; i++)
            {
                string key = CLIB.ppf_DataStore_GetKey_String(ppfDataStore, i);
                map[key] = CLIB.ppf_DataStore_GetValue_String(ppfDataStore, key);
            }

            return map;
        }
    }

    public partial class CLIB
    {
        [DllImport(LoaderName, EntryPoint = "ppf_Game_InitializeWithToken")]
        public static extern ulong ppf_Game_InitializeWithToken([MarshalAs(UnmanagedType.LPStr)] string jarg1);

        [DllImport(LoaderName, EntryPoint = "ppf_Game_UnInitialize")]
        public static extern bool ppf_Game_UnInitialize();

        [DllImport(LoaderName, EntryPoint = "ppf_DataStore_Contains")]
        public static extern uint ppf_DataStore_Contains(IntPtr jarg1, [MarshalAs(UnmanagedType.LPStr)] string jarg2);

        [DllImport(LoaderName, EntryPoint = "ppf_DataStore_GetKey")]
        public static extern IntPtr ppf_DataStore_GetKey(IntPtr jarg1, int jarg2);

        [DllImport(LoaderName, EntryPoint = "ppf_DataStore_GetNumKeys")]
        public static extern uint ppf_DataStore_GetNumKeys(IntPtr jarg1);

        [DllImport(LoaderName, EntryPoint = "ppf_DataStore_GetValue")]
        public static extern IntPtr ppf_DataStore_GetValue(IntPtr jarg1, [MarshalAs(UnmanagedType.LPStr)] string jarg2);

        [DllImport(LoaderName, EntryPoint = "ppf_Net_ReadPacket")]
        public static extern IntPtr ppf_Net_ReadPacket();

        [DllImport(LoaderName, EntryPoint = "ppf_Net_SendPacket")]
        public static extern bool ppf_Net_SendPacket([MarshalAs(UnmanagedType.LPStr)] string jarg1, uint jarg2, IntPtr jarg3);

        [DllImport(LoaderName, EntryPoint = "ppf_Net_SendPacketToCurrentRoom")]
        public static extern bool ppf_Net_SendPacketToCurrentRoom(uint jarg1, IntPtr jarg2);

        [DllImport(LoaderName, EntryPoint = "ppf_KeyValuePair_Create")]
        public static extern IntPtr ppf_KeyValuePair_Create();

        [DllImport(LoaderName, EntryPoint = "ppf_KeyValuePair_Destroy")]
        public static extern void ppf_KeyValuePair_Destroy(IntPtr jarg1);

        [DllImport(LoaderName, EntryPoint = "ppf_KeyValuePair_GetKey")]
        public static extern IntPtr ppf_KeyValuePair_GetKey(IntPtr jarg1);

        [DllImport(LoaderName, EntryPoint = "ppf_KeyValuePair_SetKey")]
        public static extern void ppf_KeyValuePair_SetKey(IntPtr jarg1, [MarshalAs(UnmanagedType.LPStr)] string jarg2);

        [DllImport(LoaderName, EntryPoint = "ppf_KeyValuePair_GetValueType")]
        public static extern int ppf_KeyValuePair_GetValueType(IntPtr jarg1);

        [DllImport(LoaderName, EntryPoint = "ppf_KeyValuePair_GetIntValue")]
        public static extern int ppf_KeyValuePair_GetIntValue(IntPtr jarg1);

        [DllImport(LoaderName, EntryPoint = "ppf_KeyValuePair_SetIntValue")]
        public static extern void ppf_KeyValuePair_SetIntValue(IntPtr jarg1, int jarg2);

        [DllImport(LoaderName, EntryPoint = "ppf_KeyValuePair_GetDoubleValue")]
        public static extern double ppf_KeyValuePair_GetDoubleValue(IntPtr jarg1);

        [DllImport(LoaderName, EntryPoint = "ppf_KeyValuePair_SetDoubleValue")]
        public static extern void ppf_KeyValuePair_SetDoubleValue(IntPtr jarg1, double jarg2);

        [DllImport(LoaderName, EntryPoint = "ppf_KeyValuePair_GetStringValue")]
        public static extern IntPtr ppf_KeyValuePair_GetStringValue(IntPtr jarg1);

        [DllImport(LoaderName, EntryPoint = "ppf_KeyValuePair_SetStringValue")]
        public static extern void ppf_KeyValuePair_SetStringValue(IntPtr jarg1, [MarshalAs(UnmanagedType.LPStr)] string jarg2);

        [DllImport(LoaderName, EntryPoint = "ppf_KeyValuePairArray_Create")]
        public static extern IntPtr ppf_KeyValuePairArray_Create(uint jarg1);

        [DllImport(LoaderName, EntryPoint = "ppf_KeyValuePairArray_Destroy")]
        public static extern void ppf_KeyValuePairArray_Destroy(IntPtr jarg1);

        [DllImport(LoaderName, EntryPoint = "ppf_KeyValuePairArray_GetElement")]
        public static extern IntPtr ppf_KeyValuePairArray_GetElement(IntPtr jarg1, uint jarg2);

        [DllImport(LoaderName, EntryPoint = "ppfKeyValuePairType_ToString")]
        public static extern IntPtr ppfKeyValuePairType_ToString(int jarg1);

        [DllImport(LoaderName, EntryPoint = "ppfKeyValuePairType_FromString")]
        public static extern int ppfKeyValuePairType_FromString([MarshalAs(UnmanagedType.LPStr)] string jarg1);

        [DllImport(LoaderName, EntryPoint = "ppf_MatchmakingAdminSnapshot_GetCandidates")]
        public static extern IntPtr ppf_MatchmakingAdminSnapshot_GetCandidates(IntPtr jarg1);

        [DllImport(LoaderName, EntryPoint = "ppf_MatchmakingAdminSnapshot_GetMyCurrentThreshold")]
        public static extern double ppf_MatchmakingAdminSnapshot_GetMyCurrentThreshold(IntPtr jarg1);

        [DllImport(LoaderName, EntryPoint = "ppf_MatchmakingAdminSnapshotCandidate_GetCanMatch")]
        public static extern bool ppf_MatchmakingAdminSnapshotCandidate_GetCanMatch(IntPtr jarg1);

        [DllImport(LoaderName, EntryPoint = "ppf_MatchmakingAdminSnapshotCandidate_GetMyTotalScore")]
        public static extern double ppf_MatchmakingAdminSnapshotCandidate_GetMyTotalScore(IntPtr jarg1);

        [DllImport(LoaderName, EntryPoint = "ppf_MatchmakingAdminSnapshotCandidate_GetTheirCurrentThreshold")]
        public static extern double ppf_MatchmakingAdminSnapshotCandidate_GetTheirCurrentThreshold(IntPtr jarg1);

        [DllImport(LoaderName, EntryPoint = "ppf_MatchmakingAdminSnapshotCandidateArray_GetElement")]
        public static extern IntPtr ppf_MatchmakingAdminSnapshotCandidateArray_GetElement(IntPtr jarg1, uint jarg2);

        [DllImport(LoaderName, EntryPoint = "ppf_MatchmakingAdminSnapshotCandidateArray_GetSize")]
        public static extern int ppf_MatchmakingAdminSnapshotCandidateArray_GetSize(IntPtr jarg1);

        [DllImport(LoaderName, EntryPoint = "ppf_MatchmakingBrowseResult_GetEnqueueResult")]
        public static extern IntPtr ppf_MatchmakingBrowseResult_GetEnqueueResult(IntPtr jarg1);

        [DllImport(LoaderName, EntryPoint = "ppf_MatchmakingBrowseResult_GetRooms")]
        public static extern IntPtr ppf_MatchmakingBrowseResult_GetRooms(IntPtr jarg1);

        [DllImport(LoaderName, EntryPoint = "ppfMatchmakingCriterionImportance_ToString")]
        public static extern IntPtr ppfMatchmakingCriterionImportance_ToString(int jarg1);

        [DllImport(LoaderName, EntryPoint = "ppfMatchmakingCriterionImportance_FromString")]
        public static extern int ppfMatchmakingCriterionImportance_FromString([MarshalAs(UnmanagedType.LPStr)] string jarg1);

        [DllImport(LoaderName, EntryPoint = "ppf_MatchmakingEnqueueResult_GetAdminSnapshot")]
        public static extern IntPtr ppf_MatchmakingEnqueueResult_GetAdminSnapshot(IntPtr jarg1);

        [DllImport(LoaderName, EntryPoint = "ppf_MatchmakingEnqueueResult_GetAverageWait")]
        public static extern uint ppf_MatchmakingEnqueueResult_GetAverageWait(IntPtr jarg1);

        [DllImport(LoaderName, EntryPoint = "ppf_MatchmakingEnqueueResult_GetMatchesInLastHourCount")]
        public static extern uint ppf_MatchmakingEnqueueResult_GetMatchesInLastHourCount(IntPtr jarg1);

        [DllImport(LoaderName, EntryPoint = "ppf_MatchmakingEnqueueResult_GetMaxExpectedWait")]
        public static extern uint ppf_MatchmakingEnqueueResult_GetMaxExpectedWait(IntPtr jarg1);

        [DllImport(LoaderName, EntryPoint = "ppf_MatchmakingEnqueueResult_GetRecentMatchPercentage")]
        public static extern uint ppf_MatchmakingEnqueueResult_GetRecentMatchPercentage(IntPtr jarg1);

        [DllImport(LoaderName, EntryPoint = "ppf_MatchmakingEnqueueResult_GetPool")]
        public static extern IntPtr ppf_MatchmakingEnqueueResult_GetPool(IntPtr jarg1);

        [DllImport(LoaderName, EntryPoint = "ppf_MatchmakingEnqueueResultAndRoom_GetMatchmakingEnqueueResult")]
        public static extern IntPtr ppf_MatchmakingEnqueueResultAndRoom_GetMatchmakingEnqueueResult(IntPtr jarg1);

        [DllImport(LoaderName, EntryPoint = "ppf_MatchmakingEnqueueResultAndRoom_GetRoom")]
        public static extern IntPtr ppf_MatchmakingEnqueueResultAndRoom_GetRoom(IntPtr jarg1);

        [DllImport(LoaderName, EntryPoint = "ppf_MatchmakingOptions_Create")]
        public static extern IntPtr ppf_MatchmakingOptions_Create();

        [DllImport(LoaderName, EntryPoint = "ppf_MatchmakingOptions_Destroy")]
        public static extern void ppf_MatchmakingOptions_Destroy(IntPtr jarg1);

        [DllImport(LoaderName, EntryPoint = "ppf_MatchmakingOptions_SetCreateRoomDataStoreString")]
        public static extern void ppf_MatchmakingOptions_SetCreateRoomDataStoreString(IntPtr jarg1, [MarshalAs(UnmanagedType.LPStr)] string jarg2, [MarshalAs(UnmanagedType.LPStr)] string jarg3);

        [DllImport(LoaderName, EntryPoint = "ppf_MatchmakingOptions_ClearCreateRoomDataStore")]
        public static extern void ppf_MatchmakingOptions_ClearCreateRoomDataStore(IntPtr jarg1);

        [DllImport(LoaderName, EntryPoint = "ppf_MatchmakingOptions_SetCreateRoomJoinPolicy")]
        public static extern void ppf_MatchmakingOptions_SetCreateRoomJoinPolicy(IntPtr jarg1, int jarg2);

        [DllImport(LoaderName, EntryPoint = "ppf_MatchmakingOptions_SetCreateRoomMaxUsers")]
        public static extern void ppf_MatchmakingOptions_SetCreateRoomMaxUsers(IntPtr jarg1, uint jarg2);

        [DllImport(LoaderName, EntryPoint = "ppf_MatchmakingOptions_ClearEnqueueAdditionalUsers")]
        public static extern void ppf_MatchmakingOptions_ClearEnqueueAdditionalUsers(IntPtr jarg1);

        [DllImport(LoaderName, EntryPoint = "ppf_MatchmakingOptions_SetEnqueueDataSettingsInt")]
        public static extern void ppf_MatchmakingOptions_SetEnqueueDataSettingsInt(IntPtr jarg1, [MarshalAs(UnmanagedType.LPStr)] string jarg2, int jarg3);

        [DllImport(LoaderName, EntryPoint = "ppf_MatchmakingOptions_SetEnqueueDataSettingsDouble")]
        public static extern void ppf_MatchmakingOptions_SetEnqueueDataSettingsDouble(IntPtr jarg1, [MarshalAs(UnmanagedType.LPStr)] string jarg2, double jarg3);

        [DllImport(LoaderName, EntryPoint = "ppf_MatchmakingOptions_SetEnqueueDataSettingsString")]
        public static extern void ppf_MatchmakingOptions_SetEnqueueDataSettingsString(IntPtr jarg1, [MarshalAs(UnmanagedType.LPStr)] string jarg2, [MarshalAs(UnmanagedType.LPStr)] string jarg3);

        [DllImport(LoaderName, EntryPoint = "ppf_MatchmakingOptions_ClearEnqueueDataSettings")]
        public static extern void ppf_MatchmakingOptions_ClearEnqueueDataSettings(IntPtr jarg1);

        [DllImport(LoaderName, EntryPoint = "ppf_MatchmakingOptions_SetEnqueueIsDebug")]
        public static extern void ppf_MatchmakingOptions_SetEnqueueIsDebug(IntPtr jarg1, bool jarg2);

        [DllImport(LoaderName, EntryPoint = "ppf_MatchmakingOptions_SetEnqueueQueryKey")]
        public static extern void ppf_MatchmakingOptions_SetEnqueueQueryKey(IntPtr jarg1, [MarshalAs(UnmanagedType.LPStr)] string jarg2);

        [DllImport(LoaderName, EntryPoint = "ppf_MatchmakingRoom_GetPingTime")]
        public static extern uint ppf_MatchmakingRoom_GetPingTime(IntPtr jarg1);

        [DllImport(LoaderName, EntryPoint = "ppf_MatchmakingRoom_GetRoom")]
        public static extern IntPtr ppf_MatchmakingRoom_GetRoom(IntPtr jarg1);

        [DllImport(LoaderName, EntryPoint = "ppf_MatchmakingRoom_HasPingTime")]
        public static extern bool ppf_MatchmakingRoom_HasPingTime(IntPtr jarg1);

        [DllImport(LoaderName, EntryPoint = "ppf_MatchmakingRoomArray_GetElement")]
        public static extern IntPtr ppf_MatchmakingRoomArray_GetElement(IntPtr jarg1, uint jarg2);

        [DllImport(LoaderName, EntryPoint = "ppf_MatchmakingRoomArray_GetSize")]
        public static extern int ppf_MatchmakingRoomArray_GetSize(IntPtr jarg1);

        [DllImport(LoaderName, EntryPoint = "ppf_MatchmakingStats_GetDrawCount")]
        public static extern uint ppf_MatchmakingStats_GetDrawCount(IntPtr jarg1);

        [DllImport(LoaderName, EntryPoint = "ppf_MatchmakingStats_GetLossCount")]
        public static extern uint ppf_MatchmakingStats_GetLossCount(IntPtr jarg1);

        [DllImport(LoaderName, EntryPoint = "ppf_MatchmakingStats_GetSkillLevel")]
        public static extern uint ppf_MatchmakingStats_GetSkillLevel(IntPtr jarg1);

        [DllImport(LoaderName, EntryPoint = "ppf_MatchmakingStats_GetSkillMean")]
        public static extern double ppf_MatchmakingStats_GetSkillMean(IntPtr jarg1);

        [DllImport(LoaderName, EntryPoint = "ppf_MatchmakingStats_GetSkillStandardDeviation")]
        public static extern double ppf_MatchmakingStats_GetSkillStandardDeviation(IntPtr jarg1);

        [DllImport(LoaderName, EntryPoint = "ppf_MatchmakingStats_GetWinCount")]
        public static extern uint ppf_MatchmakingStats_GetWinCount(IntPtr jarg1);

        [DllImport(LoaderName, EntryPoint = "ppf_Message_GetMatchmakingAdminSnapshot")]
        public static extern IntPtr ppf_Message_GetMatchmakingAdminSnapshot(IntPtr jarg1);

        [DllImport(LoaderName, EntryPoint = "ppf_Message_GetMatchmakingBrowseResult")]
        public static extern IntPtr ppf_Message_GetMatchmakingBrowseResult(IntPtr jarg1);

        [DllImport(LoaderName, EntryPoint = "ppf_Message_GetMatchmakingEnqueueResult")]
        public static extern IntPtr ppf_Message_GetMatchmakingEnqueueResult(IntPtr jarg1);

        [DllImport(LoaderName, EntryPoint = "ppf_Message_GetMatchmakingEnqueueResultAndRoom")]
        public static extern IntPtr ppf_Message_GetMatchmakingEnqueueResultAndRoom(IntPtr jarg1);

        [DllImport(LoaderName, EntryPoint = "ppf_Message_GetMatchmakingStats")]
        public static extern IntPtr ppf_Message_GetMatchmakingStats(IntPtr jarg1);

        [DllImport(LoaderName, EntryPoint = "ppf_Message_GetRoom")]
        public static extern IntPtr ppf_Message_GetRoom(IntPtr jarg1);

        [DllImport(LoaderName, EntryPoint = "ppf_Message_GetRoomArray")]
        public static extern IntPtr ppf_Message_GetRoomArray(IntPtr jarg1);

        [DllImport(LoaderName, EntryPoint = "ppf_Message_GetPlatformGameInitialize")]
        public static extern IntPtr ppf_Message_GetPlatformGameInitialize(IntPtr jarg1);

        [DllImport(LoaderName, EntryPoint = "ppf_Message_GetGameConnectionEvent")]
        public static extern int ppf_Message_GetGameConnectionEvent(IntPtr jarg1);

        [DllImport(LoaderName, EntryPoint = "ppf_Message_GetGameRequestFailedReason")]
        public static extern int ppf_Message_GetGameRequestFailedReason(IntPtr jarg1);

        // [DllImport(LoaderName, EntryPoint="ppf_Message_GetRtcAudioPlaybackDevice")]
        // public static extern IntPtr ppf_Message_GetRtcAudioPlaybackDevice(IntPtr jarg1);

        [DllImport(LoaderName, EntryPoint = "ppf_Packet_Free")]
        public static extern void ppf_Packet_Free(IntPtr jarg1);

        [DllImport(LoaderName, EntryPoint = "ppf_Packet_GetBytes")]
        public static extern IntPtr ppf_Packet_GetBytes(IntPtr jarg1);

        [DllImport(LoaderName, EntryPoint = "ppf_Packet_GetSenderID")]
        public static extern IntPtr ppf_Packet_GetSenderID(IntPtr jarg1);

        [DllImport(LoaderName, EntryPoint = "ppf_Packet_GetSize")]
        public static extern uint ppf_Packet_GetSize(IntPtr jarg1);

        [DllImport(LoaderName, EntryPoint = "ppf_PlatformGameInitialize_GetResult")]
        public static extern int ppf_PlatformGameInitialize_GetResult(IntPtr jarg1);

        [DllImport(LoaderName, EntryPoint = "ppfPlatformGameInitializeResult_ToString")]
        public static extern IntPtr ppfPlatformGameInitializeResult_ToString(int jarg1);

        [DllImport(LoaderName, EntryPoint = "ppfPlatformGameInitializeResult_FromString")]
        public static extern int ppfPlatformGameInitializeResult_FromString([MarshalAs(UnmanagedType.LPStr)] string jarg1);

        [DllImport(LoaderName, EntryPoint = "ppf_Matchmaking_Browse2")]
        public static extern ulong ppf_Matchmaking_Browse2([MarshalAs(UnmanagedType.LPStr)] string jarg1, IntPtr jarg2);

        [DllImport(LoaderName, EntryPoint = "ppf_Matchmaking_Cancel2")]
        public static extern ulong ppf_Matchmaking_Cancel2();

        [DllImport(LoaderName, EntryPoint = "ppf_Matchmaking_CreateAndEnqueueRoom2")]
        public static extern ulong ppf_Matchmaking_CreateAndEnqueueRoom2([MarshalAs(UnmanagedType.LPStr)] string jarg1, IntPtr jarg2);

        [DllImport(LoaderName, EntryPoint = "ppf_Matchmaking_Enqueue2")]
        public static extern ulong ppf_Matchmaking_Enqueue2([MarshalAs(UnmanagedType.LPStr)] string jarg1, IntPtr jarg2);

        [DllImport(LoaderName, EntryPoint = "ppf_Matchmaking_GetAdminSnapshot")]
        public static extern ulong ppf_Matchmaking_GetAdminSnapshot();

        [DllImport(LoaderName, EntryPoint = "ppf_Matchmaking_GetStats")]
        public static extern ulong ppf_Matchmaking_GetStats([MarshalAs(UnmanagedType.LPStr)] string jarg1, uint jarg2, int jarg3);

        [DllImport(LoaderName, EntryPoint = "ppf_Matchmaking_ReportResultInsecure")]
        public static extern ulong ppf_Matchmaking_ReportResultInsecure(ulong jarg1, IntPtr jarg2, uint jarg3);

        [DllImport(LoaderName, EntryPoint = "ppf_Matchmaking_StartMatch")]
        public static extern ulong ppf_Matchmaking_StartMatch(ulong jarg1);

        [DllImport(LoaderName, EntryPoint = "ppf_Matchmaking_CrashTest")]
        public static extern ulong ppf_Matchmaking_CrashTest();

        [DllImport(LoaderName, EntryPoint = "ppf_Room_CreateAndJoinPrivate2")]
        public static extern ulong ppf_Room_CreateAndJoinPrivate2(int jarg1, uint jarg2, IntPtr jarg3);

        [DllImport(LoaderName, EntryPoint = "ppf_Room_Get")]
        public static extern ulong ppf_Room_Get(ulong jarg1);

        [DllImport(LoaderName, EntryPoint = "ppf_Room_GetCurrent")]
        public static extern ulong ppf_Room_GetCurrent();

        [DllImport(LoaderName, EntryPoint = "ppf_Room_GetCurrentForUser")]
        public static extern ulong ppf_Room_GetCurrentForUser([MarshalAs(UnmanagedType.LPStr)] string jarg1);
        
        [DllImport(LoaderName, EntryPoint = "ppf_Room_GetModeratedRooms")]
        public static extern ulong ppf_Room_GetModeratedRooms(int jarg1, int jarg2);

        [DllImport(LoaderName, EntryPoint = "ppf_Room_Join2")]
        public static extern ulong ppf_Room_Join2(ulong jarg1, IntPtr jarg2);

        [DllImport(LoaderName, EntryPoint = "ppf_Room_KickUser")]
        public static extern ulong ppf_Room_KickUser(ulong jarg1, [MarshalAs(UnmanagedType.LPStr)] string jarg2, int jarg3);
        
        [DllImport(LoaderName, EntryPoint = "ppf_Room_Leave")]
        public static extern ulong ppf_Room_Leave(ulong jarg1);

        [DllImport(LoaderName, EntryPoint = "ppf_Room_SetDescription")]
        public static extern ulong ppf_Room_SetDescription(ulong jarg1, [MarshalAs(UnmanagedType.LPStr)] string jarg2);

        [DllImport(LoaderName, EntryPoint = "ppf_Room_UpdateDataStore")]
        public static extern ulong ppf_Room_UpdateDataStore(ulong jarg1, IntPtr jarg2, uint jarg3);

        [DllImport(LoaderName, EntryPoint = "ppf_Room_UpdateMembershipLockStatus")]
        public static extern ulong ppf_Room_UpdateMembershipLockStatus(ulong jarg1, int jarg2);

        [DllImport(LoaderName, EntryPoint = "ppf_Room_UpdateOwner")]
        public static extern ulong ppf_Room_UpdateOwner(ulong jarg1, [MarshalAs(UnmanagedType.LPStr)] string jarg2);

        [DllImport(LoaderName, EntryPoint = "ppf_Room_UpdatePrivateRoomJoinPolicy")]
        public static extern ulong ppf_Room_UpdatePrivateRoomJoinPolicy(ulong jarg1, int jarg2);

        [DllImport(LoaderName, EntryPoint = "ppf_Room_GetDataStore")]
        public static extern IntPtr ppf_Room_GetDataStore(IntPtr jarg1);

        [DllImport(LoaderName, EntryPoint = "ppf_Room_GetOwner")]
        public static extern IntPtr ppf_Room_GetOwner(IntPtr jarg1);

        [DllImport(LoaderName, EntryPoint = "ppf_Room_GetUsers")]
        public static extern IntPtr ppf_Room_GetUsers(IntPtr jarg1);

        [DllImport(LoaderName, EntryPoint = "ppf_Room_GetDescription")]
        public static extern IntPtr ppf_Room_GetDescription(IntPtr jarg1);

        [DllImport(LoaderName, EntryPoint = "ppf_Room_GetID")]
        public static extern ulong ppf_Room_GetID(IntPtr jarg1);

        [DllImport(LoaderName, EntryPoint = "ppf_Room_GetIsMembershipLocked")]
        public static extern bool ppf_Room_GetIsMembershipLocked(IntPtr jarg1);

        [DllImport(LoaderName, EntryPoint = "ppf_Room_GetJoinPolicy")]
        public static extern int ppf_Room_GetJoinPolicy(IntPtr jarg1);

        [DllImport(LoaderName, EntryPoint = "ppf_Room_GetJoinability")]
        public static extern int ppf_Room_GetJoinability(IntPtr jarg1);

        [DllImport(LoaderName, EntryPoint = "ppf_Room_GetMaxUsers")]
        public static extern uint ppf_Room_GetMaxUsers(IntPtr jarg1);

        [DllImport(LoaderName, EntryPoint = "ppf_Room_GetType")]
        public static extern int ppf_Room_GetType(IntPtr jarg1);

        [DllImport(LoaderName, EntryPoint = "ppf_RoomArray_GetElement")]
        public static extern IntPtr ppf_RoomArray_GetElement(IntPtr jarg1, uint jarg2);

        [DllImport(LoaderName, EntryPoint = "ppf_RoomArray_GetSize")]
        public static extern int ppf_RoomArray_GetSize(IntPtr jarg1);

        [DllImport(LoaderName, EntryPoint = "ppf_RoomArray_HasNextPage")]
        public static extern bool ppf_RoomArray_HasNextPage(IntPtr jarg1);

        [DllImport(LoaderName, EntryPoint = "ppf_RoomArray_GetPageSize")]
        public static extern int ppf_RoomArray_GetPageSize(IntPtr jarg1);

        [DllImport(LoaderName, EntryPoint = "ppf_RoomArray_GetPageIndex")]
        public static extern int ppf_RoomArray_GetPageIndex(IntPtr jarg1);

        [DllImport(LoaderName, EntryPoint = "ppfRoomJoinability_ToString")]
        public static extern IntPtr ppfRoomJoinability_ToString(int jarg1);

        [DllImport(LoaderName, EntryPoint = "ppfRoomJoinability_FromString")]
        public static extern int ppfRoomJoinability_FromString([MarshalAs(UnmanagedType.LPStr)] string jarg1);

        [DllImport(LoaderName, EntryPoint = "ppfRoomJoinPolicy_ToString")]
        public static extern IntPtr ppfRoomJoinPolicy_ToString(int jarg1);

        [DllImport(LoaderName, EntryPoint = "ppfRoomJoinPolicy_FromString")]
        public static extern int ppfRoomJoinPolicy_FromString([MarshalAs(UnmanagedType.LPStr)] string jarg1);

        [DllImport(LoaderName, EntryPoint = "ppfRoomMembershipLockStatus_ToString")]
        public static extern IntPtr ppfRoomMembershipLockStatus_ToString(int jarg1);

        [DllImport(LoaderName, EntryPoint = "ppfRoomMembershipLockStatus_FromString")]
        public static extern int ppfRoomMembershipLockStatus_FromString([MarshalAs(UnmanagedType.LPStr)] string jarg1);

        [DllImport(LoaderName, EntryPoint = "ppf_RoomOptions_Create")]
        public static extern IntPtr ppf_RoomOptions_Create();

        [DllImport(LoaderName, EntryPoint = "ppf_RoomOptions_Destroy")]
        public static extern void ppf_RoomOptions_Destroy(IntPtr jarg1);

        [DllImport(LoaderName, EntryPoint = "ppf_RoomOptions_SetDataStoreString")]
        public static extern void ppf_RoomOptions_SetDataStoreString(IntPtr jarg1, [MarshalAs(UnmanagedType.LPStr)] string jarg2, [MarshalAs(UnmanagedType.LPStr)] string jarg3);

        [DllImport(LoaderName, EntryPoint = "ppf_RoomOptions_ClearDataStore")]
        public static extern void ppf_RoomOptions_ClearDataStore(IntPtr jarg1);

        [DllImport(LoaderName, EntryPoint = "ppf_RoomOptions_SetExcludeRecentlyMet")]
        public static extern void ppf_RoomOptions_SetExcludeRecentlyMet(IntPtr jarg1, bool jarg2);

        [DllImport(LoaderName, EntryPoint = "ppf_RoomOptions_SetMaxUserResults")]
        public static extern void ppf_RoomOptions_SetMaxUserResults(IntPtr jarg1, uint jarg2);

        [DllImport(LoaderName, EntryPoint = "ppf_RoomOptions_SetRoomId")]
        public static extern void ppf_RoomOptions_SetRoomId(IntPtr jarg1, ulong jarg2);

        [DllImport(LoaderName, EntryPoint = "ppf_RoomOptions_SetTurnOffUpdates")]
        public static extern void ppf_RoomOptions_SetTurnOffUpdates(IntPtr jarg1, bool jarg2);

        [DllImport(LoaderName, EntryPoint = "ppfRoomType_ToString")]
        public static extern IntPtr ppfRoomType_ToString(int jarg1);

        [DllImport(LoaderName, EntryPoint = "ppfRoomType_FromString")]
        public static extern int ppfRoomType_FromString([MarshalAs(UnmanagedType.LPStr)] string jarg1);
    }
}