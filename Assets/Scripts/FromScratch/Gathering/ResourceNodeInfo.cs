﻿using FromScratch.Inventory;
using UnityEngine;

namespace FromScratch.Gathering
{
    [CreateAssetMenu(menuName = "From Scratch/Gathering/ResourceNodeInfo", fileName = "MyResourceNodeInfo")]
    public class ResourceNodeInfo: ScriptableObject
    {
        public string nodeName = "My Resource Node";
        public ItemData itemData; //TODO: Replace with LootTable
        public AnimationClip animation;
        public float animationDuration = -1f;
        public bool isRepeatable = true;
        public int initialStock = 99;
    }
}