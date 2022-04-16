using FromScratch.Character.Modes;
using FromScratch.Interaction;

namespace FromScratch.Exploration.Climbing
{
    [Interaction(id = "startclimb")]
    public class ClimbingInteraction: Interaction.Interaction
    {
        public ClimbingInteraction()
        {
            _id = "START_CLIMB";
            _label = "Climb";
        }

        public override bool CanInteractWith(Interactable interactable)
        {
            return interactable.GetType() == typeof(ClimbingHold);
        }

        public override void Start(IInteractor interactor, Interactable target)
        {
            var character = interactor.GetGameObject()?.GetComponent<Character.Character>();
            var climbingHold =  target.GetComponent<ClimbingHold>();
            if (character != null && climbingHold != null)
            {
                var climbingMode = character.modeManager.SetActiveMode<ClimbingMode>();
                climbingMode.StartingHold(climbingHold);
            }
        }
    }
}