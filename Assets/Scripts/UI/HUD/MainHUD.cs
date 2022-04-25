using System;
using System.Collections.Generic;
using System.Linq;
using FromScratch;
using FromScratch.Player;
using UnityEngine;
using UnityEngine.UIElements;
using Util;

namespace UI.HUD
{
    public class MainHUD: MonoSingleton<MainHUD>
    {

        private List<HudElement> hudElements;
        
        public UIDocument uiDoc;
        
        private FromScratchPlayer player;
        public FromScratchPlayer Player {
            get
            {
                if (player == null)
                {
                    player = GameManager.Instance.Player;
                }

                return player;
            }
        }

        private VisualElement root;
        public VisualElement Root => root;

        private void Start()
        {
            player = GameManager.Instance.Player;
            GameManager.Instance.OnPlayerChanged.AddListener(OnPlayerChanged);
            
            root = uiDoc.rootVisualElement;
            hudElements = new List<HudElement>();

            AddHudElement(new CheatConsoleHUD());
            AddHudElement(new PlayerStatusHUD());
            AddHudElement(new InteractionMarkerHUD());
        }
        
        private void OnPlayerChanged(FromScratchPlayer newPlayer)
        {
            player = newPlayer;
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
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void OnRuntimeStartSingleton() {
            IS_EXITING = false;
        }
    }
}