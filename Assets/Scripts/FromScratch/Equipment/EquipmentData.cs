using UnityEngine;
using Sirenix.OdinInspector;
using Util;
using System.Collections.Generic;

namespace FromScratch.Equipment
{
    [ManageableData]
    [CreateAssetMenu(fileName = "Equipment Data", menuName = "From Scratch/Equipment/Equipment Data")]
    public class EquipmentData: ScriptableData
    {
        [BoxGroup("Basics")]
        public string id;
        [BoxGroup("Basics")]
        public string equipmentName;
        [BoxGroup("Basics"), EnumToggleButtons]
        public EquipmentType type;
        [BoxGroup("Basics"), EnumToggleButtons]
        public EquipmentSlot slot;

        [PreviewField, AssetsOnly]
        [BoxGroup("Prefab")]
        public GameObject prefab;

        [BoxGroup("Audio")]
        public AudioClip audioSheathe;
        [BoxGroup("Audio")]
        public AudioClip audioDraw;

        public override string GetID() => id;
        public static string GetAssetPath() => "Assets/Data/Equipment";

        public List<string> providedActions = new List<string>();
    }
}