using System.Threading.Tasks;
using FromScratch.Character;
using FromScratch.Gathering;
using FromScratch.Inventory;
using UnityEngine;

namespace FromScratch.Interaction
{
    public class GatherResourceAction: Interaction
    {
        public GatherResourceAction()
        {
            _id = "GATHER_RESOURCE";
            _label = "Gather";
        }
        
        public override async void Start(IInteractor interactor, IInteractable target)
        {
            GameObject interactorGO = interactor.GetGameObject();
            GameObject targetGO = target.GetGameObject();
            
            Character.Character character = interactorGO.GetComponent<Character.Character>();
            CharacterAnimation charAnimation = interactorGO.GetComponent<CharacterAnimation>();
            CharacterInventory charInventory = interactorGO.GetComponent<CharacterInventory>();

            ResourceNode resourceNode = targetGO.GetComponent<ResourceNode>();
            ResourceNodeInfo resourceNodeInfo = resourceNode.resourceNodeInfo;

            character.DisableControls();
            
            charAnimation.PlayClip(resourceNodeInfo.animation);

            float waitTime = resourceNodeInfo.animationDuration > 0
                ? resourceNodeInfo.animationDuration
                : resourceNodeInfo.animation.length;

            await Task.Delay(Mathf.CeilToInt(waitTime * 1000f));

            resourceNode.DepleteResources(1);
            charInventory.Container.AddItem(new Item(resourceNodeInfo.itemData));

            character.EnableControls();
        }
    }
}