using System;
using System.Numerics;
using UnityEngine;
using UnityEngine.InputSystem;
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
            Debug.Log("PlayerInputHandler::OnInteract");
            playerInteraction.Activate();
        }

        private void Update()
        {
            var inputDirection = moveAction.ReadValue<Vector2>();
            var camT = playerCamera.transform;

            Vector3 intendedDirection = camT.forward * inputDirection.y + camT.right * inputDirection.x;
            intendedDirection.y = 0;
            intendedDirection.Normalize();
            intendedDirection *= inputDirection.magnitude;

            character.intendedDirection = intendedDirection;
        }
    }
}