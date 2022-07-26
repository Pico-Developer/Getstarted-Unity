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
using System.Runtime.InteropServices;
using Pico.Platform.Models;
using UnityEngine;

namespace Pico.Platform
{
    /**
     * \ingroup Platform
     */
    public static class NetworkService
    {
        /// <summary>
        /// Reads the messages from other users in the room.
        /// </summary>
        public static Packet ReadPacket()
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.UninitializedError);
                return null;
            }

            var handle = CLIB.ppf_Net_ReadPacket();
            if (handle == IntPtr.Zero)
                return null;
            return new Packet(handle);
        }

        /// <summary>
        /// Sends messages to a specified user.
        /// </summary>
        /// <param name="userId">The ID of the user to send messages to.</param>
        /// <param name="bytes">The message length (in bytes).</param>
        /// <returns>`true`-success; `false`-failure.</returns>
        public static bool SendPacket(string userId, byte[] bytes)
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.UninitializedError);
                return false;
            }

            GCHandle hobj = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            IntPtr pobj = hobj.AddrOfPinnedObject();
            var ok = CLIB.ppf_Net_SendPacket(userId, (uint) bytes.Length, pobj);
            if (hobj.IsAllocated)
                hobj.Free();
            return ok;
        }

        /// <summary>
        /// Sends messages to other users in the room.
        /// </summary>
        /// <param name="bytes">The message length (in bytes).</param>
        /// <returns>`true`-success; `false`-failure.</returns>
        public static bool SendPacketToCurrentRoom(byte[] bytes)
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.UninitializedError);
                return false;
            }

            GCHandle hobj = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            IntPtr pobj = hobj.AddrOfPinnedObject();
            var ok = CLIB.ppf_Net_SendPacketToCurrentRoom((uint) bytes.Length, pobj);
            if (hobj.IsAllocated)
                hobj.Free();
            return ok;
        }

        public static void SetPlatformGameInitializeAsynchronousCallback(Message<GameInitializeResult>.Handler handler)
        {
            Looper.RegisterNotifyHandler(MessageType.PlatformGameInitializeAsynchronous, handler);
        }

        public static void SetNotification_Game_ConnectionEventCallback(Message<GameConnectionEvent>.Handler handler)
        {
            Looper.RegisterNotifyHandler(MessageType.Notification_Game_ConnectionEvent, handler);
        }

        public static void SetNotification_Game_Request_FailedCallback(Message<GameRequestFailedReason>.Handler handler)
        {
            Looper.RegisterNotifyHandler(MessageType.Notification_Game_RequestFailed, handler);
        }

        public static void SetNotification_Game_StateResetCallback(Message.Handler handler)
        {
            Looper.RegisterNotifyHandler(MessageType.Notification_Game_StateReset, handler);
        }
    }
}