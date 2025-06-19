using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum TileMapEventType
{
    Shop, // 상점
    Diner, // 식사하는 방문객
    Quest, // 퀘스트 의뢰자
}

public class TileMapEventController : MonoBehaviour
{
    // 게임 내 한 시간 (7.5s)
    private static readonly float oneHourTime = SaveData.maxTime / 24;
    // 게임 내 10분 (1.25s), 10분의 개수를 계산하기 위한 역수(0.8f)
    private static readonly float tenMinutesTimeDivider = 1 / (oneHourTime / 6);
    private static readonly float maxEnterTime = SaveData.maxTime - 20.0f;

    private int _lastUpdatedDay = int.MaxValue;
    private int _lastUpdatedTenMinutes;
    private float _time;

    // 10분마다 발생할 이벤트
    private readonly SortedDictionary<int, Queue<TileMapEventType>> _eventQueueDictionary = new();

    private void FixedUpdate()
    {
        // 날짜 변경 감지
        if (_lastUpdatedDay > SaveManager.Instance.MySaveData.day)
        {
            Debug.Log($"날짜 변경 {_lastUpdatedDay} => {SaveManager.Instance.MySaveData.day}");
            InitializeDailyEvent();
            ReturnHeroes();
            _lastUpdatedDay = SaveManager.Instance.MySaveData.day;
            _time = 0.0f;
            _lastUpdatedTenMinutes = 0;
        }

        // 게임 내 10분마다 이벤트를 진행
        int currentTenMinutesCount = (int)(_time * tenMinutesTimeDivider);
        if (currentTenMinutesCount > _lastUpdatedTenMinutes)
        {
            // 이벤트 큐 처리
            if (_eventQueueDictionary.TryGetValue(currentTenMinutesCount, out Queue<TileMapEventType> tilemapEvents))
            {
                while (tilemapEvents.Count > 0)
                {
                    TileMapEventType eventType = tilemapEvents.Dequeue();
                    ProcessEvent(eventType);
                }
            }
            _lastUpdatedTenMinutes = currentTenMinutesCount;
        }
        _time += Time.fixedDeltaTime;
    }

    private void InitializeDailyEvent()
    {
        string debugString = "일일 이벤트 초기화\n";
        // 이벤트 초기화
        foreach (var (id, queue) in _eventQueueDictionary)
        {
            queue.Clear();
        }

        // 상점 방문 이벤트 추가
        int randomTenMinutesCount = GetRandomTenMinutesCount(oneHourTime * 18, maxEnterTime);
        debugString += $"상인 방문 시간: {randomTenMinutesCount / 6:D2}시 {randomTenMinutesCount % 6 * 10:D2}분\n";
        AddEvent(randomTenMinutesCount, TileMapEventType.Shop);

        int count = 20;
        debugString += $"식사 방문 예정 인원: {count}명\n";
        while (count-- > 0)
        {
            randomTenMinutesCount = GetRandomTenMinutesCount(0.0f, maxEnterTime);
            AddEvent(randomTenMinutesCount, TileMapEventType.Diner);
            debugString += $"{randomTenMinutesCount / 6:D2}시 {randomTenMinutesCount % 6 * 10:D2}분\n";
        }

        count = 5;
        debugString += $"퀘스트 의뢰자 방문 예정 인원: {count}명\n";
        while (count-- > 0)
        {
            randomTenMinutesCount = GetRandomTenMinutesCount(0.0f, maxEnterTime);
            AddEvent(randomTenMinutesCount, TileMapEventType.Quest);
            debugString += $"{randomTenMinutesCount / 6:D2}시 {randomTenMinutesCount % 6 * 10:D2}분\n";
        }
        Debug.Log(debugString);
    }

    private void ReturnHeroes()
    {
        var heroIds = SaveManager.Instance.MySaveData.processingQuests
            .Where(quest => quest.returnDay == SaveManager.Instance.MySaveData.day)
            .SelectMany(quest => quest.heroIds);
        foreach (var id in heroIds)
        {
            if (SaveManager.Instance.MySaveData.ownedHeroes.TryGetValue(id, out HeroData hero))
            {
                TileMapManager.Instance.OnHeroEntered(hero);
            }
        }
    }

    private int GetRandomTenMinutesCount(float minTime, float maxTime)
    {
        float time = Random.Range(minTime, maxTime);
        return (int)(time * tenMinutesTimeDivider);
    }

    private void AddEvent(int time, TileMapEventType type)
    {
        if (!_eventQueueDictionary.TryGetValue(time, out Queue<TileMapEventType> queue))
        {
            queue = new Queue<TileMapEventType>();
            _eventQueueDictionary[time] = queue;
        }

        queue.Enqueue(type);
    }

    private void ProcessEvent(TileMapEventType type)
    {
        switch (type)
        {
            case TileMapEventType.Diner:
                TileMapManager.Instance.OnDinerEntered();
                break;
            case TileMapEventType.Quest: 
                TileMapManager.Instance.OnQuestEntered();
                break;
            case TileMapEventType.Shop:
                TileMapManager.Instance.OnShopEntered();
                break;
        }
    }
}