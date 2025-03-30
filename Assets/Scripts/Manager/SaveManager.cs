using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using System.Collections;

public class SaveManager : Singleton<SaveManager>
{
    private SaveData saveData;
    public SaveData MySaveData => saveData;

    private readonly string savePath = Path.Combine(Application.persistentDataPath, nameof(SaveData));

    private WaitForSeconds autoSaveInterval = new (600f);
    private Coroutine autoSaveCoroutine;

    bool isDirty;

    #region Unity Life Cycles
    public void Init()
    {
        Load();
        autoSaveCoroutine = StartCoroutine(AutoSave());
        isDirty = false;
    }

    private void OnApplicationQuit()
    {
        Save();
        if (autoSaveCoroutine != null) StopCoroutine(autoSaveCoroutine);
    }
    #endregion

    #region Main Methods
    public void Save()
    {
        string json = JsonConvert.SerializeObject(saveData, Formatting.Indented);
        File.WriteAllText(savePath, json);
    }

    public void Load()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            saveData = JsonConvert.DeserializeObject<SaveData>(json);
        }
        else
        {
            saveData = DataManager.Instance.GetObj<SaveData>(nameof(SaveData));
        }
    }

    /// <summary> 저장 데이터 할당 </summary>
    /// <param name="field">SaveData의 필드명</param>
    /// <param name="value">field의 자료형에 해당하는 덮어쓰기 값</param>
    /// <param name="indexOrKey"></param>
    public object SetSaveData(string field, object value, object indexOrKey = null)
    {
        var fieldInfo = saveData.GetType().GetField(field);
        if (fieldInfo == null)
        {
            Debug.LogError($"'{field}' 필드를 찾을 수 없습니다.");
            return null;
        }

        var fieldValue = fieldInfo.GetValue(saveData);
        if (indexOrKey == null)
        {
            fieldInfo.SetValue(saveData, value);
            return value;
        }
        else if (fieldValue is IList list && indexOrKey is int idx)
        {
            if (idx >= 0 && idx < list.Count)
            {
                list[idx] = value;
            }
            else
            {
                Debug.LogError($"[SaveManager] 인덱스 범위 초과: {idx}");
                return null;
            }
        }
        else if (fieldValue is IDictionary dict)
        {
            dict[indexOrKey] = value;
        }
        else
        {
            Debug.LogError($"[SaveManager] 컬렉션이 아닌 필드에 indexOrKey를 사용할 수 없습니다.");
            return null;
        }

        isDirty = true;
        return null;
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
                Save();
                isDirty = false;
            }
        }
    }
    #endregion
}