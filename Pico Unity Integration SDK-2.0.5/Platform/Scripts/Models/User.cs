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

using System;

namespace Pico.Platform.Models
{
    public class User
    {
        public readonly string DisplayName;
        public readonly string ImageUrl;
        public readonly string ID;
        public readonly UserPresenceStatus PresenceStatus;
        public readonly Gender Gender;

        public User(IntPtr obj)
        {
            DisplayName = CLIB.ppf_User_GetDisplayName(obj);
            ImageUrl = CLIB.ppf_User_GetImageUrl(obj);
            ID = CLIB.ppf_User_GetID(obj);
            PresenceStatus = CLIB.ppf_User_GetPresenceStatus(obj);
            Gender = CLIB.ppf_User_GetGender(obj);
        }
    }


    public class UserList : MessageArray<User>
    {
        public UserList(IntPtr a)
        {
            var count = (int) CLIB.ppf_UserArray_GetSize(a);
            this.Capacity = count;
            for (int i = 0; i < count; i++)
            {
                this.Add(new User(CLIB.ppf_UserArray_GetElement(a, (UIntPtr) i)));
            }

            NextPageParam = CLIB.ppf_UserArray_GetNextPageParam(a);
        }
    }

    public class LaunchFriendResult
    {
        public readonly bool DidCancel;
        public readonly bool DidSendRequest;

        public LaunchFriendResult(IntPtr obj)
        {
            DidCancel = CLIB.ppf_LaunchFriendRequestFlowResult_GetDidCancel(obj);
            DidSendRequest = CLIB.ppf_LaunchFriendRequestFlowResult_GetDidSendRequest(obj);
        }
    }
}