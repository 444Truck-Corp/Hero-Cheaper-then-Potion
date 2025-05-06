using System.Collections.Generic;

public class HeroManager : Singleton<HeroManager>
{
    private const string nameData = "NameData";

    private List<string> nameList;
    private List<ClassData> classList;
    private List<LvData> lvList;

    protected override void Awake()
    {
        isDestroyOnLoad = true;
        base.Awake();

        // 직업 데이터 캐싱
        classList = DataManager.Instance.GetObjList<ClassData>(nameof(ClassData));
        nameList = DataManager.Instance.GetObjList<string>(nameData);
        lvList = DataManager.Instance.GetObjList<LvData>(nameof(LvData));
    }

    #region Main Methods
    public HeroData SpawnNewHero()
    {
        HeroData hero = CreateNewHero();
        TileMapManager.Instance.SpawnHero(hero);
        SaveManager.Instance.MySaveData.ownedHeros.Add(hero.id, hero);

        return hero;
    }
    #endregion

    #region Sub Methods 
    private HeroData CreateNewHero()
    {
        string name = nameList[UnityEngine.Random.Range(0, nameList.Count)];
        ClassData classData = classList[UnityEngine.Random.Range(0, classList.Count)];

        HeroData hero = new()
        {
            id = SaveManager.Instance.MySaveData.heroNum,
            name = name,
            classData = classData,
            status = classData.BaseStat,
            level = 0,
            exp = 0,
            state = EHeroState.FREE
        };

        SaveManager.Instance.SetSaveData(nameof(SaveManager.Instance.MySaveData.heroNum), hero.id + 1);

        return hero;
    }

    private void TestPrintHero(HeroData newHero)
    {
        UnityEngine.Debug.Log(
            $"=== New Hero Spawned ===\n" +
            $"ID: {newHero.id}\n" +
            $"Name: {newHero.name}\n" +
            $"Class: {newHero.classData.className}\n" + // 필드명: className
            $"Level: {newHero.level}\n" +
            $"EXP: {newHero.exp}\n" +
            $"Status:\n" +
            $" - STR: {newHero.status.STR}\n" +
            $" - DEX: {newHero.status.DEX}\n" +
            $" - INT: {newHero.status.INT}\n" +
            $" - HP: {newHero.status.HP}\n");
    }
    #endregion
}