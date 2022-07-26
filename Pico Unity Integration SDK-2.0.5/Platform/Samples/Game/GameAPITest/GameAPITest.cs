using System;
using System.Collections.Generic;
using System.IO;
using Stark;
using UnityEngine;
using UnityEngine.UI;

using Pico.Platform;
using Pico.Platform.Models;

namespace Pico.Platform.Samples.Game
{
    using Application = UnityEngine.Application;
    public partial class GameAPITest : MonoBehaviour
    {
        public GameObject typeBtnTempObj;
        public GameObject funBtnTempObj;
        public GameObject inputTempObj;
        public Button executeBtn;
        public Button funPrePageBtn;
        public Button funNextPageBtn;
        public GameObject content;

        // const
        private const string TAG = "GameAPITest";

        static Dictionary<ParamName, InputField> inputs = new Dictionary<ParamName, InputField>();
        Dictionary<string, Button> typeBtnDic = new Dictionary<string, Button>();
        Dictionary<string, Button> functionBtnDic = new Dictionary<string, Button>();

        // Record the currently selected function for execution
        string curFunctionName;
        PPFFunctionConfig curConfig;

        private void Awake()
        {
            Application.logMessageReceivedThreaded += OnLogMessage;
        }

        void OnLogMessage(string condition, string stackTrace, LogType type)
        {
            DebugPanelHelper.Log(condition, stackTrace, type);
            CaptureLogThread(condition, stackTrace, type);
        }

        private void CaptureLogThread(string condition, string stackTrace, LogType type)
        {
            string error = "";
            switch (type)
            {
                case LogType.Error:
                case LogType.Exception:
                    error = StackTraceUtility.ExtractStackTrace();
                    break;
                case LogType.Assert:
                case LogType.Warning:
                case LogType.Log:
                    break;
                default:
                    break;
            }

            string log = type + $" >> {DateTime.Now.ToString()} \n {condition} \n {error} \n";
            SaveLog(log);
        }

        void SaveLog(string log)
        {
            string path;
            if (Application.platform == RuntimePlatform.Android)
            {
                path = Application.persistentDataPath + "/log.txt";
            }
            else
            {
                path = "log.txt";
            }

            if (File.Exists(path))
            {
                File.AppendAllText(path, log);
            }
            else
            {
                File.WriteAllText(path, log);
            }
        }


        void Start()
        {
            LogHelper.LogInfo(TAG, "Start");
            RegisterCallbacks();
            InitClickListeners();
            InitTabs();
        }

        private void RegisterCallbacks()
        {
            MatchmakingService.SetMatchFoundNotificationCallback(ProcessMatchmakingMatchFound);
            RoomService.SetUpdateNotificationCallback(ProcessRoomUpdate);
            NetworkService.SetNotification_Game_ConnectionEventCallback(OnGameConnectionEvent);
            NetworkService.SetNotification_Game_Request_FailedCallback(OnRequestFailed);
            NetworkService.SetNotification_Game_StateResetCallback(OnGameStateReset);
            
            // test callback
            MatchmakingService.SetCancel2NotificationCallback(OnMatchmakingCancel2Notification);
            RoomService.SetLeaveNotificationCallback(OnRoomLeaveNotification);
            RoomService.SetJoin2NotificationCallback(OnRoomJoin2Notification);
            RoomService.SetSetDescriptionNotificationCallback(OnRoomSetDescriptionNotification);
            RoomService.SetKickUserNotificationCallback(OnRoomKickUserNotification);
            RoomService.SetUpdateOwnerNotificationCallback(OnRoomUpdateOwnerNotification);
            RoomService.SetUpdateDataStoreNotificationCallback(OnRoomUpdateDataStoreNotification);
            RoomService.SetUpdateMembershipLockStatusNotificationCallback(OnRoomUpdateMembershipLockStatusNotification);
        }


        private void InitTabs()
        {
            var typeDic = new Dictionary<string, Dictionary<string, PPFFunctionConfig>>
            {
                ["Basic"] = initDic,
                ["MatchMaking"] = matchDic,
                ["Rooms"] = roomDic
            };
            foreach (var item in typeDic)
            {
                GameObject obj = Instantiate(typeBtnTempObj, typeBtnTempObj.transform.parent);
                obj.name = item.Key;
                obj.transform.Find("Text").GetComponent<Text>().text = item.Key;
                obj.SetActive(true);
                var btn = obj.GetComponent<Button>();
                typeBtnDic.Add(item.Key, btn);
                btn.onClick.AddListener(() =>
                {
                    CheckBtnSelect(item.Key, "");
                    functionBtnDic.Clear();
                    // delete
                    RemoveAllChildren(funBtnTempObj.transform.parent.gameObject);

                    foreach (var v in item.Value)
                    {
                        GameObject funBtnObj = Instantiate(funBtnTempObj, funBtnTempObj.transform.parent);
                        funBtnObj.name = v.Key;
                        funBtnObj.transform.Find("Text").GetComponent<Text>().text = v.Key;
                        funBtnObj.SetActive(true);
                        var funButton = funBtnObj.GetComponent<Button>();
                        functionBtnDic.Add(v.Key, funButton);
                        var btnImage = funButton.GetComponent<Image>();
                        btnImage.color = Color.white;
                        funButton.GetComponent<Image>().color = Color.yellow;
                        funButton.onClick.AddListener(() =>
                        {
                            CheckBtnSelect(item.Key, v.Key);
                            btnImage.color = Color.yellow;
                            curFunctionName = v.Key;
                            curConfig = v.Value;
                            // delete
                            RemoveAllChildren(inputTempObj.transform.parent.gameObject);

                            // show params
                            foreach (var paramName in v.Value.paramList)
                            {
                                GameObject inputObj = Instantiate(inputTempObj, inputTempObj.transform.parent);
                                inputObj.transform.Find("TitleBg/Text").GetComponent<Text>().text = paramsDic[paramName][0];
                                var inputField = inputObj.transform.Find("InputField").GetComponent<InputField>();
                                inputField.text = paramsDic[paramName][1];
                                inputs[paramName] = inputField;
                                inputObj.SetActive(true);
                            }
                        });
                    }
                });
            }
        }

        private void CheckBtnSelect(string type, string funName)
        {
            foreach (var v in typeBtnDic)
            {
                v.Value.gameObject.GetComponent<Image>().color = Color.white;
            }

            typeBtnDic[type].gameObject.GetComponent<Image>().color = Color.yellow;
            foreach (var v in functionBtnDic)
            {
                v.Value.gameObject.GetComponent<Image>().color = Color.white;
            }

            if (!string.IsNullOrEmpty(funName))
            {
                functionBtnDic[funName].gameObject.GetComponent<Image>().color = Color.yellow;
            }
        }

        private void InitClickListeners()
        {
            executeBtn.onClick.AddListener(OnExecuteBtnClick);
            funPrePageBtn.onClick.AddListener(OnFunPrePageBtnClick);
            funNextPageBtn.onClick.AddListener(OnFunNextPageBtnClick);
        }
        
        void OnFunPrePageBtnClick()
        {
            var totalHeight = content.GetComponent<RectTransform>().rect.height;
            var pageHeight = content.transform.parent.GetComponent<RectTransform>().rect.height;
            var pos = content.transform.localPosition;
            if (pos.y - pageHeight < 0)
                return;
            content.transform.localPosition = new Vector3(pos.x, pos.y - pageHeight, pos.z);
        }

        void OnFunNextPageBtnClick()
        {
            var totalHeight = content.GetComponent<RectTransform>().rect.height;
            var pageHeight = content.transform.parent.GetComponent<RectTransform>().rect.height;
            var pos = content.transform.localPosition;
            if (pos.y + pageHeight > totalHeight)
                return;
            content.transform.localPosition = new Vector3(pos.x, pos.y + pageHeight, pos.z);
        }

        public static void RemoveAllChildren(GameObject parent)
        {
            Transform transform;
            for (int i = 0; i < parent.transform.childCount; i++)
            {
                transform = parent.transform.GetChild(i);
                if (transform.gameObject.activeSelf)
                {
                    Destroy(transform.gameObject);
                }
            }
        }

        void OnExecuteBtnClick()
        {
            if (curConfig == null)
                return;
            // Execute function
            string paramsStr = "";
            curConfig.Execute(curFunctionName, GetParams(curConfig.paramList, out paramsStr));
            LogHelper.LogInfo(TAG, $"Execute: {curFunctionName}({paramsStr})");
        }
        List<string> GetParams(List<ParamName> paramList, out string paramsStr)
        {
            paramsStr = "";
            List<string> paramValueList = new List<string>();
            for (int i = 0; i < paramList.Count; i++)
            {
                var value = inputs[paramList[i]].text;
                LogHelper.LogInfo(TAG, $"GetParam({i}): {value}");
                
                paramsStr += i == paramList.Count - 1 ? value : value + ",";
                paramValueList.Add(value);
            }

            return paramValueList;
        }

        private void OnDestroy()
        {
            Uninitialize();
        }

        void Update()
        {
        }

        static void Uninitialize()
        {
            CoreService.GameUninitialize();
        }

    }
}