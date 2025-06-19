using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileMapManager : Singleton<TileMapManager>
{
    private const string TileMapCharacterPrefabPath = "Prefabs/TileMapCharacter/";
    private const string DinerPrefabName = "Diner";
    private const string QuestPrefabName = "Quest";
    private const string ShopPrefabName = "Shop";
    private const string HeroPrefabName = "Hero";

    private readonly AStar _astar = new();
    private readonly TileMapEventLocationController _controller = new();
    private readonly Dictionary<int, CharacterHero> _heroes = new();
    private readonly Dictionary<int, TileMapCharacterCore> _npcs = new();
    private readonly Queue<TileMapCharacterCore> _waitingCharacters = new();

    [SerializeField] private Transform _heroParent;
    [SerializeField] private TileMapData _wallTileMap;
    [SerializeField] private CharacterShop _shopCharacter;

    public bool[,] Tiles => _wallTileMap.Tiles;

    protected override void Awake()
    {
        isDestroyOnLoad = true;
        base.Awake();

        _astar.SetTiles(_wallTileMap.Tiles);

        // 이벤트 위치 설정
        List<EventLocation> eventLocations = FindObjectsByType<EventLocation>(FindObjectsSortMode.None).ToList();
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

    private void FixedUpdate()
    {
        int count = _waitingCharacters.Count;
        if (count > 0)
        {
            var character = _waitingCharacters.Peek();
            EventLocation dinerLocation = GetEventLocation(character.TargetType);
            if (dinerLocation == null)
            {
                return;
            }
            _waitingCharacters.Dequeue();
            character.SetTargetTilePosition(dinerLocation.TilePosition);
            character.SetMoveCommand(character.SetOrder);
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
        _astar.EnqueueRouteMovementValue(start, end, route);
    }

    public void OnShopEntered()
    {
        _shopCharacter = CreateTileMapCharacter<CharacterShop>(ShopPrefabName, "도적1");
        var location = GetEventLocation(_shopCharacter.TargetType);
        _shopCharacter.SetTargetTilePosition(location.TilePosition);
        _shopCharacter.SetMoveCommand(_shopCharacter.SetOrder);
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

    private T CreateTileMapCharacter<T>(string prefabName, string textureName) where T : TileMapCharacterCore
    {
        T character = PoolManager.Instance.Get<T>(TileMapCharacterPrefabPath + prefabName, _heroParent, _controller.Entrance.transform.localPosition);
        character.Initialize(textureName);
        return character;
    }

    private void CreateTileMapHeroCharacter(HeroData heroData)
    {
        // 일부 캐릭터 스프라이트가 없는 관계로 궁사2로 통일
        _heroes[heroData.id] = CreateTileMapCharacter<CharacterHero>(HeroPrefabName, "궁사2");
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
        hero.transform.localPosition = _controller.Entrance.transform.localPosition;
        hero.Clear();
        hero.gameObject.SetActive(true);
    }

    public void SpawnHero(HeroData heroData)
    {
        CreateTileMapHeroCharacter(heroData);
        OnHeroEntered(heroData);
    }

    public void OnDinerEntered()
    {
        var dinerCharacter = CreateTileMapCharacter<CharacterDiner>(DinerPrefabName, "도적2");
        var location = GetEventLocation(dinerCharacter.TargetType);

        // 앉을 자리가 없다면 대기
        if (location == null)
        {
            _waitingCharacters.Enqueue(dinerCharacter);
            location = GetEventLocation(GuildLocationEventType.Waiting);
            if (location == null)
            {
                return;
            }
        }

        dinerCharacter.SetTargetTilePosition(location.TilePosition);
        dinerCharacter.SetMoveCommand(dinerCharacter.SetOrder);
    }

    // TODO: 풀매니저에 돌려놓을 때 자료형에 주의해야 함
    public void OnDinerCharacterExited(TileMapCharacterCore character)
    {
        character.SetTargetTilePosition(_controller.Entrance.TilePosition);
        character.SetMoveCommand(() => PoolManager.Instance.Return(character));
    }

    private void OnHeroExit(HeroData heroData)
    {
        if (!_heroes.TryGetValue(heroData.id, out var hero)) return;
        hero.SetTargetTilePosition(_controller.Entrance.TilePosition);
        hero.SetMoveCommand(() => PoolManager.Instance.Return(hero));
    }

    public void OnQuestEntered()
    {
        var questCharacter = CreateTileMapCharacter<CharacterQuest>(QuestPrefabName, "도적2");
        var location = GetEventLocation(questCharacter.TargetType);

        // 앉을 자리가 없다면 대기
        if (location == null)
        {
            _waitingCharacters.Enqueue(questCharacter);
            location = GetEventLocation(GuildLocationEventType.Waiting);
            if (location == null)
            {
                return;
            }
        }

        questCharacter.SetTargetTilePosition(location.TilePosition);
        questCharacter.SetMoveCommand(questCharacter.SetOrder);
    }
    #endregion
}