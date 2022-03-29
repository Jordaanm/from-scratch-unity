using System.Collections.Generic;
using System.Linq;
using FromScratch.Equipment;
using FromScratch.Interaction;
using FromScratch.Inventory;
using UnityEngine;

namespace FromScratch.Character
{
    public class CharacterEquipment: MonoBehaviour, IInteractionSource
    {
        private Character character;

        private Dictionary<EquipmentSlot, Equipment.Equipment> equipped =
            new Dictionary<EquipmentSlot, Equipment.Equipment>();

        private List<IEquipmentChangeListener> equipmentChangeListeners = new List<IEquipmentChangeListener>();
        public bool IsDrawing { get; private set; }
        public bool IsSheathing { get; private set; }

        private void Awake()
        {
            this.character = GetComponent<Character>();
        }

        public static HumanBodyBones GetBodyBoneForSlot(EquipmentSlot slot)
        {
            switch(slot) {
                case EquipmentSlot.Mainhand: return HumanBodyBones.LeftHand;
                case EquipmentSlot.Offhand: return HumanBodyBones.RightHand;
                case EquipmentSlot.Twohanded: return HumanBodyBones.LeftHand;
                case EquipmentSlot.Head: return HumanBodyBones.Head;
                case EquipmentSlot.Chest: return HumanBodyBones.Chest;
                case EquipmentSlot.Legs: return HumanBodyBones.Hips;
                default: return HumanBodyBones.LeftHand;
            }            
        }

        public void RegisterEquipmentChangeListener(IEquipmentChangeListener listener)
        {
            equipmentChangeListeners.Add(listener);
        }

        public Equipment.Equipment EquipItem(Item item)
        {
            if (item == null)
            {
                return null;
            }

            EquipmentData equipmentData = item.itemData.equipmentData;

            if (equipmentData == null || equipmentData.prefab == null)
            {
                return null;
            }
            
            GameObject instance = Instantiate(item.itemData.equipmentData.prefab);
            Equipment.Equipment equipment = instance.GetComponent<Equipment.Equipment>();

            if (equipment == null)
            {
                Debug.LogFormat("Unable to find equipment from {0}'s prefab", equipmentData.equipmentName);
                return null;
            }

            equipment.item = item;
            equipment.instance = instance;
            Equip(equipment);
            instance.transform.localScale = equipmentData.prefab.transform.localScale;

            item.IsEquipped = true;

            return equipment;
        }

        public void Equip(Equipment.Equipment equipment)
        {
            HumanBodyBones bone = GetBodyBoneForSlot(equipment.slot);

            UnequipSlot(equipment.slot);

            AttachToBody(
                equipment.gameObject,
                bone,
                equipment.positionOffset,
                Quaternion.Euler(equipment.rotationOffset)
            );
            
            equipped.Add(equipment.slot, equipment);
        }
        
        void AttachToBody(
            GameObject _gameObject, 
            HumanBodyBones bone, 
            Vector3 positionOffset, 
            Quaternion rotationOffset)
        {
            Transform boneTransform = character.Animator.GetBoneTransform(bone);

            // This will need to be refactored elsewhere.
            // instance.transform.localScale = item.prefab.transform.localScale;

            _gameObject.transform.SetParent(boneTransform);

            _gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
            _gameObject.transform.localPosition = positionOffset;
            _gameObject.transform.localRotation = rotationOffset;
            
        }
        
        public void UnequipSlot(EquipmentSlot slot)
        {
            Equipment.Equipment existingEquipment;
            bool isSlotOccupied = equipped.TryGetValue(slot, out existingEquipment);
            Debug.LogFormat("Is Slot Occupied? {0}", isSlotOccupied.ToString());

            if (isSlotOccupied && existingEquipment != null)
            {
                equipped.Remove(slot);
                existingEquipment.transform.SetParent(null);
                if (existingEquipment.item != null)
                {
                    existingEquipment.isEquipped = false;
                    existingEquipment.item.IsEquipped = false;
                }
                Destroy(existingEquipment.gameObject);
            }
        }

        public List<Interaction.Interaction> GetActionsForTarget(IInteractable target)
        {
            return equipped.Values.ToList()
                .FindAll(x => x != null) //Filter out empty slots
                .SelectMany(equipment => equipment.data.providedActions) //FlatMap to equipment actions
                .Distinct()
                .Select(id => Interaction.Interaction.GetInteraction(id)) //Map from IDs to actual interactions
                .ToList()
                .FindAll(x => x != null);
        }
    }
}