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

        public override bool CanInteractWith(Interactable interactable)
        {
            var bed = interactable.gameObject.GetComponent<Bed>();
            return bed != null;
        }

        public override void Start(IInteractor interactor, Interactable target)
        {
            GameObject interactorGO = interactor.GetGameObject();
            GameObject targetGO = target.gameObject;

            var bed = targetGO.GetComponent<Bed>();
            var character = interactorGO.GetComponent<Character.Character>();

            var sleepMode = character.modeManager.SetActiveMode<SleepMode>();
            sleepMode.SetBed(bed);
        }
    }
}