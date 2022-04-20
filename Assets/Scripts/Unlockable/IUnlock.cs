using System;
using System.Collections.Generic;
using FromScratch.Crafting;
using FromScratch.Skills;
using FromScratch.Technology;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Unlockable
{
    public enum UnlockType
    {
        Skill,
        Discovery,
        Concept,
        Invention,
        Recipe
    }

    public enum Significance
    {
        Minor,
        Major,
        Key
    }
    
    [Serializable]
    public class Prerequisite
    {
        [EnumToggleButtons]
        public UnlockType type;
        public string id;
        [ShowIf("@type==UnlockType.Skill"), Range(1, 10)]
        public int level;
    }
    
    public interface IUnlock
    {
        public string ID { get; }
        public List<Prerequisite> Prerequisites { get; }
        public Significance Significance { get; }

        static IUnlock FromPrereq(Prerequisite prereq)
        {
            switch (prereq.type)
            {
                case UnlockType.Concept:
                case UnlockType.Invention:
                    return Technology.FindFromPrereq(prereq);
                case UnlockType.Recipe:
                    return Recipe.FindFromPrereq(prereq);
                case UnlockType.Skill:
                    return Skill.FindFromPrereq(prereq);
                case UnlockType.Discovery:
                default:
                    return null; //TODO: Discovery type
            }
        }
    }
}