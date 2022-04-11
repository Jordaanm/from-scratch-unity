﻿using AssetReferences;
using Cheats;
using FromScratch.Player;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace UI.HUD
{
    public class CheatConsoleHUD : HudElement
    {
        private string visualTreeAssetKey = "console";
        private CheatConsole cheatConsole;
        private TextField input;

        public CheatConsoleHUD()
        {
            root = VisualTreeAssetReference.Create(visualTreeAssetKey);
            cheatConsole = new CheatConsole();
            //Find input element reference;
            input = root.Q<TextField>("input");
            input.RegisterCallback<KeyDownEvent>(OnKeyDown);
        }

        private void OnKeyDown(KeyDownEvent evt)
        {
            if (evt.keyCode == KeyCode.Return)
            {
                OnSubmit();
            }
        }

        private void OnSubmit()
        {
            //Get Text
            string text = input.text;
            cheatConsole.Execute(text, MainHUD.Instance.player);
        }

        public override void Init(MainHUD mainHUD)
        {
            base.Init(mainHUD);
            Close();
        }

        public override void SetPosition()
        {
            root.style.position = Position.Absolute;
            root.style.left = 0;
            root.style.bottom = 0;
        }

        public void Open()
        {
            //Clear input field
            input.value = "";
            
            Show();

            //Focus input
            input?.Focus();
        }

        public void Close()
        {
            Hide();
        }

        public void Toggle()
        {
            if (root.visible)
            {
                Close();
            }
            else
            {
                Open();
            }
        }
    }
}