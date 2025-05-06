using System;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : Singleton<QuestManager>
{
    private readonly Dictionary<int, QuestData> questDataDics = new();

    protected override void Awake()
    {
        isDestroyOnLoad = true;
        base.Awake();

        var list = DataManager.Instance.GetObjList<QuestData>("QuestData");
        foreach (var data in list)
        {
            data.Parse();
            questDataDics[data.id] = data;
        }

        EventManager.Instance.AddClickListener(eClickEventType.Quest_Click.ToString() + "L", OnQuestClick);
    }

    private void Start()
    {
        //테스트
        EventManager.Instance.InvokeClickEvent(eClickEventType.Quest_Click.ToString() + "L");
    }

    #region Main Methods
    public QuestData GetQuestData(int id)
    {
        return questDataDics.TryGetValue(id, out var data) ? data : null;
    }

    public void ReceiveQuest(int questIdx)
    {
        var receivedQuests = SaveManager.Instance.MySaveData.receivedQuests;
        receivedQuests.Add(questIdx);
        SaveManager.Instance.SetSaveData(nameof(SaveData.receivedQuests), receivedQuests);
    }
    
    /// <summary>
    /// 예외처리는 밖에서 미리 하고 올 것!!
    /// </summary>
    public void SendQuest(int questId, List<int> heroIds)
    {
        // 퀘스트 수락 리스트에서 제거
        var receivedList = SaveManager.Instance.MySaveData.receivedQuests;
        receivedList.Remove(questId);
        SaveManager.Instance.SetSaveData(nameof(SaveData.receivedQuests), receivedList);

        // 돌아올 날짜 계산
        int today = SaveManager.Instance.MySaveData.day;
        int returnDay = Mathf.Max(today - GetQuestData(questId).time, -1);

        // 퀘스트 진행 리스트에 추가
        var processQ = new QuestProcessInfo
        {
            questId = questId,
            heroIds = heroIds,
            returnDay = returnDay
        };
        var processList = SaveManager.Instance.MySaveData.processingQuests;
        processList.Add(processQ);
        SaveManager.Instance.SetSaveData(nameof(SaveData.processingQuests), processList);

        // 용사 상태 변경
        foreach (var heroId in heroIds)
        {
            if (SaveManager.Instance.MySaveData.ownedHeros.TryGetValue(heroId, out var hero))
                hero.state = EHeroState.QUEST;
            else
                Debug.LogWarning($"존재하지 않는 용사 ID: {heroId}");
        }
    }
    #endregion

    #region Sub Methods
    private void OnQuestClick()
    {
        int rank = SaveManager.Instance.MySaveData.rank;
        int randomQuestId = GetRandomQuest(rank);
        QuestData quest = GetQuestData(randomQuestId);

        // UIOverrideModal 열기
        UIManager.Show<UIOverrideModal>(
            quest.title, // 제목                           
            $"수락 시 {quest.time}일이 소요되고 {quest.reward} 골드를 보상으로 획득합니다.", // 설명
            true, // 확인 버튼 표시
            true, // 취소 버튼 표시
            (Action)(() => ReceiveQuest(quest.id))
        );
    }

    private int GetRandomQuest(int rank)
    {
        var candidates = new List<QuestData>();

        foreach (var quest in questDataDics.Values)
        {
            if (quest.rank == rank)
            {
                candidates.Add(quest);
            }
        }

        if (candidates.Count == 0)
        {
            Debug.LogWarning($"Rank {rank}에 해당하는 퀘스트가 없습니다.");
            return 0; // 실패시
        }

        var randomQ = candidates[UnityEngine.Random.Range(0, candidates.Count)];
        return randomQ.id;
    }
    #endregion

    #region test
#if UNITY_EDITOR
    private void TestPrintFirstQuest()
    {
        if (questDataDics.Count == 0)
        {
            Debug.LogWarning("퀘스트 데이터가 비어 있습니다.");
            return;
        }

        // 첫 번째 퀘스트 가져오기
        var enumerator = questDataDics.Values.GetEnumerator();
        enumerator.MoveNext();
        QuestData quest = enumerator.Current;

        Debug.Log($"=== Quest ID: {quest.id} ===");
        Debug.Log($"Title: {quest.title}");
        Debug.Log($"Rank: {quest.rank}");
        Debug.Log($"Reward: {quest.reward}");
        Debug.Log($"Time: {quest.time}일");
        Debug.Log("Need Specs: [STR, DEX, INT, HP]");
        Debug.Log($"[{string.Join(", ", quest.needSpecs)}]");

        if (quest.parsedDropItems == null || quest.parsedDropItems.Count == 0)
        {
            Debug.Log("DropItems: 없음");
        }
        else
        {
            Debug.Log("DropItems:");
            foreach (var drop in quest.parsedDropItems)
            {
                Debug.Log($" - ID: {drop.id}, Probability: {drop.probability}");
            }
        }
    }
#endif
    #endregion
}