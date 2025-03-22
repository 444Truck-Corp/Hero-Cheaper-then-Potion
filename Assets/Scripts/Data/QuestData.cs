using System;

[Serializable]
public class QuestData
{
    public int id;
    public int difficulty;
    public string description;
    public string target;
    public string[] rewardTypes;
    public int[] rewardValues;
    public int[] needSpecs;
    public int needTime;

    public string QuestName => description.Replace("N", target);
}