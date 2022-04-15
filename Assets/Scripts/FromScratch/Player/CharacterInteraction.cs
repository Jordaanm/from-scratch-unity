using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using FromScratch.Interaction;
using UnityEngine;

namespace FromScratch.Player
{
    public class CharacterInteraction: MonoBehaviour, IInteractor
    {
        public const float InteractionRadius = 20f;
        private Interactor interactor;
        private BasicActionSource basicActionSource;
        
        private Character.Character character;
        
        public static float ActivationHoldTime = 0.5f;
        private Coroutine activationCoroutine;
        public float activationDuration = 0;
        public string activeLabel;

        private void Awake()
        {
            character = GetComponent<Character.Character>();
            interactor = new Interactor();
            basicActionSource = new BasicActionSource();
            
        }

        private void Start()
        {
            interactor.AddInteractionSource(basicActionSource);
            interactor.AddInteractionSource(character.characterEquipment);
            interactor.AddInteractionSource(character.characterCrafting);
        }

        public GameObject GetGameObject()
        {
            return character == null ? gameObject : character.gameObject;
        }

        public void StartActivation()
        {
            Debug.Log("Start Activation");
            Interactable target = DetermineTarget();
            Debug.LogFormat("Found Target: {0}", target == null ? "Null" : target.gameObject.name);
            List<FromScratch.Interaction.Interaction> actions = interactor.GetActionsForTarget(target);

            if (actions == null || actions.Count == 0)
            {
                return;
            }
            Debug.Log("Action Found");
            var action = actions[0];

            activeLabel = action.Label;
            activationCoroutine = StartCoroutine(ActivateAfterTimer(action, target));
        }

        private IEnumerator ActivateAfterTimer(Interaction.Interaction action, Interactable target)
        {
            activationDuration = 0;
            yield return new WaitForSeconds(ActivationHoldTime);
            Debug.Log("Timer completed");
            action.Start(this, target);
            activationCoroutine = null;
            activationDuration = 0;
            activeLabel = null;
        }
        
        public void CancelActivation()
        {
            Debug.Log("Cancel Activation");
            if (activationCoroutine == null)
            {
                return;
            }
            StopCoroutine(activationCoroutine);
            activationCoroutine = null;
            activeLabel = null;
        }


        private void Update()
        {
            UpdateInteractables();
            UpdateActivationTimer();
        }

        private void UpdateActivationTimer()
        {
            if (activationCoroutine == null)
            {
                return;
            }

            activationDuration += Time.deltaTime;
        }


        private void UpdateInteractables()
        {
            Collider[] hitColliders = new Collider[10];
            int numColliders = Physics.OverlapSphereNonAlloc(character.transform.position, InteractionRadius, hitColliders,
                Physics.AllLayers, QueryTriggerInteraction.Collide);

            List<Interactable> foundInteractables = new List<Interactable>();
            for (int i = 0; i < numColliders; ++i)
            {
                Interactable interactable = hitColliders[i].gameObject.GetComponent<Interactable>();
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

        private Interactable DetermineTarget()
        {
            return GetNearestInteractable(true);
        }

        public Interactable GetNearestInteractable(bool canInteractWith = false)
        {
            var charPos = character.transform.position;
            var allInteractables = interactor?.AllNearby();
            
            if (allInteractables == null || allInteractables.Count == 0)
            {
                return null;
            }

            if (canInteractWith)
            {
                allInteractables = allInteractables.FindAll(x => interactor.GetActionsForTarget(x).Count > 0);
            }

            return allInteractables
                .OrderBy(x => Vector3.Distance(x.transform.position, charPos))
                .ToList()[0];
        }

        public Interaction.Interaction GetDefaultAction(Interactable target)
        {
            if (target == null)
            {
                return null; 
            }
            var actions = interactor.GetActionsForTarget(target);
            if (actions.Count == 0)
            {
                return null;
            }

            return actions[0];
        }
    }
}