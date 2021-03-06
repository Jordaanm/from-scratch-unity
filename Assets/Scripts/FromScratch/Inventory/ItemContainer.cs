using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace FromScratch.Inventory
{
    public class ItemContainer
    {
        public const int MaxItemSlots = 100;
        protected const int DefaultMaxSlots = 8;
        [SerializeField]
        protected string containerName;
        [SerializeField]
        protected int maxSlots;

        [SerializeField] protected List<Item> itemSlots = new List<Item>();

        public UnityEvent OnChange = new UnityEvent();

        public List<Item> Slots
        {
            get => itemSlots;
        }

        public ItemContainer(int maxSlots = DefaultMaxSlots, string name = "")
        {
            this.maxSlots = maxSlots;
            containerName = name;

            for (int x = 0; x < MaxItemSlots; ++x)
            {
                itemSlots.Add(null);
            }
        }

        public int MaxSlots
        {
            get => this.maxSlots;
            set
            {
                if (value == maxSlots) return;

                if (value > maxSlots)
                {
                    maxSlots = value;
                }

                if (value < maxSlots)
                {
                    //Drop all excess items
                    for (int i = maxSlots; i >= value; --i)
                    {
                        DropItem(i);
                    }

                    maxSlots = value;
                }
            }
        }

        public bool AddItem(ItemData itemData, int amount)
        {
            return AddItem(new Item(itemData, amount));
        }
        
        public bool AddItem(Item item)
        {
            Debug.LogFormat("Adding {0}x {1} to Item Container", item.stacks.ToString(), item.itemData.itemName);
            
            if(item.itemData.isStackable && HasStackOfItem(item.itemData))
            {
                AddStacks(item);
                
                OnChange.Invoke();
                return true;
            } else if (HasEmptySlot)
            {
                AddToEmptySlot(item);
                
                OnChange.Invoke();
                return true;
            }

            return false;
        }

        private void AddToEmptySlot(Item item)
        {
            int index = itemSlots.FindIndex(x => x == null);
            if (index != -1)
            {
                itemSlots[index] = item;
            }
        }
        
        private void AddStacks(Item item)
        {
            int index = itemSlots.FindIndex(x => x != null && x.itemData.id == item.itemData.id);
            if (index != -1)
            {
                itemSlots[index].stacks += item.stacks;
            }
            
            Debug.LogFormat("You have {0} stacks of {1}", itemSlots[index].stacks.ToString(), itemSlots[index].itemData.itemName);
        }

        private bool HasEmptySlot => itemSlots.GetRange(0, maxSlots).Any(item => item == null);

        private bool HasStackOfItem(ItemData itemData)
        {
            return itemSlots.GetRange(0, maxSlots).Any(existingItem => existingItem != null && existingItem.itemData.id == itemData.id);
        }

        private void DropItem(int i)
        {
            Item item = itemSlots[i];
            
            itemSlots[i] = null; //TODO: EmptySlot placeholder?

            if (item != null)
            {
                //TODO: Logic for Dropping Item
                Debug.LogFormat("Dropping Item: {0}", item.itemData.Name);
                OnChange.Invoke();
            }
        }

        public int GetItemCount(ItemData targetItem)
        {
            if (targetItem == null)
            {
                return 0;
            }

            return Slots
                .FindAll(item => item?.itemData != null && item.itemData.id == targetItem.id) //Find all slots with a matching item
                .Select(item => item.stacks) //Get the stacks in each matching slot
                .Aggregate(0, (sum, amount) => sum + amount); //Sum total
        }

        public int GetFirstStack(ItemData itemData)
        {
            return Slots.FindIndex(item => item?.itemData.id == itemData.id);
        }

        public void ReduceStacks(ItemData itemData, int amount)
        {
            int remaining = amount;

            while (remaining > 0 && HasStackOfItem(itemData))
            {
                int slotIndex = GetFirstStack(itemData);
                var slot = Slots[slotIndex];
                int stacksToRemove = Math.Min(remaining, slot.stacks);

                slot.stacks -= stacksToRemove;
                remaining -= stacksToRemove;
                if (slot.stacks == 0)
                {
                    Slots[slotIndex] = null;
                }
            }
            
            OnChange.Invoke();
        }

        public void SwapSlots(int source, int dest)
        {
            (Slots[source], Slots[dest]) = (Slots[dest], Slots[source]);
        }

        public void RemoveItem(Item item)
        {
            var index = Slots.FindIndex(x => x == item);
            if (index >= 0)
            {
                Slots[index] = null;
            }
            OnChange.Invoke();
        }
    }
}