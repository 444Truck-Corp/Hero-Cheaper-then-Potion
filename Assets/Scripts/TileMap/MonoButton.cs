using UnityEngine;
using UnityEngine.Events;

public class MonoButton : MonoBehaviour
{
    [SerializeField] private UnityEvent _onLeftClickEvent;
    [SerializeField] private UnityEvent _onRightClickEvent;

    public void OnLeftClick()
    {
        _onLeftClickEvent?.Invoke();
    }

    public void OnRightClick()
    {
        _onRightClickEvent?.Invoke();
    }
}