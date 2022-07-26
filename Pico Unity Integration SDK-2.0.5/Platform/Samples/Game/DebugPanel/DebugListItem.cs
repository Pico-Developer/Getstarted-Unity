using System;
using Pico.Platform.Samples.Game;
using UnityEngine;
using UnityEngine.UI;

namespace Stark
{
    public interface IDebugListItemClickListener
    {
        void OnClick(Text text);
    }

    public class DebugListItem : MonoBehaviour
    {
        public static readonly Color infoColor = new Color(0f, 1f, 0f, 1f);
        public static readonly Color warningColor = new Color(1f, 1f, 0f, 1f);
        public static readonly Color errorColor = new Color(1f, 0f, 0f, 1f);
        public static readonly Color exceptionColor = new Color(1f, 0f, 1f, 1f);
        public static readonly Color assertColor = new Color(1f, 1f, 1f, 1f);
        private bool clickListenerHasRegisted = false;
        private Text message;
        private Button button;

        void Awake()
        {
            button = GetComponent<Button>();
            if (button == null) LogHelper.LogWarning("DebugListItem", "GetComponent<Button>() return null");
            Transform trans = GameObjectUtils.FindTransform(gameObject, "Text");
            if (trans != null)
                message = trans.GetComponent<Text>();
        }

        public void Initialize(DebugItemObject item)
        {
            if (message == null)
                return;
            Color txtColor = GetProperTextColor(item.logType);
            if(item.logType == LogType.Log)
                message.text = "[Info] " + item.logInfo;
            else
                message.text = "[" + item.logType.ToString() + "] " + item.logInfo;

            message.alignment = TextAnchor.MiddleLeft;
            message.color = txtColor;
        }

        public void SetButtonOnClickListener(IDebugListItemClickListener debugItemClickListener)
        {
            if (clickListenerHasRegisted) return;
            if (button!=null)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(delegate { debugItemClickListener.OnClick(message); });
                clickListenerHasRegisted = true;
            }
        }

        Color GetProperTextColor(LogType type)
        {
            Color color = infoColor;
            if (type == LogType.Log) color = infoColor;
            else if (type == LogType.Warning) color = warningColor;
            else if (type == LogType.Error) color = errorColor;
            else if (type == LogType.Assert) color = assertColor;
            else if (type == LogType.Exception) color = exceptionColor;
            return color;
        }
    }
}