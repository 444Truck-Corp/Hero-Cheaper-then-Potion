using UnityEngine;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

public class DataManager : Singleton<DataManager>
{
    private readonly Dictionary<string, string> dataDics = new(); //Resources/Json으로부터 자동로드.

    #region Unity Life Cycles
    public void Init()
    {
        TextAsset[] jsonFiles = Resources.LoadAll<TextAsset>("Json");
        foreach (var jsonFile in jsonFiles)
        {
            string fileName = jsonFile.name;
            string jsonData = jsonFile.text;
            dataDics[fileName] = jsonData;
        }
    }
    #endregion

    #region Main Methods
    /// <summary><typeparamref name="T"/>의 데이터를 object로 parsing 후 반환</summary>
    public T GetObj<T>(string key)
    {
        if (dataDics.TryGetValue(key, out var json))
        {
            return ConvertToObj<T>(json, key);
        }
        else
        {
            Debug.LogWarning($"{key}은 DataDics에 존재하지 않습니다.");
            return default;
        }
    }

    /// <summary><typeparamref name="T"/>의 데이터를 object로 parsing 후 반환</summary>
    public List<T> GetObjList<T> (string key)
    {
        if (dataDics.TryGetValue(key, out var json))
        {
            return ConvertToList<T>(json, key);
        }
        else
        {
            Debug.LogWarning($"{key}은 DataDics에 존재하지 않습니다.");
            return null;
        }
    }

    /// <summary>데이터를 string으로 반환</summary>
    public string GetRawDataList(string key)
    {
        if (dataDics.TryGetValue(key, out var json))
        {
            return json;
        }
        else
        {
            Debug.LogWarning($"{key}은 DataDics에 존재하지 않습니다.");
            return null;
        }
    }
    #endregion

    #region Sub Methods
    private T ConvertToObj<T>(string json, string key)
    {
        try
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
        catch (Exception ex)
        {
            Debug.LogError($"'{key}'의 JSON 데이터 변환 중 오류 발생: {ex.Message}");
            return default;
        }
    }

    private List<T> ConvertToList<T>(string jsonData, string keyName)
    {
        try
        {
            return JsonConvert.DeserializeObject<List<T>>(jsonData);
        }
        catch (Exception ex)
        {
            Debug.LogError($"'{keyName}'의 JSON → List<{typeof(T).Name}> 변환 실패: {ex.Message}");
            return null;
        }
    }
    #endregion
}