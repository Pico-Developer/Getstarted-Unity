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

// This file is generated automatically. Please don't edit it.

namespace Pico.Platform
{
    public enum MessageType
    {
        Unknown = -1,
        PlatformInitializeAndroidAsynchronous = 1,
        User_GetLoggedInUser = 10000,
        User_GetAccessToken = 10001,
        User_Get = 10002,
        User_LaunchFriendRequestFlow = 10003,
        User_GetLoggedInUserFriends = 10004,
        User_GetNextUserArrayPage = 10005,

        #region RTC
        Notification_Rtc_OnRoomStats = 10200,
        Notification_Rtc_OnJoinRoom = 10201,
        Notification_Rtc_OnLeaveRoom = 10202,
        Notification_Rtc_OnUserLeaveRoom = 10203,
        Notification_Rtc_OnUserJoinRoom = 10204,
        Notification_Rtc_OnConnectionStateChange = 10205,
        Notification_Rtc_OnWarn = 10206,
        Notification_Rtc_OnRoomWarn = 10207,
        Notification_Rtc_OnRoomError = 10208,
        Notification_Rtc_OnError = 10209,
        Notification_Rtc_OnUserStartAudioCapture = 10210,
        Notification_Rtc_OnAudioPlaybackDeviceChanged = 10211,
        Notification_Rtc_OnRemoteAudioPropertiesReport = 10212,
        Notification_Rtc_OnLocalAudioPropertiesReport = 10213,
        Notification_Rtc_OnUserStopAudioCapture = 10214,
        Notification_Rtc_OnUserMuteAudio = 10215,
        Notification_Rtc_OnMediaDeviceStateChanged = 10216,
        Rtc_GetToken = 10300,
        #endregion
        #region stark game

        Matchmaking_Browse2 = 10400,
        Matchmaking_Cancel2 = 10402,
        Matchmaking_CreateAndEnqueueRoom2 = 10404,
        Matchmaking_Enqueue2 = 10408,
        Matchmaking_EnqueueRoom2 = 10410,
        Matchmaking_GetAdminSnapshot = 10411,
        Matchmaking_GetStats = 10412,
        Matchmaking_ReportResultInsecure = 10414,
        Matchmaking_StartMatch = 10415,
        Room_CreateAndJoinPrivate = 10500,
        Room_CreateAndJoinPrivate2 = 10501,
        Room_Get = 10502,
        Room_GetCurrent = 10503,
        Room_GetCurrentForUser = 10504,
        Room_GetInvitableUsers = 10505,
        Room_GetInvitableUsers2 = 10506,
        Room_GetModeratedRooms = 10507,
        Room_GetNextRoomArrayPage = 10508,
        Room_InviteUser = 10509,
        Room_Join = 10510,
        Room_Join2 = 10511,
        Room_KickUser = 10512,
        Room_LaunchInvitableUserFlow = 10513,
        Room_Leave = 10514,
        Room_SetDescription = 10515,
        Room_UpdateDataStore = 10516,
        Room_UpdateMembershipLockStatus = 10517,
        Room_UpdateOwner = 10518,
        Room_UpdatePrivateRoomJoinPolicy = 10519,
        Notification_Matchmaking_MatchFound = 10600,
        Notification_Room_InviteAccepted = 10601,
        Notification_Room_InviteReceived = 10602,
        Notification_Room_RoomUpdate = 10603,
        Notification_Game_ConnectionEvent = 10604,
        Notification_Game_RequestFailed = 10605,
        Notification_Game_StateReset = 10606,
        PlatformGameInitializeAsynchronous = 10700,

        #endregion stark game
    }
}