using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace FromScratch.Inventory
{
    [CreateAssetMenu(fileName = "New Loot Table", menuName = "From Scratch/Inventory/Loot Table")]
    public class LootTable : ScriptableObject
    {
        #region Inner Classes

        [System.Serializable]
        public class Loot
        {
            [InlineEditor(InlineEditorModes.LargePreview)]
            public ItemData itemData;
            public int amount = 1;
            public int weight = 1;

            public Loot(ItemData item, int amount, int weight)
            {
                this.itemData = item;
                this.amount = amount;
                this.weight = weight;
            }

            public Loot()
            {
                this.itemData = null;
                this.amount = 1;
                this.weight = 1;
            }
        }

        [System.Serializable]
        public class LootResult
        {
            [AssetSelector(Paths = "Assets/Data/Items"), ]
            public ItemData itemData = null;
            public int amount = 0;

            public LootResult(ItemData itemData, int amount)
            {
                if (itemData != null) this.itemData = itemData;
                this.amount = amount;
            }
        }

        #endregion

        public int noDropWeight = 0;
        [TableList]
        public List<Loot> loot = new List<Loot>();
        
        public LootResult Get()
        {
            List<Loot> chances = new List<Loot>();
            int totalWeight = 0;

            if (this.noDropWeight > 0)
            {
                totalWeight += this.noDropWeight;
                chances.Add(new Loot(null, 0, this.noDropWeight));
            }

            for (int i = 0; i < this.loot.Count; ++i)
            {
                chances.Add(this.loot[i]);
                totalWeight += this.loot[i].weight;
            }

            chances.Sort((Loot x, Loot y) => y.weight.CompareTo(x.weight));
            int random = Random.Range(0, totalWeight);

            for (int i = 0; i < chances.Count; ++i)
            {
                Loot loot = chances[i];
                if (random < loot.weight)
                {
                    LootResult result = new LootResult(loot.itemData, loot.amount);
                    if (result.itemData == null || result.amount < 1) return new LootResult(null, 0);

                    return result;
                }

                random -= loot.weight;
            }

            return new LootResult(null, 0);
        }
    }
}