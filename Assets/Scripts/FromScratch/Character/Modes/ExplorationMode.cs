using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace FromScratch.Character.Modes
{
    public class ExplorationMode: CharacterMode
    {
        //Animator Hashes
        private int speedHash;
        private int isJumpingHash;
        private int isSprintingHash;

        // Gravity
        private float gravity = -9.8f;
        private float groundedGravity = -0.05f;
        private float fallSpeed = 0f;

        // Movement
        
        public const float MinimumMovement = 0.05f;
        [SerializeField, ShowInInspector, BoxGroup("Movement")]
        public float movementSpeed = 2f;
                
        [BoxGroup("Movement")]
        public bool useAcceleration = true;
        [BoxGroup("Movement")]
        public float acceleration = 4f;
        [BoxGroup("Movement")]
        public float deceleration = 2f;
        [BoxGroup("Movement")]
        public float sprintMultiplier = 2f;
        
        // Rotation
        public const float MinimumDeltaToRotate = 0.1f;

        [BoxGroup("Rotation")] public float angularSpeed = 540f;

        // Jumping
        private bool isJumping;
        private float initialJumpVelocity;

        [BoxGroup("Jumping")]
        public float maxJumpTime = 0.75f;
        [BoxGroup("Jumping")]
        public float maxJumpHeight = 2;
        [BoxGroup("Jumping")] public float terminalVelocity = -20f;

        public ExplorationMode()
        {
            SetupAnimHashes();
            setupJumpVariables();
        }

        public override MovementType MovementType => MovementType.OnFoot;

        private void SetupAnimHashes()
        {
            speedHash = Animator.StringToHash("speed");
            isJumpingHash = Animator.StringToHash("isJumping");
            isSprintingHash = Animator.StringToHash("isSprinting");        
        }

        private void setupJumpVariables()
        {
            float timeToApex = maxJumpTime / 2;
            gravity = (-2 * maxJumpHeight) / (timeToApex * timeToApex);
            initialJumpVelocity = (2 * maxJumpHeight) / timeToApex;
        }

        public void Update()
        {
            if (!character.AreControlsDisabled)
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

        private void OnDisable()
        {
            Debug.Log("Exploration.OnDisable");
            character.direction = Vector3.zero;
            character.intendedDirection = Vector3.zero;
            character.characterController.Move(Vector3.zero);
        }


        private void HandleJump()
        {
            if (!isJumping && character.characterController.isGrounded && character.wantsToJump && !character.AreControlsDisabled)
            {
                character.Animator.SetBool(isJumpingHash, true);
                isJumping = true;
                fallSpeed = initialJumpVelocity * 0.5f; // x0.5 from Verlet Integration with previous value of 0
            } else if (!character.wantsToJump && isJumping && character.characterController.isGrounded)
            {
                character.Animator.SetBool(isJumpingHash, false);
                isJumping = false;
            }
        }

        private void HandleAnimation()
        {
            Vector3 normalizedVelocity = character.characterController.velocity / movementSpeed;
            normalizedVelocity.y = 0;
            //Walking speed bound between 0 and 0.5. Sprinting bound between 0.5 and 1;
            float speed = Mathf.Min(0.5f, normalizedVelocity.magnitude) + (character.wantsToSprint ? 0.5f : 0f);
            
            character.Animator.SetFloat(speedHash, speed);
            character.Animator.SetBool(isSprintingHash, character.wantsToSprint);
        }

        private void HandleDirection()
        {
            var targetDirection = character.intendedDirection;
            character.direction = CalculateDirection(targetDirection, character.direction);
            // direction = targetDirection;
        }

        private void HandleRotation()
        {
            Vector3 targetDirection = character.direction;
            Quaternion targetRotation = CalculateRotation(targetDirection, character.transform);
            if (targetDirection.sqrMagnitude > MinimumDeltaToRotate)
            {
                character.characterController.transform.rotation = targetRotation;
            }
        }

        public void HandleGravity()
        {
            bool isFalling = fallSpeed <= 0 || !character.wantsToJump;
            float fallMultiplier = isFalling ? 2.0f : 1.0f;
            if (character.characterController.isGrounded)
            {
                character.Animator.SetBool(isJumpingHash, false);
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
            float sprintScaling = character.wantsToSprint ? sprintMultiplier : 1;
            float scaling = Time.deltaTime * movementSpeed * sprintScaling;
            Vector3 movement = new Vector3(character.direction.x, fallSpeed, character.direction.z);
            character.characterController.Move(movement * scaling);
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
            
            float accel = Mathf.Approximately(sqrMag, 0) ? deceleration : acceleration;
            Vector3 nextDirection = Vector3.SmoothDamp(
                currentDirection, target, ref character.directionVelocity, 1f / accel, accel
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