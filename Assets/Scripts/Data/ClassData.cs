using System;

[Serializable]
public class ClassData
{
    public int id;
    public string className;

    // 기본 스탯
    public int baseStr;
    public int baseDex;
    public int baseInt;
    public int baseHP;

    // 증가 스탯
    public int incStr;
    public int incDex;
    public int incInt;
    public int incHP;

    public StatusData BaseStat => new(baseStr, baseDex, baseInt, baseHP);
    public StatusData IncStat => new(incStr, incDex, incInt, incHP);
}