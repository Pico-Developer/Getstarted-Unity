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
    public static class RoomService
    {
        /// @brief Updates the data store of the current room (the caller should be the room owner).
        /// @param  roomID The ID of the room that you currently own (call `Room.OwnerOptional` to check).
        /// @param  data The key/value pairs to add or update. Null value will clear a given key.
        /// @param  numItems The length of data.
        /// @returns The request ID of this async function.
        /// | Response Message Error Code| Description |
        /// |---|---|
        /// |3001004|change datastore failed: need room owner
        ///
        /// NOTE: Room data stores only allow string values. The maximum key length is 32 bytes and the maximum value length is 64 bytes.
        /// If you provide illegal values, this method will return an error.
        ///
        /// A message of type `MessageType.Room_UpdateDataStore` will be generated in response.
        /// First call `Message.IsError()` to check if any error has occurred.
        /// If no error has occurred, the message will contain a payload of type `Room`.
        /// Extract the payload from the message handle with `Message.Data`.
        public static Task<Room> UpdateDataStore(UInt64 roomId, Dictionary<string, string> data)
        {
            KVPairArray kvarray = new KVPairArray((uint) data.Count);
            uint n = 0;
            foreach (var d in data)
            {
                var item = kvarray.GetElement(n);
                item.SetKey(d.Key);
                item.SetStringValue(d.Value);
                n++;
            }

            return new Task<Room>(CLIB.ppf_Room_UpdateDataStore(roomId, kvarray.GetHandle(), kvarray.Size));
        }

        /// @brief This method creates a new private room and join it.
        /// This type of room can be obtained by querying the room where
        /// a friend is, so it is suitable for playing with friends
        ///
        /// @param  joinPolicy Specifies who can join the room: `0`-no one; `1`-everyone; `2`-friends of members; `3`-friends of the room owner; `4`-invited users; `5`-unknown.
        /// @param  maxUsers The maximum number of members allowed in the room, including the room creator.
        /// @param  roomOptions Room configuration for this request.
        /// @return Request information of type Task, including the request id, and its response message will contain data of type Room.
        /// | Response Message Error Code| Description |
        /// |---|---|
        /// |3001101|room create: unknown error
        /// |3001114|setting of 'room max user' is too large
        ///
        /// A message of type `MessageType.Room_CreateAndJoinPrivate2` will be generated in response.
        /// First call `message.IsError()` to check if any error has occurred.
        /// If no error has occurred, the message will contain a payload of type `Room`.
        /// Extract the payload from the message handle with `message.Data`.
        public static Task<Room> CreateAndJoinPrivate2(RoomJoinPolicy policy, uint maxUsers, RoomOptions roomOptions)
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.UninitializedError);
                return null;
            }

            return new Task<Room>(CLIB.ppf_Room_CreateAndJoinPrivate2((int) policy, maxUsers, roomOptions.GetHandle()));
        }

        /// @brief Gets the information about a specified room.
        ///
        /// @param  roomID The ID of the room to get information for.
        /// @return Request information of type Task, including the request id, and its response message will contain data of type Room.
        /// | Response Message Error Code| Description |
        /// |---|---|
        /// |3001103|invalid room
        /// |3001301|server error: unknown
        ///
        /// A message of type `MessageType.Room_Get` will be generated in response.
        /// First call `message.IsError()` to check if any error has occurred.
        /// If no error has occurred, the message will contain a payload of type `Room`.
        /// Extract the payload from the message handle with `message.Data`.
        public static Task<Room> Get(UInt64 roomId)
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.UninitializedError);
                return null;
            }

            return new Task<Room>(CLIB.ppf_Room_Get(roomId));
        }

        /// @brief Get the data of the room you are currently in.
        ///
        /// @return Request information of type Task, including the request id, and its response message will contain data of type Room.
        /// | Response Message Error Code| Description |
        /// |---|---|
        /// |3001104|not in room
        ///
        /// A message of type `MessageType.Room_GetCurrent` will be generated in response.
        /// First call `message.IsError()` to check if any error has occurred.
        /// If no error has occurred, the message will contain a payload with of type `Room`.
        /// Extract the payload from the message handle with `message.Data`.
        public static Task<Room> GetCurrent()
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.UninitializedError);
                return null;
            }

            return new Task<Room>(CLIB.ppf_Room_GetCurrent());
        }

        /// @brief Gets the current room of the specified user.
        /// It's important to note that the user's privacy settings
        /// may not allow you to access their room.
        ///
        /// @param  userID The ID of the user.
        /// @return Request information of type Task, including the request id, and its response message will contain data of type Room.
        /// | Response Message Error Code| Description |
        /// |---|---|
        /// |3001104|not in room
        /// |3001009|tgt player is not in game now
        /// |3001301|server error: unknown
        ///
        /// A message of type `MessageType.Room_GetCurrentForUser` will be generated in response.
        /// First call `message.IsError()` to check if any error has occurred.
        /// If no error has occurred, the message will contain a payload of type `Room`.
        /// Extract the payload from the message handle with `message.Data`.
        public static Task<Room> GetCurrentForUser(string userId)
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.UninitializedError);
                return null;
            }

            return new Task<Room>(CLIB.ppf_Room_GetCurrentForUser(userId));
        }


        /// @brief Gets the list of moderated rooms created for the application.
        ///
        /// @param  index Start page index.
        /// @param  size Page entry number in response (should range from `5` to `20`).
        /// @return Request information of type Task, including the request id, and its response message will contain data of type RoomList.
        /// | Response Message Error Code| Description |
        /// |---|---|
        /// |3001301|server error: unknown
        ///
        /// A message of type `MessageType.Room_GetModeratedRooms` will be generated in response.
        /// First call `message.IsError()` to check if any error has occurred.
        /// If no error has occurred, the message will contain a payload of type `RoomList`.
        /// Extract the payload from the message handle with `message.Data`.
        public static Task<RoomList> GetModeratedRooms(int index, int size)
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.UninitializedError);
                return null;
            }

            return new Task<RoomList>(CLIB.ppf_Room_GetModeratedRooms(index, size));
        }


        /// @brief Joins the target room and meanwhile leaves the current room.
        ///
        /// @param  roomID The ID of the room to join.
        /// @param  roomOptions (Optional) Additional room configuration for this request.
        /// @return Request information of type Task, including the request id, and its response message will contain data of type Room.
        /// | Response Message Error Code| Description |
        /// |---|---|
        /// |3001401|logic state checking failed
        /// |3001103|invalid room
        /// |3001102|duplicate join room(regarded as normal entry)  
        /// |3001106|exceed max room player number 
        /// |3001105|illegal enter request(Players outside the legal list enter) 
        /// |3001108|room is locked
        ///
        /// A message of type `MessageType.Room_Join2` will be generated in response.
        /// First call `message.IsError()` to check if any error has occurred.
        /// If no error has occurred, the message will contain a payload of type `Room`.
        /// Extract the payload from the message handle with `message.Data`.
        public static Task<Room> Join2(UInt64 roomId, RoomOptions options)
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.UninitializedError);
                return null;
            }

            return new Task<Room>(CLIB.ppf_Room_Join2(roomId, options.GetHandle()));
        }

        /// @brief Kicks a user out of a room. For use by homeowners only.
        ///
        /// @param  roomID The ID of the room 
        /// @param  userID The ID of the user to be kicked (cannot be yourself).
        /// @param  kickDuration The Length of the ban (in seconds).
        /// @return Request information of type Task, including the request id, and its response message will contain data of type Room.
        /// | Response Message Error Code| Description |
        /// |---|---|
        /// |3001006|kick user failed: need room owner
        /// |3001007|kick user failed: tgt user is not in the room
        /// |3001008|kick user failed: can not kick self
        ///
        /// A message of type `MessageType.Room_KickUser` will be generated in response.
        /// First call `message.IsError()` to check if any error has occurred.
        /// If no error has occurred, the message will contain a payload of type `Room`.
        /// Extract the payload from the message handle with `message.Data`.
        public static Task<Room> KickUser(UInt64 roomId, string userId, int kickDuration)
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.UninitializedError);
                return null;
            }

            return new Task<Room>(CLIB.ppf_Room_KickUser(roomId, userId, kickDuration));
        }

        /// @brief Leaves the current room. The room you
        /// are now in will be returned if the request succeeds.
        ///
        /// @param  roomID The ID of the room.
        /// @return Request information of type Task, including the request id, and its response message will contain data of type Room.
        /// | Response Message Error Code| Description |
        /// |---|---|
        /// |3001401|logic state checking failed(e.g. not in the room) 
        /// |3001301|server error: unknown
        ///
        /// A message of type `MessageType.Room_Leave` will be generated in response.
        /// First call `message.IsError()` to check if any error has occurred.
        /// If no error has occurred, the message will contain a payload of type `Room`.
        /// Extract the payload from the message handle with `message.Data`.
        public static Task<Room> Leave(UInt64 roomId)
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.UninitializedError);
                return null;
            }

            return new Task<Room>(CLIB.ppf_Room_Leave(roomId));
        }

        /// @brief Sets the description of a room.  For use by homeowners only.
        ///
        /// @param  roomID The ID of the room to set description for
        /// @param  description The new description of the room.
        /// @return Request information of type Task, including the request id, and its response message will contain data of type Room.
        /// | Response Message Error Code| Description |
        /// |---|---|
        /// |3001005|set description failed: need room owner
        ///
        /// A message of type `MessageType.Room_SetDescription` will be generated in response.
        /// First call `message.IsError()` to check if any error has occurred.
        /// If no error has occurred, the message will contain a payload of type `Room`.
        /// Extract the payload from the message handle with `message.Data`.
        public static Task<Room> SetDescription(UInt64 roomId, string desp)
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.UninitializedError);
                return null;
            }

            return new Task<Room>(CLIB.ppf_Room_SetDescription(roomId, desp));
        }

        /// @brief Locks/unlocks the membership of a room (the caller should be the room owner)
        /// to allow/disallow new members from being able to join the room.
        /// Locking membership will prevent other users from joining the room through `Join2()`,
        /// invitations, etc. Users that are in the room at the time of lock will be able to rejoin.
        ///
        /// @param  roomID The ID of the room to lock/unlock membership for.
        /// @param  membershipLockStatus The new membership status to set for the room: `0`-Unknown; `1`-lock; `2`-unlock.
        /// @return Request information of type Task, including the request id, and its response message will contain data of type Room.
        /// | Response Message Error Code| Description |
        /// |---|---|
        /// |3001104|not in room 
        /// |3001109|update membership lock: need room owner
        ///
        /// A message of type `MessageType.Room_UpdateMembershipLockStatus` will be generated in response.
        /// First call `message.IsError()` to check if any error has occurred.
        /// If no error has occurred, the message will contain a payload of type `Room`
        /// Extract the payload from the message handle with `message.Data`.
        public static Task<Room> UpdateMembershipLockStatus(UInt64 roomId, MembershipLockStatus status)
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.UninitializedError);
                return null;
            }

            return new Task<Room>(CLIB.ppf_Room_UpdateMembershipLockStatus(roomId, (int) status));
        }

        /// @brief Modify the owner of the room, this person needs to be the person in this room.
        ///
        /// @param  roomID The ID of the room to change ownership for.
        /// @param  userID The ID of the new user to own the room. The new user must be in the same room.
        /// @return Request information of type Task, including the request id, and its response message does not contain data.
        /// | Response Message Error Code| Description |
        /// |---|---|
        /// |3001001|change owner failed: need room owner 
        /// |3001003|change owner failed: duplicate setting 
        /// |3001002|change owner failed: new owner not in this room
        ///
        /// A message of type `MessageType.Room_UpdateOwner` will be generated in response.
        /// Call `message.IsError()` to check if any error has occurred.
        public static Task UpdateOwner(UInt64 roomId, string userId)
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.UninitializedError);
                return null;
            }

            return new Task(CLIB.ppf_Room_UpdateOwner(roomId, userId));
        }

        /// @brief Sets the join policy for a specified private room.
        ///
        /// @param  roomID The ID of the room you want to change its policy
        /// @param  newJoinPolicy Specifies who can join the room: `0`-no one; `1`-everyone; `2`-friends of members; `3`-friends of the room owner; `4`-invited users; `5`-unknown.
        /// @return Request information of type Task, including the request id, and its response message will contain data of type Room.
        /// | Response Message Error Code| Description |
        /// |---|---|
        /// |3001104|not in room 
        /// |3001112|update room join policy: need room owner
        ///
        /// A message of type `MessageType.Room_UpdatePrivateRoomJoinPolicy` will be generated in response.
        /// First call `message.IsError()` to check if any error has occurred.
        /// If no error has occurred, the message will contain a payload of type `Room`.
        /// Extract the payload from the message handle with `message.Data`.
        public static Task<Room> UpdatePrivateRoomJoinPolicy(UInt64 roomId, RoomJoinPolicy policy)
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.UninitializedError);
                return null;
            }

            return new Task<Room>(CLIB.ppf_Room_UpdatePrivateRoomJoinPolicy(roomId, (int) policy));
        }

        /// @brief Sets the callback to get notified when the current room has been updated. Use `Message.Data` to extract the room.
        ///
        /// @param  callback The callback function will be called when receiving the `Notification_Room_RoomUpdate` message.
        public static void SetUpdateNotificationCallback(Message<Room>.Handler handler)
        {
            Looper.RegisterNotifyHandler(MessageType.Notification_Room_RoomUpdate, handler);
        }

        /// @brief Sets the callback to get notified when the user has been kicked out of a room.
        /// Listen to this event to receive a relevant message. Use `Message.Data` to extract the room.
        ///
        /// @param  callback The callback function will be called when receiving the `Room_KickUser` message and the value of `requestID` is `0`.
        public static void SetKickUserNotificationCallback(Message<Room>.Handler handler)
        {
            Looper.RegisterNotifyHandler(MessageType.Room_KickUser, handler);
        }

        /// @brief Sets the callback to get notified when the room description has been updated.
        /// Listen to this event to receive a relevant message. Use `Message.Data` to extract the room.
        ///
        /// @param  callback The callback function will be called when receiving the `Room_SetDescription` message and the value of `requestID` is `0`.
        public static void SetSetDescriptionNotificationCallback(Message<Room>.Handler handler)
        {
            Looper.RegisterNotifyHandler(MessageType.Room_SetDescription, handler);
        }

        /// @brief Sets the callback to get notified when the room data has been modified.
        /// Listen to this event to receive a relevant message. Use `Message.Data` to extract the room.
        ///
        /// @param  callback The callback function will be called when receiving the `Room_UpdateDataStore` message and the value of `requestID` is `0`.
        public static void SetUpdateDataStoreNotificationCallback(Message<Room>.Handler handler)
        {
            Looper.RegisterNotifyHandler(MessageType.Room_UpdateDataStore, handler);
        }

        /// @brief Set the callback to get notified when someone has left the room.
        /// Listen to this event to receive a relevant message. Use `Message.Data` to extract the room.
        ///
        /// @param  callback The callback function will be called when receiving the `Room_Leave` message and the value of `requestID` is `0`.
        public static void SetLeaveNotificationCallback(Message<Room>.Handler handler)
        {
            Looper.RegisterNotifyHandler(MessageType.Room_Leave, handler);
        }

        /// @brief Sets the callback to get notified when someone has entered the room.
        /// Use `Message.Data` to extract the room.
        ///
        /// @param  callback The callback function will be called when receiving the `Room_Join2` message and the value of `requestID` is `0`.
        public static void SetJoin2NotificationCallback(Message<Room>.Handler handler)
        {
            Looper.RegisterNotifyHandler(MessageType.Room_Join2, handler);
        }

        /// @brief Sets the callback to get notified when the room owner has changed.
        /// Listen to this event to receive a relevant message. Use `Message.Data` to extract the room.
        ///
        /// @param  callback The callback function will be called when receiving the `Room_UpdateOwner` message and the value of `requestID` is `0`.
        public static void SetUpdateOwnerNotificationCallback(Message.Handler handler)
        {
            Looper.RegisterNotifyHandler(MessageType.Room_UpdateOwner, handler);
        }

        /// @brief Sets the callback to get notified when the membership status of a room has been changed.
        /// Listen to this event to receive a relevant message. Use `Message.Data` to extract the room.
        ///
        /// @param  callback The callback function will be called when receiving the "Room_UpdateMembershipLockStatus" message and the value of `requestID` is `0`.
        public static void SetUpdateMembershipLockStatusNotificationCallback(Message<Room>.Handler handler)
        {
            Looper.RegisterNotifyHandler(MessageType.Room_UpdateMembershipLockStatus, handler);
        }
    }

    public class RoomOptions
    {
        public RoomOptions()
        {
            Handle = CLIB.ppf_RoomOptions_Create();
        }

        public void SetDataStore(string key, string value)
        {
            CLIB.ppf_RoomOptions_SetDataStoreString(Handle, key, value);
        }

        public void ClearDataStore()
        {
            CLIB.ppf_RoomOptions_ClearDataStore(Handle);
        }

        public void SetExcludeRecentlyMet(bool value)
        {
            CLIB.ppf_RoomOptions_SetExcludeRecentlyMet(Handle, value);
        }

        public void SetMaxUserResults(uint value)
        {
            CLIB.ppf_RoomOptions_SetMaxUserResults(Handle, value);
        }

        public void SetRoomId(UInt64 value)
        {
            CLIB.ppf_RoomOptions_SetRoomId(Handle, value);
        }

        public void SetTurnOffUpdates(bool value)
        {
            CLIB.ppf_RoomOptions_SetTurnOffUpdates(Handle, value);
        }


        /// For passing to native C
        public static explicit operator IntPtr(RoomOptions roomOptions)
        {
            return roomOptions != null ? roomOptions.Handle : IntPtr.Zero;
        }

        ~RoomOptions()
        {
            CLIB.ppf_RoomOptions_Destroy(Handle);
        }

        IntPtr Handle;

        public IntPtr GetHandle()
        {
            return Handle;
        }
    }


    public enum RoomType
    {
        Unknown = 0,
        Matchmaking = 1,
        Moderated = 2,
        Private = 3
    }

    public enum RoomJoinability
    {
        Unknown = 0,
        AreIn = 1,
        AreKicked = 2,
        CanJoin = 3,
        IsFull = 4,
        NoViewer = 5,
        PolicyPrevents = 6,
    }

    public enum RoomJoinPolicy
    {
        None = 0,
        Everyone = 1,
        FriendsOfMembers = 2,
        FriendsOfOwner = 3,
        InvitedUsers = 4,
        Unknown = 5,
    }

    public enum MembershipLockStatus
    {
        Unknown = 0,
        Lock = 1,
        Unlock = 2,
    }
}