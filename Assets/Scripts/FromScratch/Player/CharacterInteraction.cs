using System;
using System.Collections.Generic;
using System.Linq;
using FromScratch.Interaction;
using UnityEngine;

namespace FromScratch.Player
{
    public class CharacterInteraction: MonoBehaviour, IInteractor
    {
        public const float InteractionRadius = 20f;
        private Interactor interactor;
        // private GatherActionSource gatherActionSource;
        // private PickupItemActionSource pickupItemActionSource;
        private BasicActionSource basicActionSource;
        
        private Character.Character character;
        private void Awake()
        {
            character = GetComponent<Character.Character>();
            interactor = new Interactor();
            // gatherActionSource = new GatherActionSource();
            // pickupItemActionSource = new PickupItemActionSource();
            basicActionSource = new BasicActionSource();
            interactor.AddInteractionSource(basicActionSource);

            // interactor.AddInteractionSource(gatherActionSource);
            // interactor.AddInteractionSource(pickupItemActionSource);
            
            interactor.AddInteractionSource(character.characterEquipment);
            interactor.AddInteractionSource(character.characterCrafting);
        }

        public GameObject GetGameObject()
        {
            return character == null ? gameObject : character.gameObject;
        }

        public void RemoveInteractable(IInteractable interactable)
        {
            interactor.RemoveInteractable(interactable);
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

        private void Update()
        {
            Collider[] hitColliders = new Collider[10];
            int numColliders = Physics.OverlapSphereNonAlloc(character.transform.position, InteractionRadius, hitColliders,
                Physics.AllLayers, QueryTriggerInteraction.Collide);

            List<IInteractable> foundInteractables = new List<IInteractable>();
            for (int i = 0; i < numColliders; ++i)
            {
                IInteractable interactable = hitColliders[i].gameObject.GetComponent<IInteractable>();
                if (interactable != null)
                {
                    foundInteractables.Add(interactable);
                }
            }

            if (foundInteractables.Count > 0)
            {
                interactor.SetInteractables(foundInteractables);
            }
        }

        private IInteractable DetermineTarget()
        {
            return interactor.FindClosestTo(transform);
        }

        public IInteractable GetNearestInteractable()
        {
            var charPos = character.transform.position;
            return interactor.AllNearby()
                .OrderBy(x => Vector3.Distance(x.GetGameObject().transform.position, charPos))
                .ToList()[0];
        }
        
        
    }
}