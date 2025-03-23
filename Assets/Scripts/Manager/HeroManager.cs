using System;
using System.Collections.Generic;
using System.Linq;

public enum eHeroState
{
    FREE,
    QUEST,
    TRAINING
}

public class HeroManager : Singleton<HeroManager>
{
    private struct Schedule
    {
        public int scheduleType; // -1이면 Training, 0 이상 : QuestIdx.
        public List<int> heroIdxs;
        public int dDay;
        public int successRate;
    }

    public List<HeroData> heroList = new();
    public List<eHeroState> heroStates = new();
    private List<Schedule> scheduleList = new();

    private int heroCount = 0;

    protected override void Awake()
    {
        base.Awake();
        GameManager.Instance.DayChangeAction += CheckScheduleDone;
    }

    public HeroData MakeNewHero()
    {
        HeroData data = new();
        //TODO : SpawnManager에서.
        //data.Initialize(heroCount++);
        heroList.Add(data);
        heroStates.Add(eHeroState.FREE);
        GameManager.Instance.OnFoodChangeEvent();

        GameManager.Instance.OnHeroSpawnEvent?.Invoke(data);
        return data;
    }

    //public void RemoveHero(HeroData hero)
    //{
    //    heroList.Remove(hero);
    //    heroStates.Remove(eHeroState.FREE);
    //}

    public HeroData GetHero(int index)
    {
        return heroList[index];
    }

    public void AddQuestSchedule(List<int> heroIdxs, QuestData quest, int successRate)
    {
        Schedule newS = new()
        {
            scheduleType = quest.id,
            heroIdxs = heroIdxs,
            dDay = GameManager.Instance.Day + quest.needTime,
            successRate = successRate
        };
        scheduleList.Add(newS);

        foreach (int idx in heroIdxs)
        {
            heroStates[idx] = eHeroState.QUEST;
        }

        var heroes = heroList.Where((hero) => heroIdxs.Contains(hero.id));
        GameManager.Instance.OnQuestStartEvent?.Invoke(heroes, quest);
        GameManager.Instance.OnFoodChangeEvent();
    }

    public void AddTrainingSchedule(int heroIdx, int day, int roomId)
    {
        Schedule schedule = new()
        {
            scheduleType = -1,
            heroIdxs = new() { heroIdx },
            dDay = day,
            successRate = roomId
        };

        heroStates[heroIdx] = eHeroState.TRAINING;
    }

    public void CheckTrainingScheduleDone(int heroIdx)
    {
        heroStates[heroIdx] = eHeroState.FREE;
    }


    private readonly (string StatName, Action<StatusData, int> IncreaseMethod)[] statusTuple =
    {
        ("STR", (StatusData status, int value) => { status.STR += value; }),
        ("DEX", (StatusData status, int value) => { status.DEX += value; }),
        ("INT", (StatusData status, int value) => { status.INT += value; }),
        ("LUK", (StatusData status, int value) => { status.LUK += value; }),
    };

    public void CheckScheduleDone(int skipDay)
    {
        for (int index = scheduleList.Count - 1; index >= 0; index--)
        {
            Schedule schedule = scheduleList[index];
            // 훈련일 때
            if (schedule.scheduleType == -1)
            {
                var hero = heroList[schedule.heroIdxs[0]];
                int roomId = schedule.successRate;

                int remainDay = 0; 
                //TODO : SaveManager
                //remainDay = hero.remainDay[roomId];
                int dayDifference = remainDay + GameManager.Instance.Day - schedule.dDay;
                int count = dayDifference / 5; // 5일당 3스탯씩 상승

                //TODO : 저장 쪽에서 불러오기
                //hero.remainDay[roomId] = dayDifference - count * 5;
                statusTuple[roomId].IncreaseMethod(hero.status, count * 3);

                if (count > 0)
                {
                    GameManager.Instance.OnHeroTrainingStatUpEvent?.Invoke(hero, statusTuple[roomId].StatName, count * 3);
                }
            }
            // 퀘스트일 때
            else if (schedule.dDay <= GameManager.Instance.Day)
            {
                bool isSuccess = UnityEngine.Random.Range(0, 100) < schedule.successRate; // 성공 여부
                //QuestData q = DataManager.Instance.GetData<QuestData>(nameof(QuestData), schedule.scheduleType); 분리.

                // 스케쥴 삭제
                scheduleList.RemoveAt(index);

                var heroes = heroList.Where(hero => schedule.heroIdxs.Contains(hero.id));
                GameManager.Instance.OnQuestEndEvent?.Invoke(heroes, q, isSuccess);

                if (isSuccess)
                {
                    GameManager.Instance.OnGoldChangeEvent(q.rewardValues[0]);
                    for (int i = 0; i < schedule.heroIdxs.Count; i++)
                    {
                        //heroList[schedule.heroIdxs[i]].GetExp(q.rewardValues[1]);
                    }
                }
                else
                {
                    for (int i = 0; i < schedule.heroIdxs.Count; i++)
                    {
                        //var hero = heroList[schedule.heroIdxs[i]];
                        //GameManager.Instance.OnHeroDeadEvent?.Invoke(hero);
                        //RemoveHero(hero);
                    }
                }

                for (int i = 0; i < schedule.heroIdxs.Count; i++)
                {
                    heroStates[schedule.heroIdxs[i]] = eHeroState.FREE;
                }
            }
        }

        GameManager.Instance.OnFoodChangeEvent();
    }
}