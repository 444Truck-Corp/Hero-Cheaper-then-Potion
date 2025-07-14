using System;
using System.Runtime.Serialization;

[Serializable]
public class ClassData
{
    public int id;
    public string className;

    //excel data
    public string rawBaseStr;
    public string rawBaseDex;
    public string rawBaseInt;
    public string rawBaseHp;
    public string rawIncStr;
    public string rawIncDex;
    public string rawIncInt;
    public string rawIncHp;


    // 기본 스탯
    public int[] baseStr;
    public int[] baseDex;
    public int[] baseInt;
    public int[] baseHp;
    // 증가 스탯
    public int[] incStr;
    public int[] incDex;
    public int[] incInt;
    public int[] incHp;

    [OnDeserialized]
    private void OnDeserialized(StreamingContext context)
    {
        DecodeRawData();
    }

    public StatusData SetBaseStat()
    {
        int str, dex, intel, hp;
        str = SetValue(baseStr);
        dex = SetValue(baseDex);
        intel = SetValue(baseInt);
        hp = SetValue(baseHp);

        StatusData baseStat = new(str, dex, intel, hp);
        return baseStat;
    }

    public StatusData AddIncStat(StatusData curStatus)
    {
        int str, dex, intel, hp;
        str = SetValue(incStr);
        dex = SetValue(incDex);
        intel = SetValue(incInt);
        hp = SetValue(incHp);

        StatusData incStat = new(str, dex, intel, hp);
        return curStatus + incStat;
    }

    private void DecodeRawData()
    {
        baseStr = DecodeRawData(rawBaseStr);
        baseDex = DecodeRawData(rawBaseDex);
        baseInt = DecodeRawData(rawBaseInt);
        baseHp = DecodeRawData(rawBaseHp);
        incStr = DecodeRawData(rawIncStr);
        incDex = DecodeRawData(rawIncDex);
        incInt = DecodeRawData(rawIncInt);
        incHp = DecodeRawData(rawIncHp);
    }

    private int[] DecodeRawData(string input)
    {
        input = input.Replace(" ", "");
        int constant = 0;
        int diceCount = 0;
        int diceSize = 0;

        if (input.Contains("d"))
        {
            
            string[] plusSplit = input.Split('+');
            string[] diceSplit;
            if (plusSplit.Length == 2)
            {
                // d 앞에 '+'가 있는 경우
                constant = int.Parse(plusSplit[0]);
                diceSplit = plusSplit[1].Split('d');
            }
            else
            {
                // "3d6" 처럼 상수 없이 주사위만 있는 경우
                diceSplit = input.Split('d');
            }
            diceCount = int.Parse(diceSplit[0]);
            diceSize = int.Parse(diceSplit[1]);
        }
        else
        {
            // "7"처럼 상수만 있는 경우
            constant = int.Parse(input);
        }

        return new int[] { constant, diceCount, diceSize };
    }

    private int SetValue(int[] arr)
    {
        int value = arr[0];
        int diceCount = arr[1];
        int diceSize = arr[2];

        for (int i = 0; i < diceCount; i++)
        {
            value += UnityEngine.Random.Range(1, diceSize + 1);
        }

        return value;
    }
}