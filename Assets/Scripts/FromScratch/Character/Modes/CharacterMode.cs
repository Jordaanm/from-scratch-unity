using System;
using UnityEngine;

namespace FromScratch.Character
{
    public enum MovementType
    {
        None, // Should not be used by a Character Mode
        Overview, //Top Down, ie item placement
        OnFoot, // Exploration, etc
        Vehicle, // Vehicle Controls/movement, possible use of IK for foot pedalling
        Menu, // Navigating Menus
        Stationary, // Unmoving (Meditation, Sleep, Dialogues, etc)
        Climbing,
    }
    
    public abstract class CharacterMode: MonoBehaviour
    {
        [HideInInspector]
        public Character character;

        protected virtual void Awake()
        {
            character = GetComponentInParent<Character>();
        }

        public abstract MovementType MovementType {
            get;
        }

        public virtual bool HandlesIK => false;

        public virtual void PerformIK(Animator animator)
        {
            
        }
    }
}