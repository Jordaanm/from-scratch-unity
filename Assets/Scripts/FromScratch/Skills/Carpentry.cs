using UnityEngine;

namespace FromScratch.Skills
{
    [Skill("carpentry")]
    public class Carpentry: SkillEffect
    {
        public override void Execute(Character.Character character, Skill skill)
        {
            Debug.Log("Carpentry:Execute");
        }
    }
}