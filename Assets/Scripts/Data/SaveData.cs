using System;
using System.Collections.Generic;

[Serializable]
public class SaveData
{
    #region common
    public long lastSaveTime = 0;
    public int day = 100;
    public float time = 0; // 0은 0시, 3분은 24시 (저장 단위 : 초)
    public const float maxTime = 60;
    #endregion

    #region gold
    public int gold = 5000;
    #endregion

    #region guild
    public int rank = 1; // 길드 레벨
    public int exp = 0;
    #endregion

    #region hero 
    public int heroNum = 0; // Hero Id 매기는 변수.
    public Dictionary<int, HeroData> ownedHeroes = new(); // 보유 Hero 목록.
    #endregion

    #region inventory
    public List<int> ownedRecipeId = new();
    public int equipNum = 1;
    public Dictionary<int, EquipmentData> ownedEquips = new(); //장비획득id, 장비데이터
    public Dictionary<int, int> items = new(); //id, 수량.
    #endregion

    #region quest
    public List<int> receivedQuests = new(); // 수락한 퀘스트 목록.
    public List<QuestProcessInfo> processingQuests = new(); // 진행중인 퀘스트 목록.
    #endregion

    #region results
    public Dictionary<int, int> foodProfits = new(); //음식 종류별 판매량
    #endregion

    public SaveData() { }

    public void AcquireItem(int id, int count = 1)
    {
        if (id / 10000 == 12) //id가 장비인 경우.
        {
            EquipmentData newData = ItemManager.Instance.EquipmentList[id];
            for (int i = 0; i < count; i++)
                ownedEquips.Add(equipNum++, newData);
            return;
        }

        if (!items.ContainsKey(id)) 
        {
            items[id] = count;
            return;
        }
        items[id] += count;
        if (items[id] <= 0) items.Remove(id);
    }

    /// <param name="id">일반 item인 경우 itemdata id, 장비는 고유id.</param>
    /// <param name="count">제거할 개수, 장비는 1개 고정</param>
    public void RemoveItem(bool isEquip, int id, int count = 1)
    {
        if (isEquip)
        {
            EquipmentData oldData = ownedEquips[id];
            ownedEquips.Remove(id);

            //장착하고 있는 영웅이 있다면 장착해제
            if (oldData.equippedHero != -1)
            {
                SaveManager.Instance.MySaveData.ownedHeroes[oldData.equippedHero].Equip(id);
            }
        }
        else
        {
            items[id] -= count;
            if (items[id] <= 0) items.Remove(id);
        }
    }
}