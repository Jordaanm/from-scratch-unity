namespace FromScratch.Equipment
{
    public interface IEquipmentChangeListener
    {
        float EquipStart(Equipment equipment);

        float UnequipStart(Equipment equipment);
    }
}