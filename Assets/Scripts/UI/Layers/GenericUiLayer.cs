using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AssetReferences;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Layers
{
    public class GenericUiLayer : IUserInterfaceLayer
    {
        protected static string baseVtaKey = "lt-generic";
        protected const string ContentSelector = "layer-item__content";
        protected VisualTreeAsset visualTreeAsset;
        

        protected VisualElement m_root;

        protected LayerManager m_layerManager;
        protected bool m_shouldCloseIfClickOutside = false;
        protected bool m_isOpen = false;

        #region IUserInterfaceLayer contract

        public VisualElement Root {
            get { return m_root; }
        }

        public virtual VisualElement BuildContents() {
            VisualElement root = CloneTemplate();
            m_root = root;
            root.pickingMode = PickingMode.Position;

            VisualElement dialogContents = BuildLayerContents();
            if (dialogContents != null) {
                root.Q(ContentSelector).Add(dialogContents);
            }

            BindBehaviours(root);

            string dialogTitle = GetTitle();
            var lblTitle = root.Q<Label>("layer-item__title");
            if(lblTitle != null) {
                lblTitle.text = dialogTitle;
            }

            var actions = GetInitialActions();
            if (actions != null) {
                foreach (string key in actions.Keys) {
                    AddAction(key, actions[key]);
                }
            }

            return root;
        }


        public virtual void OnClose() {
            m_isOpen = false;
            m_layerManager.HostElement.UnregisterCallback<MouseDownEvent>(OnClickOutside);
        }

        public virtual async void OnOpen() {
            m_isOpen = true;
            await Task.Delay(Mathf.CeilToInt(0.5f * 1000f));
            m_layerManager.HostElement.RegisterCallback<MouseDownEvent>(OnClickOutside);
        }

        public void SetLayerManager(LayerManager layerManager) {
            this.m_layerManager = layerManager;
        }

        #endregion

        #region Content References

        protected virtual string AssetTreeKey {
            get { return baseVtaKey; }
        }
        protected virtual string GetTitle() {
            return "";
        }

        protected virtual VisualElement BuildLayerContents() {
            return null;
        }
        protected virtual Dictionary<string, System.Action> GetInitialActions() {
            return new Dictionary<string, System.Action>();
        }

        protected virtual void BindBehaviours(VisualElement root) {
            root.Q<Button>("layer-item__title__close")?.RegisterCallback<ClickEvent>(evt => {
                Close();
                evt.StopPropagation();
            });

            root.RegisterCallback<MouseDownEvent>(OnMouseDown);
        }

        #endregion

        public virtual void SetIsModal(bool isModal) {
            if (isModal) {
                m_root.parent.pickingMode = PickingMode.Position;
                m_root.parent.AddToClassList("scrim");
                m_root.parent.RegisterCallback<MouseDownEvent>(OnClickOutside);
            } else {
                m_root.parent.pickingMode = PickingMode.Position;
                m_root.parent.RemoveFromClassList("scrim");
                m_root.parent.UnregisterCallback<MouseDownEvent>(OnClickOutside);
            }
        }

        public virtual void OnClickOutside(MouseDownEvent evt) {
            Debug.Log(string.Format("OnClickOutside {0}", this.GetType()));
            if (m_shouldCloseIfClickOutside) {
                Close();
            }
        }

        protected virtual void OnMouseDown(MouseDownEvent evt) {
            evt.StopPropagation();
        }

        protected void HideCloseButton() {
            var closeButton = m_root.Q<Button>("layer-item__title__close");
            if(closeButton != null) {
                closeButton.visible = false;
            }
        }

        protected void AddAction(string text, System.Action onClick) {
            Button button = new Button(onClick);
            button.text = text;

            m_root.Q("layer-item__actions").Add(button);
        }

        protected VisualElement CloneTemplate() {
            if (visualTreeAsset == null) {
                visualTreeAsset = VisualTreeAssetReference.Instance.GetAsset(AssetTreeKey);
            }

            VisualElement clone = visualTreeAsset.CloneTree();
            return clone.Children().First();
        }

        protected virtual void Close() {
            m_layerManager?.RemoveLayer(this);
        }
    }
}