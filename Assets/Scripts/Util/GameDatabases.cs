using FromScratch.Crafting;
using FromScratch.Equipment;
using FromScratch.Inventory;
using FromScratch.Skills;
using FromScratch.Technology;
using UnityEngine;

namespace Util
{
    public class GameDatabases: MonoSingleton<GameDatabases>
    {
        public TechnologyDatabase tech;
        public RecipeDatabase recipes;
        public EquipmentDatabase equipment;
        public ItemDatabase items;
        public SkillDatabase skills;
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void OnRuntimeStartSingleton() {
            IS_EXITING = false;
        }
    }
}