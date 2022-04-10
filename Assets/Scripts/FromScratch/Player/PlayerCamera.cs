using Cinemachine;
using FromScratch.Character;
using UnityEngine;

namespace FromScratch.Player
{
    public class PlayerCamera: MonoBehaviour
    {
        public FromScratchPlayer player;
        public CharacterModeManager modeManager;

        public Camera mainCam;
        public CinemachineVirtualCamera thirdPersonCamera;
        public CinemachineVirtualCamera topDownCamera;

        public const int InactiveCamPriority = 0;
        public const int ActiveCamPriority = 50;
        private void Awake()
        {
            mainCam = Camera.main;
            player = GetComponent<FromScratchPlayer>();
            modeManager = GetComponentInChildren<CharacterModeManager>();
        }

        private void Start()
        {
            modeManager.onModeChange.AddListener(this.onModeChange);
            onModeChange(null, modeManager.defaultMode);
        }

        private void onModeChange(CharacterMode prev, CharacterMode next)
        {
            CinemachineVirtualCamera prevCam = GetCamForMode(prev);
            CinemachineVirtualCamera nextCam = GetCamForMode(next);

            if (prevCam != null)
            {
                prevCam.Priority = InactiveCamPriority;
            }

            if (nextCam != null)
            {
                nextCam.Priority = ActiveCamPriority;
            }
        }

        private CinemachineVirtualCamera GetCamForMode(CharacterMode mode)
        {
            if (mode == null)
            {
                return null; 
            }

            switch (mode.MovementType)
            {
                case MovementType.OnFoot: return thirdPersonCamera;
                case MovementType.Overview: return topDownCamera;
                case MovementType.Stationary: return thirdPersonCamera;
                default: return null;
            }
        }
    }
}