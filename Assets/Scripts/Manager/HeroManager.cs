using System.Collections.Generic;
using System.Linq;

public class HeroManager : Singleton<HeroManager>
{
    private const string nameData = "NameData";

    private List<string> nameList;
    public List<ClassData> classList { get; private set; }
    public List<LvData> lvList { get; private set; }
    public int MaxExp => lvList.Sum(data => data.characExp);

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
        SaveManager.Instance.MySaveData.ownedHeroes.Add(hero.id, hero);

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
            status = classData.SetBaseStat(),
            maxHP = lvList[1].hp,
            curHP = lvList[1].hp,
            level = 1,
            state = EHeroState.FREE,
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
            $"Status:\n" +
            $" - STR: {newHero.status.STR}\n" +
            $" - DEX: {newHero.status.DEX}\n" +
            $" - INT: {newHero.status.INT}\n");
    }
    #endregion
}