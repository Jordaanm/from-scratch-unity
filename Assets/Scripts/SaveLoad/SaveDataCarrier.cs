using System;
using FromScratch;
using SaveLoad.SaveState;
using UnityEngine;
using UnityEngine.SceneManagement;
using Util;

namespace SaveLoad
{
    public class SaveDataCarrier: MonoSingleton<SaveDataCarrier>
    {
        public MasterSaveState saveState;

        [HideInInspector]
        public bool loadGameOnSceneLoad;

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoad;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoad;
        }

        private void OnSceneLoad(Scene scene, LoadSceneMode loadSceneMode)
        {
            if (!loadGameOnSceneLoad)
            {
                return;
            }
            
            Debug.Log("TODO: LOAD GAME HERE");
            GameManager.Instance.LoadSaveState(saveState);
        }
    }
}