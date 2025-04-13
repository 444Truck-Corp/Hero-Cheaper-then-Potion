using System;
using System.Collections.Generic;
using System.Numerics;

[Serializable]
public class SaveData
{
    #region common
    public long lastSaveTime = 0; 
    public int day = 100;
    public float time = 0; //0은 0시, 3분은 24시 (저장 단위 : 초)
    #endregion

    #region gold
    public int gold = 0;
    public int dailyConsume = 0;
    #endregion

    #region guild
    public int guildLevel = 1; //길드 레벨
    
    #endregion

    #region hero 
    public int heroNum = 0; //Hero Id 매기는 변수.
    public Dictionary<BigInteger, HeroData> ownedHeros = new(); //보유 Hero 목록.
    #endregion

    #region inventory
    public Dictionary<int, int> potionCounts = new();
    public List<EquipData> ownedEquips = new();
    #endregion

    #region progress
    public bool isFirstQuest = false;
    #endregion

    public SaveData() { }
}