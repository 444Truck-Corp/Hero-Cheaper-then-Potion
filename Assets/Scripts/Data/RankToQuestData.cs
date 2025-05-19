using System;

[Serializable]
public class RankToQuestData
{
    public int level;
    public float rank1;
    public float rank2;
    public float rank3;
    public float rank4;
    public float rank5;
    public float rank6;
    public float rank7;
    public float rank8;
    public float rank9;
    public float rank10;

    public float[] probabilities = new float[10];

    public void Parse()
    {
        probabilities[0] = rank1;
        probabilities[1] = rank2;
        probabilities[2] = rank3;
        probabilities[3] = rank4;
        probabilities[4] = rank5;
        probabilities[5] = rank6;
        probabilities[6] = rank7;
        probabilities[7] = rank8;
        probabilities[8] = rank9;
        probabilities[9] = rank10;
    }
}
