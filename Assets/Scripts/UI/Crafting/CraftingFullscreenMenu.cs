using FromScratch.Crafting;
using FromScratch.Player;
using Unity.XR.OpenVR;
using UnityEngine.UIElements;

namespace UI.Crafting
{
    public class CraftingFullscreenMenu: FullscreenMenuHost.FullScreenMenu
    {
        private FromScratchPlayer player;
        protected VisualElement m_veRoot;
        private CraftingSubmenu craftingSubmenu;
        private readonly CraftingTable craftingTable;

        #region FullScreenMenu contract

        public CraftingFullscreenMenu(CraftingTable craftingTable)
        {
            this.craftingTable = craftingTable;
        }

        public CraftingFullscreenMenu SetPlayer(FromScratchPlayer player)
        {
            this.player = player;
            return this;
        }
        
        public VisualElement GetRoot()
        {
            if (m_veRoot == null) {
                BuildUI();
            }

            return m_veRoot;
        }

        public void OpenMenu()
        {
            player?.character.DisableControls();
            craftingSubmenu.OnOpen();
        }

        public void CloseMenu()
        {
            player?.character.EnableControls();
            craftingSubmenu.OnClose();
        }
        
        #endregion
        
        virtual protected void BuildUI()
        {
            m_veRoot = new VisualElement();
            m_veRoot.AddToClassList("full");
            craftingSubmenu = new CraftingSubmenu(player, craftingTable);
            
            m_veRoot.Add(craftingSubmenu.GetRoot());
        }

        public void OpenForPlayer(FromScratchPlayer player)
        {
            this.player = player;
            OpenMenu();
        }
    }
}