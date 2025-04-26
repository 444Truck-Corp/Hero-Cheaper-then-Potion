using System;
using System.Collections.Generic;

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
    public HeroData SpawnNewHero()
    {
        var heroData = CreateNewHero();
        TileMapManager.Instance.SpawnHero(heroData);
        SaveManager.Instance.MySaveData.ownedHeros.Add(heroData.id, heroData);

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

        hero.state = eHeroState.FREE;

        return hero;
    }

    private void TestPrintHero(HeroData newHero)
    {
        UnityEngine.Debug.Log($"=== New Hero Spawned ===");
        UnityEngine.Debug.Log($"ID: {newHero.id}");
        UnityEngine.Debug.Log($"Name: {newHero.name}");
        UnityEngine.Debug.Log($"Class: {newHero.classData.className}"); // 필드명: className
        UnityEngine.Debug.Log($"Level: {newHero.level}");
        UnityEngine.Debug.Log($"EXP: {newHero.exp}");
        UnityEngine.Debug.Log($"Status:");
        UnityEngine.Debug.Log($" - STR: {newHero.status.STR}");
        UnityEngine.Debug.Log($" - DEX: {newHero.status.DEX}");
        UnityEngine.Debug.Log($" - INT: {newHero.status.INT}");
        UnityEngine.Debug.Log($" - HP: {newHero.status.HP}");
    }
    #endregion
}