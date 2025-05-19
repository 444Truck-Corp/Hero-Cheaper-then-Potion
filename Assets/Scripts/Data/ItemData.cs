using System;

public enum EItemEffect
{
    Default
}

public enum EItemCategory
{
    Material,
    Potion,
    Equipment,
}

[Serializable]
public class ItemData
{
    public int id;
    public int value;
    public int rank;
    public string name;
    public string description;
    public string icon;
    public EItemCategory category;
    public EItemEffect effect;
    //TODO : 전체 적용여부
}