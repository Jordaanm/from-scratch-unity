using System;
using UnityEngine;

namespace FromScratch.Character
{
    public class CharacterStatus: MonoBehaviour
    {
        public const float StaminaDrainRate = 5;
        public const float StaminaRecoveryRate = 2;

        public float hungerCurrent;
        public int hungerMax;
        public float thirstCurrent;
        public int thirstMax;
        public float healthCurrent;
        public int healthMax;
        public float staminaCurrent;
        public int staminaMax;
        public bool isStaminaDraining;


        private void Update()
        {
            if (isStaminaDraining)
            {
                //For now, drain at a consistent rate
                staminaCurrent = Mathf.Max(0, staminaCurrent - StaminaDrainRate * Time.deltaTime);
            }
            else
            {
                staminaCurrent = Mathf.Min(staminaMax, staminaCurrent + StaminaRecoveryRate * Time.deltaTime);
            }
        }
    }
}