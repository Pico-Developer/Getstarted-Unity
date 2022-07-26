using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Stark
{
    public class DropdownLogLevelManager : MonoBehaviour
    {
        Text m_label;
        Dropdown m_dropdown;
        Color m_defaultColor = new Color(1f, 1f, 1f, 1f);

        Dictionary<int, Color> m_colorMap;
        Dictionary<int, DebugLogType> m_logLevelMap;
        string[] m_logLevels = { "Verbose", "Info", "Warning", "Exception", "Error"};
        void Awake()
        {
            m_colorMap = new Dictionary<int, Color>();
            m_logLevelMap = new Dictionary<int, DebugLogType>();
            m_colorMap.Add(0, m_defaultColor);
            m_colorMap.Add(1, DebugListItem.infoColor);
            m_colorMap.Add(2, DebugListItem.warningColor);
            m_colorMap.Add(3, DebugListItem.exceptionColor);
            m_colorMap.Add(4, DebugListItem.errorColor);

            m_logLevelMap.Add(0, DebugLogType.Verbose);
            m_logLevelMap.Add(1, DebugLogType.Info);
            m_logLevelMap.Add(2, DebugLogType.Warning);
            m_logLevelMap.Add(3, DebugLogType.Exception);
            m_logLevelMap.Add(4, DebugLogType.Error);
        }

        void Start()
        {
            m_label = transform.Find("Label").GetComponent<Text>();
            m_label.text = m_logLevels[0];
            m_label.color = m_defaultColor;
            m_dropdown = GetComponent<Dropdown>();
            foreach (var level in m_logLevels){
                m_dropdown.options.Add(new Dropdown.OptionData(level));
            }
            m_dropdown.onValueChanged.AddListener(delegate { OnSelect(); });
        }

        public void OnSelect()
        {
            if (m_colorMap.ContainsKey(m_dropdown.value))
                m_label.color = m_colorMap[m_dropdown.value];
            else
                m_label.color = m_defaultColor;

            if (m_logLevelMap.ContainsKey(m_dropdown.value))
                DebugPanelHelper.DebugLogLevel = m_logLevelMap[m_dropdown.value];
            else
                DebugPanelHelper.DebugLogLevel = DebugLogType.Verbose;
        }
    }
}