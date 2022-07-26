using UnityEngine;
using UnityEngine.UI;
namespace Stark
{
    public class DebugItemTextDialogManager : MonoBehaviour
    {
        private Text m_text;
        private Scrollbar scrollbar;
        private const string BUTTON_CLOSE = "Title/Close";
        private const string TEXT_PATH = "Message/Text";
        private const string SCROLLBAR_PATH = "Message/Scrollbar";
        void Awake()
        {
            GameObjectUtils.AddButtonClick(gameObject, BUTTON_CLOSE, OnCloseButtonClicked);
            m_text = GameObjectUtils.GetComponent<Text>(transform, TEXT_PATH);
            scrollbar = GameObjectUtils.GetComponent<Scrollbar>(transform, SCROLLBAR_PATH);
        }

        void OnEnable()
        {
            if (scrollbar) scrollbar.value = 1f;
        }

        void OnCloseButtonClicked()
        {
            gameObject.SetActive(false);
        }

        public void Initialize(Text text)
        {
            if (m_text != null)
            {
                m_text.text = text.text;
                m_text.fontSize = 30;
                m_text.color = text.color;
            }
        }
    }
}