using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    [SerializeField] private bool _isMoving;
    [SerializeField] private float _movementFrequency;
    [SerializeField] private float _movementProgress;
    [SerializeField] private float _movementSpeed = 5;
    [SerializeField] private Direction _direction;
    [SerializeField] private float _waitTime;
    [SerializeField] private CustomAnimator _animator;
    [SerializeField] private Vector3 _lastPosition;
    [SerializeField] private Vector3 _targetStepPosition;
    [SerializeField] private Action _onMoveComplete;
    [SerializeField] private string _texturePath;
    [SerializeField] private EventLocation _targetLocation;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private bool _canAutoFinding;
    [SerializeField] private GuildLocationEventType _targetType;

    [SerializeField] private Queue<Vector2Int> _moveQueue = new();

    public Vector2Int Position => new((int)_targetStepPosition.x, (int)-_targetStepPosition.y);
    private const string PATH = "Textures/CharacterSheet/";

    private void FixedUpdate()
    {
        // 이동중일 때
        if (_isMoving)
        {
            MoveStep();
        }
        // 이동하지 않을 때
        else if (_moveQueue.Count > 0)
        {
            TryStartNextMove();
        }
        else if (_moveQueue.Count == 0 && _canAutoFinding)
        {
            TryFindTargetLocation();
        }
        else
        {
            _animator.SetPlaying(false);
        }

        UpdateAnimation();
    }

    public void Initialize(string textureName, GuildLocationEventType targetType, bool canAutoFinding)
    {
        _texturePath = PATH + (string.IsNullOrEmpty(textureName) ? "궁사2" : textureName);
        _animator = new CustomAnimator(_texturePath, 9, true, true);
        _canAutoFinding = canAutoFinding;
        _targetType = targetType;
    }

    public void Clear()
    {
        _direction = new Direction(2);
        _isMoving = false;
        _movementProgress = 0.0f;
        _targetStepPosition = transform.localPosition;
        _moveQueue.Clear();
        _animator.SetPlaying(false);
        ReturnLocation();
        UpdateAnimation();
    }

    private void MoveStep()
    {
        _movementProgress += _movementSpeed * Time.fixedDeltaTime;
        transform.localPosition = Vector3.Lerp(_lastPosition, _targetStepPosition, _movementProgress);

        if (_movementProgress >= 1.0f)
        {
            _movementProgress -= 1.0f;
            _isMoving = false;

            // 더이상 이동할 걸음이 없을 때
            if (_moveQueue.Count == 0)
            {
                transform.localPosition = _targetStepPosition;
                _movementProgress = 0.0f;
                _onMoveComplete?.Invoke();
                _waitTime = UnityEngine.Random.Range(0.0f, 3.0f);
                _animator.SetPlaying(false);
            }
        }
    }

    private void TryStartNextMove()
    {
        if (!IsWaitTimeOver()) return;

        Vector2Int next = _moveQueue.Dequeue();
        _direction.Forward = Direction.VECTOR_INDEX_MAP[next];

        _lastPosition = transform.localPosition;
        _targetStepPosition = _lastPosition + (Vector3)(Vector2)next;
        _waitTime = _movementFrequency;
        _movementProgress = 0.0f;
        _isMoving = true;

        _animator.SetPlaying(true);
    }

    private void TryFindTargetLocation()
    {
        if (!IsWaitTimeOver()) return;

        ReturnLocation();
        _targetLocation = TileMapManager.Instance.GetEventLocation(_targetType);
        if (_targetLocation != null)
        {
            int x = (int)_targetLocation.transform.localPosition.x;
            int y = -(int)_targetLocation.transform.localPosition.y;
            Vector2Int goal = new(x, y);
            TileMapManager.Instance.GetRoute(Position, goal, _moveQueue);
            SetMoveCommand(OnMoveToTargetLocationComplete);
        }
    }

    private bool IsWaitTimeOver()
    {
        if (_waitTime > 0.0f)
        {
            _waitTime -= Time.fixedDeltaTime;
            return false;
        }
        return true;
    }

    private void OnMoveToTargetLocationComplete()
    {
        _direction = _targetLocation != null ? _targetLocation.Direction : _direction;
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

    private void UpdateAnimation()
    {
        _animator.SetDirection(_direction);
        _spriteRenderer.sprite = _animator.GetSprite(Time.fixedDeltaTime);
    }

    public void SetTargetTilePosition(Vector2Int position)
    {
        TileMapManager.Instance.GetRoute(Position, position, _moveQueue);
    }

    public void SetMoveCommand(Action onMoveComplete)
    {
        ReturnLocation();
        _waitTime = _movementFrequency;
        _onMoveComplete = onMoveComplete;
    }
}