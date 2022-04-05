using UnityEngine;
using UnityEngine.UIElements;
using Util;

namespace UI
{
    public class FullscreenMenuHost : MonoSingleton<FullscreenMenuHost>
    {
        public interface FullScreenMenu
        {
            VisualElement GetRoot();
            void OpenMenu();
            void CloseMenu();
        }

        private FullScreenMenu m_menu; 
        public UIDocument m_uiDoc;
        public GameObject blurOject;
        public UIDocument hudDocument;

        private const int SORT_ORDER_OPEN = 10;
        private const int SORT_ORDER_CLOSED = -1;

        public void Start() { 
            if(m_uiDoc == null) {
                m_uiDoc = GetComponent<UIDocument>();
            }
        
            m_uiDoc.sortingOrder = SORT_ORDER_CLOSED;
        }

        public void OpenMenu(FullScreenMenu menu) {
            if(m_menu != null) {
                m_menu.CloseMenu();
                m_menu.GetRoot().RemoveFromHierarchy();                
            }

            m_menu = menu;
            m_uiDoc.rootVisualElement.Add(m_menu.GetRoot());
            m_menu.OpenMenu();
            m_uiDoc.sortingOrder = SORT_ORDER_OPEN;
            if (blurOject != null)
            {
                blurOject.SetActive(true);
            }
            hudDocument.rootVisualElement.visible = false;
        }

        public void CloseCurrentMenu() {
            if(m_menu != null) {
                m_menu.GetRoot().RemoveFromHierarchy();
                m_menu.CloseMenu();
            }
            m_menu = null;
            m_uiDoc.sortingOrder = SORT_ORDER_CLOSED;
            if (blurOject != null)
            {
                blurOject.SetActive(false);
            }
            hudDocument.rootVisualElement.visible = true;
        }

        public bool HasMenuOpen {
            get {
                return m_menu != null;
            }
        }
    }
}