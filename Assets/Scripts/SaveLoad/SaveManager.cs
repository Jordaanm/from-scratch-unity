using System.IO;
using FromScratch;
using SaveLoad.SaveState;
using Sirenix.OdinInspector;
using UnityEngine;

namespace UnityTemplateProjects.SaveLoad
{
    public class SaveManager
    {
        private GameManager gameManager;

        public void SetGameManager(GameManager gameManager)
        {
            this.gameManager = gameManager;
        }

        [Button]
        public void SaveToDisk()
        {
            var masterSaveState = new MasterSaveState();
            //TODO: Compose the master save state
            masterSaveState.playerPrefs = gameManager.Player.GetPlayerPrefs();
            masterSaveState.characters = gameManager.BuildCharacterStates();
            
            var JSON = JsonUtility.ToJson(masterSaveState);

            WriteToFile(JSON);
        }

        [Button]
        public void LoadFromDisk()
        {
            string path = Application.persistentDataPath + "/SaveData.json";
            var JSON = File.ReadAllText(path);
            var masterSaveState = JsonUtility.FromJson<MasterSaveState>(JSON);
            
            //Load static first
            
            //Load dynamic in order of priority
        }

        public void WriteToFile(string JSON)
        {
            string path = Application.persistentDataPath + "/SaveData.json";
            File.WriteAllText(path, JSON);
        }

    }
}