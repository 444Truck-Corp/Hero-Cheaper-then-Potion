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

            Type classType = Type.GetType(fileName);
            if (classType != null)
            {
                dataDics[fileName] = jsonData;
            }
            else
            {
                Debug.LogError($"'{fileName}'에 해당하는 클래스를 찾을 수 없습니다.");
            }
        }
    }
    #endregion

    #region Main Methods
    /// <summary><typeparamref name="T"/>의 데이터를 object로 parsing 후 반환</summary>
    public T GetObj<T>(string className) where T : class, new()
    {
        if (dataDics.ContainsKey(className))
        {
            return ConvertToObj<T>(className);
        }
        else
        {
            return new T();
        }
    }

    /// <summary><typeparamref name="T"/>의 데이터를 object로 parsing 후 반환</summary>
    public List<T> GetObjList<T> (string className) where T : class
    {
        if (dataDics.ContainsKey(className))
        {
            return ConvertToList<T>(className);
        }
        else
        {
            Debug.LogWarning($"{className}은 DataDics에 존재하지 않습니다.");
            return null;
        }
    }

    /// <summary>데이터를 string으로 반환</summary>
    public string GetRawDataList(string className)
    {
        if (dataDics.ContainsKey(className))
        {
            return dataDics[className];
        }
        else
        {
            Debug.LogWarning($"{className}은 DataDics에 존재하지 않습니다.");
            return null;
        }
    }
    #endregion

    #region Sub Methods
    private T ConvertToObj<T>(string className) where T : class
    {
        Type classType = Type.GetType(className);

        if (classType == null)
        {
            Debug.LogError($"'{className}'에 해당하는 클래스를 찾을 수 없습니다.");
            return null;
        }

        string jsonData = dataDics[className];
        try
        {
            T obj = JsonConvert.DeserializeObject<T>(jsonData);

            if (obj != null)
            {
                return obj;
            }
            else
            {
                Debug.LogError($"'{className}'의 데이터가 예상한 형식이 아닙니다.");
                return null;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"'{className}'의 JSON 데이터 변환 중 오류 발생: {ex.Message}");
            return null;
        }
    }

    private List<T> ConvertToList<T>(string className) where T : class
    {
        Type classType = Type.GetType(className);
       
        if (classType == null)
        {
            Debug.LogError($"'{className}'에 해당하는 클래스를 찾을 수 없습니다.");
            return null;
        }

        var objectList = new List<T>();
        string jsonData = dataDics[className];
        try
        {
            var jsonObject = JsonConvert.DeserializeObject(jsonData, typeof(List<>).MakeGenericType(classType));

            if (jsonObject is IEnumerable<T> objectListEnumerable)
            {
                objectList.AddRange(objectListEnumerable);
                return objectList;
            }
            else
            {
                Debug.LogError($"'{className}'의 데이터가 예상한 형식의 List<T>가 아닙니다.");
                return null;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"'{className}'의 JSON 데이터 변환 중 오류 발생: {ex.Message}");
            return null;
        }
    }
    #endregion
}