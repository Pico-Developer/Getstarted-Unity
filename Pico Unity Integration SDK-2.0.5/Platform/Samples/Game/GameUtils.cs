using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Pico.Platform;
using Pico.Platform.Samples.Game;


namespace Pico.Platform.Samples.Game
{
    public class GameUtils : MonoBehaviour
    {
        private const string TAG = "GameUtils";
        public static bool IsInt(string strNumber)
        {
            try
            {
                var result = Convert.ToInt32(strNumber);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsDouble(string strNumber)
        {
            try
            {
                var result = Convert.ToDouble(strNumber);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static RoomOptions GetRoomOptions(string roomID, string maxUserResults, string turnOffUpdates, string dataKeys, string dataValuses, string excludeRecentlyMet)
        {
            RoomOptions options = new RoomOptions();
            if (!string.IsNullOrEmpty(roomID))
            {
                options.SetRoomId(Convert.ToUInt64(roomID));
            }
            if (!string.IsNullOrEmpty(maxUserResults))
            {
                options.SetMaxUserResults(Convert.ToUInt32(maxUserResults));
            }
            if (!string.IsNullOrEmpty(turnOffUpdates))
            {
                options.SetTurnOffUpdates(Convert.ToBoolean(turnOffUpdates));
            }
            if (!string.IsNullOrEmpty(turnOffUpdates))
            {
                options.SetExcludeRecentlyMet(Convert.ToBoolean(excludeRecentlyMet));
            }
            if (!string.IsNullOrEmpty(dataKeys) && !string.IsNullOrEmpty(dataValuses))
            {
                string[] keys = dataKeys.Split(';');
                string[] values = dataValuses.Split(';');
                if (keys.Length != values.Length)
                {
                    LogHelper.LogError(TAG, "dataKeys.Length != dataValuses.Length");
                }
                else
                {
                    for (int i = 0; i < keys.Length; i++)
                    {
                        options.SetDataStore(keys[i], values[i]);
                    }
                }
            }

            return options;
        }
        public static Platform.MatchmakingOptions GetMatchmakingOptions(string roomMaxUsers, string enqueueIsDebug, string enqueueQueryKey
            , string enqueueKeys, string enqueueValues, string roomKeys, string roomValues)
        {
            Platform.MatchmakingOptions matchmakingOptions = new Platform.MatchmakingOptions();
            if (!string.IsNullOrEmpty(roomMaxUsers))
            {
                matchmakingOptions.SetCreateRoomMaxUsers(Convert.ToUInt32(roomMaxUsers));
            }
            if (!string.IsNullOrEmpty(enqueueIsDebug))
            {
                matchmakingOptions.SetEnqueueIsDebug(Convert.ToBoolean(enqueueIsDebug));
            }
            if (!string.IsNullOrEmpty(enqueueQueryKey))
            {
                matchmakingOptions.SetEnqueueQueryKey(enqueueQueryKey);
            }
            if (!string.IsNullOrEmpty(enqueueKeys) && !string.IsNullOrEmpty(enqueueValues))
            {
                string[] keys = enqueueKeys.Split(';');
                string[] values = enqueueValues.Split(';');
                if (keys.Length != values.Length)
                {
                    LogHelper.LogError(TAG, "enqueueKeys.Length != enqueueValues.Length");
                }
                else
                {
                    for (int i = 0; i < keys.Length; i++)
                    {
                        if (IsInt(values[i]))
                        {
                            matchmakingOptions.SetEnqueueDataSettings(keys[i], Convert.ToInt32(values[i]));
                        }
                        else if (IsDouble(values[i]))
                        {
                            matchmakingOptions.SetEnqueueDataSettings(keys[i], Convert.ToDouble(values[i]));
                        }
                        else
                        {
                            matchmakingOptions.SetEnqueueDataSettings(keys[i], values[i]);
                        }
                    }
                }
            }
            if (!string.IsNullOrEmpty(roomKeys) && !string.IsNullOrEmpty(roomValues))
            {
                string[] keys = roomKeys.Split(';');
                string[] values = roomValues.Split(';');
                if (keys.Length != values.Length)
                {
                    LogHelper.LogError(TAG, "roomKeys.Length != roomValues.Length");
                }
                else
                {
                    for (int i = 0; i < keys.Length; i++)
                    {
                        matchmakingOptions.SetCreateRoomDataStore(keys[i], values[i]);
                    }
                }
            }
            return matchmakingOptions;
        }
        public static Dictionary<string, int> GetDicData(string keysStr, string valuesStr)
        {
            Dictionary<string, int> data = new Dictionary<string, int>();
            string[] keys = keysStr.Split(';');
            string[] values = valuesStr.Split(';');
            if (keys.Length != values.Length)
            {
                LogHelper.LogError(TAG, "GetDicData keys.Length != values.Length");
            }
            else
            {
                for (int i = 0; i < keys.Length; i++)
                {
                    if (!data.ContainsKey(keys[i]))
                    {
                        try
                        {
                            int value = Convert.ToInt32(values[i]);
                            data.Add(keys[i], value);
                        }
                        catch (Exception e)
                        {
                            LogHelper.LogError(TAG, $"{e}, please check the input is correct");
                        }

                    }
                }
            }
            return data;
        }
        public static Dictionary<string, string> GetStringDicData(string keys, string values)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            string[] keyArray = keys.Split(';');
            string[] valueArray = values.Split(';');
            if (keyArray.Length != valueArray.Length)
            {
                LogHelper.LogError(TAG, "GetStringDicData keyArray.Length != valueArray.Length");
            }
            else
            {
                for (int i = 0; i < keyArray.Length; i++)
                {
                    if (!data.ContainsKey(keyArray[i]))
                    {
                        data.Add(keyArray[i], valueArray[i]);
                    }
                }
            }
            return data;
        }
        
    }
}

