using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    [SerializeField] private float _movementSpeed = 5;
    [SerializeField] private float _moveFrequency = 0.5f;
    [SerializeField] private float _moveProgress;
    [SerializeField] private SpriteRenderer _spriteRenderer;

    [SerializeField] private Direction _direction;
    [SerializeField] private float _waitTime;
    [SerializeField] private Vector3 _lastPosition;
    [SerializeField] private Vector3 _targetStepPosition;
    [SerializeField] private Action _onMoveComplete;
    [SerializeField] private string _texturePath;
    [SerializeField] private EventLocation _targetLocation;
    [SerializeField] private GuildLocationEventType _targetType;

    private readonly Queue<Vector2Int> _moveQueue = new();
    private readonly StateMachine _stateMachine = new();

    public CustomAnimator Animator { get; private set; }
    public bool CanAutoFind { get; private set; }
    public bool HasCommands => _moveQueue.Count > 0;

    public Vector2Int Position => new((int)_targetStepPosition.x, (int)-_targetStepPosition.y);
    private const string PATH = "Textures/CharacterSheet/";

    private void FixedUpdate()
    {
        _stateMachine.Update();
    }

    public void Initialize(string textureName, GuildLocationEventType targetType, bool autoFinding)
    {
        _texturePath = PATH + (string.IsNullOrEmpty(textureName) ? "궁사2" : textureName);
        Animator = new CustomAnimator(_texturePath, 9, true, true);
        _targetType = targetType;
        CanAutoFind = autoFinding;
        _stateMachine.ChangeState(new IdleState(this, _stateMachine));
    }

    public void Clear()
    {
        _direction = new Direction(2);
        _moveProgress = 0.0f;
        _targetStepPosition = transform.localPosition;
        _moveQueue.Clear();
        Animator.SetPlaying(false);
        ReleaseLocation();
        UpdateAnimation();
    }

    public void SetTargetTilePosition(Vector2Int position)
    {
        TileMapManager.Instance.GetRoute(Position, position, _moveQueue);
    }

    public void SetMoveCommand(Action onMoveComplete)
    {
        ReleaseLocation();
        _onMoveComplete = onMoveComplete;
    }

    public void StartMove()
    {
        Vector2Int next = _moveQueue.Dequeue();
        _direction.Forward = Direction.VECTOR_INDEX_MAP[next];
        _lastPosition = transform.localPosition;
        _targetStepPosition = _lastPosition + (Vector3)(Vector2)next;
        Animator.SetPlaying(true);
    }

    public bool MoveStep()
    {
        _moveProgress += _movementSpeed * Time.fixedDeltaTime;
        transform.localPosition = Vector3.Lerp(_lastPosition, _targetStepPosition, _moveProgress);

        if (_moveProgress >= 1.0f)
        {
            _moveProgress -= 1.0f;
            return true;
        }
        return false;
    }

    public void CompleteMove()
    {
        _moveProgress = 0.0f;
        transform.localPosition = _targetStepPosition;
        Animator.SetPlaying(false);
        _onMoveComplete?.Invoke();
        _waitTime = UnityEngine.Random.Range(0f, 3f);
        _direction = _targetLocation != null ? _targetLocation.Direction : _direction;
    }

    public void FindNewTarget()
    {
        var newTarget = TileMapManager.Instance.GetEventLocation(_targetType);
        if (newTarget != null)
        {
            ReleaseLocation();
            _targetLocation = newTarget;
            int x = (int)_targetLocation.transform.localPosition.x;
            int y = -(int)_targetLocation.transform.localPosition.y;
            Vector2Int goal = new(x, y);
            TileMapManager.Instance.GetRoute(Position, goal, _moveQueue);
        }
    }

    public bool WaitTimeElapsed()
    {
        if (_waitTime > 0.0f)
        {
            _waitTime -= Time.fixedDeltaTime;
            return false;
        }
        return true;
    }

    public void ReleaseLocation()
    {
        if (_targetLocation != null)
        {
            TileMapManager.Instance.ReturnLocation(_targetLocation);
            _targetLocation = null;
        }
    }

    public void UpdateAnimation()
    {
        Animator.SetDirection(_direction);
        _spriteRenderer.sprite = Animator.GetSprite(Time.fixedDeltaTime);
    }

    public void SetLocation(EventLocation location)
    {
        ReleaseLocation();
        _targetLocation = location;
    }
}