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
using Pico.Platform.Models;
using UnityEngine;

namespace Pico.Platform
{
    public class MessageQueue
    {
        public static Message Next()
        {
            if (!CoreService.Initialized)
            {
                return null;
            }

            var handle = CLIB.ppf_PopMessage();
            if (handle == IntPtr.Zero)
            {
                return null;
            }

            Message msg = ParseMessage(handle);
            CLIB.ppf_FreeMessage(handle);
            return msg;
        }

        public static Message ParseMessage(IntPtr msgPointer)
        {
            Message msg = null;
            MessageType messageType = CLIB.ppf_Message_GetType(msgPointer);
            switch (messageType)
            {
                case MessageType.PlatformInitializeAndroidAsynchronous:
                {
                    msg = new Message<PlatformInitializeResult>(msgPointer, ptr => { return (PlatformInitializeResult) CLIB.ppf_Message_GetInt32(ptr); });
                    break;
                }
                case MessageType.User_GetAccessToken:
                case MessageType.Rtc_GetToken:
                case MessageType.Notification_Rtc_OnUserStartAudioCapture:
                case MessageType.Notification_Rtc_OnUserStopAudioCapture:
                case MessageType.Notification_Room_InviteAccepted:
                {
                    msg = new Message<string>(msgPointer, ptr => { return CLIB.ppf_Message_GetString(ptr); });
                    break;
                }
                case MessageType.Notification_Rtc_OnRoomError:
                {
                    msg = new Message<RtcRoomError>(msgPointer, ptr =>
                    {
                        var obj = CLIB.ppf_Message_GetRtcRoomError(ptr);
                        return new RtcRoomError(obj);
                    });
                    break;
                }
                case MessageType.Notification_Rtc_OnRoomWarn:
                {
                    msg = new Message<RtcRoomWarn>(msgPointer, ptr =>
                    {
                        var obj = CLIB.ppf_Message_GetRtcRoomWarn(ptr);
                        return new RtcRoomWarn(obj);
                    });
                    break;
                }
                case MessageType.Notification_Rtc_OnConnectionStateChange:
                {
                    msg = new Message<RtcConnectionState>(msgPointer, ptr => { return (RtcConnectionState) CLIB.ppf_Message_GetInt32(ptr); });
                    break;
                }
                case MessageType.Notification_Rtc_OnError:
                case MessageType.Notification_Rtc_OnWarn:
                {
                    msg = new Message<Int32>(msgPointer, ptr => { return CLIB.ppf_Message_GetInt32(ptr); });
                    break;
                }
                case MessageType.User_GetLoggedInUser:
                case MessageType.User_Get:
                {
                    msg = new Message<Models.User>(msgPointer, ptr =>
                    {
                        var obj = CLIB.ppf_Message_GetUser(ptr);
                        return new Models.User(obj);
                    });
                    break;
                }
                case MessageType.User_LaunchFriendRequestFlow:
                {
                    msg = new Message<LaunchFriendResult>(msgPointer, ptr =>
                    {
                        var obj = CLIB.ppf_Message_GetLaunchFriendRequestFlowResult(ptr);
                        return new LaunchFriendResult(obj);
                    });
                    break;
                }
                case MessageType.User_GetLoggedInUserFriends:
                case MessageType.Room_GetInvitableUsers2:
                {
                    msg = new Message<UserList>(msgPointer, ptr =>
                    {
                        var obj = CLIB.ppf_Message_GetUserArray(ptr);
                        return new UserList(obj);
                    });
                    break;
                }
                case MessageType.Notification_Rtc_OnRoomStats:
                {
                    msg = new Message<RtcRoomStats>(msgPointer, ptr =>
                    {
                        var obj = CLIB.ppf_Message_GetRtcRoomStats(ptr);
                        return new RtcRoomStats(obj);
                    });
                    break;
                }
                case MessageType.Notification_Rtc_OnJoinRoom:
                {
                    msg = new Message<RtcJoinRoomResult>(msgPointer, ptr =>
                    {
                        var obj = CLIB.ppf_Message_GetRtcJoinRoomResult(ptr);
                        return new RtcJoinRoomResult(obj);
                    });
                    break;
                }
                case MessageType.Notification_Rtc_OnLeaveRoom:
                {
                    msg = new Message<RtcLeaveRoomResult>(msgPointer, ptr =>
                    {
                        var obj = CLIB.ppf_Message_GetRtcLeaveRoomResult(ptr);
                        return new RtcLeaveRoomResult(obj);
                    });
                    break;
                }

                case MessageType.Notification_Rtc_OnUserLeaveRoom:
                {
                    msg = new Message<RtcUserLeaveInfo>(msgPointer, ptr =>
                    {
                        var obj = CLIB.ppf_Message_GetRtcUserLeaveInfo(ptr);
                        return new RtcUserLeaveInfo(obj);
                    });
                    break;
                }

                case MessageType.Notification_Rtc_OnUserJoinRoom:
                {
                    msg = new Message<RtcUserJoinInfo>(msgPointer, ptr =>
                    {
                        var obj = CLIB.ppf_Message_GetRtcUserJoinInfo(ptr);
                        return new RtcUserJoinInfo(obj);
                    });
                    break;
                }

                case MessageType.Notification_Rtc_OnAudioPlaybackDeviceChanged:
                {
                    msg = new Message<RtcAudioPlaybackDevice>(msgPointer, ptr => { return (RtcAudioPlaybackDevice) CLIB.ppf_Message_GetInt32(ptr); });
                    break;
                }

                case MessageType.Notification_Rtc_OnMediaDeviceStateChanged:
                {
                    msg = new Message<RtcMediaDeviceChangeInfo>(msgPointer, ptr =>
                    {
                        var obj = CLIB.ppf_Message_GetRtcMediaDeviceChangeInfo(ptr);
                        return new RtcMediaDeviceChangeInfo(obj);
                    });
                    break;
                }

                case MessageType.Notification_Rtc_OnLocalAudioPropertiesReport:
                {
                    msg = new Message<RtcLocalAudioPropertiesReport>(msgPointer, ptr =>
                    {
                        var obj = CLIB.ppf_Message_GetRtcLocalAudioPropertiesReport(ptr);
                        return new RtcLocalAudioPropertiesReport(obj);
                    });
                    break;
                }

                case MessageType.Notification_Rtc_OnRemoteAudioPropertiesReport:
                {
                    msg = new Message<RtcRemoteAudioPropertiesReport>(msgPointer, ptr =>
                    {
                        var obj = CLIB.ppf_Message_GetRtcRemoteAudioPropertiesReport(ptr);
                        return new RtcRemoteAudioPropertiesReport(obj);
                    });
                    break;
                }

                case MessageType.Notification_Rtc_OnUserMuteAudio:
                {
                    msg = new Message<RtcMuteInfo>(msgPointer, ptr =>
                    {
                        var obj = CLIB.ppf_Message_GetRtcMuteInfo(ptr);
                        return new RtcMuteInfo(obj);
                    });
                    break;
                }

                #region stark game

                case MessageType.Matchmaking_Cancel2:
                case MessageType.Matchmaking_ReportResultInsecure:
                case MessageType.Matchmaking_StartMatch:
                case MessageType.Room_LaunchInvitableUserFlow:
                case MessageType.Room_UpdateOwner:
                case MessageType.Notification_Game_StateReset:
                {
                    msg = new Message(msgPointer);
                    break;
                }
                case MessageType.Matchmaking_GetAdminSnapshot:
                {
                    msg = new Message<MatchmakingAdminSnapshot>(msgPointer, ptr =>
                    {
                        var obj = CLIB.ppf_Message_GetMatchmakingAdminSnapshot(ptr);
                        return new MatchmakingAdminSnapshot(obj);
                    });
                    break;
                }
                case MessageType.Matchmaking_Browse2:
                {
                    msg = new Message<MatchmakingBrowseResult>(msgPointer, ptr =>
                    {
                        var obj = CLIB.ppf_Message_GetMatchmakingBrowseResult(ptr);
                        return new MatchmakingBrowseResult(obj);
                    });
                    break;
                }
                case MessageType.Matchmaking_Enqueue2:
                case MessageType.Matchmaking_EnqueueRoom2:
                {
                    msg = new Message<MatchmakingEnqueueResult>(msgPointer, ptr =>
                    {
                        var obj = CLIB.ppf_Message_GetMatchmakingEnqueueResult(ptr);
                        return new MatchmakingEnqueueResult(obj);
                    });
                    break;
                }
                case MessageType.Matchmaking_CreateAndEnqueueRoom2:
                {
                    msg = new Message<MatchmakingEnqueueResultAndRoom>(msgPointer, ptr =>
                    {
                        var obj = CLIB.ppf_Message_GetMatchmakingEnqueueResultAndRoom(ptr);
                        return new MatchmakingEnqueueResultAndRoom(obj);
                    });
                    break;
                }

                case MessageType.Matchmaking_GetStats:
                {
                    msg = new Message<MatchmakingStats>(msgPointer, ptr =>
                    {
                        var obj = CLIB.ppf_Message_GetMatchmakingStats(ptr);
                        return new MatchmakingStats(obj);
                    });
                    break;
                }
                case MessageType.Room_GetCurrent:
                case MessageType.Room_GetCurrentForUser:
                case MessageType.Notification_Room_RoomUpdate:
                case MessageType.Room_CreateAndJoinPrivate:
                case MessageType.Room_CreateAndJoinPrivate2:
                case MessageType.Room_InviteUser:
                case MessageType.Room_Join:
                case MessageType.Room_Join2:
                case MessageType.Room_KickUser:
                case MessageType.Room_Leave:
                case MessageType.Room_SetDescription:
                case MessageType.Room_UpdateDataStore:
                case MessageType.Room_UpdateMembershipLockStatus:
                case MessageType.Room_UpdatePrivateRoomJoinPolicy:
                case MessageType.Notification_Matchmaking_MatchFound:
                case MessageType.Room_Get:
                {
                    msg = new Message<Models.Room>(msgPointer, ptr =>
                    {
                        var obj = CLIB.ppf_Message_GetRoom(ptr);
                        return new Models.Room(obj);
                    });
                    break;
                }
                case MessageType.Room_GetModeratedRooms:
                case MessageType.Room_GetNextRoomArrayPage:
                {
                    msg = new Message<RoomList>(msgPointer, ptr =>
                    {
                        var obj = CLIB.ppf_Message_GetRoomArray(ptr);
                        return new RoomList(obj);
                    });
                    break;
                }
                case MessageType.PlatformGameInitializeAsynchronous:
                {
                    msg = new Message<GameInitializeResult>(msgPointer, ptr =>
                    {
                        var objHandle = CLIB.ppf_Message_GetPlatformGameInitialize(ptr);
                        var obj = CLIB.ppf_PlatformGameInitialize_GetResult(objHandle);
                        return (GameInitializeResult) obj;
                    });
                    break;
                }
                case MessageType.Notification_Game_ConnectionEvent:
                {
                    msg = new Message<GameConnectionEvent>(msgPointer, ptr =>
                    {
                        var obj = CLIB.ppf_Message_GetGameConnectionEvent(ptr);
                        return (GameConnectionEvent) obj;
                    });
                    break;
                }
                case MessageType.Notification_Game_RequestFailed:
                {
                    msg = new Message<GameRequestFailedReason>(msgPointer, ptr =>
                    {
                        var obj = CLIB.ppf_Message_GetGameRequestFailedReason(ptr);
                        return (GameRequestFailedReason) obj;
                    });
                    break;
                }

                #endregion stark game

                default:
                    Debug.LogError($"Unknown message type {messageType}");
                    break;
            }

            return msg;
        }
    }
}