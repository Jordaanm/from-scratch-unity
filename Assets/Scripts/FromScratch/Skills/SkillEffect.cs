using UnityEngine;

namespace FromScratch.Skills
{
    public abstract class SkillEffect
    {
        public virtual void Execute(Character.Character character, Skill skill)
        {
            Debug.Log("SkillEffect:Execute");
        }
    }
}