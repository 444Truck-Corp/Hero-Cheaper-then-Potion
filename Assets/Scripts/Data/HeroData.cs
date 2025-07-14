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
    public int curHP;
    public int level;
    public int exp;
    public EHeroState state;
    #endregion

    public List<EquipmentData> equipList; //현재 착용중인 장비.

    public HeroData()
    {
        equipList = new();
    }

    public HeroData(HeroData data)
    {
        id = data.id;
        name = data.name;
        classData = data.classData;
        status = data.status;
        curHP = data.curHP;
        level = data.level;
        exp = data.exp;
        state = data.state;
        equipList = new List<EquipmentData>(data.equipList);
    }

    public void GetExp(int value)
    {
        exp += value;

        while (level < HeroManager.Instance.lvList.Count - 1)
        {
            int maxExp = HeroManager.Instance.lvList[level].characExp;
            if (exp < maxExp) break;

            exp -= maxExp;
            level++;
            status = classData.AddIncStat(status);
            status.HP = HeroManager.Instance.lvList[level].hp;
            curHP = status.HP;
        }
    }
}