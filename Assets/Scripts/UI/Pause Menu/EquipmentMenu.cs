using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AssetReferences;
using FromScratch.Equipment;
using FromScratch.Player;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI
{
    public class EquipmentMenu: MultiScreenMenu.Submenu
    {
        public static string MenuName = "Equipment";
        public static string MenuID = "EQUIPMENT";
        
        private LayerManager layerManager;
        
        #region UI References        
        protected static VisualTreeAsset treeAsset;
        protected const string treeAssetKey = "equipment-menu";
        protected static VisualTreeAsset slotTreeAsset;
        protected const string slotTreeAssetKey = "equipment-menu-slot";
        
        private VisualElement veRoot = null;
        private IUserInterfaceLayer openContextMenu = null;
        #endregion
        
        private FromScratchPlayer player;

        private static Dictionary<EquipmentSlot, string > SlotClassMap = new Dictionary<EquipmentSlot, string>
        {
            { EquipmentSlot.Head, "head" },
            { EquipmentSlot.Chest, "chest" },
            { EquipmentSlot.Legs, "legs" },
            { EquipmentSlot.Feet, "feet" },
            { EquipmentSlot.Mainhand, "primary-hand" },
            { EquipmentSlot.Offhand, "secondary-hand" },
            { EquipmentSlot.Back, "back" },
        };

        private Dictionary<EquipmentSlot, VisualElement> SlotElements = new Dictionary<EquipmentSlot, VisualElement>();

        public EquipmentMenu(FromScratchPlayer player)
        {
            this.player = player;
            BuildUI();
        }
        
        
        #region BuildUI
        
        private void BuildUI()
        {
            
            if (treeAsset == null) {
                treeAsset = VisualTreeAssetReference.Instance.GetAsset(treeAssetKey);
                slotTreeAsset = VisualTreeAssetReference.Instance.GetAsset(slotTreeAssetKey);
            }

            veRoot = VisualTreeAssetReference.Create(treeAsset);
            layerManager = new LayerManager(veRoot);

            BuildSlots();
            BuildPreview();
        }

        private void BuildSlots()
        {
            var equipmentSlots = veRoot.Q("equipment-slots");
            foreach (var equipmentSlot in SlotClassMap.Keys)
            {
                string slotName = SlotClassMap[equipmentSlot];
                string selector = string.Format("slot--{0}", slotName);

                VisualElement slot = VisualTreeAssetReference.Create(slotTreeAsset);
                slot.AddToClassList(selector);
                equipmentSlots.Add(slot);
                SlotElements.Add(equipmentSlot, slot);
                slot.Q<Label>(null, "equipment-slot--label").text = equipmentSlot.ToString();
            }
        }

        private void BuildPreview()
        {
            var preview = veRoot.Q("player-preview");
            if (preview != null)
            {
                Background background = Background.FromRenderTexture(player.PreviewTexture);
                preview.style.backgroundImage = background;
            }
        }

        #endregion
        
        #region MultiScreenMenu.Submenu contract
        
        public VisualElement GetRoot()
        {            
            if (veRoot == null) {
                BuildUI();
            }

            return veRoot;
        }
        public string GetTitle()
        {            
            return MenuName;
        }

        public string GetID()
        {
            return MenuID;
        }
        
        public void OnOpen()
        {
            BuildPreview();
        }

        public void OnClose() {
            if (openContextMenu != null)
            {
                layerManager.RemoveLayer(openContextMenu);
            }
        }

        public void SetIsActive(bool state) {
            if (state) {
                // UpdateInventory();
                // UpdateHotbar();
            }
        }
        #endregion
       
    }
}