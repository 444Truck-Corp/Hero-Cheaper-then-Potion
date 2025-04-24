using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileMapManager : Singleton<TileMapManager>
{
    private const string TileMapCharacterPrefabPath = "Prefabs/TileMapCharacter";

    private readonly AStar _astar = new();
    private readonly Dictionary<int, TileMapCharacter> _heroes = new();
    private readonly Dictionary<int, TileMapCharacter> _npcs = new();
    
    [SerializeField] private TileMapData _wallTileMap;
    [SerializeField] private Transform _heroParent;
    [SerializeField] private List<EventLocation> _locations;

    private Vector2Int _entranceTilePosition;
    private Vector3 _entranceWorldPosition;

    public bool[,] Tiles => _wallTileMap.Tiles;

    protected override void Awake()
    {
        base.Awake();
        _locations = FindObjectsByType<EventLocation>(FindObjectsSortMode.None).ToList();
        _astar.SetTiles(_wallTileMap.Tiles);

        // 입장 위치 설정
        var entrance = _locations.Find(location => location.EventType.Equals(GuildLocationEventType.Entrance));
        _entranceWorldPosition = entrance.transform.localPosition;
        _entranceTilePosition = new Vector2Int((int)_entranceWorldPosition.x, -(int)_entranceWorldPosition.y);

        // 캐릭터 초기화
        foreach (var hero in _heroes)
        {
            hero.Value.Clear();
        }

        foreach (var npc in _npcs)
        {
            npc.Value.Clear();
        }

        // 이벤트 초기화
        //GameManager.Instance.OnHeroSpawnEvent += OnHeroSpawn;
        //GameManager.Instance.OnHeroDeadEvent += OnHeroDead;
        //GameManager.Instance.OnQuestEndEvent += OnQuestEnd;
        //GameManager.Instance.OnQuestStartEvent += OnQuestStart;
    }

    public EventLocation GetEmptyLocation()
    {
        
        int count = _locations.Count;
        if (count > 0)
        {
            int random = Random.Range(0, count);
            EventLocation location = _locations[random];
            _locations.RemoveAt(random);
            return location;
        }
        return null;
    }

    public void ReturnLocation(EventLocation location)
    {
        if (!_locations.Contains(location))
        {
            _locations.Add(location);
        }
    }

    public List<Vector2Int> GetRoute(Vector2Int start, Vector2Int end)
    {
        return _astar.GetRouteMovementValue(start, end);
    }

    private void OnHeroDead(HeroData heroData)
    {
        if (_heroes.TryGetValue(heroData.id, out var heroCharacter))
        {
            _heroes[heroData.id] = null;
            heroCharacter.Clear();
            PoolManager.Instance.Return(heroCharacter);
        }
    }

    private TileMapCharacter CreateTileMapCharacter(string textureName)
    {
        var character = PoolManager.Instance.Get<TileMapCharacter>(TileMapCharacterPrefabPath, _heroParent, _entranceWorldPosition);
        character.Initialize(textureName);
        return character;
    }

    private void CreateTileMapHeroCharacter(HeroData heroData)
    {
        _heroes[heroData.id] = CreateTileMapCharacter(heroData.classData.className + "1");
    }

    private void CreateTileMapNPCCharacter()
    {
        var textureName = "";
        _npcs[npcCount++] = CreateTileMapCharacter(textureName);
    }

    private int npcCount = 0;

    private void OnQuestStart(IEnumerable<HeroData> heroDatas, QuestData quest)
    {
        foreach (var heroData in heroDatas)
        {
            OnHeroExit(heroData);
        }
    }

    private void OnQuestEnd(IEnumerable<HeroData> heroDatas, QuestData quest, bool isSuccess)
    {
        foreach (HeroData heroData in heroDatas)
        {
            OnHeroEntered(heroData);
        }
    }

    private void OnHeroEntered(HeroData heroData)
    {
        var hero = _heroes[heroData.id];
        hero.transform.localPosition = _entranceWorldPosition;
        hero.Clear();
        hero.gameObject.SetActive(true);
    }

    public void SpawnHero(HeroData heroData)
    {
        CreateTileMapHeroCharacter(heroData);
        OnHeroEntered(heroData);
    }

    private void OnHeroExit(HeroData heroData)
    {
        if (!_heroes.TryGetValue(heroData.id, out var hero)) return;
        var route = _astar.GetRouteMovementValue(hero.Position, _entranceTilePosition);
        hero.SetMoveCommand(route, () => hero.gameObject.SetActive(false));
    }
}