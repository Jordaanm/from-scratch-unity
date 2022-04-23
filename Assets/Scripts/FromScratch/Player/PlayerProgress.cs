using System;
using System.Collections.Generic;
using FromScratch.Crafting;
using FromScratch.Skills;
using UnityEngine;
using UnityEngine.Events;
using Unlockable;

namespace FromScratch.Player
{
    public class PlayerProgress: MonoBehaviour
    {
        [SerializeField]
        private List<Skill> unlockedSkills = new();
        
        [SerializeField]
        private List<Technology.Technology> unlockedTechnologies = new();
        
        [SerializeField]
        private List<Recipe> unlockedRecipes = new();
        
        public List<Skill> UnlockedSkills => unlockedSkills;
        public List<Technology.Technology> UnlockedTechnologies => unlockedTechnologies;
        public List<Recipe> UnlockedRecipes => unlockedRecipes;

        public UnityEvent<FromScratchPlayer> OnUnlocksChanged = new UnityEvent<FromScratchPlayer>();
        
        private FromScratchPlayer player;


        public void Awake()
        {
            player = GetComponent<FromScratchPlayer>();
        }

        public bool HasUnlocked(Skill skill)
        {
            return unlockedSkills.Contains(skill);
        }

        public bool HasUnlocked(Recipe recipe)
        {
            return unlockedRecipes.Contains(recipe);
        }
        
        public bool HasUnlocked(Technology.Technology tech)
        {
            return unlockedTechnologies.Contains(tech);
        }
        
        public void UnlockSkill(IUnlock unlock)
        {
            var type = unlock.GetType();
            if (type == typeof(Skill))
            {
                unlockedSkills.Add(unlock as Skill);
            } else if (type == typeof(Recipe))
            {
                unlockedRecipes.Add(unlock as Recipe);
            } else if (type == typeof(Technology.Technology))
            {
                unlockedTechnologies.Add(unlock as Technology.Technology);
            }
            
            OnUnlocksChanged.Invoke(player);
        }
    }
}