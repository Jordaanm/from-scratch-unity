using System.Collections.Generic;
using UnityEngine;

namespace FromScratch.Interaction
{
    public class GatherActionSource: IInteractionSource
    {
        private static Interaction gatherResourceAction;
        public GatherActionSource()
        {
            if (gatherResourceAction == null)
            {
                gatherResourceAction = Interaction.GetInteraction("gather");
            }
        }
        
        public List<Interaction> GetActionsForTarget(IInteractable target)
        {
            if (target != null)
            {
                if (gatherResourceAction.CanInteractWith(target))
                {
                    return new List<Interaction>
                    {
                        gatherResourceAction
                    };
                }
            }

            return new List<Interaction>();
        }
    }
}