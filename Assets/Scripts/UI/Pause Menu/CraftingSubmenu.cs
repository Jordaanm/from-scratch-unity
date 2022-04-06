using System;
using System.Collections.Generic;
using System.Linq;
using AssetReferences;
using FromScratch.Character;
using FromScratch.Crafting;
using FromScratch.Player;
using UnityEngine;
using UnityEngine.UIElements;
using Util;

namespace UI
{
    public class CraftingSubmenu: MultiScreenMenu.Submenu
    {        
        public static string menuName = "Crafting";
        public static string menuID = "CRAFTING";
        
        private LayerManager layerManager;
                
        #region UI References        
        protected static VisualTreeAsset treeAsset;
        protected const string TreeAssetKey = "crafting-menu";
        protected static VisualTreeAsset itemTreeAsset;
        protected const string ItemTreeAssetKey = "crafting-menu--item";
        protected static VisualTreeAsset detailsPanelTreeAsset;
        protected const string DetailsPanelTreeAssetKey = "crafting-menu--details-panel";
        protected static VisualTreeAsset ingredientTreeAsset;
        protected const string IngredientTreeAssetKey = "crafting-menu--ingredient";
        
        private VisualElement veRoot;
        private VisualElement recipeListContainer;
        private ListView recipeListView;
        private Button craftingButton;
        
        private IUserInterfaceLayer openContextMenu = null;
        #endregion
        
        private FromScratchPlayer player;
        private CraftingTable craftingTable;
        private List<Recipe> recipes;
        private Recipe selectedRecipe = null;
        
        //Filters
        private bool onlyRecipesWithAllIngredients = false;

        public CraftingSubmenu(FromScratchPlayer player = null, CraftingTable table = null)
        {
            this.player = player;
            this.craftingTable = table;
            BuildUI();
            recipes = new List<Recipe>();
        }

        public void SetPlayer(FromScratchPlayer player)
        {
            this.player = player;
        }
        
        #region BuildUI
        
        private void BuildUI()
        {
            
            if (treeAsset == null) {
                treeAsset = VisualTreeAssetReference.Instance.GetAsset(TreeAssetKey);
                itemTreeAsset = VisualTreeAssetReference.Instance.GetAsset(ItemTreeAssetKey);
                detailsPanelTreeAsset = VisualTreeAssetReference.Instance.GetAsset(DetailsPanelTreeAssetKey);
                ingredientTreeAsset = VisualTreeAssetReference.Instance.GetAsset(IngredientTreeAssetKey);
            }

            veRoot = VisualTreeAssetReference.Create(treeAsset);
            layerManager = new LayerManager(veRoot);

            BuildRecipeList();
            BuildDetailsSection();
        }

        private void BuildRecipeList()
        {
            recipeListContainer = veRoot.Q("recipe-list-container");
            
            // The "makeItem" function is called when the ListView needs more items to render.
            Func<VisualElement> makeItem = () => itemTreeAsset.CloneTree();
            // Binds the list to the items
            Action<VisualElement, int> bindItem = BindRecipeItem;
            
            // Provide the list view with an explict height for every row
            // so it can calculate how many items to actually display
            const int itemHeight = 200;

            recipeListView = new ListView(new List<Recipe>(), itemHeight, makeItem, bindItem);
            
            recipeListView.selectionType = SelectionType.Single;
            recipeListView.onItemsChosen += objects => Debug.Log(objects);
            recipeListView.onSelectionChange += SelectionChanged;
            recipeListView.style.flexGrow = 1.0f;

            recipeListContainer.Add(recipeListView);
        }
        
        private void BuildDetailsSection()
        {
            var section = veRoot.Q("details-section");
            var panelContents = VisualTreeAssetReference.Create(detailsPanelTreeAsset);
            section.Add(panelContents);
            
            var outputDetails = veRoot.Q("output-details");

            var iconVE = outputDetails.Q("item-icon");
            iconVE.style.backgroundImage = null;

            var label = outputDetails.Q<Label>("item-name");
            label.text = "No Item Selected";

            var description = outputDetails.Q<Label>("item-description");
            description.text = "Select an item on the left to craft";

            craftingButton = veRoot.Q<Button>("btnCraft");
            craftingButton.SetEnabled(false);

            craftingButton.clicked += (() => {
                player?.character?.characterCrafting?.Craft(selectedRecipe, null);
                Debug.Log("CRAFTARINO");
            });
        }
        
        private void BindRecipeItem(VisualElement element, int index) {
            Recipe recipe = recipes[index];
            element.Q<Label>("recipe-name").text = recipe.label;
            element.Q("recipe-item").style.backgroundImage = new StyleBackground(recipe.output.item.icon);
        }
        
        private void SelectionChanged(IEnumerable<object> selection) {
            List<Recipe> selectedRecipes = selection
                .Select(x => x as Recipe)
                .Where(x => x != null)
                .ToList();
            
            Recipe recipe = selectedRecipes.Count > 0 ? selectedRecipes[0] : null;
            Debug.LogFormat("Selected Recipe: {0}", recipe?.label??"None");
            selectedRecipe = recipe;
            UpdateDetailsSection(recipe);
        }

        private VisualElement BuildIngredientUI(RecipeIngredient ingredient)
        {
            var ingredientVE = VisualTreeAssetReference.Create(ingredientTreeAsset);
            var listItem = ingredientVE.Q("ingredients-list-item");

            ingredientVE.Q("ingredient-icon").style.backgroundImage = new StyleBackground(ingredient.item.icon);
            ingredientVE.Q<Label>("ingredient-name").text = ingredient.item.itemName;

            CharacterCrafting crafting = player.character.characterCrafting;
            int amount = crafting.IngredientCount(ingredient.item);

            if (crafting.HasEnoughIngredient(ingredient))
            {
                listItem.RemoveFromClassList("not-enough");
            }
            else
            {
                listItem.AddToClassList("not-enough");
            }
            
            ingredientVE.Q<Label>("ingredient-count").text = String.Format("{0}/{1}", amount.ToString(), ingredient.amount.ToString());

            return ingredientVE;
        }
        
        #endregion

        #region Update UI
        
        private void UpdateAvailableRecipes()
        {
            List<Recipe> allRecipes = GameDatabases.Instance.recipes.entries;

            recipes = allRecipes
                .FindAll(x => x != null) // Not Null
                .FindAll(StationCanCraftRecipe) // Station is capable of crafting
                .FindAll(CharacterKnowsRecipe) // Recipe has been unlocked by the character/player
                .FindAll(HasAllIngredients) // Character has all the needed ingredients (controlled by UI toggle)
                .ToList();
            UpdateRecipeList();
        }


        private void UpdateRecipeList()
        {
            recipeListView.itemsSource = recipes;
        }
        
        private void UpdateDetailsSection(Recipe recipe)
        {
            var outputDetails = veRoot.Q("output-details");
            var iconVE = outputDetails.Q("item-icon");
            var label = outputDetails.Q<Label>("item-name");
            var description = outputDetails.Q<Label>("item-description");
            var ingredients = veRoot.Q("ingredients-section");
        
            while (ingredients.childCount > 0) {
                ingredients.RemoveAt(0);
            }

            if (recipe != null) {
                var item = recipe.output.item;
                //Render recipe details
                iconVE.style.backgroundImage = new StyleBackground(item.icon);
                label.text = item.itemName;
                description.text = item.itemDescription;
                foreach (RecipeIngredient ingredient in recipe.input) {
                    var ingredVE = BuildIngredientUI(ingredient);
                    ingredients.Add(ingredVE);
                }
            } else {
                //Render placeholder for recipe details
                iconVE.style.backgroundImage = null;
                label.text = "No Item Selected";
                description.text = "Select an item on the left to craft";
            }

            bool canBuild = player.character.characterCrafting.CanCraft(recipe);
            
            craftingButton.SetEnabled(canBuild);
        }
        
        #endregion
        
        #region Filter Predicates

        private bool HasAllIngredients(Recipe recipe)
        {
            if (!onlyRecipesWithAllIngredients)
            {
                return true;
            }

            var crafting = player?.character?.characterCrafting;
            if (crafting == null)
            {
                return false;
            }

            return crafting.HasAllIngredients(recipe);
        }
        
        private bool CharacterKnowsRecipe(Recipe recipe)
        {
            var crafting = player?.character?.characterCrafting;
            if (crafting == null)
            {
                return false;
            }

            return crafting.KnowsRecipe(recipe);
        }

        private bool StationCanCraftRecipe(Recipe recipe)
        {
            if (recipe == null)
            {
                return false;
            }
            
            if (craftingTable == null) //Table = null means it's being accessed through the Pause Crafting submenu, simple recipes only
            {
                return recipe.requiredCapability == CraftingStationCapability.None;
            }

            return craftingTable.capabilities.HasFlag(recipe.requiredCapability);
        }
        
        #endregion
        
        #region MultiScreenMenu.Submenu contract
        public string GetTitle()
        {
            return menuName;
        }

        public string GetID()
        {
            return menuID;
        }

        public VisualElement GetRoot()
        {
            if (veRoot == null) {
                BuildUI();
            }

            return veRoot;
        }

        public void OnOpen()
        {
            //TODO: Setup
            UpdateAvailableRecipes();
        }

        public void OnClose()
        {
            selectedRecipe = null;
            UpdateDetailsSection(null);
            //TODO: Cleanup
        }

        public void SetIsActive(bool state)
        {
            //TODO: Cleanup when Inactive
        }
        
        #endregion
    }
}