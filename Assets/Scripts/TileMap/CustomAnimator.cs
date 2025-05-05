using System;
using UnityEngine;

[Serializable]
public class CustomAnimator
{
    [SerializeField] private bool _isPlaying;
    [SerializeField] private bool _isLooping;
    [SerializeField] private int _currentFrame;
    [SerializeField] private int _maxFrame;
    [SerializeField] private Direction _direction;
    [SerializeField] private float _frameDuration;
    [SerializeField] private float _framePerSecond;
    [SerializeField] private float _currentDuration;
    [SerializeField] private Action _onEndAnimation;
    [SerializeField] private Sprite[] _sprites;

    public CustomAnimator(string path, int framePerSecond, bool isDirectional, bool isLooping, Action onEndAnimation)
    {
        _isLooping = isLooping;
        _onEndAnimation = onEndAnimation;
        _framePerSecond = framePerSecond;
        _frameDuration = 1.0f / framePerSecond;
        _direction = new Direction(0);
        _sprites = Resources.LoadAll<Sprite>(path);
        if (_sprites == null) return;
        _maxFrame = isDirectional ? (_sprites.Length >> 2) : _sprites.Length;
    }

    public void SetOnEndAnimation(Action action)
    {
        _onEndAnimation = action;
    }

    public void SetDirection(Direction direction)
    {
        _direction.Forward = direction.Forward;
    }

    public void SetPlaying(bool value)
    {
        if (_isPlaying != value)
        {
            _currentFrame = 0;
            _currentDuration = 0;
        }
        _isPlaying = value;
    }

    public Sprite GetSprite(float deltaTime)
    {
        if (_sprites == null) return null;

        _currentDuration += deltaTime;
        int addFrame = (int)(_currentDuration * _framePerSecond);
        if (addFrame > 0)
        {
            _currentFrame += addFrame;
            if (_currentFrame >= _maxFrame)
            {
                _onEndAnimation?.Invoke();
                if (_isLooping)
                {
                    _currentFrame %= _maxFrame;
                }
                else
                {
                    _currentFrame = 0;
                    _isPlaying = false;
                }
            }
            _currentDuration -= addFrame * _frameDuration;
        }
        _currentFrame = (_isPlaying) ? _currentFrame : 0;
        return _sprites[_direction.Forward * _maxFrame + _currentFrame];
    }
}