using System;
using UnityEngine;
using UnityEngine.UI;

public class Emotion : MonoBehaviour
{
    private const string PATH = "";

    [SerializeField] private SpriteRenderer _iconSpriteRenderer;
    [SerializeField] private Image _progress;

    private float _originTime;
    private float _time;
    private Action _action;
    private Color _originColor = Color.green;
    private Color _color = Color.red;

    public void SetIcon(string path)
    {
        var sprite = Resources.Load<Sprite>(PATH + path);
        if (sprite == null)
        {
            return;
        }
        _iconSpriteRenderer.sprite = sprite;
    }

    public void SetTimer(float time, Action action)
    {
        _originTime = 1.0f / time;
        _time = time;
        _action = action;
        _progress.gameObject.SetActive(true);
        _progress.color = _originColor;
        _progress.fillAmount = 1.0f;
    }

    public void CancleTimer()
    {
        _action = null;
        _progress.gameObject.SetActive(false);
    }

    private void FixedUpdate()
    {
        if (_time > 0 || _action == null)
        {
            float amount = _originTime * _time;
            _progress.fillAmount = amount;
            _progress.color = Color.Lerp(_color, _originColor, amount);
            _time -= Time.fixedDeltaTime;
            return;
        }

        _action.Invoke();
        CancleTimer();
    }

    public void SetAction(Action value)
    {
        
    }
}