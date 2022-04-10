using System;
using System.Numerics;
using FromScratch.Character;
using FromScratch.Character.Modes;
using UnityEngine;
using UnityEngine.InputSystem;
using Plane = UnityEngine.Plane;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace FromScratch.Player
{
    public class PlayerInputHandler: MonoBehaviour
    {
        private FromScratchPlayer player;
        private PlayerInteraction playerInteraction;
        private Character.Character character;
        private Camera playerCamera;
        
        private FromScratchControls fromScratchControls;
        private InputAction interactAction;
        private InputAction cancelAction;
        private InputAction jumpAction;
        private InputAction moveAction;
        private InputAction sprintAction;
        
        private void Awake()
        {
            this.player = GetComponent<FromScratchPlayer>();
            this.playerInteraction = GetComponentInChildren<PlayerInteraction>();
            character = player.GetComponentInChildren<Character.Character>();

            fromScratchControls = new FromScratchControls();
            interactAction = fromScratchControls.Player.Interact;
            cancelAction = fromScratchControls.Player.Cancel;
            jumpAction = fromScratchControls.Player.Jump;
            moveAction = fromScratchControls.Player.Move;
            sprintAction = fromScratchControls.Player.Sprint;

            playerCamera = Camera.main;
        }
        
        private void OnEnable()
        {
            moveAction.Enable();
            
            interactAction.Enable();
            interactAction.performed += OnInteract;

            cancelAction.Enable();
            cancelAction.performed += OnCancel;
            
            jumpAction.Enable();
            jumpAction.started += OnJump;
            jumpAction.canceled += OnJump;
            
            sprintAction.Enable();
            sprintAction.started += OnSprint;
            sprintAction.canceled += OnSprint;
        }

        private void OnDisable()
        {
            moveAction.Disable();
            
            sprintAction.started -= OnSprint;
            sprintAction.canceled -= OnSprint;

            interactAction.Disable();
            interactAction.performed -= OnInteract;
            
            cancelAction.Disable();
            cancelAction.performed -= OnCancel;
            
            jumpAction.Disable();
            jumpAction.started -= OnJump;
            jumpAction.canceled -= OnJump;
        }

        private void OnSprint(InputAction.CallbackContext context)
        {
            character.wantsToSprint = context.ReadValueAsButton();
        }

        private void OnJump(InputAction.CallbackContext context)
        {
            character.wantsToJump = context.ReadValueAsButton();
        }
        
        private void OnInteract(InputAction.CallbackContext context)
        {
            MovementType movementType = character.modeManager.MovementType;
            CharacterMode activeMode = character.modeManager.GetActiveMode();

            if (movementType == MovementType.OnFoot || movementType == MovementType.Vehicle)
            {
                Debug.Log("PlayerInputHandler::OnInteract");
                playerInteraction.Activate();
            }
            
            if (movementType == MovementType.Overview)
            {
                PlacementMode placementMode = activeMode as PlacementMode;
                if (placementMode != null)
                {
                    placementMode.ConfirmItemPlacement();
                }
            }
        }
        
        private void OnCancel(InputAction.CallbackContext obj)
        {
            MovementType movementType = character.modeManager.MovementType;
            CharacterMode activeMode = character.modeManager.GetActiveMode();

            if (movementType == MovementType.Overview)
            {
                
                PlacementMode placementMode = activeMode as PlacementMode;
                if (placementMode != null)
                {
                    placementMode.CancelItemPlacement();
                }
            }
        }


        private void Update()
        {
            var camT = playerCamera.transform;

            MovementType movementType = character.modeManager.MovementType;
            CharacterMode activeMode = character.modeManager.GetActiveMode();
            
            if (movementType == MovementType.OnFoot || movementType == MovementType.Vehicle)
            {
                var inputDirection = moveAction.ReadValue<Vector2>();
            
                Vector3 intendedDirection = camT.forward * inputDirection.y + camT.right * inputDirection.x;
                intendedDirection.y = 0;
                intendedDirection.Normalize();
                intendedDirection *= inputDirection.magnitude;

                character.intendedDirection = intendedDirection;
            }

            if (movementType == MovementType.Menu)
            {
                //TODO: Pass Inputs to open menu to Parse
            }

            if (movementType == MovementType.Overview)
            {
                //In overview (Top Down), parse inputs to move camera around
                //Mouse movement should move the reticule
                Plane plane = new Plane(Vector3.up, 0);
                float distance;
                Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
                if (plane.Raycast(ray, out distance))
                {
                    Vector3 position = ray.GetPoint(distance);
                    PlacementMode placementMode = activeMode as PlacementMode;
                    if (placementMode != null)
                    {
                        placementMode.MoveReticule(position);
                    }
                }
            }
        }
    }
}