using System.Collections.Generic;
using FromScratch.Interaction;
using Sirenix.OdinInspector;
using UnityEngine;

namespace FromScratch.Crafting
{
    [System.Flags]
    public enum CraftingStationCapability
    {
        None = 0,
        Basic = 1 << 1,
        Carpentry = 1 << 2,
        Masonry = 1 << 3,
        Forging = 1 << 4,
        Electronics = 1 << 5,
    }
    
    public class CraftingTable : MonoBehaviour, IInteractable
    {
        [EnumToggleButtons]
        public CraftingStationCapability capabilities;
        
        public InteractionType GetInteractionType()
        {
            return InteractionType.CraftingStation;
        }

        public GameObject GetGameObject()
        {
            return gameObject;
        }
    }
}