using System;
using System.Collections;
using System.Collections.Generic;
using Pico.Platform;
using Pico;
using UnityEngine;
using UnityEngine.UI;

namespace Pico.Platform.Samples.Game
{
    public enum ParamName
    {
        PICO_ID,
        ROOM_ID,
        USER_ID,

        //SUBSCRIBE_TO_UPDATES,
        KICK_DURATION_SECONDS,
        DESCRIPTION,
        MEMBERSHIP_LOCK_STATUS,

        INIT_HOST,
        INIT_PORT,
        INIT_TOKEN,
        INIT_APPID,

        POOL_NAME,
        JOIN_POLICY,

        MATCH_MAX_LEVEL,
        MATCH_APPROACH,
        INVITE_TOKEN,

        //ROOM_OPTION_EXCLUDE_RECENTLY_MET,
        ROOM_OPTION_MAX_USER_RESULTS,
        ROOM_OPTION_TURN_OFF_UPDATES,
        ROOM_OPTION_DATASTORE_KEYS,
        ROOM_OPTION_DATASTORE_VALUES,
        ROOM_OPTION_ELCLUDERECENTLYMET,

        MATCHMAKING_OPTION_ROOM_MAX_USERS,
        MATCHMAKING_OPTION_ENQUEUE_IS_DEBUG,
        MATCHMAKING_OPTION_ENQUEUE_QUERY_KEY,

        MATCHMAKING_OPTION_ENQUEUE_KEYS,
        MATCHMAKING_OPTION_ENQUEUE_VALUES,
        MATCHMAKING_OPTION_ROOM_KEYS,
        MATCHMAKING_OPTION_ROOM_VALUES,

        MATCHMAKING_REPORT_RESULT_KEYS,
        MATCHMAKING_REPORT_RESULT_VALUES,

        MAX_USERS,
        PACKET_BYTES,

        ROOM_PAGE_INDEX,
        ROOM_PAGE_SIZE,

    }
    public delegate object PPFFunction(List<string> paramList);
    public class PPFFunctionConfig
    {
        public const string TAG = "PPFFunctionConfig";
        public List<ParamName> paramList = new List<ParamName>();
        PPFFunction function;
    
        public PPFFunctionConfig(PPFFunction fun, List<ParamName> list = null)
        {
            function = fun;
            if (list != null)
                paramList = list;
        }
    
        public void Execute(string functionName, List<string> paramValueList)
        {
            var rst = function(paramValueList);
            if (rst != null)
            {
                var o = rst as Task;
                if (o != null)
                {
                    ResultResultProcess(functionName, (rst as Task).TaskId);
                    return;
                }
    
                ResultResultProcess(functionName, rst);
                return;
            }
    
            LogHelper.LogError(TAG, $"{functionName} cannot identify result!");
        }
    
        private static void ResultResultProcess(string msg, object result)
        {
            LogHelper.LogInfo(TAG, $"{msg} result(string) is {result}!");
            if (0 == Convert.ToUInt64(result))
            {
                LogHelper.LogError(TAG, $"{msg} result is 0!");
            }
            else
            {
                LogHelper.LogInfo(TAG, $"{msg} result = {result}");
            }
        }
                
    }
}
