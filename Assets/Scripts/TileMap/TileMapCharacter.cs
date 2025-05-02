using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class TileMapCharacter : Poolable
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
    [SerializeField] private Emotion _emotion;

    private float _movementProgress;
    private Direction _direction;
    private Vector3 _lastPosition;
    private Vector3 _targetStepPosition;
    private EventLocation _targetLocation;
    private GuildLocationEventType _targetType;
    private Action _onMoveComplete;

    private readonly Queue<Vector2Int> _moveCommand = new();

    public Vector2Int Position => new((int)_targetStepPosition.x, (int)-_targetStepPosition.y);

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
                _movementProgress = 0.0f;
                _isMoving = false;
                if (_moveCommand.Count == 0)
                {
                    _onMoveComplete?.Invoke();
                    _time = UnityEngine.Random.Range(0.0f, 3.0f);
                    ReturnLocation();
                    _animator.SetPlaying(false);
                }
            }
        }
        // 이동하지 않을 때
        else
        {
            if (_moveCommand.Count == 0 && _targetLocation == null)
            {
                FindNewTargetLocation();
            }
            else if (_moveCommand.Count > 0)
            {
                UpdateStep();
            }
            else
            {
                _animator.SetPlaying(false);
            }
        }
        UpdateAnimation();
    }

    public void Initialize(string textureName, GuildLocationEventType targetType)
    {
        _texturePath = textureName;
        if (string.IsNullOrEmpty(_texturePath))
        {
            _texturePath = "궁사2";
        }
        _animator = new CustomAnimator(PATH + textureName, 9, true, true, null);
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

        EventLocation target = TileMapManager.Instance.GetEventLocation(_targetType);
        if (target != null)
        {
            Vector2Int position = new((int)target.transform.localPosition.x, -(int)target.transform.localPosition.y);
            TileMapManager.Instance.GetRoute(Position, position, _moveCommand);
            SetMoveCommand(OnMoveToTargetLocationComplete);
            _targetLocation = target;
        }
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
}