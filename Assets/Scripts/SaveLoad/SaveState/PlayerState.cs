using System;

namespace SaveLoad.SaveState
{    
    [Serializable]
    public class PlayerState
    {
        public string characterGuid;
        public string[] unlockedSkills;
        public string[] unlockedTech;
        public string[] unlockedRecipes;
    }
}