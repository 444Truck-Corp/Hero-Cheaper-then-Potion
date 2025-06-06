using UnityEngine;
using UnityEngine.Events;

public enum eClickEventType
{
    None,
    Hero_Click,
    Food_Click,
    Quest_Click
}

public class MonoButton : MonoBehaviour
{
    [SerializeField] private eClickEventType clickType;
    [SerializeField] protected UnityEvent _onLeftClickEvent;
    [SerializeField] protected UnityEvent _onRightClickEvent;

    public void OnLeftClick()// => HandleClick("L");
    {
        _onLeftClickEvent?.Invoke();
    }

    public void OnRightClick()// => HandleClick("R");
    {
        _onRightClickEvent?.Invoke();
    }

    private void HandleClick(string suffix)
    {
        string eventKey = clickType switch
        {
            eClickEventType.Hero_Click => $"{eClickEventType.Hero_Click}{suffix}",
            eClickEventType.Food_Click => $"{eClickEventType.Food_Click}{suffix}",
            eClickEventType.Quest_Click => $"{eClickEventType.Quest_Click}{suffix}",
            _ => null
        };

        if (!string.IsNullOrEmpty(eventKey))
        {
            EventManager.Instance.InvokeClickEvent(eventKey);
        }
    }
}