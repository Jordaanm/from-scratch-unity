using System.Collections.Generic;
using FromScratch.Interaction;
using UnityEngine;

namespace FromScratch.Player
{
    public class PlayerInteraction: MonoBehaviour, IInteractor
    {
        private Interactor interactor;
        private GatherActionSource gatherActionSource;
        private PickupItemActionSource pickupItemActionSource;
        
        private Character.Character character;
        private void Awake()
        {
            character = GetComponent<Character.Character>();
            interactor = new Interactor();
            gatherActionSource = new GatherActionSource();
            pickupItemActionSource = new PickupItemActionSource();
            interactor.AddInteractionSource(gatherActionSource);
            interactor.AddInteractionSource(pickupItemActionSource);
            interactor.AddInteractionSource(character.characterEquipment);
        }

        public GameObject GetGameObject()
        {
            return character == null ? gameObject : character.gameObject;
        }

        public void Activate()
        {
            Debug.Log("PlayerInteraction::Activate");
            IInteractable target = DetermineTarget();
            Debug.LogFormat("Found Target: {0}", target == null ? "Null" : target.GetGameObject().name);
            List<FromScratch.Interaction.Interaction> actions = interactor.GetActionsForTarget(target);
            if (actions == null || actions.Count == 0)
            {
                return;
            }
            Debug.Log("Action Found");
            var action = actions[0];
            action.Start(this, target);
        }

        private IInteractable DetermineTarget()
        {
            return interactor.FindClosestTo(transform);
        }
        
        public void OnTriggerEnter(Collider other)
        {
            Debug.LogFormat("Interactor::TriggerEntered {0}", other.name);
            interactor.AddInteractable(other.gameObject);
        }

        public void OnTriggerExit(Collider other)
        {
            Debug.LogFormat("Interactor::TriggerExited {0}", other.name);
            interactor.RemoveInteractable(other.gameObject);
        }
    }
}