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
        
        public override bool CanInteractWith(Interactable interactable)
        {
            return interactable.GetInteractionType() == InteractionType.Pickup;
        } 
        
        public override void Start(IInteractor interactor, Interactable target)
        {
            GameObject interactorGO = interactor.GetGameObject();
            GameObject targetGO = target.gameObject;
            
            CharacterInventory charInventory = interactorGO.GetComponent<CharacterInventory>();

            ItemData itemData = targetGO.GetComponent<Pickup>().item;

            GameObject.Destroy(targetGO);
            charInventory.Container.AddItem(new Item(itemData));
        }
    }
}