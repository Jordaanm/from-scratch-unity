using System;
using System.Collections.Generic;
using System.Linq;
using FromScratch.Crafting;
using FromScratch.Inventory;
using UnityEngine;

namespace FromScratch.Character
{
    public class CharacterCrafting: MonoBehaviour
    {
        private Character character;
        private CharacterInventory characterInventory;
        private CharacterEquipment characterEquipment;
        
        private List<Recipe> knownRecipes = new List<Recipe>();

        private void Awake()
        {
            character = GetComponent<Character>();
            characterInventory = GetComponent<CharacterInventory>();
            characterEquipment = GetComponent<CharacterEquipment>();
        }

        public bool CanCraft(Recipe recipe)
        {
            return KnowsRecipe(recipe) && HasAllIngredients(recipe);
        }

        public bool KnowsRecipe(Recipe recipe)
        {
            return knownRecipes.Contains(recipe);
        }

        public bool HasAllIngredients(Recipe recipe)
        {
            if (recipe == null)
            {
                return false; 
            }
            return recipe.input.All(ingredient => HasEnoughIngredient(ingredient));
        }

        public void Craft(Recipe recipe)
        {
            
        }

        public int IngredientCount(ItemData itemData)
        {
            return characterInventory.Container.GetItemCount(itemData);
        }

        public bool HasEnoughIngredient(RecipeIngredient ingredient)
        {
            int count = characterInventory.Container.GetItemCount(ingredient.item);
            return count >= ingredient.amount;
        }
    }
}