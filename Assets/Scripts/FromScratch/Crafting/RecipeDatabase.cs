using UnityEngine;
using Util;

namespace FromScratch.Crafting
{    
    [CreateAssetMenu(fileName = "RecipeDatabase", menuName = "From Scratch/Recipe Database")]
    public class RecipeDatabase: ScriptableDatabase<Recipe> {}
}