using System.Linq;
using AssetReferences;
using FromScratch.Inventory;
using FromScratch.Player;
using UI.Layers;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI
{
    public class InventoryMenu: MultiScreenMenu.Submenu
    {
        public static string MenuName = "Inventory";
        public static string MenuID = "INVENTORY";
        public static string EmptyItemName = "None";

        private LayerManager layerManager;
        #region UI References
        protected static VisualTreeAsset treeAsset;
        protected const string treeAssetKey = "inventory-menu";
        protected static VisualTreeAsset itemTreeAsset;
        protected const string itemTreeAssetKey = "inventory-menu__item";

        private VisualElement veRoot = null;
        private VisualElement veItemContainer = null;
        #endregion

        private FromScratchPlayer player;
        
        public InventoryMenu(FromScratchPlayer player)
        {
            this.player = player;
            BuildUI();
        }
        
        private ItemContainer Inventory{
            get
            {
                if (this.player == null)
                {
                    return null;
                }

                return this.player.character.characterInventory.Container;
            }
        }

        #region BuildUI
        
        private void BuildUI()
        {
            
            if (treeAsset == null) {
                treeAsset = VisualTreeAssetReference.Instance.GetAsset(treeAssetKey);
                itemTreeAsset = VisualTreeAssetReference.Instance.GetAsset(itemTreeAssetKey);
            }

            veRoot = treeAsset.CloneTree().Children().First();
            layerManager = new LayerManager(veRoot);
            
            //TODO: Setup Drag and Drop
            
            veItemContainer = veRoot.Q("item-container");
            DrawInventory();
        }

        private void DrawInventory()
        {
            veItemContainer.Clear();
            Debug.LogFormat("Draw Inventory {0}", Inventory == null ? "Null" : Inventory.MaxSlots.ToString());
            if (Inventory == null)
            {
                return;
            }

            for (int x = 0; x < Inventory.MaxSlots; ++x)
            {
                VisualElement slot = BuildInventorySlot(x);
                veItemContainer.Add(slot);
            }
        }
        
        private VisualElement BuildInventorySlot(int index) {
            Item itemStack = Inventory.Slots[index];

            var itemRoot = itemTreeAsset.CloneTree().Children().First();
            itemRoot.RegisterCallback<MouseDownEvent>(OnItemClick);
            UpdateItemStack(itemStack, itemRoot);

            // runDnd.MakeDraggable(itemRoot, "ItemStack", () => index.ToString());

            return itemRoot;
        }

        private void OnItemClick(MouseDownEvent evt)
        {
            VisualElement veTarget = (VisualElement)evt.currentTarget;
            if (veTarget != null)
            {
                Item itemStack = (Item)veTarget.userData;
                ItemData itemData = itemStack?.itemData;
                if (itemData != null)
                {
                    if (evt.clickCount > 1)
                    {
                        Debug.Log("Double Click");
                        DefaultActionForItem(itemStack);
                    }
                    else
                    {
                        DisplayMenuForItem(itemStack, veTarget);
                    }
                }
                Debug.LogFormat("You clicked on a stack of {0}", itemData == null ? "NONE" : itemData.itemName);
            }
        }

        private void DefaultActionForItem(Item itemStack)
        {
            ItemData itemData = itemStack?.itemData;
            if (itemData == null)
            {
                return;
            }

            //TODO: Refactor to PlayerEquipment.UseItem
            if (itemData.usageType == ItemData.UsageType.Equip)
            {
                player.character.characterEquipment.EquipItem(itemStack);
            }
            else
            {
                Debug.Log("I dunno");
            }
        }

        private void DisplayMenuForItem(Item itemStack, VisualElement visualElement)
        {
            ContextMenuAction testAction = new ContextMenuAction("test", "Test Action", () => Debug.Log("Woo"));
            ContextMenuAction equipAction = new ContextMenuAction("equip", "Equip",
                () => player.character.characterEquipment.EquipItem(itemStack));
            var layer = new ContextMenuLayer(visualElement);
            layer.AddAction(testAction);
            layer.AddAction(equipAction);
            // var layer = new PopoutLayer(visualElement, false);
            layerManager.AddLayer(layer);
        }

        #endregion
        
        #region Update UI 

        void UpdateItemStack(Item itemStack, VisualElement itemRoot) {
            itemRoot.ClearClassList();
            itemRoot.userData = itemStack;
            if (itemStack == null || itemStack.stacks == 0) {
                itemRoot.AddToClassList("empty");
            
                itemRoot.Q<Label>("item-count").text = "";
                itemRoot.Q("item-icon").style.backgroundImage = null;            
            } else
            {
                ItemData itemData = itemStack.itemData;
                string usageClass = itemData.usageType.ToString().ToLower();
                itemRoot.AddToClassList(usageClass);

                itemRoot.Q<Label>("item-count").text = itemStack.stacks.ToString();
                itemRoot.Q("item-icon").style.backgroundImage = new StyleBackground(itemData.icon);
            }
        }
        
        void UpdateInventory() {
            VisualElement[] slots = veItemContainer.Children().ToArray();

            for (int x = 0; x < veItemContainer.childCount; ++x) {
                VisualElement itemRoot = slots[x];
                var stack = Inventory.Slots[x];
                UpdateItemStack(stack, itemRoot);
            }
        }
        #endregion
        
        #region MultiScreenMenu.Submenu contract
        public VisualElement GetRoot() {
            if (veRoot == null) {
                BuildUI();
            }

            return veRoot;
        }

        public string GetTitle() {
            return MenuName;
        }

        public string GetID() {
            return MenuID;
        }

        public void OnMount() {
        }

        public void OnUnmount() {
        }

        public void SetIsActive(bool state) {
            if (state) {
                UpdateInventory();
                // UpdateHotbar();
            }
        }

        #endregion
    }
}