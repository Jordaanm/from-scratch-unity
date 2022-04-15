using FromScratch.Inventory;
using UnityEngine;

namespace FromScratch.Interaction
{
    public class Pickup: Interactable
    {
        public ItemData item;
        public override InteractionType GetInteractionType() => InteractionType.Pickup;
    }
}