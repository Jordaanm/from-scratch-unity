using UnityEngine;
using Util;

namespace FromScratch.Inventory
{
    [CreateAssetMenu(fileName = "ItemDatabase", menuName = "From Scratch/Inventory/Item Database")]
    public class ItemDatabase: ScriptableDatabase<ItemData>
    { }
}