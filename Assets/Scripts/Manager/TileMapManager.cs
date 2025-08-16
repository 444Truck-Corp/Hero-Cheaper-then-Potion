using System;
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
    }

    public EventLocation GetEventLocation(GuildLocationEventType type)
    {
        return _controller.GetEmptyEventLocationByType(type);
    }

    public void ReturnLocation(EventLocation location)
    {
        bool wasWaitingLocation = location.EventType == GuildLocationEventType.Waiting;
        _controller.ReturnLocation(location);
        if (!wasWaitingLocation)
        {
            CheckWaitingQueue();
        }
    }

    private void CheckWaitingQueue()
    {
        if (_waitingCharacters.Count == 0) return;

        var character = _waitingCharacters.Peek();
        EventLocation destination = GetEventLocation(character.TargetType);

        if (destination != null)
        {
            _waitingCharacters.Dequeue();

            EventLocation oldWaitingSpot = character.TargetLocation;

            character.SetTargetTilePosition(destination.TilePosition);
            character.SetTargetLocation(destination);
            character.SetMoveCommand(character.SetOrder);

            UpdateWaitingCharacterPositions(oldWaitingSpot);
        }
    }

    private void UpdateWaitingCharacterPositions(EventLocation freedWaitingSpot)
    {
        if (freedWaitingSpot == null) return;

        foreach (var character in _waitingCharacters)
        {
            var previousSpot = character.TargetLocation;
            character.SetTargetTilePosition(freedWaitingSpot.TilePosition);
            character.SetTargetLocation(freedWaitingSpot);
            character.SetMoveCommand(null);
            freedWaitingSpot = previousSpot;
        }

        if (freedWaitingSpot != null)
        {
            _controller.ReturnLocation(freedWaitingSpot);
        }
    }

    public void GetRoute(Vector2Int start, Vector2Int end, Queue<Vector2Int> route)
    {
        _astar.EnqueueRouteMovementValue(start, end, route);
    }

    public void OnShopEntered()
    {
        _shopCharacter = CreateTileMapCharacter<CharacterShop>(ShopPrefabName, "도적1");
        var location = GetEventLocation(_shopCharacter.TargetType);

        if (location == null)
        {
            _waitingCharacters.Enqueue(_shopCharacter);
            location = GetEventLocation(GuildLocationEventType.Waiting);
            if (location == null)
            {
                PoolManager.Instance.Return(_shopCharacter);
                return;
            }
            _shopCharacter.SetTargetTilePosition(location.TilePosition);
            _shopCharacter.SetTargetLocation(location);
            _shopCharacter.SetMoveCommand(null);
        }
        else
        {
            _shopCharacter.SetTargetTilePosition(location.TilePosition);
            _shopCharacter.SetTargetLocation(location);
            _shopCharacter.SetMoveCommand(_shopCharacter.SetOrder);
        }
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

    public void OnQuestStart(IEnumerable<HeroData> heroDatas, QuestData quest)
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

    public void OnHeroEntered(HeroData heroData)
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

        if (location == null)
        {
            _waitingCharacters.Enqueue(dinerCharacter);
            location = GetEventLocation(GuildLocationEventType.Waiting);
            if (location == null)
            {
                PoolManager.Instance.Return(dinerCharacter);
                return;
            }
            dinerCharacter.SetTargetTilePosition(location.TilePosition);
            dinerCharacter.SetTargetLocation(location);
            dinerCharacter.SetMoveCommand(null);
        }
        else
        {
            dinerCharacter.SetTargetTilePosition(location.TilePosition);
            dinerCharacter.SetTargetLocation(location);
            dinerCharacter.SetMoveCommand(dinerCharacter.SetOrder);
        }
    }

    // TODO: 풀매니저에 돌려놓을 때 자료형에 주의해야 함
    public void OnCharacterExit(TileMapCharacterCore character)
    {
        var locationToRelease = character.TargetLocation;
        
        character.SetTargetTilePosition(_controller.Entrance.TilePosition);

        Action onExitComplete = () =>
        {
            if (locationToRelease != null)
            {
                _controller.ReturnLocation(locationToRelease);
            }
            PoolManager.Instance.Return(character);
        };

        character.SetMoveCommand(onExitComplete);
    }

    public void OnHeroExit(HeroData heroData)
    {
        if (!_heroes.TryGetValue(heroData.id, out var hero)) return;

        var locationToRelease = hero.TargetLocation;
        
        hero.SetTargetTilePosition(_controller.Entrance.TilePosition);

        Action onExitComplete = () =>
        {
            if (locationToRelease != null)
            {
                _controller.ReturnLocation(locationToRelease);
            }
            PoolManager.Instance.Return(hero);
        };

        hero.SetMoveCommand(onExitComplete);
    }

    public void OnQuestEntered()
    {
        var questCharacter = CreateTileMapCharacter<CharacterQuest>(QuestPrefabName, "도적2");
        var location = GetEventLocation(questCharacter.TargetType);

        if (location == null)
        {
            _waitingCharacters.Enqueue(questCharacter);
            location = GetEventLocation(GuildLocationEventType.Waiting);
            if (location == null)
            {
                PoolManager.Instance.Return(questCharacter);
                return;
            }
            questCharacter.SetTargetTilePosition(location.TilePosition);
            questCharacter.SetTargetLocation(location);
            questCharacter.SetMoveCommand(null);
        }
        else
        {
            questCharacter.SetTargetTilePosition(location.TilePosition);
            questCharacter.SetTargetLocation(location);
            questCharacter.SetMoveCommand(questCharacter.SetOrder);
        }
    }
    #endregion
}