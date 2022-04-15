using AssetReferences;
using FromScratch.Character;
using FromScratch.Interaction;
using FromScratch.Player;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.HUD
{
    public class InteractionMarkerHUD: HudElement
    {
        private const int MaxBorderThickness = 10;

        private Character character;
        private Camera camera;

        private GameObject target;
        
        private const string visualTreeAssetKey = "interaction-marker";
        private VisualElement veIndicator;
        private readonly Label veLabel;

        public InteractionMarkerHUD()
        {
            root = VisualTreeAssetReference.Create(visualTreeAssetKey);
            veIndicator = root.Q("indicator");
            veLabel = root.Q<Label>("label");
        }
        
        public override void Init(MainHUD mainHUD)
        {
            base.Init(mainHUD);
            character = mainHUD.player.character;
            camera = Camera.main;
        }
        
        public override void Update()
        {
            IInteractable interactable = character.characterInteration.GetNearestInteractable();
            if (interactable != null)
            {
                AttachTo(interactable.GetGameObject());
            }
            else
            {
                Detach();
            }

            var charInteraction = character.characterInteration;
            int borderThickness =
                Mathf.CeilToInt((charInteraction.activationDuration / CharacterInteraction.ActivationHoldTime) *
                                MaxBorderThickness);

            veIndicator.style.borderBottomWidth = borderThickness + 1;
            veIndicator.style.borderTopWidth = borderThickness + 1;
            veIndicator.style.borderLeftWidth = borderThickness + 1;
            veIndicator.style.borderRightWidth = borderThickness + 1;

            veIndicator.style.marginBottom = 10 - borderThickness;
            veIndicator.style.marginTop = 10 - borderThickness;
            veIndicator.style.marginLeft = 10 - borderThickness;
            veIndicator.style.marginRight = 10 - borderThickness;

            if (charInteraction.activeLabel != null && charInteraction.activeLabel != veLabel.text)
            {
                veLabel.text = charInteraction.activeLabel;
            }
        }

        void AttachTo(GameObject newTarget)
        {
            if (newTarget == target)
            {
                
            }
            
            var screenPoint = camera.WorldToScreenPoint(newTarget.transform.position);
            root.visible = true;
            float width = root.resolvedStyle.width;
            root.style.position = Position.Absolute;
            root.style.left = new Length(screenPoint.x - width * 0.5f, LengthUnit.Pixel);
            root.style.bottom = new Length(screenPoint.y + 40, LengthUnit.Pixel);
        }

        void Detach()
        {
            root.visible = false;
        }
    }
}