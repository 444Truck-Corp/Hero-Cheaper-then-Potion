using System;
using System.Collections.Generic;

[Serializable]
public class HeroData
{
    public int id;
    public string name;
    public bool spriteType;
    public ClassData classData;
    public StatusData status;
    public int level;
    public int exp;
    public int spriteIdx;

    public EquipData[] equippedDatas; //현재 착용중인 장비.

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