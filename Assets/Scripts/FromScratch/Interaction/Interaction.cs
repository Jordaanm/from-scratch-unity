using System;
using System.Collections.Generic;
using System.Linq;

namespace FromScratch.Interaction
{
    public abstract class Interaction
    {
        protected string _label;
        protected string _id;
        
        public string Label => _label;
        public string ID => _id;

        public virtual bool CanInteractWith(Interactable interactable)
        {
            return false;
        }

        public abstract void Start(IInteractor interactor, Interactable target);

        private static Dictionary<string, Interaction> GetAllInteractions()
        {
            var interactionTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes().Where(t => t.IsDefined(typeof(InteractionAttribute), false)));

            Dictionary<string, Interaction> dictionary = new Dictionary<string, Interaction>();
            foreach (var interactionType in interactionTypes)
            {
                string key = interactionType
                    .GetCustomAttributes(typeof(InteractionAttribute), false)
                    .Cast<InteractionAttribute>()
                    .First()
                    .id;

                Interaction interaction = Activator.CreateInstance(interactionType) as Interaction;
                
                dictionary.Add(key, interaction);
            }

            return dictionary;
        }
        
        private static Dictionary<string, Interaction> allInteractions = null;

        public static Interaction GetInteraction(string id)
        {
            if (allInteractions == null)
            {
                allInteractions = GetAllInteractions();
            }

            return allInteractions[id];
        }
    }
}