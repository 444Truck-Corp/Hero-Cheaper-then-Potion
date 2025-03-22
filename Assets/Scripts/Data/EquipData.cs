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
    public eEquipType equipType;
}