using System;
using UnityEngine;
namespace Stark
{
    public class DebugItemObject
    {
        public LogType logType;
        public string logInfo;
        public DebugItemObject(string logInfo, LogType type)
        {
            this.logInfo = $"{logInfo}{DateTime.Now.ToString()}";
            this.logType = type;
        }
    }
}