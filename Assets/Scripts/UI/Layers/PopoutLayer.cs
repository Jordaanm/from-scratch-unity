using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;

namespace UI.Layers
{
    public enum PopoutAnchorPosition
    {
        AlignLeftEdges,
        RightOfParent,
        LeftOfParent
    }
    public class PopoutLayer : GenericUiLayer
    {
        protected static string vtaKey = "lt-popout";

        bool animateOpen;
        bool matchWidth;
        VisualElement anchor;
        bool isMounted = false;
        ValueAnimation<float> openingAnim;
        private PopoutAnchorPosition anchorPosition = PopoutAnchorPosition.AlignLeftEdges;

        public PopoutLayer(VisualElement anchor, PopoutAnchorPosition popoutAnchorPosition, bool animateOpen = true, bool matchWidth = true) {
            this.anchor = anchor;
            this.anchorPosition = popoutAnchorPosition;
            this.animateOpen = animateOpen;
            this.matchWidth = matchWidth;
            m_shouldCloseIfClickOutside = true;
        }

        #region IEditorWindowLayer contract
        public override void OnClose() {
            base.OnClose();
            if(openingAnim != null && openingAnim.isRunning) {
                openingAnim.Stop();
            }
        }

        public override void OnOpen() {
            base.OnOpen();
            Debug.Log("OnOpen");
            if(animateOpen) {
                PlayOpenAnim();
            }
        }

        public override VisualElement BuildContents() {
            var ve = base.BuildContents();
            ve.RegisterCallback<AttachToPanelEvent>(OnMount);
            return ve;
        }

        protected void OnMount(AttachToPanelEvent evt) {
            isMounted = true;
            SetPosition();
        }

        protected virtual void SetPosition()
        {
            bool hasHost = m_layerManager?.HostElement != null;


            float x = 0;
            if (hasHost)
            {
                float leftOffset = anchor.worldBound.x - anchor.resolvedStyle.borderLeftWidth -
                                   anchor.resolvedStyle.paddingLeft - m_layerManager.HostElement.worldBound.x;
                Debug.LogFormat("Popout Position {2}: {0}, {1}", leftOffset, anchor.resolvedStyle.width, anchorPosition.ToString());

                switch (anchorPosition)
                {
                    case PopoutAnchorPosition.AlignLeftEdges:
                        x = leftOffset;
                        break;
                    case PopoutAnchorPosition.LeftOfParent:
                        x = leftOffset - Root.resolvedStyle.width;
                        break;
                    case PopoutAnchorPosition.RightOfParent:
                        x = leftOffset + anchor.resolvedStyle.width;
                        break;
                }
            }

            float y = hasHost ? anchor.worldBound.y
                                - anchor.resolvedStyle.borderTopWidth
                                - anchor.resolvedStyle.paddingTop
                                - m_layerManager.HostElement.worldBound.y : 0; 

            x = Mathf.Max(x, 0);
            y = Mathf.Max(y, 0);

            m_root.style.left = new StyleLength(x);
            m_root.style.top = new StyleLength(y);

            if(matchWidth) {
                m_root.style.width = anchor.resolvedStyle.width;
            }

        }

        #endregion

        float ZeroIfNan(float value) {
            if(float.IsNaN(value)) { return 0; }
            return value;
        }

        void PlayOpenAnim() {
            if(!isMounted || !m_isOpen) { return; }
            float currentHeight = ZeroIfNan(m_root.worldBound.height);
            float anchorHeight = ZeroIfNan(anchor.resolvedStyle.height);
            float p = (currentHeight - anchorHeight) / 100;

            var overflow = m_root.style.overflow.value;
            var minHeight = m_root.resolvedStyle.minHeight.value;

            if(anchorHeight != currentHeight) {
                m_root.style.overflow = Overflow.Hidden;
                m_root.style.minHeight = 0;
                openingAnim = m_root.experimental.animation.Start(
                    0, 
                    100, 
                    500, (e, v) => {
                        Debug.LogFormat("H {0}", v);
                        m_root.style.height = anchorHeight + p * v;
                }).Ease(Easing.Linear);

                openingAnim.onAnimationCompleted += () => {
                    Debug.Log("Animation Completed");
                    m_root.style.height = new StyleLength(StyleKeyword.Auto);
                    m_root.style.overflow = overflow;
                    m_root.style.minHeight = minHeight;
                };

                openingAnim.Start();

            }

        }

        protected override string AssetTreeKey {
            get { return vtaKey; }
        }
    }

}