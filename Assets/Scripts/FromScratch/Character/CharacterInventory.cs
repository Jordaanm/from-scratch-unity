using FromScratch.Inventory;
using UnityEngine;

namespace FromScratch.Character
{
    public class CharacterInventory: MonoBehaviour
    {
        private Character character;
        private static readonly string PlayerInventoryName = "Player Inventory";

        private ItemContainer itemContainer;


        public ItemContainer Container => itemContainer;
        private void Awake()
        {
            this.character = GetComponent<Character>();

            if (this.itemContainer == null)
            {
                this.itemContainer = new ItemContainer(16, PlayerInventoryName);
            }
        }
    }
}