using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using System.Collections;

public class SaveManager : Singleton<SaveManager>
{
    private SaveData saveData;
    public SaveData MySaveData => saveData;

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
        //TODO : 시작화면에서 원하는 세이브파일 로드하도록 바꾸기.
        LoadAuto();
        autoSaveCoroutine = StartCoroutine(AutoSave());
        isDirty = false;
    }

    private void OnApplicationQuit()
    {
        SaveAuto();
        if (autoSaveCoroutine != null) StopCoroutine(autoSaveCoroutine);
    }
    #endregion

    #region Main Methods
    public void SaveAuto()
    {
        string json = JsonConvert.SerializeObject(saveData, Formatting.Indented);
        File.WriteAllText(autoPath, json);
    }

    public void SaveSlot(int slot)
    {
        string path = GetSlotPath(slot);
        string json = JsonConvert.SerializeObject(saveData, Formatting.Indented);
        File.WriteAllText(path, json);
    }

    public void LoadAuto()
    {
        if (File.Exists(autoPath))
        {
            string json = File.ReadAllText(autoPath);
            saveData = JsonConvert.DeserializeObject<SaveData>(json);
        }
        else
        {
            saveData = DataManager.Instance.GetObj<SaveData>(nameof(SaveData));
        }
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
                SaveAuto();
                isDirty = false;
            }
        }
    }

    private string GetSlotPath(int slot)
    {
        string path = Application.persistentDataPath;
        switch (slot)
        {
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