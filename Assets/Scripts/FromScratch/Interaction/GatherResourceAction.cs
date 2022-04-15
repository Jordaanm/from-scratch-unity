using System.Threading.Tasks;
using FromScratch.Character;
using FromScratch.Gathering;
using FromScratch.Inventory;
using UnityEngine;

namespace FromScratch.Interaction
{
    [Interaction(id = "gather")]
    public class GatherResourceAction: Interaction
    {
        public GatherResourceAction()
        {
            _id = "GATHER_RESOURCE";
            _label = "Gather";
        }
        
        
        public override bool CanInteractWith(Interactable interactable)
        {
            return interactable.GetInteractionType() == InteractionType.ResourceNode;
        } 
        
        public override void Start(IInteractor interactor, Interactable target)
        {
            GameObject interactorGameObject = interactor.GetGameObject();
            GameObject targetGameObject = target.gameObject;
            
            Character.Character character = interactorGameObject.GetComponent<Character.Character>();
            CharacterAnimation charAnimation = interactorGameObject.GetComponent<CharacterAnimation>();
            CharacterInventory charInventory = interactorGameObject.GetComponent<CharacterInventory>();

            ResourceNode resourceNode = targetGameObject.GetComponent<ResourceNode>();
            ResourceNodeInfo resourceNodeInfo = resourceNode.resourceNodeInfo;

            character.DisableControls();

            //TODO: Allow for dynamic play time

            charAnimation.PlayClip(resourceNodeInfo.animation, () =>
            {
                LootTable.LootResult lootResult = resourceNodeInfo.lootTable.Get();
                resourceNode.DepleteResources(lootResult.amount);
                charInventory.Container.AddItem(new Item(lootResult.itemData, lootResult.amount));

                character.EnableControls();
            });
        }
    }
}