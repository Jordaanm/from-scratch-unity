using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace FromScratch.Character
{
    public class Character : MonoBehaviour
    {
        private @FromScratchControls fromScratchControls;
        private Animator animator;
        
        [BoxGroup("Character"), HideInInspector]
        public CharacterAnimation characterAnimation;
        [BoxGroup("Character")]
        public CharacterController characterController;
        [BoxGroup("Character"), HideInInspector]
        public CharacterInventory characterInventory;
        [BoxGroup("Character"), HideInInspector]
        public CharacterCrafting characterCrafting;
        [BoxGroup("Character"), HideInInspector]
        public CharacterEquipment characterEquipment;

        private bool areControlsDisabled;
        
        // Gravity
        private float gravity = -9.8f;
        private float groundedGravity = -0.05f;
        private float fallSpeed = 0f;

        // Jumping
        private bool isJumping;
        private float initialJumpVelocity;

        [HideInInspector]
        public bool wantsToJump;
        [BoxGroup("Jumping")]
        public float maxJumpTime = 0.75f;
        [BoxGroup("Jumping")]
        public float maxJumpHeight = 2;
        [BoxGroup("Jumping")] public float terminalVelocity = -20f; 
        
        // Movement
        [SerializeField, ShowInInspector, BoxGroup("Movement")]
        private float movementSpeed = 2f;
        private const float MinimumMovement = 0.05f;
        private Vector3 direction = Vector3.zero;
        private Vector3 directionVelocity = Vector3.zero;
        [BoxGroup("Movement"), HideInInspector] public Vector3 intendedDirection;
        [BoxGroup("Movement"), HideInInspector] public bool wantsToSprint; 
        
        [BoxGroup("Movement")]
        public bool useAcceleration = true;
        [BoxGroup("Movement")]
        public float acceleration = 4f;
        [BoxGroup("Movement")]
        public float deceleration = 2f;
        [BoxGroup("Movement")]
        public float sprintMultiplier = 2f;

        // Rotation
        private const float MinimumDeltaToRotate = 0.1f;
        
        [BoxGroup("Rotation")]
        public float angularSpeed = 540f;
    
        // Animation
        private int speedHash;
        private int isJumpingHash;
        private int isSprintingHash;
        
        public Animator Animator => animator;
        
        private void Awake()
        {
            fromScratchControls = new FromScratchControls();
            characterAnimation = GetComponent<CharacterAnimation>();
            characterInventory = GetComponent<CharacterInventory>();
            characterEquipment = GetComponent<CharacterEquipment>();
            characterCrafting = GetComponent<CharacterCrafting>();
            
            areControlsDisabled = false;
            setupJumpVariables();
        }

        private void setupJumpVariables()
        {
            float timeToApex = maxJumpTime / 2;
            gravity = (-2 * maxJumpHeight) / (timeToApex * timeToApex);
            initialJumpVelocity = (2 * maxJumpHeight) / timeToApex;
        }
    
        private void Start()
        {
            animator = GetComponent<Animator>();
            characterAnimation.Setup(animator);
            speedHash = Animator.StringToHash("speed");
            isJumpingHash = Animator.StringToHash("isJumping");
            isSprintingHash = Animator.StringToHash("isSprinting");
        }

        void Update()
        {
            if (!areControlsDisabled)
            {
                HandleDirection();
                HandleRotation();
            }
            //Handle Gravity after movement so that Gravity has an accurate reading for characterController.isGrounded
            HandleMovement();
            HandleGravity();
            HandleJump();
            HandleAnimation();
        }

        public void DisableControls()
        {
            direction = Vector3.zero;
            areControlsDisabled = true;
        }

        public void EnableControls()
        {
            areControlsDisabled = false;
        }

        private void HandleJump()
        {
            if (!isJumping && characterController.isGrounded && wantsToJump && !areControlsDisabled)
            {
                animator.SetBool(isJumpingHash, true);
                isJumping = true;
                fallSpeed = initialJumpVelocity * 0.5f; // x0.5 from Verlet Integration with previous value of 0
            } else if (!wantsToJump && isJumping && characterController.isGrounded)
            {
                animator.SetBool(isJumpingHash, false);
                isJumping = false;
            }
        }

        private void HandleAnimation()
        {
            Vector3 normalizedVelocity = characterController.velocity / this.movementSpeed;
            normalizedVelocity.y = 0;
            //Walking speed bound between 0 and 0.5. Sprinting bound between 0.5 and 1;
            float speed = Mathf.Min(0.5f, normalizedVelocity.magnitude) + (wantsToSprint ? 0.5f : 0f);
            
            animator.SetFloat(speedHash, speed);
            animator.SetBool(isSprintingHash, wantsToSprint);
        }

        private void HandleDirection()
        {
            var targetDirection = intendedDirection;
            direction = CalculateDirection(targetDirection, this.direction);
            // direction = targetDirection;
        }

        private void HandleRotation()
        {
            Vector3 targetDirection = this.direction;
            Quaternion targetRotation = CalculateRotation(targetDirection, transform);
            if (targetDirection.sqrMagnitude > MinimumDeltaToRotate)
            {
                characterController.transform.rotation = targetRotation;
            }
        }

        public void HandleGravity()
        {
            bool isFalling = fallSpeed <= 0 || !wantsToJump;
            float fallMultiplier = isFalling ? 2.0f : 1.0f;
            if (characterController.isGrounded)
            {
                animator.SetBool(isJumpingHash, false);
                fallSpeed = groundedGravity;
            }
            else
            {
                float previousFallSpeed = fallSpeed;
                float newFallSpeed = fallSpeed + (gravity * fallMultiplier * Time.deltaTime);
                float nextFallSpeed = (previousFallSpeed + newFallSpeed) * 0.5f; // Verlet Integration
                nextFallSpeed = Mathf.Max(nextFallSpeed, terminalVelocity);
                fallSpeed = nextFallSpeed;
            }
        }

        private void HandleMovement()
        {
            float sprintScaling = wantsToSprint ? sprintMultiplier : 1;
            float scaling = Time.deltaTime * movementSpeed * sprintScaling;
            Vector3 movement = new Vector3(direction.x, fallSpeed, direction.z);
            characterController.Move(movement * scaling);
        }
        private Vector3 CalculateDirection(Vector3 target, Vector3 currentDirection)
        {
            if (float.IsNaN(target.x) || float.IsNaN(target.y) || float.IsNaN(target.z))
            {
                return Vector3.zero;
            }

            if (!useAcceleration)
            {
                return target;
            }

            float sqrMag = target.sqrMagnitude;
            
            float accel = Mathf.Approximately(sqrMag, 0) ? this.deceleration : this.acceleration;
            Vector3 nextDirection = Vector3.SmoothDamp(
                currentDirection, target, ref this.directionVelocity, 1f / accel, accel
            );

            if (Mathf.Abs(sqrMag) < MinimumMovement && Mathf.Abs(nextDirection.sqrMagnitude) < MinimumMovement)
            {
                return Vector3.zero;
            }
            else
            {
                return nextDirection;
            }
        }

        private Quaternion CalculateRotation(Vector3 target, Transform characterTransform)
        {
            if (target == Vector3.zero)
            {
                return characterTransform.rotation;
            }

            // Face movement direction
            Quaternion srcRotation = characterTransform.rotation;
            Quaternion destRotation = Quaternion.LookRotation(target, Vector3.up);

            return Quaternion.RotateTowards(
                srcRotation,
                destRotation,
                Time.deltaTime * angularSpeed);
        }
    }
}