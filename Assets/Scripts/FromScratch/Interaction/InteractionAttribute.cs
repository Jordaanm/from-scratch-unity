using System;

namespace FromScratch.Interaction
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class InteractionAttribute: Attribute
    {
        public string id { get; set; }
    }
}