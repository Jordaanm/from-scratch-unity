using FromScratch.Character.Modes;
using FromScratch.Character.Status;
using UnityEngine;

namespace FromScratch.Interaction
{
    
    [Interaction(id = "sleep")]
    public class SleepInteraction: Interaction
    {
        public SleepInteraction()
        {
            _id = "SLEEP";
            _label = "Sleep";
        }

        public override bool CanInteractWith(IInteractable interactable)
        {
            var bed = interactable.GetGameObject().GetComponent<Bed>();
            return bed != null;
        }

        public override void Start(IInteractor interactor, IInteractable target)
        {
            GameObject interactorGO = interactor.GetGameObject();
            GameObject targetGO = target.GetGameObject();

            var bed = targetGO.GetComponent<Bed>();
            var character = interactorGO.GetComponent<Character.Character>();

            var sleepMode = character.modeManager.SetActiveMode<SleepMode>();
            sleepMode.SetBed(bed);
        }
    }
}