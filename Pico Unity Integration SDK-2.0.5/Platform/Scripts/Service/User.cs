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
    /**
     * \ingroup Platform
     */
    public class UserService
    {
        /// <summary>
        /// Returns an access token for this user. NOTE: User's permission is required if the user uses this app firstly.
        /// </summary>
        /// <returns>The access token for the current user.</returns>
        public static Task<string> GetAccessToken()
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.UninitializedError);
                return null;
            }

            return new Task<string>(CLIB.ppf_User_GetAccessToken());
        }

        /// <summary>
        /// Gets the information about the current logged-in user.
        /// </summary>
        /// <returns>The User structure that contains the details about the user.</returns>
        public static Task<User> GetLoggedInUser()
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.UninitializedError);
                return null;
            }

            return new Task<User>(CLIB.ppf_User_GetLoggedInUser());
        }

        /// <summary>
        /// Gets the information about a specified user.
        /// NOTE: The same user has different user IDs for different apps.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>The User structure that contains the details about the specified user.</returns>
        public static Task<User> Get(string userId)
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.UninitializedError);
                return null;
            }

            return new Task<User>(CLIB.ppf_User_Get(userId));
        }

        /// <summary>
        /// Gets the friend list of the current user. Friends who don't use this app won't appear in this list.
        /// </summary>
        /// <returns>The friend list of the current user.</returns>
        public static Task<UserList> GetFriends()
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.UninitializedError);
                return null;
            }

            return new Task<UserList>(CLIB.ppf_User_GetLoggedInUserFriends());
        }

        /// <summary>
        /// Launches the flow to apply for friendship with someone.
        /// </summary>
        /// <param name="userId">The ID of the user that the friend request is sent to.</param>
        /// <returns>`LaunchFriendRequest` that indicates whether the request is sent successfully.</returns>
        public static Task<LaunchFriendResult> LaunchFriendRequestFlow(string userId)
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.UninitializedError);
                return null;
            }

            return new Task<LaunchFriendResult>(CLIB.ppf_User_LaunchFriendRequestFlow(userId));
        }
        /// <summary>
        /// Gets the next page of user list.
        /// </summary>
        /// <param name="list">The user list from the current page.</param>
        /// <returns>The user list from the next page.</returns>
        public static Task<UserList> GetNextUserListPage(UserList list)
        {
            if (!CoreService.Initialized)
            {
                Debug.LogError(CoreService.UninitializedError);
                return null;
            }

            if (!list.HasNextPage)
            {
                Debug.LogWarning("GetNextUserListPage: List has no next page");
                return null;
            }

            if (String.IsNullOrEmpty(list.NextPageParam))
            {
                Debug.LogWarning("GetNextUserListPage: list.NextPageParam is empty");
                return null;
            }

            return new Task<UserList>(CLIB.ppf_User_GetNextUserArrayPage(list.NextPageParam));
        }
    }

    public enum Gender
    {
        Unknown = 0,
        Male = 1,
        Female = 2,
    }

    public enum UserPresenceStatus
    {
        Unknown = 0,
        OnLine = 1,
        OffLine = 2,
    }
}