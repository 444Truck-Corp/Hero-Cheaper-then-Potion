using System;

public enum eEquipType{
    Weapon,
    Helmet,
    ChestArmor,
    PantsArmor
}

[Serializable]
public class EquipData : ItemData
{
    //장착하고 있는 영웅 idx : -1이면 미장착.
    public int equippedHero = -1; 
    public eEquipType equipType;
}