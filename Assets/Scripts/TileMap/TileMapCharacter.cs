using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class TileMapCharacter : Poolable
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private float _movementFrequency;
    [SerializeField] private float _movementSpeed = 5;
    [SerializeField] private List<Vector2Int> _moveCommand;

    [SerializeField] private bool _isMoving;
    [SerializeField] private float _time;
    [SerializeField] private string _texturePath;
    [SerializeField] private CustomAnimator _animator;
    [SerializeField] private Emotion _emotion;

    private const string PATH = "Textures/CharacterSheet/";

    public Vector2Int Position => new((int)_targetStepPosition.x, (int)-_targetStepPosition.y);

    private float _movementProgress;
    private Direction _direction;
    private Vector3 _lastPosition;
    private Vector3 _targetStepPosition;
    private EventLocation _targetLocation;
    private Action _onMoveComplete;

    private void Reset()
    {
        _movementSpeed = 5;
    }

    public void Initialize(string textureName)
    {
        _texturePath = textureName;
        if (string.IsNullOrEmpty(_texturePath))
        {
            _texturePath = "궁사1";
        }
        _animator = new CustomAnimator(PATH + textureName, 9, true, true, null);
        Clear();
    }

    public void Clear()
    {
        _direction = new Direction(2);
        _targetStepPosition = transform.localPosition;
        if (_targetLocation != null) TileMapManager.Instance.ReturnLocation(_targetLocation);
        _targetLocation = null;
        _moveCommand = null;
        _movementProgress = 0.0f;
        _animator.SetPlaying(false);
        _emotion.gameObject.SetActive(false);
        UpdateAnimation();
    }

    public void SetMoveCommand(List<Vector2Int> route, Action onMoveComplete = null)
    {
        if (_targetLocation != null)
        {
            TileMapManager.Instance.ReturnLocation(_targetLocation);
            _targetLocation = null;
        }
        _moveCommand = route;
        _time = _movementFrequency;
        _onMoveComplete = onMoveComplete;
    }

    private void FixedUpdate()
    {
        _spriteRenderer.sortingOrder = -(int)transform.localPosition.y;

        if (_isMoving)
        {
            _movementProgress += _movementSpeed * Time.fixedDeltaTime;
            transform.localPosition = Vector3.Lerp(_lastPosition, _targetStepPosition, _movementProgress);
            if (_movementProgress > 1.0f)
            {
                transform.localPosition = _targetStepPosition;
                _movementProgress = 0.0f;
                _isMoving = false;
                if (_moveCommand != null && _moveCommand.Count == 0)
                {
                    _onMoveComplete?.Invoke();
                    _time = UnityEngine.Random.Range(0.0f, 3.0f);
                    TileMapManager.Instance.ReturnLocation(_targetLocation);
                    _targetLocation = null;
                    _animator.SetPlaying(false);
                }
            }
        }
        else
        {
            if ((_moveCommand == null || _moveCommand.Count == 0) && _targetLocation == null)
            {
                FindNewTargetLocation();
            }
            else if (_moveCommand != null && _moveCommand.Count > 0)
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
        _direction.Forward = Direction.VECTOR_INDEX_MAP[_moveCommand[0]];
        _targetStepPosition = transform.localPosition + (Vector3)(Vector2)_moveCommand[0];
        _moveCommand.RemoveAt(0);
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

        var target = TileMapManager.Instance.GetEmptyLocation();
        if (target != null)
        {
            Vector2Int position = new((int)target.transform.localPosition.x, -(int)target.transform.localPosition.y);
            var route = TileMapManager.Instance.GetRoute(Position, position);
            SetMoveCommand(route, OnMoveToTargetLocationComplete);
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