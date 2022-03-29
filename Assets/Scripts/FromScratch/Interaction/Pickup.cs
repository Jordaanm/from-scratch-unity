using FromScratch.Inventory;
using UnityEngine;

namespace FromScratch.Interaction
{
    public class Pickup: MonoBehaviour, IInteractable
    {
        public ItemData item;
        public InteractionType GetInteractionType() => InteractionType.Pickup;
        public GameObject GetGameObject() => gameObject;
    }
}