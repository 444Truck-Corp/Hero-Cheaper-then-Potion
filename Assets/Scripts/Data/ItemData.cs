using System;

public enum EItemEffect
{
    Default
}

[Serializable]
public class ItemData
{
    public int id;
    public int name;
    public string description;
    public EItemEffect effect;
    //TODO : 전체 적용여부
}