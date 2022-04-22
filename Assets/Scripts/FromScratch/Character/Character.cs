using System;
using System.Reflection;
using FromScratch.Inventory;
using FromScratch.Player;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace FromScratch.Character
{
    public class Character : MonoBehaviour
    {
        private Animator animator;

        [BoxGroup("Character"), HideInInspector]
        public CharacterAnimation characterAnimation;

        [BoxGroup("Character")] public CharacterController characterController;

        [BoxGroup("Character"), HideInInspector]
        public CharacterInventory characterInventory;

        [BoxGroup("Character"), HideInInspector]
        public CharacterCrafting characterCrafting;

        [BoxGroup("Character"), HideInInspector]
        public CharacterEquipment characterEquipment;

        [BoxGroup("Character"), HideInInspector]
        public CharacterModeManager modeManager;

        [BoxGroup("Character"), HideInInspector]
        public CharacterInteraction characterInteraction;

        private bool areControlsDisabled;
        
        
        //Shared Movement Fields
        [HideInInspector] public Vector3 direction = Vector3.zero;
        [HideInInspector] public Vector3 directionVelocity = Vector3.zero;
        [HideInInspector] public Vector3 intendedDirection;
        [HideInInspector] public bool wantsToSprint;
        [HideInInspector] public bool wantsToJump;
        [HideInInspector] public CharacterStatus characterStatus;

        public Animator Animator => animator;
        public bool AreControlsDisabled => areControlsDisabled;

        private void Awake()
        {
            characterAnimation = GetComponent<CharacterAnimation>();
            characterInventory = GetComponent<CharacterInventory>();
            characterEquipment = GetComponent<CharacterEquipment>();
            characterCrafting = GetComponent<CharacterCrafting>();
            modeManager = GetComponent<CharacterModeManager>();
            characterStatus = GetComponent<CharacterStatus>();
            characterInteraction = GetComponent<CharacterInteraction>();
            animator = GetComponent<Animator>();
            
            areControlsDisabled = false;
        }

        private void Start()
        {
            characterAnimation.Setup(this, animator);
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

        public void Consume(ItemData item)
        {
            var consumptionData = item.consumptionData;
            if (item.usageType != ItemData.UsageType.Consume || consumptionData == null)
            {
                return; 
            }

            MethodInfo effect = ConsumptionEffects.Get(consumptionData.consumptionEffect);
            effect?.Invoke(null, new object[]
            {
                this,
                item
            });
        }

        public void StartSprint()
        {
            wantsToSprint = true;
            characterStatus.isStaminaDraining = true;
        }

        public void StopSprint()
        {
            wantsToSprint = false;
            characterStatus.isStaminaDraining = false;
        }
    }
}