using System;
using FromScratch.Interaction;
using UI;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FromScratch.Player
{
    public class PlayerUI : MonoBehaviour
    {
        public FromScratchPlayer player;

        private PauseMenu pauseMenu;
        private @FromScratchControls fromScratchControls;
        private InputAction openMenuAction;
        private InputAction moveAction;

        private void Awake()
        {
            pauseMenu = new PauseMenu(player);
            fromScratchControls = new FromScratchControls();
        }

        private void Start()
        {
            pauseMenu.AddSubmenu(new EquipmentMenu(player));
            pauseMenu.AddSubmenu(new InventoryMenu(player));
            pauseMenu.AddSubmenu(new CraftingSubmenu(player));
        }

        private void Update()
        {
            // Interactable interactable = player.character.characterInteration.GetNearestInteractable(true);
            // if (interactable != null)
            // {
            //     InteractionMarker.Instance.Show(interactable);
            // }
            // else
            // {
            //     InteractionMarker.Instance.Hide();
            // }
        }

        private void OnEnable()
        {
            openMenuAction = fromScratchControls.Player.Pause;
            openMenuAction.Enable();
            openMenuAction.performed += OnTogglePauseMenu;

            // TODO: Create new action map for menus
            moveAction = fromScratchControls.Player.Move;
            moveAction.Enable();
            moveAction.started += OnMenuMove;
        }

        private void OnMenuMove(InputAction.CallbackContext context)
        {
            float direction = context.ReadValue<Vector2>().x;
            if (pauseMenu.IsOpen())
            {
                if (direction > 0.5)
                {
                    pauseMenu.NextMenu();
                } else if (direction < -0.5)
                {
                    pauseMenu.NextMenu(-1);
                }
            }
        }

        private void OnDisable()
        {
            openMenuAction.Disable();
            openMenuAction.performed -= OnTogglePauseMenu;
            
            moveAction.Disable();
        }

        private void OnTogglePauseMenu(InputAction.CallbackContext context)
        {
            Debug.Log("Toggling Pause Menu");
            if (FullscreenMenuHost.Instance.HasMenuOpen)
            {
                FullscreenMenuHost.Instance.CloseTopLayer();
            }
            else
            {
                FullscreenMenuHost.Instance.OpenMenu(pauseMenu);
            }
        }
    }
}
