using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;

public enum eSaveSlot
{
    Auto = 0,
    Slot1 = 1,
    Slot2 = 2,
    Slot3 = 3
}

public class SaveManager : Singleton<SaveManager>
{
    private SaveData saveData;
    public SaveData MySaveData => saveData;
    private Dictionary<string, FieldInfo> fieldCache = new();

    private bool isAutoDirty;
    private string dir;

    #region Unity Life Cycles
    public void Init()
    {
        dir = Path.Combine(Application.persistentDataPath, "Save");

        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

        var fields = typeof(SaveData).GetFields(BindingFlags.Public | BindingFlags.Instance);
        foreach (var field in fields)
        {
            if (!fieldCache.ContainsKey(field.Name))
                fieldCache.Add(field.Name, field);
        }
        
        isAutoDirty = false;
    }

    private void OnApplicationQuit()
    {
        try
        {
            SaveSlot(0);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[SaveManager] 종료 저장 실패: {e.Message}");
        }
    }
    #endregion

    #region Main Methods
    public void CreateSaveData()
    {
        saveData = new();
        string path = GetSlotPath(0);
        string json = JsonConvert.SerializeObject(saveData, Formatting.Indented);
        File.WriteAllText(path, json);
    }

    public void SaveSlot(eSaveSlot slot)
    {
        if (slot == 0 && !isAutoDirty) return;
        SetSaveData(nameof(SaveData.lastSaveTime), DateTime.Now.Ticks);
        string path = GetSlotPath(slot);
        string json = JsonConvert.SerializeObject(saveData, Formatting.Indented);
        File.WriteAllText(path, json);
    }

    public void LoadSlot(eSaveSlot slot)
    {
        string path = GetSlotPath(slot);
        if (!File.Exists(path))
        {
            Debug.LogWarning($"파일 없음: {path}");
            saveData = new SaveData();
            return;
        }

        try
        {
            string json = File.ReadAllText(path);
            saveData = JsonConvert.DeserializeObject<SaveData>(json);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"로딩 실패: {e.Message}");
            saveData = new SaveData();
        }
    }

    /// <summary> 저장 데이터 할당 </summary>
    /// <param name="field">SaveData의 필드명</param>
    /// <param name="value">field의 자료형에 해당하는 덮어쓰기 값</param>
    /// <param name="indexOrKey"></param>
    public void SetSaveData(string field, object value, object indexOrKey = null)
    {
        if (!fieldCache.TryGetValue(field, out var fieldInfo))
        {
            Debug.LogError($"{field} 필드를 찾을 수 없습니다.");
            return;
        }

        var fieldValue = fieldInfo.GetValue(saveData);

        if (indexOrKey == null)
        {
            fieldInfo.SetValue(saveData, value);
        }
        else if (fieldValue is IList list && indexOrKey is int idx)
        {
            if (idx >= 0 && idx < list.Count)
            {
                list[idx] = value;
            }
            else
            {
                Debug.LogError($"인덱스 범위 초과: {idx}");
                return;
            }
        }
        else if (fieldValue is IDictionary dict)
        {
            dict[indexOrKey] = value;
        }
        else
        {
            Debug.LogError($"컬렉션이 아닌 필드에 indexOrKey를 사용할 수 없습니다.");
            return;
        }

        isAutoDirty = true;
        EventManager.Instance.InvokeSaveDataChanged(field);
    }

    #endregion

    #region Sub Methods
    private string GetSlotPath(eSaveSlot slot)
    {
        return Path.Combine(dir, $"{slot}.json");
    }

    #endregion
}