namespace FromScratch.Inventory
{
    public class Item
    {
        public ItemData itemData;
        public int stacks;
        //TODO: Stats for Equipment
        public bool IsEquipped { get; set; }
        //TODO: Durability

        public Item(ItemData itemData, int stacks = 1)
        {
            this.itemData = itemData;
            this.stacks = stacks;
            IsEquipped = false;
        }
    }
}
