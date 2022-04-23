using System;
using UnityEngine;

namespace FromScratch.Player
{
    public class FromScratchPlayer : MonoBehaviour
    {
        public RenderTexture PreviewTexture;
        public Character.Character character;
        public PlayerProgress progress;

        private void Awake()
        {
            progress = GetComponent<PlayerProgress>();
        }
    }
}
