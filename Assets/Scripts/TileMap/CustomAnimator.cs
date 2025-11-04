using System;
using UnityEngine;

namespace HeroPotion
{
    [Serializable]
    public class CustomAnimator
    {
        [SerializeField] private bool _isPlaying;
        [SerializeField] private bool _isLooping;
        [SerializeField] private int _currentFrame;
        [SerializeField] private int _maxFrame;
        [SerializeField] private int _direction;
        [SerializeField] private float _frameDuration;
        [SerializeField] private float _framePerSecond;
        [SerializeField] private float _currentDuration;
        [SerializeField] private Action _onEndAnimation;
        [SerializeField] private Sprite[] _sprites;

        private const string DEFAULT_PATH = "Textures/CharacterSheet/100001";

        public CustomAnimator(string path, int framePerSecond, bool isDirectional, bool isLooping, Action onEndAnimation = null)
        {
            _isLooping = isLooping;
            _onEndAnimation = onEndAnimation;
            _framePerSecond = framePerSecond;
            _frameDuration = 1.0f / framePerSecond;
            _direction = 0;
            _sprites = Resources.LoadAll<Sprite>(path);
            if (_sprites == null || _sprites.Length == 0)
            {
                Debug.LogError($"[CustomAnimator] 경로를 찾을 수 없습니다! : {path}");
                _sprites = Resources.LoadAll<Sprite>(DEFAULT_PATH);
                return;
            }
            _maxFrame = isDirectional ? (_sprites.Length >> 2) : _sprites.Length;
        }

        public void SetOnEndAnimation(Action action)
        {
            _onEndAnimation = action;
        }

        public void SetDirection(int direction)
        {
            _direction = direction;
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
            return _sprites[_direction * _maxFrame + _currentFrame];
        }
    }
}