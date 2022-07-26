using System;
using System.Collections.Generic;
using System.Text;
using Pico.Platform.Models;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Pico.Platform.Samples.RtcDemo
{
    public static class UIBehaviourExtension
    {
        public static T OnEventTriggerEvent<T>(this T self, EventTriggerType type, UnityAction<BaseEventData> action) where T : UIBehaviour
        {
            var eventTrigger = self.GetComponent<EventTrigger>() ?? self.gameObject.AddComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry {eventID = type};
            entry.callback.AddListener(action);
            eventTrigger.triggers.Add(entry);
            return self;
        }
    }

    class Room
    {
        public string id;
        public GameObject GameObject;
        public Button leaveRoomButton;
        public Toggle publishToggle;
        public Text textRoomId;
        public Button destroyRoomButton;
        public Toggle toggleRemoteAudio;
    }

    public class Phone : MonoBehaviour
    {
        public GameObject roomPrefab;
        private Text outputText;
        private InputField _inputFieldRoomId;
        private InputField _inputFieldUserId;
        float _lastRoomStatsTime = 0; //The timestamp of print room stats.Used for control frequency.
        private List<Room> _rooms = new List<Room>();
        private GameObject _roomListPanel;
        private Dropdown dropdownRoomProfile;

        public static GameObject FindChild(GameObject parent, string childName)
        {
            for (var ind = 0; ind < parent.transform.childCount; ind++)
            {
                var i = parent.transform.GetChild(ind);
                if (i.gameObject.name.Trim().Equals(childName.Trim()))
                {
                    return i.gameObject;
                }
            }

            return null;
        }

        bool CheckRoomId()
        {
            if (String.IsNullOrWhiteSpace(_inputFieldRoomId.text))
            {
                Log($"Please input Room Id");
                return false;
            }

            return true;
        }

        bool CheckUserId()
        {
            if (String.IsNullOrWhiteSpace(_inputFieldUserId.text))
            {
                Log($"Please input User Id");
                return false;
            }

            return true;
        }

        void initRtc()
        {
            var res = RtcService.InitRtcEngine();
            if (res != RtcEngineInitResult.Success)
            {
                Log($"Init RTC Engine Failed{res}");
                throw new UnityException($"Init RTC Engine Failed:{res}");
            }

            RtcService.EnableAudioPropertiesReport(2000);
        }

        private void Start()
        {
            Log("start");
            outputText = GameObject.Find("TextOutput").GetComponent<Text>();
            _inputFieldRoomId = GameObject.Find("InputFieldRoomId").GetComponent<InputField>();
            _inputFieldUserId = GameObject.Find("InputFieldUserId").GetComponent<InputField>();
            _roomListPanel = GameObject.Find("RoomListPanel");
            dropdownRoomProfile = GameObject.Find("DropdownRoomProfile").GetComponent<Dropdown>();

            var buttonEnterRoom = GameObject.Find("ButtonEnterRoom").GetComponent<Button>();
            var toggleEarMonitorMode = GameObject.Find("ToggleEarMonitor").GetComponent<Toggle>();
            var toggleMute = GameObject.Find("ToggleMute").GetComponent<Toggle>();
            var toggleCapture = GameObject.Find("ToggleCapture").GetComponent<Toggle>();
            var sliderRecordingVolume = GameObject.Find("SliderRecordingVolume").GetComponent<Slider>();
            var sliderPlaybackVolume = GameObject.Find("SliderPlaybackVolume").GetComponent<Slider>();
            var sliderEarMonitorVolume = GameObject.Find("SliderEarMonitorVolume").GetComponent<Slider>();
            var dropDownScenarioType = GameObject.Find("DropdownScenarioType").GetComponent<Dropdown>();
            try
            {
                if (CoreService.Initialized)
                {
                    initRtc();
                }
                else
                {
                    CoreService.AsyncInitialize().OnComplete(m =>
                    {
                        if (m.IsError)
                        {
                            Log($"Init PlatformSdk failed:code={m.Error.Code},message={m.Error.Message}");
                            return;
                        }

                        if (m.Data == PlatformInitializeResult.Success || m.Data == PlatformInitializeResult.AlreadyInitialized)
                        {
                            Log($"Init PlatformSdk successfully");
                            initRtc();
                        }
                        else
                        {
                            Log($"Init PlatformSdk failed:{m.Data}");
                        }
                    });
                }
            }
            catch (Exception e)
            {
                Log($"Init Platform SDK failed {e}");
                throw;
            }

            buttonEnterRoom.onClick.AddListener(OnClickJoinRoom);

            #region endregionEngine Config List

            toggleCapture.onValueChanged.AddListener((v) =>
            {
                if (v)
                {
                    Log($"Before StartAudioCapture");
                    RtcService.StartAudioCapture();
                    Log($"StartAudioCapture Done");
                }
                else
                {
                    Log($"Before StopAudioCapture");
                    RtcService.StopAudioCapture();
                    Log($"StopAudioCapture Done");
                }
            });
            toggleEarMonitorMode.onValueChanged.AddListener(earMonitorMode =>
            {
                Log($"EarMonitorMode {earMonitorMode}");
                if (earMonitorMode)
                {
                    RtcService.SetEarMonitorMode(RtcEarMonitorMode.On);
                }
                else
                {
                    RtcService.SetEarMonitorMode(RtcEarMonitorMode.Off);
                }

                Log($"EarMonitorMode {earMonitorMode} Done");
            });
            toggleMute.onValueChanged.AddListener(mute =>
            {
                Log($"MuteLocalAudio {mute}");
                if (mute)
                {
                    RtcService.MuteLocalAudio(RtcMuteState.On);
                }
                else
                {
                    RtcService.MuteLocalAudio(RtcMuteState.Off);
                }

                Log($"MuteLocalAudio {mute} Done");
            });
            sliderPlaybackVolume.OnEventTriggerEvent(EventTriggerType.PointerUp, x =>
            {
                var v = sliderPlaybackVolume.value;
                Log($"SetPlaybackAudio{v}");
                RtcService.SetPlaybackVolume((int) v);
            });
            sliderRecordingVolume.OnEventTriggerEvent(EventTriggerType.PointerUp, e =>
            {
                var v = sliderRecordingVolume.value;
                Log($"SetRecordingVolume{v}");
                RtcService.SetCaptureVolume((int) v);
            });
            sliderEarMonitorVolume.OnEventTriggerEvent(EventTriggerType.PointerUp, e =>
            {
                var v = sliderEarMonitorVolume.value;
                Log($"SetEarMonitorVolume {v}");
                RtcService.SetEarMonitorVolume((int) v);
            });
            dropDownScenarioType.onValueChanged.AddListener(v =>
            {
                Log($"setting scenarioType {v}");
                RtcService.SetAudioScenario((RtcAudioScenarioType) v);
                Log($"setting scenarioType {v} done");
            });

            #endregion

            #region Callbacks

            RtcService.SetOnJoinRoomResultCallback(OnJoinRoom);
            RtcService.SetOnLeaveRoomResultCallback(OnLeaveRoom);
            RtcService.SetOnUserLeaveRoomResultCallback(OnUserLeaveRoom);
            RtcService.SetOnUserJoinRoomResultCallback(OnUserJoinRoom);
            RtcService.SetOnRoomStatsCallback(OnRoomStats);
            RtcService.SetOnWarnCallback(OnWarn);
            RtcService.SetOnErrorCallback(OnError);
            RtcService.SetOnRoomWarnCallback(OnRoomWarn);
            RtcService.SetOnRoomErrorCallback(OnRoomError);
            RtcService.SetOnConnectionStateChangeCallback(OnConnectionStateChange);
            RtcService.SetOnUserMuteAudio(OnUserMuteAudio);
            RtcService.SetOnUserStartAudioCapture(OnUserStartAudioCapture);
            RtcService.SetOnUserStopAudioCapture(OnUserStopAudioCapture);
            RtcService.SetOnLocalAudioPropertiesReport(OnLocalAudioPropertiesReport);
            RtcService.SetOnRemoteAudioPropertiesReport(OnRemoteAudioPropertiesReport);

            #endregion
        }

        private void OnRemoteAudioPropertiesReport(Message<RtcRemoteAudioPropertiesReport> message)
        {
            var d = message.Data;
            StringBuilder builder = new StringBuilder();
            foreach (var usr in d.AudioPropertiesInfos)
            {
                if (usr.AudioPropertiesInfo.Volume > 5)
                {
                    builder.Append($"user={usr.StreamKey.UserId} roomId={usr.StreamKey.RoomId} volume={usr.AudioPropertiesInfo.Volume} ");
                }
            }

            var report = builder.ToString();
            Log($@"[RemoteAudioPropertiesReport]totalVolume={d.TotalRemoteVolume} {d.AudioPropertiesInfos.Length}
{report}
");
        }

        private void OnLocalAudioPropertiesReport(Message<RtcLocalAudioPropertiesReport> message)
        {
            var d = message.Data;
            StringBuilder builder = new StringBuilder();
            foreach (var i in d.AudioPropertiesInfos)
            {
                builder.Append(i.AudioPropertyInfo.Volume).Append(",");
            }

            Log($@"[LocalAudioPropertiesReport] {d.AudioPropertiesInfos.Length} LocalVolume={builder}
");
        }

        private void OnUserStopAudioCapture(Message<string> message)
        {
            var d = message.Data;
            Log($"[UserStopAudioCapture]UserId={d}");
        }

        private void OnUserStartAudioCapture(Message<string> message)
        {
            var d = message.Data;
            Log($"[UserStartAudioCapture]UserId={d}");
        }

        private void OnUserMuteAudio(Message<RtcMuteInfo> message)
        {
            var d = message.Data;
            Log($"[UserMuteAudio]userId={d.UserId} muteState={d.MuteState}");
        }

        private void OnClickJoinRoom()
        {
            if (!(CheckRoomId() && CheckUserId()))
            {
                return;
            }

            var roomProfile = (RtcRoomProfileType) dropdownRoomProfile.value;
            var roomId = _inputFieldRoomId.text;
            var userId = _inputFieldUserId.text;
            Log($"userId={userId} roomId={roomId} scenarioType={roomProfile}");
            var privilege = new Dictionary<RtcPrivilege, int>();
            privilege.Add(RtcPrivilege.PublishStream, 3600 * 2);
            privilege.Add(RtcPrivilege.SubscribeStream, 3600 * 2);
            RtcService.GetToken(roomId, userId, 3600 * 2, privilege).OnComplete(msg =>
            {
                if (msg.IsError)
                {
                    Log($"Get rtc token failed: code={msg.GetError().Code} message={msg.GetError().Message}");
                    return;
                }

                var token = msg.Data;
                Log($"Got RTC Token:{token}");
                int result = RtcService.JoinRoom(roomId, userId, token, roomProfile, true);
                Log($"Join Room Result={result} RoomId={_inputFieldRoomId.text}");
            });
        }

        private void OnConnectionStateChange(Message<RtcConnectionState> message)
        {
            Log($"[ConnectionStateChange] {message.Data}");
        }

        private void OnRoomError(Message<RtcRoomError> message)
        {
            var e = message.Data;
            Log($"[RtcRoomError]RoomId={e.RoomId} Code={e.Code}");
        }

        private void OnRoomWarn(Message<RtcRoomWarn> message)
        {
            var e = message.Data;
            Log($"[RtcRoomWarn]RoomId={e.RoomId} Code={e.Code}");
        }

        private void OnError(Message<int> message)
        {
            Log($"[RtcError] {message.Data}");
        }

        private void OnWarn(Message<int> message)
        {
            Log($"[RtcWarn] {message.Data}");
        }

        private void OnJoinRoom(Message<RtcJoinRoomResult> msg)
        {
            if (msg.IsError)
            {
                var err = msg.GetError();
                var joinRoomResult = msg.Data;
                var roomId = "";
                if (joinRoomResult != null)
                {
                    roomId = joinRoomResult.RoomId;
                }

                Log($"[JoinRoomError]code={err.Code} message={err.Message} roomId={roomId}");
                return;
            }

            var rtcJoinRoomResult = msg.Data;
            if (rtcJoinRoomResult.ErrorCode != 0)
            {
                Log($"[JoinRoomError]code={rtcJoinRoomResult.ErrorCode} RoomId={rtcJoinRoomResult.RoomId} UserId={rtcJoinRoomResult.UserId}");
                return;
            }

            Log("Before Join Room");
            Log($"[JoinRoomOk] Elapsed:{rtcJoinRoomResult.Elapsed} JoinType:{rtcJoinRoomResult.JoinType} RoomId:{rtcJoinRoomResult.RoomId} UserId:{rtcJoinRoomResult.UserId}");
            var room = new Room();
            room.id = rtcJoinRoomResult.RoomId;
            room.GameObject = Instantiate(roomPrefab, _roomListPanel.transform);
            room.leaveRoomButton = FindChild(room.GameObject, "ButtonLeaveRoom").GetComponent<Button>();
            room.publishToggle = FindChild(room.GameObject, "TogglePublish").GetComponent<Toggle>();
            room.destroyRoomButton = FindChild(room.GameObject, "ButtonDestroyRoom").GetComponent<Button>();
            room.toggleRemoteAudio = FindChild(room.GameObject, "ToggleRemoteAudio").GetComponent<Toggle>();
            room.textRoomId = FindChild(room.GameObject, "TextRoomId").GetComponent<Text>();
            room.textRoomId.text = room.id;
            room.leaveRoomButton.onClick.AddListener(() =>
            {
                Log($"Doing leave room:{room.id}");
                int result = RtcService.LeaveRoom(room.id);
                // RemoveRoomFromUi(room.id);//在OnLeaveRoom回调中从UI上移除房间
                Log($"[LeaveRoomResult]={result} RoomId={room.id}");
            });
            room.toggleRemoteAudio.onValueChanged.AddListener(v =>
            {
                if (v)
                {
                    Log($"RoomPauseAllSubscribedStream {room.id}");
                    RtcService.RoomPauseAllSubscribedStream(room.id);
                    Log($"RoomPauseAllSubscribedStream {room.id} done");
                }
                else
                {
                    Log($"RoomResumeAllSubscribedStream {room.id}");
                    RtcService.RoomResumeAllSubscribedStream(room.id);
                    Log($"RoomResumeAllSubscribedStream {room.id} done");
                }
            });
            room.publishToggle.onValueChanged.AddListener(v =>
            {
                if (v)
                {
                    Log($"Publish {room.id}");
                    RtcService.PublishRoom(room.id);
                    Log($"Publish {room.id} done");
                }
                else
                {
                    Log($"UnPublish {room.id}");
                    RtcService.UnPublishRoom(room.id);
                    Log($"UnPublish {room.id} done");
                }
            });
            room.destroyRoomButton.onClick.AddListener(() =>
            {
                Log($"DestroyRoom {room.id}");
                RtcService.DestroyRoom(room.id);
                RemoveRoomFromUi(room.id);
                Log($"DestroyRoom {room.id} done");
            });
            _rooms.Add(room);
        }

        private void OnLeaveRoom(Message<RtcLeaveRoomResult> msg)
        {
            if (msg.IsError)
            {
                var err = msg.GetError();
                Log($"[LeaveRoomResult]code={err.Code} message={err.Message}");
                return;
            }

            var res = msg.Data;
            Log($"[LeaveRoomOk]RoomId={res.RoomId}");
            RemoveRoomFromUi(res.RoomId);
        }

        void RemoveRoomFromUi(string roomId)
        {
            Room it = null;
            foreach (var r in _rooms)
            {
                if (r.id.Equals(roomId))
                {
                    it = r;
                    break;
                }
            }

            if (it != null)
            {
                Log($"Remove Room {it.id} from ui");
                _rooms.Remove(it);
                if (it.GameObject != null)
                {
                    Destroy(it.GameObject);
                    it.GameObject = null;
                }
            }
            else
            {
                Log($"Cannot find room in ui:{roomId}");
            }
        }

        private void OnUserLeaveRoom(Message<RtcUserLeaveInfo> msg)
        {
            if (msg.IsError)
            {
                var err = msg.GetError();
                return;
            }

            var res = msg.Data;
            Log($"[UserLeave]User[{res.UserId}] left room[{res.RoomId}],offline reason：{res.OfflineReason}");
        }

        private void OnUserJoinRoom(Message<RtcUserJoinInfo> msg)
        {
            if (msg.IsError)
            {
                var err = msg.GetError();
                return;
            }

            var res = msg.Data;
            Log($"[UserJoin]user={res.UserId} join room={res.RoomId},UserExtra={res.UserExtra},TimeElapsed{res.Elapsed}");
        }

        private void OnRoomStats(Message<RtcRoomStats> msg)
        {
            if (msg.IsError)
            {
                var err = msg.GetError();
                Log($"[RoomStats]Error {err.Code} {err.Message}");
                return;
            }

            if (Time.realtimeSinceStartup - _lastRoomStatsTime < 10)
            {
                return;
            }

            _lastRoomStatsTime = Time.realtimeSinceStartup;
            var res = msg.Data;
            Log($"[RoomStats]RoomId={res.RoomId} UserCount={res.UserCount} Duration={res.TotalDuration}");
        }

        void Log(string s)
        {
            Debug.Log($"[RtcDemo]{s}");
            if (outputText != null)
            {
                outputText.text = s + "\n" + outputText.text;
                if (outputText.text.Length > 1000)
                {
                    outputText.text = outputText.text.Substring(0, 1000);
                }
            }
        }
    }
}