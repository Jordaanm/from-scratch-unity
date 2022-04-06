using FromScratch.Equipment;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using Util;

namespace FromScratch.Inventory
{
    [ManageableData]
    [CreateAssetMenu(fileName = "New Item Data", menuName = "From Scratch/Inventory/ItemData")]
    [System.Serializable]
    public class ItemData : ScriptableData
    {   
        [System.Serializable]
        public enum UsageType
        {
            None,
            Use,
            Consume,
            Place,
            Equip
        }
        
        #region Basic Properties

        [TitleGroup("Basic Info")]
        public string id;
        public string itemName = "New Item";
        [PreviewField(Alignment = ObjectFieldAlignment.Left, Height = 100f)]
        public Sprite icon;
        [Multiline(4)]
        public string itemDescription = string.Empty;

        public bool isStackable = true;
        // [Range(1, 99)]
        // public int maxStacks = 1;
        [SerializeField, EnumToggleButtons]
        public UsageType usageType = UsageType.None;
        #endregion Basic Properties
        
        #region Equipment Properties
        [TitleGroup("Equipment")]
        [HideIf("@this.usageType != UsageType.Equip")]
        public  EquipmentData equipmentData;
        #endregion

        #region Use Properties
        [TitleGroup("Usage Setting")]
        [ShowIf("IsUsable")]
        public UnityEvent<Item> onUse = new UnityEvent<Item>();
        
        [ShowIf("@usageType == UsageType.Equip")]
        public UnityEvent<Item> onEquip = new UnityEvent<Item>();
        [ShowIf("@usageType == UsageType.Equip")]
        public UnityEvent<Item> onUnequip = new UnityEvent<Item>();

        [PreviewField, AssetsOnly]
        public GameObject prefab;
        #endregion Use Properties

        public bool IsUsable => usageType != UsageType.None;

        public string Name
        {
            get => name; 
        }
        
        public override string GetID() => id;        
        public static string GetAssetPath() => "Assets/Data/Items";
    }
    
}
