using UnityEngine;
using Util;

namespace FromScratch.Equipment
{
    [CreateAssetMenu(fileName = "EquipmentDatabase", menuName = "From Scratch/Databases/Equipment Database")]
    public class EquipmentDatabase: ScriptableDatabase<EquipmentData>
    { }
}