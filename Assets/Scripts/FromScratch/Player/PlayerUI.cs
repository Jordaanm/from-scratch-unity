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

        private void Awake()
        {
            pauseMenu = new PauseMenu();
            fromScratchControls = new FromScratchControls();
        }

        private void Start()
        {
            pauseMenu.AddSubmenu(new InventoryMenu(player));
        }

        private void OnEnable()
        {
            openMenuAction = fromScratchControls.Player.Pause;
            openMenuAction.Enable();
            openMenuAction.performed += OnTogglePauseMenu;
        }

        private void OnDisable()
        {
            openMenuAction.Disable();
            openMenuAction.performed -= OnTogglePauseMenu;
        }

        private void OnTogglePauseMenu(InputAction.CallbackContext context)
        {
            Debug.Log("Toggling Pause Menu");
            if (!pauseMenu.IsOpen())
            {
                FullscreenMenuHost.Instance.OpenMenu(pauseMenu);
            }
            else
            {
                FullscreenMenuHost.Instance.CloseCurrentMenu();
            }
        }
    }
}
