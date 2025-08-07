using System;

[Serializable]
public struct LvData
{
    public int level;
    public int characExp;
    public int hp;
    public int guildExp;
}

[Serializable]
public struct StatusData
{
    public int STR; // 근력
    public int DEX; // 민첩
    public int INT; // 지능

    public StatusData(int _str, int _dex, int _int)
    {
        STR = _str;
        DEX = _dex;
        INT = _int;
    }

    public StatusData SetStatus(int _str, int _dex, int _int)
    {
        STR = _str;
        DEX = _dex;
        INT = _int;
        return this;
    }

    public static StatusData operator +(StatusData a, StatusData b)
    {
        return new StatusData(
            a.STR + b.STR,
            a.DEX + b.DEX,
            a.INT + b.INT
        );
    }

    public static StatusData operator -(StatusData a, StatusData b)
    {
        return new StatusData(
            a.STR - b.STR,
            a.DEX - b.DEX,
            a.INT - b.INT
        );
    }

    public static StatusData operator -(StatusData a)
    {
        return new StatusData(
            -a.STR,
            -a.DEX,
            -a.INT
        );
    }
}