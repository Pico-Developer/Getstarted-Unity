using System;
using System.Text;
using System.Text.RegularExpressions;
using Pico.Platform.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Pico.Platform.Samples.SimpleSample
{
    delegate void Handler(string[] args);

    class Fun
    {
        public Handler handler;
        public string desc;
        public string key;

        public Fun(string key, string desc, Handler handler)
        {
            this.key = key;
            this.desc = desc;
            this.handler = handler;
        }
    }

    public class TestEntry : MonoBehaviour
    {
        public Text dataOutput;
        public Text commandList;
        public GameObject inputField;
        public GameObject executeButton;
        public bool useAsyncInit = true;

        private Fun[] funList;

        private UserList cacheUserList;
        void Start()
        {
            Log($"UseAsyncInit={useAsyncInit}");
            if (useAsyncInit)
            {
                try
                {
                    CoreService.AsyncInitialize().OnComplete(m =>
                    {
                        if (m.IsError)
                        {
                            Log($"Async initialize failed: code={m.GetError().Code} message={m.GetError().Message}");
                            return;
                        }

                        if (m.Data != PlatformInitializeResult.Success && m.Data != PlatformInitializeResult.AlreadyInitialized)
                        {
                            Log($"Async initialize failed: result={m.Data}");
                            return;
                        }

                        Log("AsyncInitialize Successfully");
                    });
                }
                catch (Exception e)
                {
                    Log($"Async Initialize Failed:{e}");
                    return;
                }
            }
            else
            {
                try
                {
                    CoreService.Initialize();
                }
                catch (UnityException e)
                {
                    Log($"Init Platform SDK error:{e}");
                    throw;
                }
            }

            funList = new[]
            {
                new Fun("a", "a : GetAccessToken", GetAccessToken),
                new Fun("aa", "aa : GetLoggedInUser", GetLoggedInUser),
                new Fun("b", "b <userId> : LaunchFriendRequest to <userId>", LaunchFriendRequest),
                //c : 获取我的好友列表
                new Fun("c", "c : GetLoggedInUserFriends", GetLoggedInUserFriends),
                new Fun("d", "d <userId> : GetUser by <userId>", GetUser),
                new Fun("e", "e : GetUserArrayNextPage", GetUserArrayNextPage),
                new Fun("z", "z : Change to RTC test Scene", args => { SceneManager.LoadScene("RtcDemo"); })
            };
            StringBuilder s = new StringBuilder();
            foreach (var i in funList)
            {
                s.AppendLine(i.desc);
            }

            commandList.text = s.ToString();
            executeButton.GetComponent<Button>().onClick.AddListener(OnButtonClick);
        }

        private void OnButtonClick()
        {
            var inputText = inputField.GetComponent<InputField>();
            string currentText = inputText.text.Trim();
            if (String.IsNullOrWhiteSpace(currentText))
            {
                return;
            }

            Log($"Got command text {currentText}");
            var args = Regex.Split(currentText, @"\s+");
            if (args.Length == 0)
            {
                Log("Please input a command");
                return;
            }

            var key = args[0];
            var handled = false;
            foreach (var cmd in funList)
            {
                if (cmd.key.Equals(key))
                {
                    try
                    {
                        cmd.handler(args);
                    }
                    catch (Exception e)
                    {
                        Log($"Handle command error:{cmd.desc} e={e}");
                    }

                    handled = true;
                    break;
                }
            }

            if (!handled)
            {
                Log($"Cannot find command for :{key}");
            }

            inputField.GetComponent<InputField>().text = "";
        }

        string UserList2String(UserList userList)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append($"UserList:Count{userList.Count} hasNext={userList.HasNextPage} bodyParams={userList.NextPageParam}");
            foreach (var u in userList)
            {
                builder.Append(u.DisplayName).Append(",");
            }

            return builder.ToString();
        }

        string User2String(User user)
        {
            return $"name={user.DisplayName},ID={user.ID},headImage={user.ImageUrl},presenceStatus={user.PresenceStatus}";
        }
        void GetAccessToken(string[] args)
        {
            Log("GetAccessToken...");
            UserService.GetAccessToken().OnComplete(delegate(Message<string> message)
            {
                if (message.IsError)
                {
                    var err = message.GetError();
                    Log($"Got access token error {err.Message} code={err.Code}");
                    return;
                }

                string accessToken = message.Data;
                Log($"Got accessToken {accessToken}");
            });
        }

        void LaunchFriendRequest(string[] args)
        {
            if (args.Length != 2)
            {
                Log("Lack <userId> argument");
                return;
            }

            UserService.LaunchFriendRequestFlow(args[1]).OnComplete(msg =>
            {
                if (msg.IsError)
                {
                    var err = msg.GetError();
                    Log($"Launch friend request error:{err.Message};code={err.Code}");
                    return;
                }

                var launchResult = msg.Data;
                Log(
                    $"Launch friend request ok:DidCancel={launchResult.DidCancel},DidSend={launchResult.DidSendRequest}");
            });
        }

        void GetLoggedInUserFriends(string[] args)
        {
            Log($"Trying to get FriendList");
            UserService.GetFriends().OnComplete(msg =>
            {
                if (msg.IsError)
                {
                    var err = msg.GetError();
                    Log($"Get Friends error {err.Message} code={err.Code}");
                    return;
                }

                var userList = msg.Data;
                cacheUserList = userList;
                Log($"Your friend list:{UserList2String(userList)}");
            });
        }

        void GetUser(string[] args)
        {
            if (args.Length != 2)
            {
                Log("Argument error");
                return;
            }

            Log($"Trying to get user {args[1]}");
            UserService.Get(args[1]).OnComplete(msg =>
            {
                if (msg.IsError)
                {
                    var err = msg.GetError();
                    Log($"Get user info failed:{err.Message} {err.Code}");
                }
                else
                {
                    var usr = msg.Data;
                    Log($"get user info by id={args[1]}：{User2String(usr)}");
                }
            });
        }

        void GetLoggedInUser(string[] args)
        {
            Log("Trying to get currently logged in user");
            UserService.GetLoggedInUser().OnComplete(msg =>
            {
                if (!msg.IsError)
                {
                    Log("Received get user success");
                    User user = msg.Data;
                    Log($"User: {User2String(user)}");
                    Debug.Log(JsonUtility.ToJson(user));
                }
                else
                {
                    Log("Received get user error");
                    Error error = msg.GetError();
                    Log("Error: " + error.Message);
                }
            });
        }

        void GetUserArrayNextPage(string[] args)
        {
            if (cacheUserList == null)
            {
                Log("userList is empty,please init userList first");
                return;
            }

            if (!cacheUserList.HasNextPage)
            {
                Log("Has no next page");
                return;
            }

            Log("User.GetNextUserListPage...");
            UserService.GetNextUserListPage(cacheUserList).OnComplete(msg =>
            {
                if (msg.IsError)
                {
                    var err = msg.GetError();
                    Log($"Get next page failed：{err.Message}");
                    return;
                }

                Log($"Get next page successfully");
                var userList = msg.Data;
                cacheUserList = userList;
                Log($"next page data is ：{UserList2String(userList)}");
            });
        }

        void Log(String newLine)
        {
            Debug.Log(newLine);
            dataOutput.text = "> " + newLine + Environment.NewLine + dataOutput.text;
            if (dataOutput.text.Length > 1000)
            {
                dataOutput.text = dataOutput.text.Substring(0, 1000);
            }
        }
    }
}