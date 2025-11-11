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
    public StatusData potionStatus;
    public int curHP;
    public int maxHP;
    public int level;
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
        state = data.state;
        equipList = data.equipList;
    }

    public void GetLevel(int value)
    {
        level += value;
        maxHP = HeroManager.Instance.LevelList[level].hp;
        curHP = maxHP;

        for (int i = 0; i < value; i++)
            status = classData.AddIncStat(status);
    }

    public StatusData ResultStatus()
    {
        return status + equipStatus + potionStatus;
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

    public void UsePotion(ItemData data)
    {
        // 포션 제거
        SaveManager.Instance.MySaveData.RemoveItem(false, data.id);

        switch (data.id)
        {
            case 140001:
                curHP += 10;
                if (curHP > maxHP) curHP = maxHP;

                //테스트 코드 : 삭제필요.
                potionStatus.STR += 1;
                potionStatus.DEX += 1;
                potionStatus.INT += 1;
                break;
            case 140002:
                curHP += 20;
                if (curHP > maxHP) curHP = maxHP;
                break;
            case 140003:
                curHP += 30;
                if (curHP > maxHP) curHP = maxHP;
                break;
            case 140004:
                curHP += 40;
                if (curHP > maxHP) curHP = maxHP;
                break;
            case 140005:
                curHP += 50;
                if (curHP > maxHP) curHP = maxHP;
                break;
            case 140006:
                curHP += 60;
                if (curHP > maxHP) curHP = maxHP;
                break;
            case 140007:
                curHP += 70;
                if (curHP > maxHP) curHP = maxHP;
                break;
            case 140008:
                curHP += 80;
                if (curHP > maxHP) curHP = maxHP;
                break;
            case 140009:
                curHP += maxHP / 2; //체력 50% 회복
                if (curHP > maxHP) curHP = maxHP;
                break;
            case 140010:
                curHP = maxHP; // 최대 체력 회복
                break;
            default:
                potionStatus.STR += 0; // 포션에 따라 상태 증가 로직 추가 필요
                break;
        }
    }
}