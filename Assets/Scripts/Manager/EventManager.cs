using System.Collections.Generic;
using UnityEngine.Events;

public class EventManager : Singleton<EventManager>
{
    private readonly Dictionary<string, UnityEvent> clickEvents = new();
    private readonly Dictionary<string, UnityEvent> saveDataChangeEvents = new();

    #region Click Events
    public void AddClickListener(string eventKey, UnityAction action)
    {
        if (!clickEvents.ContainsKey(eventKey))
            clickEvents[eventKey] = new UnityEvent();

        clickEvents[eventKey].AddListener(action);
    }

    public void RemoveClickListener(string eventKey, UnityAction action)
    {
        if (clickEvents.ContainsKey(eventKey))
            clickEvents[eventKey].RemoveListener(action);
    }

    public void InvokeClickEvent(string eventKey)
    {
        if (clickEvents.TryGetValue(eventKey, out var unityEvent))
        {
            unityEvent.Invoke();
        }
    }
    #endregion

    #region SaveData Change Events
    public void AddSaveDataListener(string fieldName, UnityAction action)
    {
        if (!saveDataChangeEvents.ContainsKey(fieldName))
            saveDataChangeEvents[fieldName] = new();

        saveDataChangeEvents[fieldName].AddListener(action);
    }

    public void RemoveSaveDataListener(string fieldName, UnityAction action)
    {
        if (saveDataChangeEvents.ContainsKey(fieldName))
            saveDataChangeEvents[fieldName].RemoveListener(action);
    }

    public void InvokeSaveDataChanged(string fieldName)
    {
        if (saveDataChangeEvents.TryGetValue(fieldName, out var unityEvent))
        {
            unityEvent.Invoke();
        }
    }
    #endregion
}