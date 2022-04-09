using System.Collections.Generic;
using FromScratch.Character.Modes;
using FromScratch.Interaction;
using FromScratch.Inventory;
using UnityEngine;
using UnityEngine.Events;

namespace FromScratch.Character
{
    public class CharacterInventory: MonoBehaviour
    {
        private Character character;
        public static readonly int QuickbarSize = 8;
        private static readonly string PlayerInventoryName = "Player Inventory";

        private List<Item> quickbar;
        private ItemContainer itemContainer;

        public UnityEvent OnQuickbarChange = new UnityEvent();

        public ItemContainer Container => itemContainer;
        private void Awake()
        {
            this.character = GetComponent<Character>();
            quickbar = new List<Item>(QuickbarSize);
            for (int x = 0; x < QuickbarSize; ++x) {
                quickbar.Add(null);
            }
            if (this.itemContainer == null)
            {
                this.itemContainer = new ItemContainer(16, PlayerInventoryName);
            }
        }

        public void SetQuickbarSlot(int quickbarIndex, int inventoryIndex)
        {
            quickbar[quickbarIndex] = Container.Slots[inventoryIndex];
            OnQuickbarChange.Invoke();
        }
        
        public Item GetQuickbarSlot(int quickbarIndex) {
            if(quickbarIndex < 0 || quickbarIndex >= QuickbarSize) { return null; }

            return quickbar[quickbarIndex];
        }

        public void DropItem(Item item)
        {
            //Remove from item container
            Container.RemoveItem(item);
            //If has prefab, spawn prefab, and add a pickup script to it
            if (!(item == null || item.itemData == null || item.itemData.prefab == null))
            {
                //Spawn Prefab at character's feet
                Transform t = character.transform;
                var pickSpawnPos = t.position + t.forward * 1;
                var instance = GameObject.Instantiate(item.itemData.prefab, pickSpawnPos, t.rotation);
                //Add a pickup script to it.
                Pickup pickupComp = instance.AddComponent<Pickup>();
                pickupComp.item = item.itemData;

            }
        }

        public void Consume(Item item)
        {
            Debug.Log("TODO: Implement Consume functionality");
        }

        public void StartItemPlacement(Item item)
        {
            Debug.Log("TODO: Implement Place Functionality");
            var placementMode = character.modeManager.SetActiveMode<PlacementMode>();
            if (placementMode == null)
            {
                return; 
            }
            placementMode.SetItemToPlace(item);
        }

        public void PlaceItem(Item item, Vector3 position, Quaternion rotation)
        {
            Debug.Log("Placing Item");
        }
    }
}