using FromScratch.Character;
using FromScratch.Inventory;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FromScratch.Player
{
    public class PlayerDebug : MonoBehaviour
    {
        [HideInInspector]
        public FromScratchPlayer player;
        public ItemData itemData;
        public CharacterEquipment characterEquipment;

        private @FromScratchControls fromScratchControls;
        private InputAction debugAction;
        private void Awake()
        {
            player = GetComponent<FromScratchPlayer>();
            fromScratchControls = new FromScratchControls();
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

            if (characterEquipment != null && itemData != null)
            {
                Item item = new Item(itemData);
                player.character.characterInventory.Container.AddItem(item);
            }
        }
    }
}
