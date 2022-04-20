using UnityEngine;
using Util;

namespace FromScratch.Skills
{
    [CreateAssetMenu(fileName = "SkillDatabase", menuName = "From Scratch/Databases/Skill Database")]
    public class SkillDatabase: ScriptableDatabase<Skill>
    { }
}