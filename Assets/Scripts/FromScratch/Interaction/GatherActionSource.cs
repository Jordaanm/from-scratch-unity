using System.Collections.Generic;

namespace FromScratch.Interaction
{
    public class GatherActionSource: IInteractionSource
    {
        private static GatherResourceAction gatherResourceAction;
        public GatherActionSource()
        {
            if (gatherResourceAction == null)
            {
                gatherResourceAction = new GatherResourceAction();
            }
        }
        
        public List<Interaction> GetActionsForTarget(IInteractable target)
        {
            if (target != null)
            {
                InteractionType interactionType = target.GetInteractionType();
                if (interactionType == InteractionType.ResourceNode)
                {
                    return new List<Interaction>()
                    {
                        gatherResourceAction
                    };
                }
            }

            return new List<Interaction>();
        }
    }
}