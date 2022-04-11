using AssetReferences;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.HUD
{
    public abstract class HudElement
    {
        protected VisualElement root;
        protected MainHUD hud;
        protected bool isActive;

        public VisualElement Root => root;

        public bool IsActive => isActive;

        public virtual void Init(MainHUD mainHUD)
        {
            hud = mainHUD;
            SetPosition();
            Show();
        }

        public virtual void Update()
        {
            
        }

        public virtual void SetPosition()
        {
            root.style.position = Position.Absolute;
        }

        public void Hide()
        {
            root.visible = false;
            isActive = false;
        }

        public void Show()
        {
            root.visible = true;
            isActive = true;
        }
    }
}