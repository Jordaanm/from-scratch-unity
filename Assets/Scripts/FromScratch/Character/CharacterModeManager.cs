using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace FromScratch.Character
{
    public class CharacterModeManager: MonoBehaviour
    {
        private List<CharacterMode> modes = new List<CharacterMode>();

        private CharacterMode activeMode;
        
        [ChildGameObjectsOnly]
        public CharacterMode defaultMode;

        public UnityEvent<CharacterMode, CharacterMode> onModeChange = new UnityEvent<CharacterMode, CharacterMode>();
        private void Awake()
        {
            modes.AddRange(GetComponentsInChildren<CharacterMode>());
            SetDefaultModeAsActive();
        }

        private void SetDefaultModeAsActive()
        {
            if (modes.Count == 0)
            {
                Debug.LogError("No Character Modes found");
                return;
            }

            CharacterMode defaultMode = modes.Find(x => x == this.defaultMode);
            if (defaultMode == null)
            {
                Debug.LogError("No Character Modes found matching default mode, using first mode instead");
                defaultMode = modes[0];
            }

            SetActiveMode(defaultMode);
        }

        public CharacterMode SetActiveMode(CharacterMode characterMode)
        {
            //TODO: Sanity checking for no mode found to activate;
            CharacterMode modeToActivate = null;
            foreach (CharacterMode mode in modes)
            {
                if (mode == characterMode)
                {
                    mode.enabled = true;
                    modeToActivate = mode;
                }
            }

            CharacterMode prevMode = activeMode;
            activeMode = modeToActivate;
            onModeChange.Invoke(prevMode, activeMode);

            return activeMode;
        }

        public T SetActiveMode<T>() where T : CharacterMode
        {
            var mode = modes.Find(characterMode => characterMode.GetType() == typeof(T));
            if (mode != null)
            {
                return SetActiveMode(mode) as T;
            }
            
            Debug.LogErrorFormat("SetActiveMode: No Mode Found of type {0}", typeof(T).Name);
            return null;
        }

        public MovementType MovementType => activeMode == null ? MovementType.None : activeMode.MovementType;

        public CharacterMode GetActiveMode()
        {
            return activeMode;
        }
    }
    
}