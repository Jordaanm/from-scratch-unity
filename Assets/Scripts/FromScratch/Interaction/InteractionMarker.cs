using System;
using FromScratch.Player;
using UnityEngine;
using UnityEngine.Events;
using Util;

namespace FromScratch.Interaction
{
    public class InteractionMarker: MonoSingleton<InteractionMarker>
    {
        private FromScratchPlayer player;
        public Interactable attachedTo;

        private void Start()
        {
            player = GameManager.Instance.Player;
            GameManager.Instance.OnPlayerChanged.AddListener(OnPlayerChanged);
        }

        private void OnPlayerChanged(FromScratchPlayer newPlayer)
        {
            player = newPlayer;
        }

        private void Update()
        {
            FaceTowardsPlayerCamera();
        }

        private void FaceTowardsPlayerCamera()
        {
            Camera cam = Camera.main;
            if (cam != null)
            {
                transform.LookAt(cam.transform);
            }
        }

        public void Show(Interactable target)
        {
            if (attachedTo == target)
            {
                return; 
            }

            attachedTo = target;
            transform.SetParent(target.transform);
            transform.localPosition = Vector3.up;
            gameObject.SetActive(true);
        }
        public void Hide()
        {
            attachedTo = null;
            transform.SetParent(null);
            transform.localPosition = Vector3.zero;
            gameObject.SetActive(false);
        }
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void OnRuntimeStartSingleton() {
            IS_EXITING = false;
        }
    }
}