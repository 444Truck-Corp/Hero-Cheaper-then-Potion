#if UNITY_EDITOR
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class ItemDataAssetChecker : EditorWindow
{
    private TextAsset _jsonFile;
    private string _searchingTarget = "icon";
    private string _prefix = "Assets/Resources/Textures/";
    private string _suffix = ".png";
    private Vector2 _scrollPosition;
    private Vector2 _jsonScrollPosition;
    private string _jsonContent = "";
    private string lastJsonPath = "";

    private List<(string name, string path, bool exists)> results = new();

    [MenuItem("Tools/ItemData Asset Checker")]
    public static void ShowWindow()
    {
        GetWindow<ItemDataAssetChecker>("ItemData Asset Checker");
    }

    private void OnGUI()
    {
        GUILayout.Label("ItemData Asset Checker", EditorStyles.boldLabel);

        var newJsonFile = (TextAsset)EditorGUILayout.ObjectField("JSON File", _jsonFile, typeof(TextAsset), false);
        if (newJsonFile != _jsonFile)
        {
            _jsonFile = newJsonFile;
            LoadJsonFromFile();
        }

        _searchingTarget = EditorGUILayout.TextField("Searching Target", _searchingTarget);
        _prefix = EditorGUILayout.TextField("Prefix Path", _prefix);
        _suffix = EditorGUILayout.TextField("Suffix", _suffix);

        if (GUILayout.Button("Load JSON"))
        {
            LoadJsonFromFile();
        }

        GUILayout.Label("Edit JSON Content:", EditorStyles.label);
        _jsonScrollPosition = EditorGUILayout.BeginScrollView(_jsonScrollPosition, GUILayout.Height(150));
        _jsonContent = EditorGUILayout.TextArea(_jsonContent, GUILayout.ExpandHeight(true));
        EditorGUILayout.EndScrollView();

        if (GUILayout.Button("Check Assets"))
        {
            CheckAssets();
        }

        if (GUILayout.Button("Save JSON Changes"))
        {
            SaveJsonToFile();
        }

        if (results.Count > 0)
        {
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            foreach (var result in results)
            {
                GUIStyle style = new GUIStyle(EditorStyles.label);
                style.normal.textColor = result.exists ? Color.green : Color.red;
                EditorGUILayout.LabelField($"{result.name} -> {result.path}", style);
            }
            EditorGUILayout.EndScrollView();
        }
    }

    private void CheckAssets()
    {
        results.Clear();

        if (string.IsNullOrEmpty(_jsonContent))
        {
            Debug.LogWarning("Please provide valid JSON content.");
            return;
        }

        var jsonArray = JArray.Parse(_jsonContent);
        foreach (var item in jsonArray)
        {
            var name = item["name"]?.ToString() ?? "Unnamed";
            var value = item[_searchingTarget]?.ToString();
            if (string.IsNullOrEmpty(value)) continue;

            string fullPath = Path.Combine(_prefix, value + _suffix);
            bool exists = AssetDatabase.LoadAssetAtPath<Object>(fullPath) != null;
            results.Add((name, fullPath, exists));
        }
    }

    private void SaveJsonToFile()
    {
        if (_jsonFile == null)
        {
            Debug.LogWarning("No JSON file selected to save.");
            return;
        }

        string path = AssetDatabase.GetAssetPath(_jsonFile);
        File.WriteAllText(path, _jsonContent);
        AssetDatabase.Refresh();
        Debug.Log("JSON content saved to: " + path);
    }

    private void LoadJsonFromFile()
    {
        if (_jsonFile == null) return;
        string path = AssetDatabase.GetAssetPath(_jsonFile);
        _jsonContent = File.ReadAllText(path);
        lastJsonPath = path;
        Debug.Log("JSON content loaded from: " + path);
    }
}
#endif