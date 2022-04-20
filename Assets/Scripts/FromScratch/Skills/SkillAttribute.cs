using System;

namespace FromScratch.Skills
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SkillAttribute: Attribute
    {
        public string id;

        public SkillAttribute(string id)
        {
            this.id = id;
        }
    }
}