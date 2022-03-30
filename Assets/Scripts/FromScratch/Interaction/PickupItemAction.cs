using System.Threading.Tasks;
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
        
        public override async void Start(IInteractor interactor, IInteractable target)
        {
            GameObject interactorGO = interactor.GetGameObject();
            GameObject targetGO = target.GetGameObject();
            
            CharacterInventory charInventory = interactorGO.GetComponent<CharacterInventory>();

            ItemData itemData = targetGO.GetComponent<Pickup>().item;

            //Temporary hack to force onTriggerExist before Destroy is called
            var collider = targetGO.GetComponent<Collider>();
            collider.transform.localScale = Vector3.zero;
            await Task.Delay(100);
            
            GameObject.Destroy(targetGO);
            charInventory.Container.AddItem(new Item(itemData));
        }
    }
}