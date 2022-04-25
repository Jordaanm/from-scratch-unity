using System.Collections;
using AssetReferences;
using FromScratch.Character;
using UnityEngine;
using UnityEngine.UIElements;
using Util;

namespace UI.HUD
{
    public class PlayerStatusHUD: HudElement
    {
        private enum FadeState
        {
            FadeIn,
            FadeOut,
            Shown,
            Hidden
        }
        
        private CharacterStatus characterStatus;
        private string visualTreeAssetKey = "player-status";
        private VisualElement healthBar;
        private VisualElement hungerBar;
        private VisualElement thirstBar;
        
        private VisualElement staminaMeter;
        private VisualElement staminaBar;
        private Coroutine staminaFadeCoroutine;
        private FadeState staminaFadeState = FadeState.Shown; 
        
        
        public PlayerStatusHUD()
        {
            root = VisualTreeAssetReference.Create(visualTreeAssetKey);

            healthBar = root.Q("health").Q("bar");
            hungerBar = root.Q("hunger").Q("bar");
            thirstBar = root.Q("thirst").Q("bar");
            
            staminaMeter = root.Q("stamina");
            staminaBar = staminaMeter.Q("bar");
        }
        
        public override void Update()
        {
            if (characterStatus == null)
            {
                characterStatus = MainHUD.Instance.Player.character.characterStatus;
            }
            var healthPc = Percent(characterStatus.healthCurrent, characterStatus.healthMax);
            var hungerPc = Percent(characterStatus.hungerCurrent, characterStatus.hungerMax);
            var thirstPc = Percent(characterStatus.thirstCurrent, characterStatus.thirstMax);
            var staminaPc = Percent(characterStatus.staminaCurrent, characterStatus.staminaMax);
            
            healthBar.style.width = new Length(healthPc, LengthUnit.Percent);
            hungerBar.style.width = new Length(hungerPc, LengthUnit.Percent);
            thirstBar.style.width = new Length(thirstPc, LengthUnit.Percent);
            staminaBar.style.width = new Length(staminaPc, LengthUnit.Percent);

            HandleStaminaBar();
        }

        private void HandleStaminaBar()
        {
            if (characterStatus.isStaminaDraining && staminaFadeState != FadeState.Shown && staminaFadeState != FadeState.FadeIn)
            {
                if(staminaFadeCoroutine != null) { CoroutinesManager.Instance.StopCoroutine(staminaFadeCoroutine);}
                staminaFadeCoroutine = CoroutinesManager.Instance.StartCoroutine(FadeStaminaMeter(true));
            }

            if (!characterStatus.isStaminaDraining && staminaFadeState != FadeState.FadeOut &&
                staminaFadeState != FadeState.Hidden)
            {
                if(staminaFadeCoroutine != null) { CoroutinesManager.Instance.StopCoroutine(staminaFadeCoroutine); }

                staminaFadeCoroutine = CoroutinesManager.Instance.StartCoroutine(FadeStaminaMeter(false));
            }
        }

        private IEnumerator FadeStaminaMeter(bool fadeIn)
        {
            staminaFadeState = fadeIn ? FadeState.FadeIn : FadeState.FadeOut;
            float rate = 4f; // 0.25s to fade in from fully hidden

            if (fadeIn)
            {
                while (staminaMeter.style.opacity.value < 1.0f)
                {
                    staminaMeter.style.opacity =
                        new StyleFloat(Mathf.Max(staminaMeter.style.opacity.value + Time.deltaTime * rate, 1.0f));
                    yield return null;
                }
            }
            else
            {
                yield return new WaitForSeconds(2.5f);
                while (staminaMeter.style.opacity.value > 0.0f)
                {
                    staminaMeter.style.opacity =
                        new StyleFloat(Mathf.Min(staminaMeter.style.opacity.value + Time.deltaTime * rate, 0));
                    yield return null;
                }
            }

            staminaFadeState = fadeIn ? FadeState.Shown : FadeState.Hidden;
            staminaFadeCoroutine = null;
        }


        private float  Percent(float numerator, int denominator)
        {
            if (denominator == 0)
            {
                return 100f;
            }

            return (numerator / denominator) * 100;
        }
    }
}