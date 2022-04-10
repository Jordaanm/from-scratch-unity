using System;
using System.Collections;
using FromScratch.Character.Status;
using UnityEngine;

namespace FromScratch.Character.Modes
{
    public class SleepMode: CharacterMode
    {
        public RuntimeAnimatorController animatorController;
        public AnimationClip clip;

        private Bed bed;
        
        public override MovementType MovementType => MovementType.Stationary;

        public void SetBed(Bed bed)
        {
            this.bed = bed;
        }
        
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

            StartCoroutine(TempRestore());
        }

        private IEnumerator TempRestore()
        {
            yield return new WaitForSeconds(5);
            character.modeManager.RestorePreviousMode();
        }
        
        private void OnDisable()
        {
            character.characterAnimation.ResetState(0.5f, 0);
            bed = null;
        }
    }
}