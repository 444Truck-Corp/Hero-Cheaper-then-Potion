using System;

public enum eItemEffect
{
    Default
}

[Serializable]
public class ItemData
{
    public int id;
    public int name;
    public string description;
    public eItemEffect effect;
    //TODO : 전체 적용여부
}