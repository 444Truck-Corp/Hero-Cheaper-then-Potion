using System;
using System.Collections.Generic;
using System.Numerics;

[Serializable]
public class SaveData
{
    #region common
    public int gold = 0;
    public int day = 100;
    public int[] remainDay; // 훈련 후 남은 누적일. 방 4개가 상이
    #endregion

    #region hero 
    public int heroNum = 0; //Hero Id 매기는 변수.
    public Dictionary<BigInteger, HeroData> ownedHeros; //보유 Hero 목록.
    #endregion

    #region inventory
    public Dictionary<int, int> potionCounts;
    public List<EquipData> ownedEquips;
    #endregion

    #region progress
    public bool isFirstQuest = false;
    #endregion

    public SaveData() { }
}