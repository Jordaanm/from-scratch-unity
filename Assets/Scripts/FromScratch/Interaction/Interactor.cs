using System.Collections.Generic;
using System.Linq;
using Sirenix.Utilities;
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
    }

    public abstract class Interactable: MonoBehaviour
    {
        public abstract InteractionType GetInteractionType();
    }

    public class Interactor
    {
        public UnityEvent OnChange = new UnityEvent();
        private List<Interactable> nearbyInteractables = new List<Interactable>();

        private HashSet<IInteractionSource> localInteractionSources;

        public Interactor()
        {
            localInteractionSources = new HashSet<IInteractionSource>();
        }

        public void AddInteractionSource(IInteractionSource interactionSource)
        {
            localInteractionSources.Add(interactionSource);
        }

        public List<Interaction> GetActionsForTarget(Interactable target)
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

        public List<Interactable> AllNearby() {
            return nearbyInteractables;
        }

        public void SetInteractables(List<GameObject> gameObjects)
        {
            List<Interactable> interactables = gameObjects
                .Select(go => go.GetComponent<Interactable>())
                .ToList()
                .FindAll(x => x != null);

            SetInteractables(interactables);
        }

        public void SetInteractables(List<Interactable> interactables)
        {
            nearbyInteractables.Clear();
            nearbyInteractables.AddRange(interactables);
            
            OnChange.Invoke();
        }

    }

}