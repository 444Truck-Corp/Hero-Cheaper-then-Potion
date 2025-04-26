using System;
using System.Collections.Generic;

public enum eHeroState
{
    FREE,
    QUEST
}

public class HeroManager : Singleton<HeroManager>
{
    private List<ClassData> classList;

    private readonly string nameData = "NameData";  
    private List<string> nameList;
    
    public List<LvData> lvList { get; private set; }
    
    protected override void Awake()
    {
        base.Awake();
        isDestroyOnLoad = true;

        // 직업 데이터 Caching
        classList = DataManager.Instance.GetObjList<ClassData>(nameof(ClassData));
        nameList = DataManager.Instance.GetObjList<string>(nameData);
        lvList = DataManager.Instance.GetObjList<LvData>(nameof(LvData));
    }

    #region Main Methods
    public HeroData SpawnNewHero(int id)
    {
        var heroData = CreateNewHero();
        TileMapManager.Instance.SpawnHero(heroData);
        SaveManager.Instance.MySaveData.ownedHeros.Add(id, heroData);

        return heroData;
    }
    #endregion

    #region Sub Methods 
    private HeroData CreateNewHero()
    {
        HeroData hero = new();

        hero.id = SaveManager.Instance.MySaveData.heroNum;
        SaveManager.Instance.SetSaveData(nameof(SaveManager.Instance.MySaveData.heroNum), hero.id + 1);

        hero.name = nameList[UnityEngine.Random.Range(0, nameList.Count)];

        hero.classData = classList[UnityEngine.Random.Range(0, classList.Count)];

        hero.status = hero.classData.BaseStat;

        hero.level = 0;

        hero.exp = 0;

        return hero;
    }
    #endregion
}