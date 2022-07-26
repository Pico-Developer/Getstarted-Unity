using System.Collections.Generic;
using Pico.Platform.Samples.Game;
using UnityEngine;

namespace Stark
{
    public interface IDebugInfoListener
    {
        void OnChanged();
        void OnClearAll();
    }

    public interface IDebugErrorListener
    {
        void OnErrorOccurred(LogType type);
    }

    public interface IDebugLogListener
    {
        void Log(string logString, LogType type);
    }

    public enum DebugLogType
    {
        Verbose, Info, Warning, Exception, Error
    }

    public static class DebugPanelHelper
    {
        static bool m_errorOccurred;
        public static bool ErrorOccurred
        {
            get { return m_errorOccurred; }
            set { m_errorOccurred = value; }
        }

        static LogType m_errorLogType = LogType.Log;
        public static LogType ErrorLogType
        {
            get
            {
                return m_errorLogType;
            }
            set
            {
                m_errorLogType = value;
            }
        }

        static IDebugInfoListener m_debugListener;
        static IDebugErrorListener m_debugErrorListener;
        static List<IDebugLogListener> m_debugLogListeners = new List<IDebugLogListener>();

        static List<DebugItemObject> m_verboseLogs;
        static List<DebugItemObject> m_infoLogs;
        static List<DebugItemObject> m_warningLogs;
        static List<DebugItemObject> m_errorLogs;
        static List<DebugItemObject> m_exceptionLogs;
        static List<DebugItemObject> m_assertLogs;

        static DebugLogType m_debugLogType = DebugLogType.Verbose;
        static readonly int MAX_LOG_COUNT = 2000;
        static readonly int REMOVED_LOG_COUNT = (int)(MAX_LOG_COUNT * 0.1);
        static readonly int DEFAULT_ERROR_LOG_CAPACITY = 10;
        static readonly int DEFAULT_WARNING_LOG_CAPACITY = 100;
        static readonly int DEFAULT_INFO_LOG_CAPACITY = 200;
        static readonly int DEFAULT_VERBOSE_LOG_CAPACITY = 400;
        static readonly int DEFAULT_EXCEPTION_LOG_CAPACITY = 10;
        public static DebugLogType DebugLogLevel
        {
            get
            {
                return m_debugLogType;
            }
            set
            {
                m_debugLogType = value;
            }
        }

        static DebugPanelHelper()
        {
            m_verboseLogs = new List<DebugItemObject>(DEFAULT_VERBOSE_LOG_CAPACITY);
            m_infoLogs = new List<DebugItemObject>(DEFAULT_INFO_LOG_CAPACITY);
            m_warningLogs = new List<DebugItemObject>(DEFAULT_WARNING_LOG_CAPACITY);
            m_errorLogs = new List<DebugItemObject>(DEFAULT_ERROR_LOG_CAPACITY);
            m_exceptionLogs = new List<DebugItemObject>(DEFAULT_EXCEPTION_LOG_CAPACITY);
            m_assertLogs = new List<DebugItemObject>();
        }

        public static void RegisterDebugInfoListener(IDebugInfoListener debugListener)
        {
            m_debugListener = debugListener;
            LogHelper.LogInfo("DebugPanelHelper", "Regist debug info listener");
        }

        public static void RegisterDebugErrorListener(IDebugErrorListener errorListener)
        {
            m_debugErrorListener = errorListener;
        }

        public static void RegisterDebugLogListener(IDebugLogListener logListener)
        {
            m_debugLogListeners.Add(logListener);
        }

        public static void Log(string log, string stackTrace, LogType type)
        {
            DebugItemObject item = new DebugItemObject(log, type);
            AddLogToList(item, type);
            if (m_debugListener != null) m_debugListener.OnChanged();

            if (IsErrorOccurred(type))
            {
                string temp = string.Format("{0}\nStack Trace:\n{1}", log, stackTrace);
                item.logInfo = temp;
                m_errorOccurred = true;
                m_errorLogType = type;
                if (m_debugErrorListener!=null) m_debugErrorListener.OnErrorOccurred(type);
            }
        }

        private static bool IsErrorOccurred(LogType type)
        {
            if (type == LogType.Exception || type == LogType.Error || type == LogType.Assert)
                return true;
            return false;
        }

        public static List<DebugItemObject> GetNewestDebugLogs()
        {
            switch (m_debugLogType)
            {
                case DebugLogType.Verbose:
                    return m_verboseLogs;
                case DebugLogType.Info:
                    return m_infoLogs;
                case DebugLogType.Warning:
                    return m_warningLogs;
                case DebugLogType.Exception:
                    return m_exceptionLogs;
                case DebugLogType.Error:
                    return m_errorLogs;
                default:
                    break;
            }
            return m_verboseLogs;
        }

        public static void ClearAll()
        {
            m_verboseLogs.Clear();
            m_infoLogs.Clear();
            m_errorLogs.Clear();
            m_exceptionLogs.Clear();
            m_warningLogs.Clear();

            if (m_debugListener != null) m_debugListener.OnClearAll();
        }

        static void AddLog(List<DebugItemObject> list, DebugItemObject item)
        {
            CheckLogListSize(list);
            list.Add(item);
        }

        private static void CheckLogListSize(List<DebugItemObject> list)
        {
            if (list.Count >= MAX_LOG_COUNT)
            {
                list.RemoveRange(0, REMOVED_LOG_COUNT);
            }
            else if(list.Count >= list.Capacity / 2)
            {
                int capacity = list.Count * 4;
                if (capacity > MAX_LOG_COUNT)
                    capacity = MAX_LOG_COUNT;
                list.Capacity = capacity;
            }
        }

        static void AddLogToList(DebugItemObject item, LogType type)
        {
            switch (type)
            {
                case LogType.Error:
                    AddLog(m_errorLogs, item);
                    break;
                case LogType.Assert:
                    AddLog(m_assertLogs, item);
                    break;
                case LogType.Warning:
                    AddLog(m_warningLogs, item);
                    break;
                case LogType.Log:
                    AddLog(m_infoLogs, item);
                    break;
                case LogType.Exception:
                    AddLog(m_exceptionLogs, item);
                    break;
                default:
                    break;
            }
            AddLog(m_verboseLogs, item);
        }

        public static void SetNoError()
        {
            m_errorOccurred = false;
            m_errorLogType = LogType.Log;
        }

    }
}
