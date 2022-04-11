using Sirenix.OdinInspector;
using UnityEngine;

namespace FromScratch.Interaction
{
    public class GenericInteractable: MonoBehaviour, IInteractable
    {
        [EnumToggleButtons]
        public InteractionType interactionType = InteractionType.Other;
        public InteractionType GetInteractionType()
        {
            return interactionType;
        }

        public GameObject GetGameObject()
        {
            Debug.Log(":|");
            return gameObject;
        }
    }
}