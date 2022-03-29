using System.Collections.Generic;

namespace FromScratch.Interaction
{
    public interface IInteractionSource
    {
        List<Interaction> GetActionsForTarget(IInteractable target);
    }
}