using System;
using SaveLoad.SaveState;
using UnityEngine;
using UnityEngine.Events;

namespace FromScratch.Player
{
    public class FromScratchPlayer : MonoBehaviour
    {
        public RenderTexture PreviewTexture;
        public Character.Character character;
        public PlayerProgress progress;

        private UnityEvent onAttachToCharacter = new();
        private bool isAttachedToCharacter = false;
        private void Awake()
        {
            progress = GetComponent<PlayerProgress>();
        }

        public PlayerPreferencesSaveState GetPlayerPrefs()
        {
            return new PlayerPreferencesSaveState()
            {
                musicVolume = 0.25f,
                soundVolume = 0.5f
            };
        }

        public void AttachToCharacter(Character.Character character)
        {
            this.character = character;
            character.transform.SetParent(transform);
            isAttachedToCharacter = true;
            onAttachToCharacter.Invoke();
        }

        public void OnceCharacterIsSet(UnityAction onCharacterSet)
        {
            onAttachToCharacter.AddListener(onCharacterSet);
            if (isAttachedToCharacter)
            {
                onCharacterSet.Invoke();
            }
        }
    }
}
