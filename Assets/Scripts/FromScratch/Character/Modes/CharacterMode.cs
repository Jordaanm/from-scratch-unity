using System;
using UnityEngine;

namespace FromScratch.Character
{
    public enum MovementType
    {
        None, // Should not be used by a Character Mode
        Overview,
        OnFoot,
        Vehicle,
        Menu,
    }
    
    public abstract class CharacterMode: MonoBehaviour
    {
        [HideInInspector]
        public Character character;

        private void Awake()
        {
            character = GetComponentInParent<Character>();
        }

        public abstract MovementType MovementType {
            get;
        }
    }
}