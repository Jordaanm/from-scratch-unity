using FromScratch.Inventory;
using UnityEngine;

namespace FromScratch.Equipment
{
    public enum EquipmentType
    {
        Tool, Weapon, Armor
    }
    
    public enum EquipmentSlot
    {
        Mainhand,
        Offhand,
        Twohanded,
        Head,
        Chest,
        Legs
    }
    
    public class Equipment: MonoBehaviour
    {
        public EquipmentType type;
        public EquipmentSlot slot;
        public EquipmentData data;
        public string equipmentName;

        public Vector3 positionOffset;
        public Vector3 rotationOffset;

        [HideInInspector] public Item item;
        [HideInInspector] public GameObject instance;
        [HideInInspector] public bool isEquipped = false;
    }
}