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
        private CharacterInteraction characterInteraction;
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
            this.characterInteraction = GetComponentInChildren<CharacterInteraction>();
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
            interactAction.canceled += OnInteractCanceled;

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
            interactAction.canceled -= OnInteractCanceled;
            
            cancelAction.Disable();
            cancelAction.performed -= OnCancel;
            
            jumpAction.Disable();
            jumpAction.started -= OnJump;
            jumpAction.canceled -= OnJump;
        }

        private void OnSprint(InputAction.CallbackContext context)
        {
            if (context.ReadValueAsButton())
            {
                character.StartSprint();
            }
            else
            {
                character.StopSprint();
            }
        }

        private void OnJump(InputAction.CallbackContext context)
        {
            character.wantsToJump = context.ReadValueAsButton();
        }
        
        private void OnInteract(InputAction.CallbackContext context)
        {
            if (characterInteraction == null)
            {
                characterInteraction = GetComponentInChildren<CharacterInteraction>();
            }
            
            MovementType movementType = character.modeManager.MovementType;
            CharacterMode activeMode = character.modeManager.GetActiveMode();

            if (movementType == MovementType.OnFoot || movementType == MovementType.Vehicle)
            {
                if (characterInteraction != null)
                {
                    Debug.Log("PlayerInputHandler::OnInteract");
                    characterInteraction.StartActivation();
                }
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
        private void OnInteractCanceled(InputAction.CallbackContext obj)
        {
            MovementType movementType = character.modeManager.MovementType;
            if (movementType == MovementType.OnFoot || movementType == MovementType.Vehicle)
            {
                characterInteraction.CancelActivation();
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

            if (movementType == MovementType.Climbing)
            {
                character.modeManager.SetActiveMode<ExplorationMode>();
            }
        }


        private void Update()
        {
            if (character == null)
            {
                if (player.character == null)
                {
                    return;
                }
                character = player.character;
            }
            
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

            if (movementType == MovementType.Climbing)
            {
                var inputDirection = moveAction.ReadValue<Vector2>();
            
                Vector3 intendedDirection = camT.up * inputDirection.y + camT.right * inputDirection.x;
                intendedDirection.Normalize();

                character.intendedDirection = intendedDirection;
            }
        }
    }
}