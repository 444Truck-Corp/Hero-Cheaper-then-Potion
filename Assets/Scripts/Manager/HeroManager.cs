using HeroPotion;
using System.Collections.Generic;
using System.Linq;

public class HeroManager : Singleton<HeroManager>
{
    public int MaxExp => LevelList.Sum(data => data.characExp);

    private const string NAME_DATA = "NameData";

    public List<ClassData> ClassList { get; private set; }
    public List<LvData> LevelList { get; private set; }

    private List<string> _nameList;

    protected override void Awake()
    {
        isDestroyOnLoad = true;
        base.Awake();

        // 직업 데이터 캐싱
        ClassList = DataManager.Instance.GetObjList<ClassData>(nameof(ClassData));
        _nameList = DataManager.Instance.GetObjList<string>(NAME_DATA);
        LevelList = DataManager.Instance.GetObjList<LvData>(nameof(LvData));
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
        string name = _nameList[UnityEngine.Random.Range(0, _nameList.Count)];
        ClassData classData = ClassList[UnityEngine.Random.Range(0, ClassList.Count)];

        HeroData hero = new()
        {
            id = SaveManager.Instance.MySaveData.heroNum,
            name = name,
            classData = classData,
            status = classData.SetBaseStat(),
            maxHP = LevelList[1].hp,
            curHP = LevelList[1].hp,
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