using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Stark;

namespace Stark
{
    public class DebugListViewManager : MonoBehaviour, IDebugInfoListener, IDebugListItemClickListener
    {
        enum ScrollType
        {
            TOP, BOTTOM, NONE
        }

        public const int MAX_SHOW_LOG_COUNT = 15; 
        public const int MIN_SCROLL_DIST_Y_THRESHOLD = 50;
        private const string BUTTON_CLEAR_ALL = "TitleBar/ClearAll";
        private const string BUTTON_FIRST_PAGE = "TitleBar/FirstPage";
        private const string BUTTON_LAST_PAGE = "TitleBar/LastPage";
        private const string LOG_LIST_ROOT = "LogList";
        private const string LOG_LIST_PANEL = "LogList/Viewport/Content";
        private const string LOG_LIST_ITEM = "LogList/Viewport/ListItemTemplate";
        private const string LOG_LIST_VERTICAL_SCROLLBAR = "LogList/VerticalScrollbar";
        private const string DEBUG_INFO_DIALOG = "DebugInfoDialog";
        private Transform listPanel;
        private GameObject rowItemPrefab;
        private RectTransform listPanelRectTransform;
        private DebugItemTextDialogManager debugItemTextDlgMgr;
        private Scrollbar m_verticalScrollbar;
        private UITouchEvent m_uiTouchEvent;
        private DebugLogType m_showingLogLevel = DebugPanelHelper.DebugLogLevel;
        private bool m_logIsChanged;
        private int m_childCount = 0;
        private int m_logEndPosition = -1;
        private float m_listPanelStartY = 0;

        void Awake()
        {
            DebugPanelHelper.RegisterDebugInfoListener(this);
            InitializeVariables();
            AddListTouchEvent();
            AddButtonsClickedListener();
        }

        void Start()
        {
            UpdateDebugUI();
            Canvas.ForceUpdateCanvases();
            ScrollToBottom();
            Canvas.ForceUpdateCanvases();
        }

        void Update()
        {
            if (m_logIsChanged || m_showingLogLevel != DebugPanelHelper.DebugLogLevel)
            {
                float tmpValue = m_verticalScrollbar.value;
                UpdateDebugUI();
                if (tmpValue < 0.15f)
                    ScrollToBottom();
                m_logIsChanged = false;
            }
        }

        void OnEnable()
        {
            ScrollToBottom();
        }

        void InitializeVariables()
        {
            m_uiTouchEvent = GameObjectUtils.GetComponent<UITouchEvent>(GameObjectUtils.FindTransform(gameObject, LOG_LIST_ROOT), true);
            listPanel = GameObjectUtils.FindTransform(gameObject, LOG_LIST_PANEL);
            listPanelRectTransform = GameObjectUtils.GetComponent<RectTransform>(listPanel, null);
            Transform listItemTransfrom = GameObjectUtils.FindTransform(gameObject, LOG_LIST_ITEM);
            if (listItemTransfrom != null)
            {
                rowItemPrefab = listItemTransfrom.gameObject;
                rowItemPrefab.SetActive(false);
            }
            Transform debugInfoDlgTransform = GameObjectUtils.FindTransform(gameObject, DEBUG_INFO_DIALOG);
            if (debugInfoDlgTransform != null)
                debugInfoDlgTransform.gameObject.SetActive(false);
            debugItemTextDlgMgr = GameObjectUtils.GetComponent<DebugItemTextDialogManager>(debugInfoDlgTransform, true);
            m_verticalScrollbar = GameObjectUtils.GetComponent<Scrollbar>(transform, LOG_LIST_VERTICAL_SCROLLBAR);
        }

        private void AddButtonsClickedListener()
        {
            GameObjectUtils.AddButtonClick(gameObject, BUTTON_CLEAR_ALL, OnClearAllClicked);
            GameObjectUtils.AddButtonClick(gameObject, BUTTON_FIRST_PAGE, OnFirstPageBtnClicked);
            GameObjectUtils.AddButtonClick(gameObject, BUTTON_LAST_PAGE, OnLastPageBtnClicked);
        }

        private void OnFirstPageBtnClicked()
        {
            if (m_logEndPosition != MAX_SHOW_LOG_COUNT)
            {
                m_logEndPosition = MAX_SHOW_LOG_COUNT;
                m_logIsChanged = true;
            }
        }

        private void OnLastPageBtnClicked()
        {
            if (m_logEndPosition != -1)
            {
                m_logEndPosition = -1;
                m_logIsChanged = true;
            }
        }

        private void OnClearAllClicked()
        {
            DebugPanelHelper.ClearAll();
        }

        private void AddListTouchEvent()
        {
            if (m_uiTouchEvent == null) return;
            m_uiTouchEvent.onBeginDrag = OnBeginDrag;
            m_uiTouchEvent.onEndDrag = OnEndDrag;
        }

        private void OnBeginDrag(PointerEventData eventData)
        {
            m_listPanelStartY = listPanelRectTransform.localPosition.y;
        }

        private void OnEndDrag(PointerEventData eventData)
        {
            float startY = m_listPanelStartY;
            float endY = listPanelRectTransform.localPosition.y;
            ScrollType type = CheckVericalScroll(startY, endY);
            RefresLogStartPosition(type);
        }

        private ScrollType CheckVericalScroll(float startY, float endY)
        {
            if (Math.Abs(endY - startY) >= MIN_SCROLL_DIST_Y_THRESHOLD)
            {
                if (endY < startY) // scroll to up
                {
                    return ScrollType.TOP;
                }
                else // scroll to bottom
                {
                    return ScrollType.BOTTOM;
                }
            }
            return ScrollType.NONE;
        }

        private void RefresLogStartPosition(ScrollType type)
        {
            if (type == ScrollType.TOP)
            {
                if (m_logEndPosition - MAX_SHOW_LOG_COUNT > 0)
                {
                    m_logEndPosition -= MAX_SHOW_LOG_COUNT;
                    m_logIsChanged = true;
                }
            }
            else if (type == ScrollType.BOTTOM)
            {
                if (m_logEndPosition < 0) return; // �Ѿ����ײ�
                int logCount = DebugPanelHelper.GetNewestDebugLogs().Count;
                if (m_logEndPosition + MAX_SHOW_LOG_COUNT >= logCount)
                {
                    m_logEndPosition = -1;
                }
                else
                {
                    m_logEndPosition += MAX_SHOW_LOG_COUNT;
                }
                m_logIsChanged = true;
            }
        }

        void UpdateDebugUI()
        {
            if (m_showingLogLevel != DebugPanelHelper.DebugLogLevel)
                m_logEndPosition = -1;

            List<DebugItemObject> logs = GetFragmentLogs();
            OnClearAll();
            int index = 0;
            foreach (var item in logs)
            {
                AddItem(item, index);
                ++index;
            }

            m_showingLogLevel = DebugPanelHelper.DebugLogLevel;
        }

        List<DebugItemObject> logs = new List<DebugItemObject>();
        private List<DebugItemObject> GetFragmentLogs()
        {
            List<DebugItemObject> newestLogs = DebugPanelHelper.GetNewestDebugLogs();
            int totalCount = newestLogs.Count;
            logs.Clear();
            if(totalCount == 0)
            {
                m_logEndPosition = -1;
                return logs;
            }
            if (m_logEndPosition < 0)
                m_logEndPosition = totalCount;
            else if (m_logEndPosition > newestLogs.Count)
                m_logEndPosition = newestLogs.Count;
            int startPosition = Math.Max(m_logEndPosition - MAX_SHOW_LOG_COUNT, 0);
            for (int i = startPosition; i < m_logEndPosition; i++)
            {
                logs.Add(newestLogs[i]);
            }
            return logs;
        }

        void ScrollToBottom()
        {
            if (m_verticalScrollbar)
                m_verticalScrollbar.value = 0f;
        }

        DebugListItem infoItem;
        public void AddItem(DebugItemObject item, int index)
        {
            if (index < listPanel.childCount)
            {
                Transform trans = listPanel.GetChild(index);
                infoItem = trans.GetComponent<DebugListItem>();
                if (infoItem == null)
                    infoItem = trans.gameObject.AddComponent<DebugListItem>();
                infoItem.Initialize(item);
                trans.gameObject.SetActive(true);
            }
            else
            {
                GameObject row = Instantiate(rowItemPrefab, Vector3.zero, Quaternion.identity) as GameObject;
                row.SetActive(true);
                row.transform.SetParent(listPanel, false);
                infoItem = row.GetComponent<DebugListItem>();
                if (infoItem == null)
                    infoItem = row.gameObject.AddComponent<DebugListItem>();
                infoItem.Initialize(item);
                infoItem.SetButtonOnClickListener(this);
            }
            ++m_childCount;
        }

        int GetChildCount()
        {
            if (listPanel == null) return 0;
            int count = 0;
            foreach (Transform child in listPanel)
            {
                if (child.gameObject.activeSelf) ++count;
                else break;
            }
            return count;
        }

        public void OnClearAll()
        {
            if (listPanel == null || listPanel.childCount <= 0) return;
            foreach (Transform child in listPanel)
            {
                child.gameObject.SetActive(false);
            }
            m_childCount = 0;
        }

        public void OnClick(Text text)
        {
            if (debugItemTextDlgMgr != null)
            {
                debugItemTextDlgMgr.gameObject.SetActive(true);
                debugItemTextDlgMgr.Initialize(text);
            }
        }

        public void OnChanged()
        {
            m_logIsChanged = true;
            m_logEndPosition = -1;
        }
    }
}