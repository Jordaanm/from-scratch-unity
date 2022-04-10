using FromScratch.Interaction;
using FromScratch.Player;
using UI;
using UI.Crafting;
using UnityEditor;
using UnityEngine;

namespace FromScratch.Crafting
{
    [Interaction(id="craftingtable")]
    public class CraftingTableInteraction: Interaction.Interaction
    {
        public CraftingTableInteraction()
        {
            _id = "CRAFTING_TABLE";
            _label = "Craft at Station";
        }

        public override bool CanInteractWith(IInteractable interactable)
        {
            return interactable?.GetInteractionType() == InteractionType.CraftingStation;
        }
        
        public override void Start(IInteractor interactor, IInteractable target)
        {
            var player = interactor.GetGameObject().GetComponentInParent<FromScratchPlayer>();
            var craftingTable = target.GetGameObject().GetComponent<CraftingTable>();
            var menu = new CraftingFullscreenMenu(craftingTable).SetPlayer(player);
            FullscreenMenuHost.Instance.OpenMenu(menu);
            // menu.OpenForPlayer(player);
        }
    }
}