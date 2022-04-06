using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace FromScratch.Interaction
{
    public enum InteractionType
    {
        Other,
        ResourceNode,
        Pickup,
        CraftingStation
    }

    public interface IInteractor
    {
        GameObject GetGameObject();
        void RemoveInteractable(IInteractable interactable);
    }

    public interface IInteractable
    {
        InteractionType GetInteractionType();
        GameObject GetGameObject();
    }

    public class Interactor
    {
        public UnityEvent OnChange = new UnityEvent();
        private List<IInteractable> nearbyInteractables = new List<IInteractable>();

        private HashSet<IInteractionSource> localInteractionSources;

        public Interactor()
        {
            localInteractionSources = new HashSet<IInteractionSource>();
        }

        public void AddInteractionSource(IInteractionSource interactionSource)
        {
            localInteractionSources.Add(interactionSource);
        }
        
        // [SerializeField] public IInteractionReceiver interactionReceiver;

        // public void Activate()
        // {
        //     IInteractable closest = FindClosest();
        //
        //     if (closest != null)
        //     {
        //         if (interactionReceiver != null)
        //         {
        //             Debug.Log("Interactor::Activate");
        //             interactionReceiver.InteractWith(closest);
        //         }
        //     }
        // }

        public List<Interaction> GetActionsForTarget(IInteractable target)
        {
            return AllInteractionSources()
                .SelectMany(actionSource =>
                    actionSource == null
                        ? new List<Interaction>()
                        : actionSource.GetActionsForTarget(target))
                .ToList()
                .FindAll(x => x.CanInteractWith(target));
        }

        List<IInteractionSource> AllInteractionSources()
        {
            return localInteractionSources.ToList();
        }

        public IInteractable FindClosestTo(Transform transform)
        {
            float closestDist = 9999999f;
            IInteractable closest = null;
            Vector3 pos = transform.position;

            foreach (IInteractable interactable in nearbyInteractables)
            {
                float dist = (pos - interactable.GetGameObject().transform.position).sqrMagnitude;
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closest = interactable;
                }
            }

            return closest;
        }    
        
        public List<IInteractable> AllNearby() {
            return nearbyInteractables;
        }

        public void AddInteractable(GameObject gameObject)
        {
            IInteractable interactable = gameObject.GetComponent<IInteractable>();

            if (interactable != null)
            {
                nearbyInteractables.Add(interactable);
                OnChange.Invoke();
            }
        }

        public void RemoveInteractable(IInteractable interactable)
        {
            if (interactable != null)
            {
                nearbyInteractables.Remove(interactable);
                OnChange.Invoke();
                Debug.LogFormat("{0} Interactables remaining nearby", nearbyInteractables.Count.ToString());
            }
        }

        public void RemoveInteractable(GameObject gameObject)
        {
            IInteractable interactable = gameObject.GetComponent<IInteractable>();
            RemoveInteractable(interactable);
        }
    }

}