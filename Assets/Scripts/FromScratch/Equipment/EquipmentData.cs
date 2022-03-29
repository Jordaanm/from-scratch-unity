using UnityEngine;
using Sirenix.OdinInspector;
using Util;

namespace FromScratch.Equipment
{
    [ManageableData]
    [CreateAssetMenu(fileName = "Equipment Data", menuName = "From Scratch/Equipment/Equipment Data")]
    public class EquipmentData: ScriptableData
    {
        [BoxGroup("Basics")]
        public string id;
        public string equipmentName;
        public EquipmentType type;
        public EquipmentSlot slot;

        [BoxGroup("Prefab")]
        [PreviewField, AssetsOnly]
        public GameObject prefab;

        [BoxGroup("Audio")]
        public AudioClip audioSheathe;
        public AudioClip audioDraw;

        public override string GetID() => id;
        public static string GetAssetPath() => "Assets/Data/Equipment";
    }
}