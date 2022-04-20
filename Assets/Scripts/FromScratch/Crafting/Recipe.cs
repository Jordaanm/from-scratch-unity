using System;
using System.Collections.Generic;
using FromScratch.Inventory;
using Sirenix.OdinInspector;
using UnityEngine;
using Unlockable;
using Util;

namespace FromScratch.Crafting
{
    [Serializable]
    public struct RecipeIngredient
    {
        [HorizontalGroup("fields", width:50), SuffixLabel("x"), HideLabel]
        public int amount;

        [HorizontalGroup("fields"), PreviewField(ObjectFieldAlignment.Left), HideLabel]
        public ItemData item;


        public RecipeIngredient(ItemData item, int amount = 1)
        {
            this.item = item;
            this.amount = amount;
        }
    }
    
    [ManageableData]
    [CreateAssetMenu(menuName = "From Scratch/Recipe")]
    public class Recipe: ScriptableData, IUnlock
    {
        [BoxGroup("BasicInfo")]
        public string id;

        [BoxGroup("BasicInfo")]
        public string label;

        [BoxGroup("BasicInfo")]
        public string description;

        [BoxGroup("Capabilities")] [EnumToggleButtons]
        public CraftingStationCapability requiredCapability;
        
        [BoxGroup("Ingredients")]
        public List<RecipeIngredient> input;

        [BoxGroup("Ingredients"), Space]
        public RecipeIngredient output;

        [BoxGroup("Visuals")]
        public float craftingTime = -1;

        [BoxGroup("Visuals")]
        public AnimationClip animation;
        
        [SerializeField, TableList, BoxGroup("Unlock")]
        private List<Prerequisite> prerequisites;

        [SerializeField, EnumToggleButtons, BoxGroup("Unlock")] private Significance significance;
        public override string GetID()
        {
            return id;
        }

        #region IUnlock
        public string ID => id;
        public List<Prerequisite> Prerequisites => prerequisites;
        public Significance Significance => significance;

        #endregion


        public static Recipe FindFromPrereq(Prerequisite prereq)
        {
            var DB = GameDatabases.Instance.recipes.GetDatabase();
            if (DB != null && DB.ContainsKey(prereq.id))
            {
                return DB[prereq.id];
            }

            return null;
        }
    }
}