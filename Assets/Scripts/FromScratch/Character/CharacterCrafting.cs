using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FromScratch.Crafting;
using FromScratch.Interaction;
using FromScratch.Inventory;
using UnityEngine;

namespace FromScratch.Character
{
    public class CharacterCrafting: MonoBehaviour, IInteractionSource
    {
        private Character character;
        private CharacterInventory characterInventory;
        private CharacterEquipment characterEquipment;
        
        private List<Recipe> knownRecipes = new List<Recipe>();
        private Interaction.Interaction craftingTableInteraction;


        private void Awake()
        {
            character = GetComponent<Character>();
            characterInventory = GetComponent<CharacterInventory>();
            characterEquipment = GetComponent<CharacterEquipment>();
            craftingTableInteraction = Interaction.Interaction.GetInteraction("craftingtable");
        }

        public bool CanCraft(Recipe recipe)
        {
            return KnowsRecipe(recipe) && HasAllIngredients(recipe);
        }

        public bool KnowsRecipe(Recipe recipe)
        {
            return true;
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

        public async void Craft(Recipe recipe, GameObject station)
        {
            if(!CanCraft(recipe)) { return; }

            if (station != null)
            {
                Debug.Log("TODO: Turn to face crafting station");
                await Task.Delay(100);
            }
            
            character.characterAnimation.PlayClip(recipe.animation, () => CompleteCraft(recipe));
        }

        private void CompleteCraft(Recipe recipe)
        {
            ConsumeIngredients(recipe);
            characterInventory.Container.AddItem(recipe.output.item, recipe.output.amount);
        }

        private void ConsumeIngredients(Recipe recipe)
        {
            foreach (var recipeIngredient in recipe.input)
            {
                characterInventory.Container.ReduceStacks(recipeIngredient.item, recipeIngredient.amount);
            }
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

        public List<Interaction.Interaction> GetActionsForTarget(Interactable target)
        {
            return new List<Interaction.Interaction>
            {
                craftingTableInteraction
            };
        }
    }
}