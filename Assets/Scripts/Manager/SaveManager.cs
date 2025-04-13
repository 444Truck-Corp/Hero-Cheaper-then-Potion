using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public class SaveManager : Singleton<SaveManager>
{
    private SaveData saveData;
    public SaveData MySaveData => saveData;
    private Dictionary<string, FieldInfo> fieldCache = new();

    #region paths
    private string autoPath;
    private readonly string autoSave = "auto";
    private readonly string saveSlot1 = "slot1";
    private readonly string saveSlot2 = "slot2";
    private readonly string saveSlot3 = "slot3";
    #endregion

    private WaitForSeconds autoSaveInterval = new (600f);
    private Coroutine autoSaveCoroutine;

    bool isDirty;

    #region Unity Life Cycles
    public void Init()
    {
        autoPath = Path.Combine(Application.persistentDataPath, autoSave);
        
        var fields = typeof(SaveData).GetFields(BindingFlags.Public | BindingFlags.Instance);
        foreach (var field in fields)
        {
            fieldCache.Add(field.Name, field);
        }
        
        isDirty = false;
    }

    private void OnApplicationQuit()
    {
        SaveSlot(0);
        if (autoSaveCoroutine != null) StopCoroutine(autoSaveCoroutine);
    }
    #endregion

    #region Main Methods
    public void CreateSaveData()
    {
        saveData = new();
        autoSaveCoroutine ??= StartCoroutine(AutoSave());
    }

    public void SaveSlot(int slot)
    {
        string path = GetSlotPath(slot);
        string json = JsonConvert.SerializeObject(saveData, Formatting.Indented);
        File.WriteAllText(path, json);
    }

    public void LoadSlot(int slot)
    {
        string path = GetSlotPath(slot);
        saveData = JsonConvert.DeserializeObject<SaveData>(File.ReadAllText(path));
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

        isDirty = true;
    }

    #endregion

    #region Sub Methods
    public IEnumerator AutoSave()
    {
        while (true)
        {
            yield return autoSaveInterval;

            if (isDirty)
            {
                SaveSlot(0);
                isDirty = false;
            }
        }
    }

    private string GetSlotPath(int slot)
    {
        string path = Application.persistentDataPath;
        switch (slot)
        {
            case 0:
                Path.Combine(path, autoSave);
                break;
            case 1:
                Path.Combine(path, saveSlot1);
                break;
            case 2:
                Path.Combine(path, saveSlot2);
                break;
            case 3:
                Path.Combine(path, saveSlot3);
                break;
            default:
                Debug.LogError("잘못된 저장 경로입니다.");
                return null;
        }
        return path;
    }
    #endregion
}