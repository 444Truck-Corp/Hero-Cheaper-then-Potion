using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json;

public class SpawnManager : Singleton<SpawnManager>
{
    private const string nameData = "NameData";
    //private string[] nameArr;
    private List<ClassData> classLists;
    private readonly List<int> levelExpList = new() { 1, 2, 5, 6, 6, 8, 8, 10, 11, 12, 12 };

    #region Unity Life Cycles
    /// <summary> Awake 대용 : Manager 실행순서 관리하기 위함. </summary>
    public void Init()
    {
        ////이름 데이터 Caching
        //string rawData = DataManager.Instance.GetRawDataList(nameData);
        //nameArr = JsonConvert.DeserializeObject<string[]>(rawData);

        //직업 데이터 Caching
        classLists = DataManager.Instance.GetObjList<ClassData>(nameof(ClassData));
    }

    #endregion

    #region Main Methods
    //TODO: 하나만 생성하는거나 기존거 생성하는 메서드 필요할듯 이게 없어져도 되고.
    public List<HeroData> SpawnNewHeros(int count)
    {
        List<HeroData> heroes = new(count);

        foreach (var hero in heroes)
        {
            //TODO : id 설정.
            //hero.name = nameArr[Random.Range(0, nameArr.Length)];
            hero.classData = classLists[Random.Range(0, classLists.Count)];
            hero.status = hero.classData.BaseStat;
            
            //수정 필요 : 너무 하드코딩임
            hero.spriteType = Random.Range(0, 2) == 0;
            hero.spriteIdx = hero.classData.id * 2;
            hero.spriteIdx -= (hero.spriteType) ? 1 : 2;
        }

        return heroes;
    }
    #endregion
}