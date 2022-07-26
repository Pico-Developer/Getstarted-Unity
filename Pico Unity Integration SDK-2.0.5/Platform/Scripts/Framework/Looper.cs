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
using System.Collections.Concurrent;
using UnityEngine;

namespace Pico.Platform
{
    public class Looper
    {
        private static readonly ConcurrentDictionary<ulong, Delegate> TaskMap = new ConcurrentDictionary<ulong, Delegate>();

        private static readonly ConcurrentDictionary<MessageType, Delegate> NotifyMap = new ConcurrentDictionary<MessageType, Delegate>();

        public static void ProcessMessages(uint limit = 0)
        {
            if (limit == 0)
            {
                while (true)
                {
                    var msg = MessageQueue.Next();
                    if (msg == null)
                    {
                        break;
                    }

                    dispatchMessage(msg);
                }
            }
            else
            {
                for (var i = 0; i < limit; ++i)
                {
                    var msg = MessageQueue.Next();
                    if (msg == null)
                    {
                        break;
                    }

                    dispatchMessage(msg);
                }
            }
        }

        private static void dispatchMessage(Message msg)
        {
            if (msg.RequestID != 0)
            {
                // handle task
                if (TaskMap.TryGetValue(msg.RequestID, out var handler))
                {
                    try
                    {
                        handler.DynamicInvoke(msg);
                    }
                    finally
                    {
                        TaskMap.TryRemove(msg.RequestID, out handler);
                    }
                }
                else
                {
                    Debug.LogError($"No handler for task: requestId={msg.RequestID}, msg.Type = {msg.Type}");
                }

                return;
            }
            else
            {
                // handle notification
                if (NotifyMap.TryGetValue(msg.Type, out var handler))
                {
                    handler.DynamicInvoke(msg);
                }
                else
                {
                    //Debug.LogError($"No handler for notification: msg.Type = {msg.Type}");
                }
            }
        }

        public static bool RegisterTaskHandler(ulong taskId, Delegate handler)
        {
            if (taskId == 0)
            {
                Debug.LogError("The task is invalid.");
                return false;
            }

            TaskMap[taskId] = handler;
            return true;
        }

        public static bool RegisterNotifyHandler(MessageType type, Delegate handler)
        {
            if (handler == null)
            {
                Debug.LogError("Cannot register null notification handler.");
                return false;
            }

            NotifyMap[type] = handler;
            return true;
        }


        public static void Clear()
        {
            TaskMap.Clear();
            NotifyMap.Clear();
        }
    }
}