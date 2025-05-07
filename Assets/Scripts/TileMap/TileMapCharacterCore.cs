using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class TileMapCharacterCore : Poolable
{
    private const string PATH = "Textures/CharacterSheet/";

    [Header("Movement")]
    [SerializeField] private bool _isMoving;
    [SerializeField] private float _movementFrequency;
    [SerializeField] private float _movementSpeed = 5;
    [SerializeField] private float _time;
    [SerializeField] private string _texturePath;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private CustomAnimator _animator;
    [SerializeField] private Direction _direction;
    [SerializeField] private float _movementProgress;
    
    [SerializeField] private Emotion _emotion;

    [Header("AI")]
    [SerializeField] private Vector3 _lastPosition;
    [SerializeField] private Vector3 _targetStepPosition;
    [SerializeField] private EventLocation _targetLocation;
    [SerializeField] private GuildLocationEventType _targetType;
    [SerializeField] private Action _onMoveComplete;

    private readonly Queue<Vector2Int> _moveCommand = new();
    private bool _canAutoFinding;

    public Vector2Int Position => new((int)_targetStepPosition.x, (int)-_targetStepPosition.y);

    public GuildLocationEventType TargetType => _targetType;

    private void Reset()
    {
        _movementSpeed = 5;
    }

    private void FixedUpdate()
    {
        // 이동중일 때
        if (_isMoving)
        {
            _movementProgress += _movementSpeed * Time.fixedDeltaTime;
            transform.localPosition = Vector3.Lerp(_lastPosition, _targetStepPosition, _movementProgress);
            if (_movementProgress > 1.0f)
            {
                transform.localPosition = _targetStepPosition;
                _movementProgress -= 1.0f;
                _isMoving = false;
                if (_moveCommand.Count == 0)
                {
                    _onMoveComplete?.Invoke();
                    _time = UnityEngine.Random.Range(0.0f, 3.0f);
                    _animator.SetPlaying(false);
                }
            }
        }
        // 이동하지 않을 때
        else
        {
            if (_moveCommand.Count > 0)
            {
                UpdateStep();
            }
            else if (_moveCommand.Count == 0 && _canAutoFinding)
            {
                FindNewTargetLocation();
            }
            else
            {
                _animator.SetPlaying(false);
            }
        }
        UpdateAnimation();
    }

    public void Initialize(string textureName, GuildLocationEventType targetType = GuildLocationEventType.None, bool canAutoFinding = true)
    {
        _texturePath = string.IsNullOrEmpty(textureName) ? "궁사2" : textureName;
        _animator = new CustomAnimator(PATH + textureName, 9, true, true);
        _targetType = targetType;
        if (targetType != GuildLocationEventType.None)
        {
            _canAutoFinding = canAutoFinding;
        }
        Clear();
    }

    public void Clear()
    {
        _direction = new Direction(2);
        _targetStepPosition = transform.localPosition;
        ReturnLocation();
        _moveCommand.Clear();
        _movementProgress = 0.0f;
        _animator.SetPlaying(false);
        _emotion.gameObject.SetActive(false);
        UpdateAnimation();
    }

    private void ReturnLocation()
    {
        if (_targetLocation == null)
        {
            return;
        }
        TileMapManager.Instance.ReturnLocation(_targetLocation);
        _targetLocation = null;
    }

    public void SetTargetTilePosition(Vector2Int position)
    {
        TileMapManager.Instance.GetRoute(Position, position, _moveCommand);
    }

    public void SetMoveCommand(Action onMoveComplete = null)
    {
        ReturnLocation();
        _time = _movementFrequency;
        _onMoveComplete = onMoveComplete;
    }

    private void UpdateStep()
    {
        if (_time > 0)
        {
            _time -= Time.fixedDeltaTime;
            return;
        }

        _time = _movementFrequency;
        _movementProgress = 0;
        _lastPosition = transform.localPosition;
        Vector2Int currentCommand = _moveCommand.Dequeue();
        _direction.Forward = Direction.VECTOR_INDEX_MAP[currentCommand];
        _targetStepPosition = transform.localPosition + (Vector3)(Vector2)currentCommand;
        _isMoving = true;
        _animator.SetPlaying(true);
    }

    private void FindNewTargetLocation()
    {
        if (_time > 0)
        {
            _time -= Time.fixedDeltaTime;
            return;
        }

        ReturnLocation();
        EventLocation target = TileMapManager.Instance.GetEventLocation(_targetType);
        if (target != null)
        {
            Vector2Int position = new((int)target.transform.localPosition.x, -(int)target.transform.localPosition.y);
            TileMapManager.Instance.GetRoute(Position, position, _moveCommand);
            SetMoveCommand(OnMoveToTargetLocationComplete);
            _targetLocation = target;
        }
    }

    public void SetDirection()
    {
        _direction = _targetLocation.Direction;
    }

    private void OnMoveToTargetLocationComplete()
    {
        _direction = _targetLocation.Direction;
    }

    private void UpdateAnimation()
    {
        _animator.SetDirection(_direction);
        _spriteRenderer.sprite = _animator.GetSprite(Time.fixedDeltaTime);
    }

    public void SetOrder()
    {
        _emotion.gameObject.SetActive(true);
        if (_targetType.HasFlag(GuildLocationEventType.Shop))
        {
            _emotion.SetIcon("Textures/Icon/PixelFood/20_bagel");
        }
        else
        {
            _emotion.SetIcon("Textures/Icon/PixelFood/11_bun");
        }
    }
}