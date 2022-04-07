using System;
using System.Collections;
using System.Collections.Generic;
using AssetReferences;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI
{
    public class MultiScreenMenu : FullscreenMenuHost.FullScreenMenu
    {
        public interface Submenu
        {
            string GetTitle();
            string GetID();
            VisualElement GetRoot();
            void OnOpen();
            void OnClose();

            /// <summary>
            /// Used to disable interactions while transitioning between screens.
            /// A Submenu is only active while it is the main screen AND not transitioning to/from.
            /// </summary>
            /// <param name="state">Whether or not the Submenu is active</param>
            void SetIsActive(bool state);

            void Update();
            bool HasOpenLayer();
            void CloseTopLayer();
        }

        protected List<Submenu> submenus = new List<Submenu>();
        protected Submenu activeSubmenu = null;
        protected Submenu nextSubmenu = null;
        protected int currentSubmenuIndex = 0;

        #region UI References
        protected static VisualTreeAsset treeAsset;
        protected const string treeAssetKey = "multiscreen";
        protected VisualElement veRoot;
        protected Label lblMainTitle;
        protected Label lblPrevTitle;
        protected Label lblNextTitle;
        protected VisualElement veMain;
        protected VisualElement veNext;
        #endregion

        private bool isAnimating = false;
        private const float ScreenTransitionDuration = 0.3f;
    
        public Submenu CurrentSubmenu {
            get {
                return GetSubmenuAtIndex(currentSubmenuIndex);
            }
        }

        public MultiScreenMenu AddSubmenu(Submenu menu) {
            submenus.Add(menu);
            return this;
        }

        public MultiScreenMenu RemoveMenu(Submenu menu) {
            submenus.Remove(menu);
            return this;
        }
        
        #region FullScreenMenu contract

        public VisualElement GetRoot() {
            if (veRoot == null) {
                BuildUI();
            }

            return veRoot;
        }

        public virtual void OpenMenu() {
            currentSubmenuIndex = 0;
            SetActiveSubmenu(CurrentSubmenu);
            
        }
        
        public void OpenTo(string id) {
            int index = submenus.FindIndex(x => x.GetID() == id);
            if(index != -1) {
                currentSubmenuIndex = index;
                SetActiveSubmenu(CurrentSubmenu);
            }
        }


        public virtual void CloseMenu() {

        }

        public void Update()
        {
            CurrentSubmenu.Update();
        }

        public bool HasOpenLayer()
        {
            return CurrentSubmenu.HasOpenLayer();
        }

        public void CloseTopLayer()
        {
            CurrentSubmenu.CloseTopLayer();
        }

        #endregion

        virtual protected void BuildUI() {
            if(treeAsset == null) {
                treeAsset = VisualTreeAssetReference.Instance.GetAsset(treeAssetKey);
            }

            veRoot = treeAsset.CloneTree().Q(null, "root");
            lblMainTitle = veRoot.Q<Label>("title__main");
            lblPrevTitle = veRoot.Q<Label>("title__prev");
            lblNextTitle = veRoot.Q<Label>("title__next");
            veMain = veRoot.Q("main");
            veNext = veRoot.Q("next");
        }

        public virtual void NextMenu(int offset = 1)
        {
            if (isAnimating)
            {
                return;
            }
        
            var nextSubmenuIndex = GetRelativeIndex(offset);
            var nextMenu = GetSubmenuAtIndex(nextSubmenuIndex);

            var currentIndex = currentSubmenuIndex;
            var currentSub = CurrentSubmenu;
        
            //Step 1: Set Next Menu, Reset Position
            SetNextMenu(nextMenu);
            currentSub.SetIsActive(false);
            nextMenu.SetIsActive(false);

            //Step 2: Animate Menus into final position
            isAnimating = true;
            float nextStart = offset > 0 ? 100 : -100;
            float mainEnd = offset > 0 ? -100 : 100;
            FullscreenMenuHost.Instance.StartCoroutine(AnimateLeft(veMain, 0, mainEnd));
            FullscreenMenuHost.Instance.StartCoroutine(AnimateLeft(veNext, nextStart, 0, () =>
            {
                //Clear Next Menu
                veNext.Clear();
                nextMenu.GetRoot().RemoveFromHierarchy();
            
                //Clear Current Menu
                veMain.Clear();
                currentSub.GetRoot().RemoveFromHierarchy();

                //Close "Prev" menu
                currentSub.OnClose();
            
                //Set current Menu
                currentSubmenuIndex = nextSubmenuIndex;
                activeSubmenu = nextMenu;
                CurrentSubmenu.SetIsActive(true);
                veMain.Add(CurrentSubmenu.GetRoot());
                
                //Set Positions
                veMain.style.left = 0;
                veNext.style.left = new Length(100, LengthUnit.Percent);

                UpdateTitle();
                isAnimating = false;
            }));
        }
    
        private void SetNextMenu(Submenu submenu)
        {
            nextSubmenu = submenu;
            veNext.Add(nextSubmenu.GetRoot());

            nextSubmenu.OnOpen();
            //Keep inactive while animating
            nextSubmenu.SetIsActive(false);
        }

        private IEnumerator AnimateLeft(VisualElement element, float from, float to, Action callback = null)
        {
            //TODO: Make Non-Linear
            
            float changePerMS = (to - from) / (ScreenTransitionDuration * 1000f);

            float value = from;
            float lastUpdateTime = Time.realtimeSinceStartup;
            bool Predicate(float val) => @from > to ? val > to : val < to;

            while (Predicate(value))
            {
                yield return null;
                float now = Time.realtimeSinceStartup;

                float deltaTime = (now - lastUpdateTime) * 1000;
                lastUpdateTime = now;

                float delta = deltaTime * changePerMS;
                Debug.LogFormat("Delta {0}, {1}", delta.ToString(), deltaTime.ToString());
                
                value += delta;
                element.style.left = new Length(value, LengthUnit.Percent);
            }
        
            callback?.Invoke();
        } 
    
        private void SetActiveSubmenu(Submenu submenu) {
            if(activeSubmenu != null) {
                //DEACTIVATE
                activeSubmenu.OnClose();
                activeSubmenu.GetRoot().RemoveFromHierarchy();
            }

            // m_previousSubmenu = activeSubmenu;
            activeSubmenu = submenu;
            activeSubmenu.SetIsActive(true);
            veMain.Add(activeSubmenu.GetRoot());
            activeSubmenu.OnOpen();
            UpdateTitle();
        }

        private void UpdateTitle() {
            string title = CurrentSubmenu?.GetTitle();
            string prevTitle = GetSubmenuAtIndex(currentSubmenuIndex - 1)?.GetTitle();
            string nextTitle = GetSubmenuAtIndex(currentSubmenuIndex + 1)?.GetTitle();

            lblMainTitle.text = title;
            lblNextTitle.text = nextTitle;
            lblPrevTitle.text = prevTitle;
        }

        private Submenu GetSubmenuAtIndex(int index) {
            //Normalise index
            index = GetRelativeIndex(index, 0);

            if (submenus == null || index >= submenus.Count) {
                return null;
            } else {
                return submenus[index];
            }
        }

        private int GetRelativeIndex(int offset) {
            return GetRelativeIndex(currentSubmenuIndex, offset);
        }

        private int GetRelativeIndex(int current, int offset) {
            //Edge case, avoid divide by zero
            int count = submenus.Count;
            if (count == 0) { return 0; }
            int basic = current + offset;

            while (basic < 0) {
                basic += count;
            }

            return basic % count;
        }
    }
}
