using System;

public enum EEquipType
{
    Head,
    Chest,
    Arm,
    Leg,
    Accessory,
    Weapon,
}

[Serializable]
public class EquipmentData : ItemData
{
    //장착하고 있는 영웅 idx : -1이면 미장착.
    public int equippedHero = -1; 
    public EEquipType parts;
    public bool isEquipped;
}