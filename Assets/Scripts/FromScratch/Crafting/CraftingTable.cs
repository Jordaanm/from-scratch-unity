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
    
    public class CraftingTable : Interactable
    {
        [EnumToggleButtons]
        public CraftingStationCapability capabilities;
        
        public override InteractionType GetInteractionType()
        {
            return InteractionType.CraftingStation;
        }
    }
}