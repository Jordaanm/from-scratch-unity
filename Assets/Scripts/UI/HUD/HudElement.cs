using AssetReferences;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.HUD
{
    public abstract class HudElement
    {
        protected VisualElement root;
        protected MainHUD hud;

        public VisualElement Root => root;

        public virtual void Init(MainHUD mainHUD)
        {
            hud = mainHUD;
            SetPosition();
            Show();
        }

        public virtual void SetPosition()
        {
            root.style.position = Position.Absolute;
        }

        public void Hide()
        {
            root.visible = false;
        }

        public void Show()
        {
            root.visible = true;
        }
    }
}