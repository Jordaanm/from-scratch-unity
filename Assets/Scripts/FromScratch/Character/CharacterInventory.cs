using System.Collections.Generic;
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
    }
}