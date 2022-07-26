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

namespace Pico.Platform
{
    public class Message
    {
        public delegate void Handler(Message message);

        public readonly MessageType Type;
        public readonly ulong RequestID;
        public readonly Error Error;

        public Message(IntPtr msgPointer)
        {
            Type = CLIB.ppf_Message_GetType(msgPointer);
            RequestID = CLIB.ppf_Message_GetRequestID(msgPointer);
            if (CLIB.ppf_Message_IsError(msgPointer))
            {
                Error = new Error(CLIB.ppf_Message_GetError(msgPointer));
            }
        }

        public bool IsError => Error != null;

        public Error GetError()
        {
            return Error;
        }
    }

    public class Message<T> : Message
    {
        public new delegate void Handler(Message<T> message);

        public readonly T Data;

        public delegate T GetDataFromMessage(IntPtr msgPointer);

        public Message(IntPtr msgPointer, GetDataFromMessage getData) : base(msgPointer)
        {
            if (!IsError)
            {
                Data = getData(msgPointer);
            }
        }
    }
}