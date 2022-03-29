using System.Collections.Generic;

namespace FromScratch.Interaction
{
    public class PickupItemActionSource: IInteractionSource
    {
        private static PickupItemAction pickupItemAction;
        public PickupItemActionSource()
        {
            if (pickupItemAction == null)
            {
                pickupItemAction = new PickupItemAction();
            }
        }
        
        public List<Interaction> GetActionsForTarget(IInteractable target)
        {
            InteractionType interactionType = target.GetInteractionType();
            if (interactionType == InteractionType.Pickup)
            {
                return new List<Interaction>()
                {
                    pickupItemAction
                };
            }

            return new List<Interaction>();
        }
    }
}