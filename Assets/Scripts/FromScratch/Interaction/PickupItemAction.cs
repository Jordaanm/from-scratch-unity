using FromScratch.Character;
using FromScratch.Inventory;
using UnityEngine;

namespace FromScratch.Interaction
{
    public class PickupItemAction: Interaction
    {
        public PickupItemAction()
        {
            _id = "PICKUP_ITEM";
            _label = "Pick Up";
        }
        
        public override bool CanInteractWith(IInteractable interactable)
        {
            return interactable.GetInteractionType() == InteractionType.Pickup;
        } 
        
        public override void Start(IInteractor interactor, IInteractable target)
        {
            GameObject interactorGO = interactor.GetGameObject();
            GameObject targetGO = target.GetGameObject();
            
            Character.Character character = interactorGO.GetComponent<Character.Character>();
            CharacterInventory charInventory = interactorGO.GetComponent<CharacterInventory>();

            ItemData itemData = targetGO.GetComponent<Pickup>().item;
            
            GameObject.Destroy(targetGO);
            charInventory.Container.AddItem(new Item(itemData));
        }
    }
}