using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileMapManager : DestructibleSingleton<TileMapManager>
{
    private const string TileMapCharacterPrefabPath = "Prefabs/TileMapCharacter";

    private readonly AStar _astar = new();
    private readonly TileMapEventLocationController _controller = new();
    private readonly Dictionary<int, TileMapCharacterCore> _heroes = new();
    private readonly Dictionary<int, TileMapCharacterCore> _npcs = new();

    [SerializeField] private Transform _heroParent;
    [SerializeField] private TileMapData _wallTileMap;
    [SerializeField] private TileMapCharacterCore _shopCharacter;

    private int npcCount = 0;

    public bool[,] Tiles => _wallTileMap.Tiles;

    protected override void Awake()
    {
        base.Awake();
        _astar.SetTiles(_wallTileMap.Tiles);

        // 이벤트 위치 설정
        List<EventLocation> eventLocations = FindObjectsByType<EventLocation>(FindObjectsSortMode.None).ToList();
        Debug.Log($"Found EventLocation {eventLocations.Count}");
        _controller.Initialize(eventLocations);

        // 캐릭터 초기화
        foreach (var hero in _heroes)
        {
            hero.Value.Clear();
        }

        foreach (var npc in _npcs)
        {
            npc.Value.Clear();
        }
    }

    public EventLocation GetEventLocation(GuildLocationEventType type)
    {
        return _controller.GetEmptyEventLocationByType(type);
    }

    public void ReturnLocation(EventLocation location)
    {
        _controller.ReturnLocation(location);
    }

    public void GetRoute(Vector2Int start, Vector2Int end, Queue<Vector2Int> route)
    {
        Debug.Log("루트를 가져옵니다.");
        _astar.EnqueueRouteMovementValue(start, end, route);
    }

    public void OnShopCharacterEntered()
    {
        _shopCharacter = CreateTileMapCharacter("", GuildLocationEventType.Shop);
    }

    #region Hero
    private void OnHeroDead(HeroData heroData)
    {
        if (_heroes.TryGetValue(heroData.id, out var heroCharacter))
        {
            _heroes[heroData.id] = null;
            heroCharacter.Clear();
            PoolManager.Instance.Return(heroCharacter);
        }
    }

    private TileMapCharacterCore CreateTileMapCharacter(string textureName, GuildLocationEventType type = GuildLocationEventType.None)
    {
        TileMapCharacterCore character = PoolManager.Instance.Get<TileMapCharacterCore>(TileMapCharacterPrefabPath, _heroParent, _controller.EntrancePosition);
        character.Initialize(textureName, type);
        return character;
    }

    private void CreateTileMapHeroCharacter(HeroData heroData)
    {
        _heroes[heroData.id] = CreateTileMapCharacter("궁사2", GuildLocationEventType.QuestBoard);
    }

    private void CreateTileMapNPCCharacter()
    {
        string textureName = "";
        _npcs[npcCount++] = CreateTileMapCharacter(textureName, GuildLocationEventType.QuestBoard);
    }

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
        hero.transform.localPosition = _controller.EntrancePosition;
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
        hero.SetTargetTilePosition(_controller.EntranceTilePosition);
        hero.SetMoveCommand(() => hero.gameObject.SetActive(false));
    }
    #endregion
}