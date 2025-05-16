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
    public string name;
    public string description;
    public EItemCategory category;
    public EItemEffect effect;
    //TODO : 전체 적용여부
}