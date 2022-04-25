using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AssetReferences;
using FromScratch.Player;
using SaveLoad;
using SaveLoad.SaveState;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityTemplateProjects.SaveLoad;
using Util;

namespace FromScratch
{
    public class GameManager: MonoSingleton<GameManager>
    {

        public readonly string playerPrefabID = "player";
        
        [SerializeField]
        private FromScratchPlayer player;

        private List<Character.Character> trackedCharacters = new();
        
        [ShowInInspector]
        private SaveManager saveManager = new();

        public FromScratchPlayer Player => player;

        public VirtualCameras virtualCameras;
        public UnityEvent<FromScratchPlayer> OnPlayerChanged { get; private set; } = new();
        
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void OnRuntimeStartSingleton() {
            IS_EXITING = false;
        }
        private void Awake()
        {
            saveManager.SetGameManager(this);
            if (virtualCameras == null)
            {
                virtualCameras = FindObjectOfType<VirtualCameras>();
            }
        }

        public CharacterState[] BuildCharacterStates()
        {
            return trackedCharacters
                .Select(character => character.BuildSaveState())
                .ToArray();
        }

        public void LoadSaveState(MasterSaveState saveState)
        {
            //Spawn Characters
            Dictionary<string, Character.Character> allCharacters = new();
            foreach (var saveStateCharacter in saveState.characters)
            {
                GameObject prefab = PrefabAssetReference.Instance.GetAsset(saveStateCharacter.prefabID);
                if (prefab != null)
                {
                    var charGO = Instantiate(prefab, saveStateCharacter.location, saveStateCharacter.rotation);
                    Character.Character character = charGO.GetComponent<Character.Character>();
                    if (character != null)
                    {
                        character.guid = saveStateCharacter.guid;
                        //TODO: Restore State
                        // character.LoadState(saveStateCharacter.customState);

                        allCharacters.Add(character.guid, character);
                    }
                }
            }
            
            // Spawn Player
            var playerPrefab = PrefabAssetReference.Instance.GetAsset(playerPrefabID);
            var playerGO = Instantiate(playerPrefab);
            player = playerGO.GetComponent<FromScratchPlayer>();
            player.AttachToCharacter(allCharacters[saveState.player.characterGuid]);
            OnPlayerChanged.Invoke(player);
        }
    }
}