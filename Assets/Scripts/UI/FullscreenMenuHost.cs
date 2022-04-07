using System;
using UnityEngine;
using UnityEngine.Serialization;
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

            void Update();

            bool HasOpenLayer();
            void CloseTopLayer();
        }

        private FullScreenMenu currentMenu; 
        public UIDocument uiDoc;
        public GameObject blurOject;
        public UIDocument hudDocument;

        private const int SortOrderOpen = 10;
        private const int SortOrderClosed = -1;

        public void Start() { 
            if(uiDoc == null) {
                uiDoc = GetComponent<UIDocument>();
            }
        
            uiDoc.sortingOrder = SortOrderClosed;
        }

        private void Update()
        {
            if (currentMenu != null)
            {
                currentMenu.Update();
            }
        }

        public void OpenMenu(FullScreenMenu menu) {
            if(currentMenu != null) {
                currentMenu.CloseMenu();
                currentMenu.GetRoot().RemoveFromHierarchy();                
            }

            currentMenu = menu;
            uiDoc.rootVisualElement.Add(currentMenu.GetRoot());
            currentMenu.OpenMenu();
            uiDoc.sortingOrder = SortOrderOpen;
            if (blurOject != null)
            {
                blurOject.SetActive(true);
            }
            hudDocument.rootVisualElement.visible = false;
        }

        public void CloseTopLayer()
        {
            if (currentMenu.HasOpenLayer())
            {
                currentMenu.CloseTopLayer();
            }
            else
            {
                CloseCurrentMenu();
            }
        }

        public void CloseCurrentMenu() {
            if(currentMenu != null) {
                currentMenu.GetRoot().RemoveFromHierarchy();
                currentMenu.CloseMenu();
            }
            currentMenu = null;
            uiDoc.sortingOrder = SortOrderClosed;
            if (blurOject != null)
            {
                blurOject.SetActive(false);
            }
            hudDocument.rootVisualElement.visible = true;
        }

        public bool HasMenuOpen => currentMenu != null;
    }
}