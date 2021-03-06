using FromScratch.Interaction;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace FromScratch.Gathering
{
    [System.Serializable]
    public class ResourceNodeEvent : UnityEvent { }

    public class ResourceNode: Interactable
    {
        public ResourceNodeInfo resourceNodeInfo;
        public InteractionType interactionType = InteractionType.ResourceNode;
        public GameObject host;
        [HideInInspector] public int remainingStock;

        [HideInInspector] public ResourceNodeEvent onGathered = new ResourceNodeEvent();
        [HideInInspector] public ResourceNodeEvent onDepleted = new ResourceNodeEvent();

        void Start()
        {
            if (resourceNodeInfo != null)
            {
                remainingStock = resourceNodeInfo.initialStock;
            }

            if (host == null)
            {
                host = transform.parent.gameObject;
            }
        }
        
        void OnMouseEnter() {
            Debug.LogFormat("Mouse Enter {0} {1}", this.name, this.transform.parent.gameObject.name);
        }
        
        public bool HasRemainingStock() => remainingStock > 0;
        
        public int ReduceRemainingStock(int reduceBy) => remainingStock = Mathf.Max(remainingStock - reduceBy, 0);
        
        public bool CanCharacterHarvest(Character.Character character)
        {
            return true; // Placeholder for equipment/skill restrictions
        }
        
        public bool DepleteResources(int amount = 1) {
            ReduceRemainingStock(amount);
            onGathered.Invoke();
            if(remainingStock <= 0) {
                onDepleted.Invoke();
                return false;
            }
            return true;
        }
        
        public void DestroyMe() {
            GameObject.Destroy(host.gameObject);
        }
        
        public void Shrink() {
            Debug.Log("Shrinking Node");
            host.transform.localScale = host.transform.localScale * 0.8f;
        }

        public override InteractionType GetInteractionType()
        {
            return interactionType;
        }
    }
}