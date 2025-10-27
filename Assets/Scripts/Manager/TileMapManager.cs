using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HeroPotion
{
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
        private readonly Queue<TileMapCharacterCore> _waitingCharacters = new();

        private Dictionary<TileMapCharacterType, (string prefabName, string textureName)> _characterSpawnData;

        [SerializeField] private Transform _heroParent;
        [SerializeField] private TileMapData _wallTileMap;

        public bool[,] Tiles => _wallTileMap.Tiles;

        protected override void Awake()
        {
            isDestroyOnLoad = true;
        base.Awake();

        List<EventLocation> eventLocations = FindObjectsByType<EventLocation>(FindObjectsSortMode.None).ToList();
        Debug.Log($"Found {eventLocations.Count} event locations.");
        _controller.Initialize(eventLocations);
            _characterSpawnData = new Dictionary<TileMapCharacterType, (string, string)>
            {
                { TileMapCharacterType.Shop, (ShopPrefabName, "도적1") },
                { TileMapCharacterType.Diner, (DinerPrefabName, "도적2") },
                { TileMapCharacterType.Quest, (QuestPrefabName, "도적2") }
            };
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
        
            public void InitializeAStar(bool[,] tiles)
            {
                _astar.SetTiles(tiles);
            }
        private void HandleCharacterEntry(TileMapCharacterCore character)
        {
            var location = GetEventLocation(character.TargetType);

            if (location == null)
            {
                _waitingCharacters.Enqueue(character);
                location = GetEventLocation(GuildLocationEventType.Waiting);
                if (location == null)
                {
                    PoolManager.Instance.Return(character);
                    return;
                }
                character.SetTargetTilePosition(location.TilePosition);
                character.SetTargetLocation(location);
                character.SetMoveCommand(null);
            }
            else
            {
                character.SetTargetTilePosition(location.TilePosition);
                character.SetTargetLocation(location);
                character.SetMoveCommand(character.SetOrder);
            }
        }

        public void CreateAndEnterCharacter<T>(TileMapCharacterType type) where T : TileMapCharacterCore
        {
            var (prefabName, textureName) = _characterSpawnData[type];
            var character = CreateTileMapCharacter<T>(prefabName, textureName);
            HandleCharacterEntry(character);
        }

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

        public void OnHeroExit(HeroData heroData)
        {
            if (!_heroes.TryGetValue(heroData.id, out var hero)) return;

            var locationToRelease = hero.TargetLocation;

            hero.SetTargetTilePosition(_controller.Entrance.TilePosition);


            hero.SetMoveCommand(() =>
            {
                if (locationToRelease != null)
                {
                    _controller.ReturnLocation(locationToRelease);
                }
                PoolManager.Instance.Return(hero);
            });
        }
        #endregion
    }
}