using UnityEngine;
namespace Stark
{
    public class DebugPanelManager : MonoBehaviour
    {
        private const string DROPDOWN_LOG_LEVEL = "Container/LogListPanel/TitleBar/Dropdown";
        private const string LOG_LIST_PANEL = "Container/LogListPanel";
        
        void Start()
        {
            AddNecessaryScripts();
        }

        private void AddNecessaryScripts()
        {
            GameObjectUtils.AddComponent<DebugListViewManager>(gameObject, LOG_LIST_PANEL);
            GameObjectUtils.AddComponent<DropdownLogLevelManager>(gameObject, DROPDOWN_LOG_LEVEL);
        }
    }
}