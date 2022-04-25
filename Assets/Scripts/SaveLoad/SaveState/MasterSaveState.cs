using System;

namespace SaveLoad.SaveState
{
    [Serializable]
    public class MasterSaveState
    {
        public PlayerPreferencesSaveState playerPrefs;
        // public WorldEnvironmentState worldEnviron;
        // public WorldState worldFeatures;
        public CharacterState[] characters;
        public PlayerState player;
        // public ItemContainerState[] containers;
        // public PlacedItemState[] placedItems;
    }
}