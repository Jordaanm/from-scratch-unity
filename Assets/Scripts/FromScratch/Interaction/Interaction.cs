namespace FromScratch.Interaction
{
    public abstract class Interaction
    {
        protected string _label;
        protected string _id;
        
        public string Label => _label;
        public string ID => _id;

        public abstract void Start(IInteractor interactor, IInteractable target);

    }
}