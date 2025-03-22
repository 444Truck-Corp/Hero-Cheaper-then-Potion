using System;
using System.Collections.Generic;

[Serializable]
public struct Status
{
    public int STR; // 근력
    public int DEX; // 민첩
    public int INT; // 지능
    public int LUK; // 행운

    public Status(int _str, int _dex, int _int, int _luk)
    {
        STR = _str;
        DEX = _dex;
        INT = _int;
        LUK = _luk;
    }

    public Status SetStatus(int _str, int _dex, int _int, int _luk)
    {
        STR = _str;
        DEX = _dex;
        INT = _int;
        LUK = _luk;
        return this;
    }

    public static Status operator +(Status a, Status b)
    {
        return new Status(
            a.STR + b.STR,
            a.DEX + b.DEX,
            a.INT + b.INT,
            a.LUK + b.LUK
        );
    }
}

[Serializable]
public class HeroData
{
    public int id;
    public string name;
    public bool spriteType;
    public ClassData classData;
    public Status status;
    public int level;
    public int exp;
    public int spriteIdx;
    

    public HeroData()
    {
        id = 0;
        name = "";
        classData = null;
        spriteType = false;
        level = 0;
        exp = 0;
        spriteIdx = 0;
    }

   

    public void GetExp(int delta)
    {
        if (delta == 0) return;
        exp += delta;
        GameManager.Instance.OnGetExpEvent?.Invoke(this, delta);
        while (exp >= levelExpList[level] && level < 10)
        {
            exp -= levelExpList[level++];
            status += classData.IncStat;
            GameManager.Instance.OnHeroLevelUpEvent?.Invoke(this, level);
            GameManager.Instance.OnHeroStatUpEvent?.Invoke(this, classData.IncStat);
        }
    }
}