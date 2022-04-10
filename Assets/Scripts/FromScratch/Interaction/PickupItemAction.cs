using System.Threading.Tasks;
using FromScratch.Character;
using FromScratch.Inventory;
using UnityEngine;

namespace FromScratch.Interaction
{
    
    [Interaction(id = "pickup")]
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
            
            CharacterInventory charInventory = interactorGO.GetComponent<CharacterInventory>();

            ItemData itemData = targetGO.GetComponent<Pickup>().item;

            interactor.RemoveInteractable(target);
            
            GameObject.Destroy(targetGO);
            charInventory.Container.AddItem(new Item(itemData));
        }
    }
}