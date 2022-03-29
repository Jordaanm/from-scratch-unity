﻿using System;
using System.Collections.Generic;
using System.Linq;
using AssetReferences;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Layers
{
    public struct ContextMenuAction
    {
        public string id;
        public string label;
        public Action action;

        public ContextMenuAction(string id, string label, Action action)
        {
            this.id = id;
            this.label = label;
            this.action = action;
        }
    }
    
    public class ContextMenuLayer: PopoutLayer
    {
        private List<ContextMenuAction> menuActions;
        private const string menuActionTreeAssetKey = "content-menu-action";
        private VisualTreeAsset menuActionTreeAsset;
        
        public ContextMenuLayer(VisualElement anchor, bool animateOpen = false, bool matchWidth = false) : base(anchor, animateOpen, matchWidth)
        {
            menuActions = new List<ContextMenuAction>();
            menuActionTreeAsset = VisualTreeAssetReference.Instance.GetAsset(menuActionTreeAssetKey);
        }

        protected override VisualElement BuildLayerContents()
        {
            var ve = new VisualElement();
            RenderActions(ve);
            return ve;
        }

        public void AddAction(ContextMenuAction action)
        {
            menuActions.Add(action);
            RenderActions();
        }

        public void AddActions(IEnumerable<ContextMenuAction> actions)
        {
            menuActions.AddRange(actions);
            RenderActions();
        }

        public void RemoveAction(string id)
        {
            ContextMenuAction action = menuActions.Find(a => a.id == id);
            menuActions.Remove(action);
            RenderActions();
        }

        private void RenderActions(VisualElement host = null)
        {
            VisualElement contents = host != null ? host : Root?.Q(ContentSelector)?.Children().First();
            if (contents != null)
            {
                contents.Clear();
                foreach (var contextMenuAction in menuActions)
                {
                    VisualElement veAction = RenderAction(contextMenuAction);
                    contents.Add(veAction);
                }
            }
        }

        private VisualElement RenderAction(ContextMenuAction action)
        {
            VisualElement ve = VisualTreeAssetReference.Create(menuActionTreeAsset);
            ve.Q<Label>("label").text = action.label;
            ve.userData = action.id;
            ve.RegisterCallback<MouseDownEvent>(OnClickAction);
            return ve;
        }

        private void OnClickAction(MouseDownEvent evt)
        {
            VisualElement target = (VisualElement)evt.currentTarget;
            string id = (string)target.userData;
            ContextMenuAction action = menuActions.Find(x => x.id == id);
            if(action.action != null) { action.action.Invoke(); }
            Close();
        }
    }
}