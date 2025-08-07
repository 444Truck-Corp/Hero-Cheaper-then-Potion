using System;

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
    public StatusData equipStatus;
    public int curHP;
    public int maxHP;
    public int level;
    public int exp;
    public EHeroState state;
    #endregion

    public int[] equipList; //현재 착용중인 장비 : EEquipType 순.

    public HeroData()
    {
        equipList = new int[5];
    }

    public HeroData(HeroData data)
    {
        id = data.id;
        name = data.name;
        classData = data.classData;
        status = data.status;
        equipStatus = data.equipStatus;
        curHP = data.curHP;
        level = data.level;
        exp = data.exp;
        state = data.state;
        equipList = data.equipList;
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
            maxHP = HeroManager.Instance.lvList[level].hp;
            curHP = maxHP;
        }
    }

    public StatusData ResultStatus()
    {
        return status + equipStatus;
    }

    public void Equip(int equipId)
    {
        EquipmentData curData = SaveManager.Instance.MySaveData.ownedEquips[equipId];
        int partIdx = (int)curData.parts;
        int prevEquipId = equipList[partIdx];

        // 본인이 이미 착용한 경우 → 해제
        if (curData.equippedHero == id)
        {
            equipList[partIdx] = 0;
            curData.equippedHero = -1;
            equipStatus -= curData.EquipmentStat;
            return;
        }

        // 다른 영웅이 착용 중인 경우 → 해당 영웅에서 해제
        if (curData.equippedHero != -1)
        {
            SaveManager.Instance.MySaveData.ownedHeroes[curData.equippedHero].Equip(equipId);
        }

        // 기존 부위에 착용 중인 장비가 있으면 해제
        if (prevEquipId != 0)
        {
            EquipmentData prevData = SaveManager.Instance.MySaveData.ownedEquips[prevEquipId];
            equipStatus -= prevData.EquipmentStat;
            prevData.equippedHero = -1;
        }

        // 현재 장비 착용
        equipList[partIdx] = equipId;
        curData.equippedHero = id;
        equipStatus += curData.EquipmentStat;
    }
}