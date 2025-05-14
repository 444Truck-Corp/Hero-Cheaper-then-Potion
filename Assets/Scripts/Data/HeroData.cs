using System;
using System.Collections.Generic;

public enum EHeroState
{
    FREE,
    QUEST
}

[Serializable]
public class HeroData
{
    #region HeroManager 초기화 영역
    public int id;
    public string name;
    public ClassData classData;
    public StatusData status;
    public int level;
    public int exp;
    public EHeroState state;
    #endregion

    public List<EquipmentData> equipList; //현재 착용중인 장비.

    public HeroData()
    {
        equipList = new();
    }

    //TODO : 위치 이동
    //public void GetExp(int delta)
    //{
    //    if (delta == 0) return;
    //    exp += delta;
    //    GameManager.Instance.OnGetExpEvent?.Invoke(this, delta);
    //    while (exp >= levelExpList[level] && level < 10)
    //    {
    //        exp -= levelExpList[level++];
    //        status += classData.IncStat;
    //        GameManager.Instance.OnHeroLevelUpEvent?.Invoke(this, level);
    //        GameManager.Instance.OnHeroStatUpEvent?.Invoke(this, classData.IncStat);
    //    }
    //}
}