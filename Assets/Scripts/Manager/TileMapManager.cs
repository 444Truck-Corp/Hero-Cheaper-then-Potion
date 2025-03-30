using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileMapManager : Singleton<TileMapManager>
{
    //[SerializeField] private TileMapData wallTileMap;
    //[SerializeField] private Transform heroParent;
    //[SerializeField] private GameObject entryPointObject;

    //public bool[,] Tiles => wallTileMap.Tiles;
    //private Vector2Int doorPosition;
    //private AStar astar;
    //private Dictionary<int, HeroPresenter> heroes;
    //[SerializeField] private List<EventLocation> locations;

    //protected override void Awake()
    //{
    //    base.Awake();
    //    astar = new AStar();
    //    doorPosition = new Vector2Int((int)entryPointObject.transform.localPosition.x, -(int)entryPointObject.transform.localPosition.y);
    //    heroes = new Dictionary<int, HeroPresenter>();

    //    GameManager.Instance.OnHeroSpawnEvent += OnHeroSpawn;
    //    GameManager.Instance.OnHeroDeadEvent += OnHeroDead;
    //    GameManager.Instance.OnQuestEndEvent += OnQuestEnd;
    //    GameManager.Instance.OnQuestStartEvent += OnQuestStart;
    //}

    //private void Start()
    //{
    //    InitializeEventLocations();
    //    astar.SetTiles(wallTileMap.Tiles);
    //}

    //private void InitializeEventLocations()
    //{
    //    locations = FindObjectsByType<EventLocation>(FindObjectsSortMode.None).ToList();
    //}

    //public EventLocation GetEmptyLocation()
    //{
    //    int count = locations.Count;
    //    if (count > 0)
    //    {
    //        int random = Random.Range(0, count);
    //        EventLocation location = locations[random];
    //        locations.RemoveAt(random);
    //        return location;
    //    }
    //    return null;
    //}

    //public void ReturnLocation(EventLocation location)
    //{
    //    if (!locations.Contains(location))
    //    {
    //        locations.Add(location);
    //    }
    //}

    //public List<Vector2Int> GetRoute(Vector2Int start, Vector2Int end)
    //{
    //    return astar.GetRouteMovementValue(start, end);
    //}

    //private void OnHeroDead(HeroData heroData)
    //{
    //    if (heroes.TryGetValue(heroData.id, out HeroPresenter hero))
    //    {
    //        heroes[heroData.id] = null;
    //        hero.Clear();
    //        PoolManager.Instance.Return(hero);
    //    }
    //}

    //private void OnHeroSpawn(HeroData heroData)
    //{
    //    HeroPresenter hero = PoolManager.Instance.Get<HeroPresenter>("Prefabs/HeroPresenter", heroParent, entryPointObject.transform.localPosition);
    //    string path = DataManager.Instance.GetCharacterSheetPath(heroData.spriteIdx);
    //    hero.Initialize(path);
    //    heroes[heroData.id] = hero;
    //}

    //private void OnQuestStart(IEnumerable<HeroData> heroDatas, QuestData quest)
    //{
    //    foreach (HeroData heroData in heroDatas)
    //    {
    //        OnHeroExit(heroData);
    //    }
    //}

    //private void OnQuestEnd(IEnumerable<HeroData> heroDatas, QuestData quest, bool isSuccess)
    //{
    //    foreach (HeroData heroData in heroDatas)
    //    {
    //        OnHeroEntered(heroData);
    //    }
    //}

    //private void OnHeroEntered(HeroData heroData)
    //{
    //    var hero = heroes[heroData.id];
    //    hero.transform.localPosition = entryPointObject.transform.localPosition;
    //    hero.Clear();
    //    hero.gameObject.SetActive(true);
    //}

    //private void OnHeroExit(HeroData heroData)
    //{
    //    if (!heroes.TryGetValue(heroData.id, out HeroPresenter hero)) return;
    //    List<Vector2Int> route = astar.GetRouteMovementValue(hero.Position, doorPosition);
    //    hero.SetMoveCommand(route, () => hero.gameObject.SetActive(false));
    //}
}