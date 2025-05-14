using System;

public enum EEquipType{
    Helmet,
    Chest,
    Pants,
    Deco,
    Weapon,
}

[Serializable]
public class EquipmentData : ItemData
{
    //장착하고 있는 영웅 idx : -1이면 미장착.
    public int equippedHero = -1; 
    public EEquipType equipType;
    public bool isEquipped;
}