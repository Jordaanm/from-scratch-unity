using System;
using System.Collections.Generic;
using System.Linq;
using FromScratch.Player;
using UnityEngine.UIElements;
using Util;

namespace UI.HUD
{
    public class MainHUD: MonoSingleton<MainHUD>
    {

        private List<HudElement> hudElements;
        
        public UIDocument uiDoc;
        public FromScratchPlayer player;

        private VisualElement root;
        public VisualElement Root => root;

        private void Start()
        {
            root = uiDoc.rootVisualElement;
            hudElements = new List<HudElement>();

            AddHudElement(new CheatConsoleHUD());
            AddHudElement(new PlayerStatusHUD());
        }

        private void AddHudElement(HudElement hudElement)
        {
            hudElements.Add(hudElement);
            var elementRoot = hudElement.Root;
            root.Add(elementRoot);
            hudElement.Init(this);
        }

        private void Update()
        {
            foreach (var hudElement in hudElements)
            {
                if(hudElement.IsActive) { hudElement.Update(); }
            }
        }

        public T GetHudElement<T>() where T : HudElement
        {
            return hudElements.First(e => e.GetType() == typeof(T)) as T;
        }
    }
}