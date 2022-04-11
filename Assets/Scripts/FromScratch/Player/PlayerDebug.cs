using FromScratch.Character;
using FromScratch.Character.Modes;
using FromScratch.Inventory;
using UI.HUD;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FromScratch.Player
{
    public class PlayerDebug : MonoBehaviour
    {
        [HideInInspector]
        public FromScratchPlayer player;
        public ItemData itemData;
        private CharacterEquipment characterEquipment;
        private CharacterModeManager modeManager; 
        
        private @FromScratchControls fromScratchControls;
        private InputAction debugAction;
        private void Awake()
        {
            player = GetComponent<FromScratchPlayer>();
            fromScratchControls = new FromScratchControls();

            characterEquipment = player.GetComponentInChildren<CharacterEquipment>();
            modeManager = player.GetComponentInChildren<CharacterModeManager>();
        }

        private void OnEnable()
        {
            debugAction = fromScratchControls.Player.Debug;
            debugAction.Enable();
            debugAction.performed += OnDebug;
        }

        private void OnDisable()
        {
            debugAction.Disable();
            debugAction.performed -= OnDebug;
        }

        private void OnDebug(InputAction.CallbackContext obj)
        {
            Debug.Log("Debug");
            MainHUD.Instance.GetHudElement<CheatConsoleHUD>().Toggle();
            //
            // if (modeManager != null)
            // {
            //     modeManager.SetActiveMode<SleepMode>();
            // }
            // if (characterEquipment != null && itemData != null)
            // {
            //     Item item = new Item(itemData);
            //     player.character.characterInventory.Container.AddItem(item);
            // }
        }
    }
}
