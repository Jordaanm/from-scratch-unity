using System;
using Cinemachine;
using FromScratch.Character;
using UnityEngine;

namespace FromScratch.Player
{
    public class PlayerCamera: MonoBehaviour
    {
        public FromScratchPlayer player;
        public CharacterModeManager modeManager;

        public CinemachineVirtualCamera thirdPersonCamera;
        public CinemachineVirtualCamera topDownCamera;

        public const int InactiveCamPriority = 0;
        public const int ActiveCamPriority = 50;

        private bool hasSetup = false;

        private void Start()
        {
            player = GetComponent<FromScratchPlayer>();
            player.OnceCharacterIsSet(Setup);
        }

        private void Update()
        {
            if (!hasSetup && player.character != null )
            {
                Debug.Log("PlayerCamera::Update::Setup");
                Setup();
            }
        }

        private void Setup()
        {
            modeManager = GetComponentInChildren<CharacterModeManager>();

            if (player == null || player.character == null || player.character.transform == null)
            {
                return;
            }

            if (thirdPersonCamera == null || topDownCamera == null)
            {
                thirdPersonCamera = GameManager.Instance.virtualCameras.ThirdPersonCamera;
                topDownCamera = GameManager.Instance.virtualCameras.TopDownCamera;
            }
            
            var characterTransform = player.character.transform;
            thirdPersonCamera.Follow = characterTransform;
            thirdPersonCamera.LookAt = characterTransform;
            topDownCamera.Follow = characterTransform;
            topDownCamera.LookAt = characterTransform;
            
            if (modeManager != null)
            {
                modeManager.onModeChange.AddListener(this.onModeChange);
                onModeChange(null, modeManager.defaultMode);
                hasSetup = true;
            }

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