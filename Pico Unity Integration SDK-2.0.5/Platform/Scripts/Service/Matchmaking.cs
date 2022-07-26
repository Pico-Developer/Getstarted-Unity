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

namespace Pico.Platform
{
    /**
     * \ingroup Platform
     */
    public static class MatchmakingService
    {
        /// @brief Reports the result of a skill-rating match.
        ///
        /// Applicable to the following matchmaking modes: Quickmatch, Browse (+ Skill Pool)
        /// 
        /// @param roomID The room ID.
        /// @param data The key-value pairs.
        /// @return Request information of type Task, including the request id, and its response message does not contain data.
        /// | Response Message Error Code| Description |
        /// |---|---|
        /// |3001209|match result report: not in match
        /// |3001210|match result report: error report data
        /// |3001211|match result report: duplicate report
        /// |3001212|match result report: conflict with other's report
        ///
        /// 
        /// Only for pools with skill-based matchmaking. 
        /// Call this method after calling `StartMatch()` to begin a skill-rating
        /// match. After the match finishes, the server will record the result and
        /// update the skill levels of all players involved based on the result. This
        /// method is insecure because, as a client API, it is susceptible to tampering
        /// and therefore cheating to manipulate skill ratings.
        ///
        /// A message of type `MessageType.Matchmaking_ReportResultInsecure` will be generated in response.
        /// First call `Message.IsError()` to check if any error has occurred.
        /// This response has no payload. If no error has occurred, the request is successful.
        public static Task ReportResultsInsecure(UInt64 roomId, Dictionary<string, int> data)
        {
            KVPairArray kvarray = new KVPairArray((uint) data.Count);
            uint n = 0;
            foreach (var d in data)
            {
                var item = kvarray.GetElement(n);
                item.SetKey(d.Key);
                item.SetIntValue(d.Value);
                n++;
            }

            return new Task(CLIB.ppf_Matchmaking_ReportResultInsecure(roomId, kvarray.GetHandle(), kvarray.Size));
        }

        /// @brief Gets the matchmaking statistics for the current user.
        ///
        /// Applicable to the following matchmaking modes: Quickmatch, Browse
        /// 
        /// @param pool The pool to look in.
        /// @param maxLevel (beta feature, don't use it)
        /// @param approach (beta feature, don't use it)
        /// @return Request information of type Task, including the request id, and its response message will contain data of type MatchmakingStats.
        /// | Response Message Error Code| Description |
        /// |---|---|
        /// |3001201|match enqueue: invalid pool name|
        /// |3001208|match enqueue: no skill|
        ///
        ///
        /// When given a pool, the system will look up the current user's wins, losses, draws and skill
        /// level. The skill level returned will be between `1` and the maximum level. The approach
        /// will determine how should the skill level rise toward the maximum level.
        ///
        /// A message of type `MessageType.Matchmaking_GetStats` will be generated in response.
        /// First call `Message.IsError()` to check if any error has occurred.
        /// If no error has occurred, the message will contain a payload of type `MatchmakingStats`.
        /// Extract the payload from the message handle with `message.Data`.
        public static Task<MatchmakingStats> GetStats(string pool, uint maxLevel, MatchmakingStatApproach approach = MatchmakingStatApproach.Trailing)
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.UninitializedError);
                return null;
            }

            return new Task<MatchmakingStats>(CLIB.ppf_Matchmaking_GetStats(pool, maxLevel, (int) approach));
        }

        /// @brief Get matching rooms based on matching pool name.
        /// The user can call RoomService.Join2 to join the room. The
        /// user can cancel the retrieval by calling MatchmakingService.Cancel.
        ///
        /// Applicable to the following matchmaking mode: Browse
        /// 
        /// @param  pool The matchmaking pool name you want to browse.
        /// @param  matchmakingOptions (Optional) The matchmaking configuration of the browse request.
        /// @return Request information of type Task, including the request id, and its response message will contain data of type MatchmakingBrowseResult.
        /// | Response Message Error Code| Description |
        /// |---|---|
        /// |3001201|match enqueue: invalid pool name|
        /// |3001205|match browse: access denied|
        /// |3001207|match enqueue: invalid query key|
        ///
        /// A message of type `MessageType.Matchmaking_Browse2` will be generated in response.
        /// First call `Message.IsError()` to check if any error has occurred.
        /// If no error has occurred, the message will contain a payload of type `MatchmakingBrowseResult`.
        /// Extract the payload from the message handle with `message.Data`.
        public static Task<MatchmakingBrowseResult> Browse2(string pool, MatchmakingOptions matchmakingOptions = null)
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.UninitializedError);
                return null;
            }

            return new Task<MatchmakingBrowseResult>(CLIB.ppf_Matchmaking_Browse2(pool, matchmakingOptions.GetHandle()));
        }

        /// @brief Cancels a matchmaking request. Call this method
        /// to cancel an enqueue request before a match
        /// is made. This is typically triggered when a user gives up waiting.
        /// If you do not cancel the request but the user goes offline, the user/room
        /// will be timed out according to the 'reserve period' setting on the dashboard.
        ///
        /// Applicable to the following matchmaking modes: Quickmatch, Browse
        /// 
        /// @return Request information of type Task, including the request id, and its response message does not contain data.
        /// | Response Message Error Code| Description |
        /// |---|---|
        /// |3001201|match enqueue: invalid pool name
        /// |3001206|match cancel: not in match
        /// |3001301|server error: unknown
        /// 
        ///
        /// A message of type `MessageType.Matchmaking_Cancel2` will be generated in response.
        /// Call `Message.IsError()` to check if any error has occurred.
        /// This response has no payload. If no error has occurred, the request is successful.
        public static Task Cancel()
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.UninitializedError);
                return null;
            }

            return new Task(CLIB.ppf_Matchmaking_Cancel2());
        }

        /// @brief This method will creates a matchmaking room, then enqueues and joins it.
        ///
        /// Applicable to the following matchmaking modes: Quickmatch, Browse, Advanced (Can Users Create Rooms=`true`)
        /// 
        /// @param  pool The matchmaking pool to use, which is defined on the dashboard.
        /// @param  matchmakingOptions (Optional) Additional matchmaking configuration for this request.
        /// @return Request information of type Task, including the request id, and its response message will contain data of type MatchmakingEnqueueResultAndRoom.
        /// | Response Message Error Code| Description |
        /// |---|---|
        /// |3001201|match enqueue: invalid pool name
        /// |3001203|match create room: pool config not allow user create room
        /// |3001207|match enqueue: invalid query key 
        /// |3001301|server error: unknown 
        /// |3001204|match enqueue: invlid room id(Assigned room id, present in this context, indicates an internal server error) 
        /// |3001103|invalid room(The room was found to be invalid when joining the room, which appears in this context, indicating an internal server error) 
        /// |3001102|duplicate join room(Duplicate joins are found when joining a room, which appears in this context, indicating an internal server error) 
        /// |3001106|exceed max room player number(Exceeding the maximum number of people when joining a room, appears in this context, indicating an internal server error) 
        /// |3001105|illegal enter request(Illegal incoming requests, such as not in the allowed whitelist, appear in this context, indicating an internal server error) 
        /// |3001108|room is locked(When joining a room, it is found that the room is locked, appears in this context, indicating an internal server error)
        ///
        /// A message of type `MessageType.Matchmaking_CreateAndEnqueueRoom2` will be generated in response.
        /// First call `message.IsError()` to check if any error has occurred.
        /// If no error has occurred, the message will contain a payload of type `MatchmakingEnqueueResultAndRoom`.
        /// Extract the payload from the message handle with `message.Data`.
        public static Task<MatchmakingEnqueueResultAndRoom> CreateAndEnqueueRoom2(string pool, MatchmakingOptions matchmakingOptions = null)
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.UninitializedError);
                return null;
            }

            return new Task<MatchmakingEnqueueResultAndRoom>(CLIB.ppf_Matchmaking_CreateAndEnqueueRoom2(pool, matchmakingOptions.GetHandle()));
        }

        /// @brief Enqueues for an available matchmaking room to join.
        /// When the server finds a match, it will return a message of
        /// type MessageType.Notification_Matchmaking_MatchFound. You
        /// can join found matching rooms by calling RoomService.Join2. 
        /// If you want to cancel the match early, you can use MatchmakingService.Cancel.
        ///
        /// Applicable to the following matchmaking mode: Quickmatch
        /// 
        /// @param  pool The matchmaking pool to use, which is defined on the dashboard.
        /// @param  matchmakingOptions (Optional) Match configuration for Enqueue.
        /// @return Request information of type Task, including the request id, and its response message will contain data of type MatchmakingEnqueueResult.
        /// | Response Message Error Code| Description |
        /// |---|---|
        /// |3001201|match enqueue: invalid pool name
        /// |3001401|logic state checking failed
        /// |3001207|match enqueue: invalid query key  
        /// |3001301|server error: unknown
        ///
        /// A message of type `MessageType.Matchmaking_Enqueue2` will be generated in response.
        /// First call `message.IsError()` to check if any error has occurred.
        /// If no error has occurred, the message will contain a payload of type `MatchmakingEnqueueResult`.
        /// Extract the payload from the message handle with `message.Data`.
        public static Task<MatchmakingEnqueueResult> Enqueue2(string pool, MatchmakingOptions matchmakingOptions = null)
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.UninitializedError);
                return null;
            }

            var o = matchmakingOptions == null ? new MatchmakingOptions() : matchmakingOptions;
            return new Task<MatchmakingEnqueueResult>(CLIB.ppf_Matchmaking_Enqueue2(pool, o.GetHandle()));
        }

        /// @brief Debugs the state of the current matchmaking pool queue.
        /// This method should not be used in production.
        ///
        /// Applicable to the following matchmaking modes: Quickmatch, Browse
        /// 
        /// @return Request information of type Task, including the request id, and its response message will contain data of type MatchmakingAdminSnapshot.
        /// | Response Message Error Code| Description |
        /// |---|---|
        /// |3001201|match enqueue: invalid pool name
        /// |3001301|server error: unknown 
        ///
        /// A message of type `MessageType.Matchmaking_GetAdminSnapshot` will be generated in response.
        /// First call `message.IsError()` to check if any error has occurred.
        /// If no error has occurred, the message will contain a payload of type `MatchmakingAdminSnapshot`.
        /// Extract the payload from the message handle with `message.Data`.
        public static Task<MatchmakingAdminSnapshot> GetAdminSnapshot()
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.UninitializedError);
                return null;
            }

            return new Task<MatchmakingAdminSnapshot>(CLIB.ppf_Matchmaking_GetAdminSnapshot());
        }

        /// @brief Reports that a skill-rating match has started.
        /// Only for pools with skill-based matching. 
        /// You can use this method after joining the room. 
        ///
        /// Applicable to the following matchmaking modes: Quickmatch, Browse (+ Skill Pool)
        /// 
        /// @param  roomID The ID of the room you want to match.
        /// @return Request information of type Task, including the request id, and its response message does not contain data.
        ///
        /// A message of type `MessageType.Matchmaking_StartMatch` will be generated in response.
        /// Call `message.IsError()` to check if any error has occurred.
        public static Task StartMatch(UInt64 roomId)
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.UninitializedError);
                return null;
            }

            return new Task(CLIB.ppf_Matchmaking_StartMatch(roomId));
        }

        /// @brief Sets the callback to get notified when a match has been found, for example,
        /// When you call MatchmakingService.Enqueue, when the match is successful, you will
        /// receive Notification_Matchmaking_MatchFound, and then execute the processing function
        /// set by this function
        /// 
        /// @param  callback The callback function will be called when receiving the `Notification_Matchmaking_MatchFound` message.
        public static void SetMatchFoundNotificationCallback(Message<Room>.Handler handler)
        {
            Looper.RegisterNotifyHandler(MessageType.Notification_Matchmaking_MatchFound, handler);
        }

        /// @brief Sets the callback to get notified when a matchmaking has been canceled. Listen to the event to receive a message. 
        /// 
        /// @param  callback The callback function will be called when receiving the `Matchmaking_Cancel2` message and the value of `requestID` is `0`.
        public static void SetCancel2NotificationCallback(Message.Handler handler)
        {
            Looper.RegisterNotifyHandler(MessageType.Matchmaking_Cancel2, handler);
        }
    }

    public enum MatchmakingStatApproach
    {
        Unknown = 0,
        Trailing = 1,
        Swingy = 2,
    }

    public enum MatchmakingCriterionImportance
    {
        Required = 0,
        High = 1,
        Medium = 2,
        Low = 3,
        Unknown = 4,
    }


    public class MatchmakingOptions
    {
        public MatchmakingOptions()
        {
            Handle = CLIB.ppf_MatchmakingOptions_Create();
        }

        public void SetCreateRoomDataStore(string key, string value)
        {
            CLIB.ppf_MatchmakingOptions_SetCreateRoomDataStoreString(Handle, key, value);
        }

        public void ClearCreateRoomDataStore()
        {
            CLIB.ppf_MatchmakingOptions_ClearCreateRoomDataStore(Handle);
        }

        public void SetCreateRoomJoinPolicy(RoomJoinPolicy value)
        {
            CLIB.ppf_MatchmakingOptions_SetCreateRoomJoinPolicy(Handle, (int) value);
        }

        public void SetCreateRoomMaxUsers(uint value)
        {
            CLIB.ppf_MatchmakingOptions_SetCreateRoomMaxUsers(Handle, value);
        }

        public void SetEnqueueDataSettings(string key, int value)
        {
            CLIB.ppf_MatchmakingOptions_SetEnqueueDataSettingsInt(Handle, key, value);
        }

        public void SetEnqueueDataSettings(string key, double value)
        {
            CLIB.ppf_MatchmakingOptions_SetEnqueueDataSettingsDouble(Handle, key, value);
        }

        public void SetEnqueueDataSettings(string key, string value)
        {
            CLIB.ppf_MatchmakingOptions_SetEnqueueDataSettingsString(Handle, key, value);
        }

        public void ClearEnqueueDataSettings()
        {
            CLIB.ppf_MatchmakingOptions_ClearEnqueueDataSettings(Handle);
        }

        public void SetEnqueueIsDebug(bool value)
        {
            CLIB.ppf_MatchmakingOptions_SetEnqueueIsDebug(Handle, value);
        }

        public void SetEnqueueQueryKey(string value)
        {
            CLIB.ppf_MatchmakingOptions_SetEnqueueQueryKey(Handle, value);
        }


        /// For passing to native C
        public static explicit operator IntPtr(MatchmakingOptions matchmakingOptions)
        {
            return matchmakingOptions != null ? matchmakingOptions.Handle : IntPtr.Zero;
        }

        ~MatchmakingOptions()
        {
            CLIB.ppf_MatchmakingOptions_Destroy(Handle);
        }

        IntPtr Handle;

        public IntPtr GetHandle()
        {
            return Handle;
        }
    }
}