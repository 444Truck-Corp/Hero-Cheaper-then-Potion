using System;

[Serializable]
public struct LvData
{
    public int level;
    public int exp;
}

[Serializable]
public struct StatusData
{
    public int STR; // 근력
    public int DEX; // 민첩
    public int INT; // 지능
    public int HP; // 체력

    public StatusData(int _str, int _dex, int _int, int _luk)
    {
        STR = _str;
        DEX = _dex;
        INT = _int;
        HP = _luk;
    }

    public StatusData SetStatus(int _str, int _dex, int _int, int _luk)
    {
        STR = _str;
        DEX = _dex;
        INT = _int;
        HP = _luk;
        return this;
    }

    public static StatusData operator +(StatusData a, StatusData b)
    {
        return new StatusData(
            a.STR + b.STR,
            a.DEX + b.DEX,
            a.INT + b.INT,
            a.HP + b.HP
        );
    }
}