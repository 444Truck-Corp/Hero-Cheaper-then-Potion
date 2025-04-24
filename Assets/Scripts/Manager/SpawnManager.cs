using System.Collections.Generic;

public class SpawnManager : Singleton<SpawnManager>
{
    private const string nameData = "NameData";
    //private string[] nameArr;
    private readonly List<int> levelExpList = new() { 1, 2, 5, 6, 6, 8, 8, 10, 11, 12, 12 };

    #region Unity Life Cycles
    /// <summary> Awake 대용 : Manager 실행순서 관리하기 위함. </summary>
    public void Init()
    {
        // 이름 데이터 Caching
    }
    #endregion

    #region Main Methods
    public HeroData SpawnNewHero(int id)
    {
        var heroData = HeroManager.Instance.CreateNewHero();
        TileMapManager.Instance.SpawnHero(heroData);
        SaveManager.Instance.MySaveData.ownedHeros.Add(id, heroData);

        return heroData;
    }
    #endregion
}