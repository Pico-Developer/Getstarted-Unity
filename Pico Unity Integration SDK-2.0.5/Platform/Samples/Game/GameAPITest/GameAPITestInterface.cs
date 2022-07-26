using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Pico.Platform;
using Pico.Platform.Models;


namespace Pico.Platform.Samples.Game
{
    public partial class GameAPITest : MonoBehaviour
    {
        private static string tempToken = string.Empty;
        Dictionary<ParamName, string[]> paramsDic = new System.Collections.Generic.Dictionary<ParamName, string[]>()
        {
            [ParamName.PICO_ID] = new string[] { "PicoID", "" },
            [ParamName.ROOM_ID] = new string[] { "RoomID", "" },
            [ParamName.USER_ID] = new string[] { "UserID", "" },

            //SUBSCRIBE_TO_UPDATES,
            [ParamName.KICK_DURATION_SECONDS] = new string[] { "Kick time", "0" },
            [ParamName.DESCRIPTION] = new string[] { "Room Description", "room desc" },
            [ParamName.MEMBERSHIP_LOCK_STATUS] = new string[] { "Member lock status", "0" },
            [ParamName.INIT_HOST] = new string[] { "Host", "testhost" },
            [ParamName.INIT_PORT] = new string[] { "Port", "80" },
            [ParamName.INIT_TOKEN] = new string[] { "Token", "testtoken" },
            [ParamName.INIT_APPID] = new string[] { "AppID", "testappid" },

            [ParamName.POOL_NAME] = new string[] { "Pool name", "test_match_pool" }, //
            [ParamName.JOIN_POLICY] = new string[] { "JoinPolicy", "0" },

            [ParamName.MATCH_MAX_LEVEL] = new string[] { "MatchMaxLevel", "0" },
            [ParamName.MATCH_APPROACH] = new string[] { "MatchApproach", "0" },
            [ParamName.INVITE_TOKEN] = new string[] { "InviteToken", "" },

            //ROOM_OPTION_EXCLUDE_RECENTLY_MET,
            [ParamName.ROOM_OPTION_MAX_USER_RESULTS] = new string[] { "Max user results", "" },
            [ParamName.ROOM_OPTION_TURN_OFF_UPDATES] = new string[] { "Turn off updates", "" },

            [ParamName.ROOM_OPTION_DATASTORE_KEYS] = new string[] { "Keys", "" },
            [ParamName.ROOM_OPTION_DATASTORE_VALUES] = new string[] { "Valuse", "" },
            [ParamName.ROOM_OPTION_ELCLUDERECENTLYMET] = new string[] { "Elcluderecentlymet", "false" },


            [ParamName.MATCHMAKING_OPTION_ROOM_MAX_USERS] = new string[] { "Max users", "2" },
            [ParamName.MATCHMAKING_OPTION_ENQUEUE_IS_DEBUG] = new string[] { "EnqueueIsDebug", "" },
            [ParamName.MATCHMAKING_OPTION_ENQUEUE_QUERY_KEY] = new string[] { "Enqueue query key", "" },


            [ParamName.MATCHMAKING_OPTION_ENQUEUE_KEYS] = new string[] { "EnqueueKeys", "" },
            [ParamName.MATCHMAKING_OPTION_ENQUEUE_VALUES] = new string[] { "EnqueueValues", "" },
            [ParamName.MATCHMAKING_OPTION_ROOM_KEYS] = new string[] { "RoomKeys", "" },
            [ParamName.MATCHMAKING_OPTION_ROOM_VALUES] = new string[] { "RoomValues", "" },

            [ParamName.MATCHMAKING_REPORT_RESULT_KEYS] = new string[] { "ReportResultKeys", "" },
            [ParamName.MATCHMAKING_REPORT_RESULT_VALUES] = new string[] { "ReportResultValues", "" },

            [ParamName.MAX_USERS] = new string[] { "Max users", "2" },
            [ParamName.PACKET_BYTES] = new string[] { "Content", "test packetBytes" },
            [ParamName.ROOM_PAGE_INDEX] = new string[] { "Page index", "0" },
            [ParamName.ROOM_PAGE_SIZE] = new string[] { "Page size", "5" },
        };

        private static Dictionary<string, PPFFunctionConfig> initDic = new Dictionary<string, PPFFunctionConfig>()
        {
            ["GameInitialize"] = new PPFFunctionConfig(new PPFFunction((paramList) => {
                CoreService.Initialize();
                if (!CoreService.Initialized)
                {
                    LogHelper.LogError(TAG, "pico initialize failed");
                    return -1;
                }
                UserService.GetAccessToken().OnComplete(delegate (Message<string> message)
                {
                    if (message.IsError)
                    {
                        var err = message.GetError();
                        LogHelper.LogError(TAG, $"Got access token error {err.Message} code={err.Code}");
                        return;
                    }

                    string accessToken = message.Data;
                    LogHelper.LogInfo(TAG, $"Got accessToken {accessToken}, GameInitialize begin");

                    var request = CoreService.GameInitialize(accessToken);
                    if (request.TaskId != 0)
                    {
                        request.OnComplete(OnGameInitialize);
                    }
                    else
                    {
                        Debug.Log($"Core.GameInitialize requestID is 0! Repeated initialization or network error");
                    }
                });
                return 1;
            })),
            ["Uninitialize"] = new PPFFunctionConfig(new PPFFunction((paramList) => {
                Uninitialize();
                return true;
            })),
            ["GetLoggedInUser"] = new PPFFunctionConfig((paramList) =>
            {
                return UserService.GetLoggedInUser().OnComplete(OnLoggedInUser); 
            }),
            ["Only Pico initialize"] = new PPFFunctionConfig(new PPFFunction((paramList) => {
                CoreService.Initialize();
                return 1;
            })),
            ["GetAccessToken"] = new PPFFunctionConfig(new PPFFunction((paramList) => {
                if (!CoreService.Initialized)
                {
                    LogHelper.LogError(TAG, "pico initialize failed");
                    return -1;
                }
                UserService.GetAccessToken().OnComplete(delegate (Message<string> message)
                {
                    if (message.IsError)
                    {
                        var err = message.GetError();
                        LogHelper.LogError(TAG, $"Got access token error {err.Message} code={err.Code}");
                        return;
                    }

                    string accessToken = message.Data;
                    LogHelper.LogInfo(TAG, $"Got accessToken {accessToken}, GameInitialize begin");
                    tempToken = accessToken;
                });
                return 1;
            })),
            ["Only game initialize"] = new PPFFunctionConfig(new PPFFunction((paramList) => {
                var request = CoreService.GameInitialize(tempToken);
                if (request.TaskId != 0)
                {
                    request.OnComplete(OnGameInitialize);
                }
                else
                {
                    Debug.Log($"Core.GameInitialize requestID is 0! Repeated initialization or network error");
                }
                return 1;
            })),
        };
        // Matchmaking
        Dictionary<string, PPFFunctionConfig> matchDic = new Dictionary<string, PPFFunctionConfig>()
        {
            ["Crash test"] = new PPFFunctionConfig(new PPFFunction((paramList) => {
                return CLIB.ppf_Matchmaking_CrashTest();
            })),
            ["Matchmaking\nEnqueue2"] = new PPFFunctionConfig(new PPFFunction((paramList) => {
                MatchmakingOptions options = GameUtils.GetMatchmakingOptions(paramList[1], paramList[2], paramList[3], paramList[4], paramList[5], paramList[6], paramList[7]);
                return MatchmakingService.Enqueue2(paramList[0], options).OnComplete(ProcessMatchmakingEnqueue);
            }), new List<ParamName>() {
            ParamName.POOL_NAME,
            ParamName.MATCHMAKING_OPTION_ROOM_MAX_USERS,    // 1
            ParamName.MATCHMAKING_OPTION_ENQUEUE_IS_DEBUG,  // 2
            ParamName.MATCHMAKING_OPTION_ENQUEUE_QUERY_KEY, // 3
            ParamName.MATCHMAKING_OPTION_ENQUEUE_KEYS,       // 4
            ParamName.MATCHMAKING_OPTION_ENQUEUE_VALUES,     // 5
            ParamName.MATCHMAKING_OPTION_ROOM_KEYS,          // 6
            ParamName.MATCHMAKING_OPTION_ROOM_VALUES,        // 7
        }),
            ["Cancel2"] = new PPFFunctionConfig(new PPFFunction((paramList) => {
                return MatchmakingService.Cancel().OnComplete(OnMatchmakingCancelComplete);
            })),
            ["ReportResultInsecure"] = new PPFFunctionConfig(new PPFFunction((paramList) => {
                return MatchmakingService.ReportResultsInsecure(Convert.ToUInt64(paramList[0]), GameUtils.GetDicData(paramList[1], paramList[2])).OnComplete(OnReportResultsInsecureComplete);
            }), new List<ParamName>() { ParamName.ROOM_ID,
        ParamName.MATCHMAKING_REPORT_RESULT_KEYS,
        ParamName.MATCHMAKING_REPORT_RESULT_VALUES,}),
            ["StartMatch"] = new PPFFunctionConfig(new PPFFunction((paramList) => { // roomID
                return MatchmakingService.StartMatch(Convert.ToUInt64(paramList[0])).OnComplete(OnStartMatchComplete);
            }), new List<ParamName>() { ParamName.ROOM_ID }),
            ["GetAdminSnapshot"] = new PPFFunctionConfig(new PPFFunction((paramList) => { // 
                return MatchmakingService.GetAdminSnapshot().OnComplete(ProcessMatchmakingGetAdminSnapshot);
            })),
            ["GetStats"] = new PPFFunctionConfig(new PPFFunction((paramList) => { // pool, maxLevel, approach
                return MatchmakingService.GetStats(paramList[0], Convert.ToUInt32(paramList[1]), (MatchmakingStatApproach)Convert.ToInt32(paramList[2])).OnComplete(ProcessMatchmakingGetStats);

            }), new List<ParamName>() { ParamName.POOL_NAME, ParamName.MATCH_MAX_LEVEL, ParamName.MATCH_APPROACH }),

            ["CreateAndEnqueueRoom2"] = new PPFFunctionConfig(new PPFFunction((paramList) => {
                MatchmakingOptions options = GameUtils.GetMatchmakingOptions(paramList[1], paramList[2], paramList[3], paramList[4], paramList[5], paramList[6], paramList[7]);
                return MatchmakingService.CreateAndEnqueueRoom2(paramList[0], options).OnComplete(ProcessCreateAndEnqueueRoom2);
            }), new List<ParamName>() {
            ParamName.POOL_NAME,
            ParamName.MATCHMAKING_OPTION_ROOM_MAX_USERS,
            ParamName.MATCHMAKING_OPTION_ENQUEUE_IS_DEBUG,
            ParamName.MATCHMAKING_OPTION_ENQUEUE_QUERY_KEY,
            ParamName.MATCHMAKING_OPTION_ENQUEUE_KEYS,       // 4
            ParamName.MATCHMAKING_OPTION_ENQUEUE_VALUES,     // 5
            ParamName.MATCHMAKING_OPTION_ROOM_KEYS,          // 6
            ParamName.MATCHMAKING_OPTION_ROOM_VALUES,        // 7
        }),
            ["Browse2"] = new PPFFunctionConfig(new PPFFunction((paramList) => {
                MatchmakingOptions options = GameUtils.GetMatchmakingOptions(paramList[1], paramList[2], paramList[3], paramList[4], paramList[5], paramList[6], paramList[7]);
                return MatchmakingService.Browse2(paramList[0], options).OnComplete(ProcessMatchmakingBrowse2);
            }), new List<ParamName>() {
            ParamName.POOL_NAME,
            ParamName.MATCHMAKING_OPTION_ROOM_MAX_USERS,
            ParamName.MATCHMAKING_OPTION_ENQUEUE_IS_DEBUG,
            ParamName.MATCHMAKING_OPTION_ENQUEUE_QUERY_KEY,
            ParamName.MATCHMAKING_OPTION_ENQUEUE_KEYS,       // 4
            ParamName.MATCHMAKING_OPTION_ENQUEUE_VALUES,     // 5
            ParamName.MATCHMAKING_OPTION_ROOM_KEYS,          // 6
            ParamName.MATCHMAKING_OPTION_ROOM_VALUES,        // 7
        }),
        };
        // Rooms
        Dictionary<string, PPFFunctionConfig> roomDic = new Dictionary<string, PPFFunctionConfig>()
        {
            ["Get"] = new PPFFunctionConfig(new PPFFunction((paramList) => { // roomID
                return RoomService.Get(Convert.ToUInt64(paramList[0])).OnComplete(OnRoomMessage);
            }), new List<ParamName>() { ParamName.ROOM_ID }),
            ["GetCurrentForUser"] = new PPFFunctionConfig(new PPFFunction((paramList) => { // userID
                return RoomService.GetCurrentForUser(paramList[0]).OnComplete(OnRoomMessage);
            }), new List<ParamName>() { ParamName.USER_ID }),
            ["Join2"] = new PPFFunctionConfig(new PPFFunction((paramList) => {
                var ulongRoomID = Convert.ToUInt64(paramList[0]);
                var roomOptions = GameUtils.GetRoomOptions(paramList[0], paramList[1], paramList[2], paramList[3], paramList[4], paramList[5]);
                return RoomService.Join2(ulongRoomID, roomOptions).OnComplete(ProcessRoomJoin2);
            }), new List<ParamName>() {
            ParamName.ROOM_ID,
            ParamName.ROOM_OPTION_MAX_USER_RESULTS,
            ParamName.ROOM_OPTION_TURN_OFF_UPDATES,
            ParamName.ROOM_OPTION_DATASTORE_KEYS,
            ParamName.ROOM_OPTION_DATASTORE_VALUES,
            ParamName.ROOM_OPTION_ELCLUDERECENTLYMET,
        }),
            ["KickUser"] = new PPFFunctionConfig(new PPFFunction((paramList) => { // roomID, userID, kickDurationSeconds
                return RoomService.KickUser(Convert.ToUInt64(paramList[0]), paramList[1], Convert.ToInt32(paramList[2])).OnComplete(OnRoomMessage);
            }), new List<ParamName>() { ParamName.ROOM_ID, ParamName.USER_ID, ParamName.KICK_DURATION_SECONDS }),
            ["GetCurrent"] = new PPFFunctionConfig(new PPFFunction((paramList) => { // 
                return RoomService.GetCurrent().OnComplete(OnRoomMessage);
            })),
            ["Leave"] = new PPFFunctionConfig(new PPFFunction((paramList) => { // roomID
                return RoomService.Leave(Convert.ToUInt64(paramList[0])).OnComplete(ProcessRoomLeave);
            }), new List<ParamName>() { ParamName.ROOM_ID }),
            ["SetDescription"] = new PPFFunctionConfig(new PPFFunction((paramList) => { // roomID, desc
                return RoomService.SetDescription(Convert.ToUInt64(paramList[0]), paramList[1]).OnComplete(OnRoomMessage);
            }), new List<ParamName>() { ParamName.ROOM_ID, ParamName.DESCRIPTION }),
            ["UpdateDataStore"] = new PPFFunctionConfig(new PPFFunction((paramList) => { // roomID
                Dictionary<string, string> data = new Dictionary<string, string>();
                var rst = RoomService.UpdateDataStore(Convert.ToUInt64(paramList[0]), GameUtils.GetStringDicData(paramList[1], paramList[2])).OnComplete(OnRoomMessage);
                return rst;
            }), new List<ParamName>() { ParamName.ROOM_ID, ParamName.ROOM_OPTION_DATASTORE_KEYS, ParamName.ROOM_OPTION_DATASTORE_VALUES }),
            ["UpdateMembershipLockStatus"] = new PPFFunctionConfig(new PPFFunction((paramList) => { // roomID, membershipLockStatus
                return RoomService.UpdateMembershipLockStatus(Convert.ToUInt64(paramList[0]), (MembershipLockStatus)Convert.ToInt32(paramList[1])).OnComplete(OnRoomMessage);

            }), new List<ParamName>() { ParamName.ROOM_ID, ParamName.MEMBERSHIP_LOCK_STATUS }),
            ["UpdateOwner"] = new PPFFunctionConfig(new PPFFunction((paramList) => { // roomID, userID
                return RoomService.UpdateOwner(Convert.ToUInt64(paramList[0]), paramList[1]).OnComplete(ProcessRoomUpdateOwner);
            }), new List<ParamName>() { ParamName.ROOM_ID, ParamName.USER_ID }),
            ["CreateAndJoinPrivate2"] = new PPFFunctionConfig(new PPFFunction((paramList) => {
                var roomOptions = GameUtils.GetRoomOptions(paramList[2], paramList[3], paramList[4], paramList[5], paramList[6], paramList[7]);
                return RoomService.CreateAndJoinPrivate2((RoomJoinPolicy)Convert.ToInt32(paramList[0]), Convert.ToUInt32(paramList[1]), roomOptions).OnComplete(OnRoomMessage);
            }), new List<ParamName>() {
            ParamName.JOIN_POLICY,                      // 0
            ParamName.MAX_USERS,
            ParamName.ROOM_ID,
            ParamName.ROOM_OPTION_MAX_USER_RESULTS,     // 3
            ParamName.ROOM_OPTION_TURN_OFF_UPDATES,
            ParamName.ROOM_OPTION_DATASTORE_KEYS,       // 5
            ParamName.ROOM_OPTION_DATASTORE_VALUES,
            ParamName.ROOM_OPTION_ELCLUDERECENTLYMET,
        }),
            ["UpdatePrivateRoomJoinPolicy"] = new PPFFunctionConfig(new PPFFunction((paramList) => { // roomID, joinPolicy
                return RoomService.UpdatePrivateRoomJoinPolicy(Convert.ToUInt64(paramList[0]), (RoomJoinPolicy)Convert.ToInt32(paramList[1])).OnComplete(OnRoomMessage);
            }), new List<ParamName>() { ParamName.ROOM_ID, ParamName.JOIN_POLICY }),
            
            ["GetModeratedRooms"] = new PPFFunctionConfig(new PPFFunction((paramList) => { // 
                return RoomService.GetModeratedRooms(Convert.ToInt32(paramList[0]), Convert.ToInt32(paramList[1])).OnComplete(OnRoomListMessage);
            }), new List<ParamName>() { ParamName.ROOM_PAGE_INDEX, ParamName.ROOM_PAGE_SIZE }),

            ["Net.SendPacket"] = new PPFFunctionConfig(new PPFFunction((paramList) => {
                return NetworkService.SendPacket(paramList[0], System.Text.Encoding.UTF8.GetBytes(paramList[1]));
            }), new List<ParamName>() { ParamName.USER_ID, ParamName.PACKET_BYTES }),
            
            ["Net.SendPacketToCurrentRoom"] = new PPFFunctionConfig(new PPFFunction((paramList) => {
                return NetworkService.SendPacketToCurrentRoom(System.Text.Encoding.UTF8.GetBytes(paramList[0]));
            }), new List<ParamName>() { ParamName.PACKET_BYTES }),
            ["Net.ReadPacket"] = new PPFFunctionConfig(new PPFFunction((paramList) =>
            {
                var packet = NetworkService.ReadPacket();
                if (packet != null)
                {
                    var str = packet.GetBytes();
                    if (!string.IsNullOrEmpty(str))
                    {
                        LogHelper.LogInfo(TAG, $"ReadPacket: {str}");
                        return true;
                    }
                }
                LogHelper.LogInfo(TAG, $"ReadPacket: null");
                return false;
            })),

        };
        
    }
}
