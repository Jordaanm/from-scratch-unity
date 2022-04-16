using System.Collections.Generic;

namespace FromScratch.Interaction
{
    public class BasicActionSource: IInteractionSource
    {

        private List<Interaction> interactions;

        public BasicActionSource()
        {
            interactions = new List<Interaction>
            {
                Interaction.GetInteraction("sleep"),
                Interaction.GetInteraction("pickup"),
                Interaction.GetInteraction("gather"),
                Interaction.GetInteraction("startclimb")
            };
        }
        
        public List<Interaction> GetActionsForTarget(Interactable target)
        {
            return interactions.FindAll(x => x.CanInteractWith(target));
        }
    }
}