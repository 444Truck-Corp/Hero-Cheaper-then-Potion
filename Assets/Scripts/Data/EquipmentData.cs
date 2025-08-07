using System;

public enum EEquipType
{
    Head = 0,
    Chest,
    Leg,
    Accessory,
    Weapon,
    Arm,
}

[Serializable]
public class EquipmentData : ItemData
{
    //장착하고 있는 영웅 idx : -1이면 미장착.
    public int equippedHero = -1;
    public EEquipType parts;

    public StatusData EquipmentStat => SetStatus();
    public string stats;

    public StatusData SetStatus()
    {
        string trimmed = stats.Trim('[', ']');
        string[] tokens = trimmed.Split(',');

        // 필요한 값만 int로 변환
        int str = tokens.Length > 0 ? int.Parse(tokens[0]) : 0;
        int dex = tokens.Length > 1 ? int.Parse(tokens[1]) : 0;
        int intel = tokens.Length > 2 ? int.Parse(tokens[2]) : 0;

        return new StatusData
        {
            STR = str,
            DEX = dex,
            INT = intel
        };
    }
}