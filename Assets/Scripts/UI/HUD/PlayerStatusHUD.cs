using AssetReferences;
using FromScratch.Character;
using UnityEngine.UIElements;

namespace UI.HUD
{
    public class PlayerStatusHUD: HudElement
    {
        private CharacterStatus characterStatus;
        private string visualTreeAssetKey = "player-status";
        private VisualElement healthBar;
        private VisualElement hungerBar;
        private VisualElement thirstBar;

        public PlayerStatusHUD()
        {
            root = VisualTreeAssetReference.Create(visualTreeAssetKey);

            healthBar = root.Q("health").Q("bar");
            hungerBar = root.Q("hunger").Q("bar");
            thirstBar = root.Q("thirst").Q("bar");
        }

        public override void Init(MainHUD mainHUD)
        {
            base.Init(mainHUD);
            characterStatus = mainHUD.player.character.characterStatus;
        }

        public override void Update()
        {
            var healthPc = Percent(characterStatus.healthCurrent, characterStatus.healthMax);
            var hungerPc = Percent(characterStatus.hungerCurrent, characterStatus.hungerMax);
            var thirstPc = Percent(characterStatus.thirstCurrent, characterStatus.thirstMax);

            healthBar.style.width = new Length(healthPc, LengthUnit.Percent);
            hungerBar.style.width = new Length(hungerPc, LengthUnit.Percent);
            thirstBar.style.width = new Length(thirstPc, LengthUnit.Percent);
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