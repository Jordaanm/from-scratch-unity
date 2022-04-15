using Sirenix.OdinInspector;
using UnityEngine;

namespace FromScratch.Interaction
{
    public class GenericInteractable: Interactable
    {
        [EnumToggleButtons]
        public InteractionType interactionType = InteractionType.Other;
        public override InteractionType GetInteractionType()
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