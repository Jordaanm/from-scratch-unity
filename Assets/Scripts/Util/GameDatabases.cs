using FromScratch.Crafting;
using FromScratch.Equipment;
using FromScratch.Inventory;
using UnityEngine;

namespace Util
{
    public class GameDatabases: MonoSingleton<GameDatabases>
    {
        public RecipeDatabase recipes;
        public EquipmentDatabase equipment;
        public ItemDatabase items;
    }
}