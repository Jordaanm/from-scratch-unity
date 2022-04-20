using UnityEngine;
using Util;

namespace FromScratch.Technology
{
    [CreateAssetMenu(fileName = "TechDatabase", menuName = "From Scratch/Databases/Technology Database")]
    public class TechnologyDatabase: ScriptableDatabase<Technology>
    {
    }
}