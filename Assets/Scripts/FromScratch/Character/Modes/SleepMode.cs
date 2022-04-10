using System;
using UnityEngine;

namespace FromScratch.Character.Modes
{
    public class SleepMode: CharacterMode
    {
        public RuntimeAnimatorController animatorController;
        public AnimationClip clip;
        
        public override MovementType MovementType => MovementType.Stationary;

        private void OnEnable()
        {
            if (animatorController == null)
            {
                character.characterAnimation.SetState(clip, null, transition:0.5f);
            }
            else
            {
                character.characterAnimation.SetState(animatorController, null, 1.0f, 0.0f, 1.0f, 0, true, null);
            }
        }

        private void OnDisable()
        {
            character.characterAnimation.ResetState(0, 0);
        }
    }
}