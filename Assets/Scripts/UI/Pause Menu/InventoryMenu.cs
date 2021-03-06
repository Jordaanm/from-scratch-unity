using System.Collections.Generic;
using System.Linq;
using AssetReferences;
using FromScratch.Character;
using FromScratch.Inventory;
using FromScratch.Player;
using UI.Layers;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Util;

namespace UI
{
    public class InventoryMenu: MultiScreenMenu.Submenu
    {
        public static string MenuName = "Inventory";
        public static string MenuID = "INVENTORY";
        public static string EmptyItemName = "None";
        
        private LayerManager layerManager;
        private RuntimeDragDrop runDnd;
        
        #region UI References
        protected static VisualTreeAsset treeAsset;
        protected const string treeAssetKey = "inventory-menu";
        protected static VisualTreeAsset itemTreeAsset;
        protected const string itemTreeAssetKey = "inventory-menu__item";

        private VisualElement veRoot = null;
        private VisualElement veItemContainer = null;
        private VisualElement veHotbarContainer = null;

        private IUserInterfaceLayer openContextMenu = null;
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

        private void DropItemStack(Item item)
        {
            player.character.characterInventory.DropItem(item);
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

            SetupDragDrop();
            
            veItemContainer = veRoot.Q("item-container");
            veHotbarContainer = veRoot.Q("hotbar-container");
            DrawInventory();
            DrawHotbar();
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

        private void DrawHotbar()
        {
            for(int x = 0; x < CharacterInventory.QuickbarSize; ++x) {
                VisualElement hotbarSlot  = BuildHotbarSlot(x);
                veHotbarContainer.Add(hotbarSlot);
            }
        }
        
        private VisualElement BuildHotbarSlot(int index) {
            CharacterInventory characterInventory = player.character.characterInventory;
            var itemRoot = VisualTreeAssetReference.Create(itemTreeAsset);
            Item itemStack = characterInventory.GetQuickbarSlot(index);
            UpdateItemStack(itemStack, itemRoot);
            runDnd.MakeDropTarget(itemRoot, "QuickBar", () => index.ToString());
            return itemRoot;
        }
        
        private VisualElement BuildInventorySlot(int index) {
            Item itemStack = Inventory.Slots[index];

            var itemRoot = itemTreeAsset.CloneTree().Children().First();
            itemRoot.RegisterCallback<ClickEvent>(OnItemClick);
            UpdateItemStack(itemStack, itemRoot);

            runDnd.MakeDropTarget(itemRoot, "ItemStack", () => index.ToString());
            runDnd.MakeDraggable(itemRoot, "ItemStack", () => index.ToString());

            return itemRoot;
        }

        private void OnItemClick(ClickEvent evt)
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
            if (openContextMenu != null)
            {
                layerManager.RemoveLayer(openContextMenu);
            }

            
            var layer = new ContextMenuLayer(visualElement);

            var title = itemStack?.itemData != null ? itemStack.itemData.itemName : "";
            layer.SetTitle(title);
            
            var actions = ContextMenuActionsForItem(itemStack);
            layer.AddActions(actions);

            layerManager.AddLayer(layer);
            openContextMenu = layer;
        }

        private List<ContextMenuAction> ContextMenuActionsForItem(Item item)
        {
            var actionList = new List<ContextMenuAction>();
            
            ContextMenuAction inspectAction = new ContextMenuAction("test", "Inspect", () => Debug.Log("TODO: Implement Inspect Menu"));
            actionList.Add(inspectAction);

            switch (item?.itemData.usageType)
            {
                case ItemData.UsageType.Consume:
                {
                    actionList.Add(new ContextMenuAction("consume", "Consume", () => 
                        player.character.Consume(item.itemData)
                    ));
                    break;
                }
                case ItemData.UsageType.Equip:
                {
                    actionList.Add(new ContextMenuAction("equip", "Equip", () =>
                        player.character.characterEquipment.EquipItem(item)
                    ));
                    break;
                }
                case ItemData.UsageType.Place:
                {
                    actionList.Add(new ContextMenuAction("place", "Place", () =>
                        {
                            FullscreenMenuHost.Instance.CloseCurrentMenu();
                            player.character.characterInventory.StartItemPlacement(item);
                        }
                    ));
                    break;
                }
            }
            ContextMenuAction dropAction = new ContextMenuAction("drop", "Drop", () => DropItemStack(item));
            actionList.Add(dropAction);

            return actionList;
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
        
        void UpdateHotbar()
        {
            var characterInventory = player.character.characterInventory;
            VisualElement[] slots = veHotbarContainer.Children().ToArray();

            for (int x = 0; x < CharacterInventory.QuickbarSize; ++x) {
                var itemStack = characterInventory.GetQuickbarSlot(x);
                var itemRoot = slots[x];
                UpdateItemStack(itemStack, itemRoot);
            }
        }
        #endregion
        
        #region Drag and Drop
        
        
        private void SetupDragDrop() {
            runDnd = RuntimeDragDrop.Initialise(veRoot);

            runDnd.AddDragCase(new RunDndDragCase(
                (type, data) => type == "ItemStack",
                (type, data) => CreateItemPreview(int.Parse(data as string))
            ));

            runDnd.AddDropCase(new RunDndDropCase(
                (dragType, dropType, dragData, dropData) => dragType == "ItemStack" && dropType == "QuickBar",
                (dragType, dropType, dragData, dropData) =>
                {
                    var characterInventory = player.character.characterInventory;
                    
                    int quickbarIndex = int.Parse(dropData as string ?? "");
                    int inventoryIndex = int.Parse(dragData as string ?? "");

                    characterInventory.SetQuickbarSlot(quickbarIndex, inventoryIndex);
                    UpdateHotbar();
                }
            ));

            runDnd.AddDropCase(new RunDndDropCase(
                (dragType, dropType, dragData, dropData) => dragType == "ItemStack" && dropType == "ItemStack",
                (dragType, dropType, dragData, dropData) =>
                {
                    int dest = int.Parse(dropData as string);
                    int source = int.Parse(dragData as string);

                    player.character.characterInventory.Container.SwapSlots(source, dest);
                    UpdateInventory();
                }
            ));
        }

        private VisualElement CreateItemPreview(int index) {
            var container = player.character.characterInventory.Container;

            Item itemStack = container.Slots[index];
            var itemRoot = itemTreeAsset.CloneTree().Children().First();
            UpdateItemStack(itemStack, itemRoot);
            itemRoot.Q("stack-wrapper").pickingMode = PickingMode.Ignore;
            itemRoot.Q("item-wrapper").pickingMode = PickingMode.Ignore;
            itemRoot.Q("item-icon").pickingMode = PickingMode.Ignore;
            return itemRoot;
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

        public void OnOpen()
        {
            player.character.characterInventory.OnQuickbarChange.AddListener(UpdateHotbar);
            player.character.characterInventory.Container.OnChange.AddListener(UpdateInventory);
            UpdateInventory();
            UpdateHotbar();
        }

        public void OnClose() {
            if (openContextMenu != null)
            {
                layerManager.RemoveLayer(openContextMenu);
            }
            player.character.characterInventory.OnQuickbarChange.RemoveListener(UpdateHotbar);
            player.character.characterInventory.Container.OnChange.RemoveListener(UpdateInventory);
        }
        
        public void Update()
        {
            
        }
        
        public bool HasOpenLayer()
        {
            return layerManager.HasOpenLayer;
        }

        public void CloseTopLayer()
        {
            layerManager.CloseTopLayer();
        }

        public void SetIsActive(bool state) {}

        #endregion
    }
}