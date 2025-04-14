using System.Collections.Generic;
using UnityEngine.Events;

public class EventManager : Singleton<EventManager>
{
    private Dictionary<string, UnityEvent> saveDataChangeEvents = new();

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