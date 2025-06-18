#if UNITY_EDITOR
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class GoogleSheetDownloader : EditorWindow
{
    private static string SheetId = "1gH6vRdQ9325hOCLg-Som3nugJ595n58Avn_ra8iiUF8";
    private static string APIKey = "AIzaSyCwLYTrf5ZEDpOJLcWpdvquVfi3a5CfUBc";
    private static string DefaultDownloadPath = "Resources/Json";
    private static string ConfigPath => Path.Combine(Application.dataPath, "Settings/GoogleSheetDownloaderConfig.json");

    private bool _isSheetLoaded;
    private bool _selectAll;
    private Vector2 _scrollPos;

    private readonly List<SheetInfo> _sheets = new();

    private void SaveSheetSettings()
    {
        var settings = _sheets.Select(s => new SheetSettings
        {
            Gid = s.Gid,
            Selected = s.Selected,
            OutputFileName = s.OutputFileName
        }).ToList();

        File.WriteAllText(ConfigPath, JsonConvert.SerializeObject(settings, Formatting.Indented));
    }

    private void LoadSheetSettings()
    {
        if (!File.Exists(ConfigPath)) return;

        var settingsJson = File.ReadAllText(ConfigPath);
        var settings = JsonConvert.DeserializeObject<List<SheetSettings>>(settingsJson);

        foreach (var setting in settings)
        {
            var sheet = _sheets.FirstOrDefault(s => s.Gid == setting.Gid);
            if (sheet != null)
            {
                sheet.Selected = setting.Selected;
                sheet.OutputFileName = setting.OutputFileName;
            }
        }
    }

    [MenuItem("Tools/GoogleSheetDownloader")]
    public static void ShowWindow()
    {
        GetWindow<GoogleSheetDownloader>("GoogleSheetDownloader");
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Google Sheet API 설정", EditorStyles.boldLabel);
        SheetId = EditorGUILayout.TextField("Spreadsheet ID", SheetId);
        APIKey = EditorGUILayout.TextField("Google API Key", APIKey);

        if (GUILayout.Button("시트 목록 불러오기"))
        {
            LoadSheetListAsync();
        }

        if (_sheets.Count > 0)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("변환할 시트 선택", EditorStyles.boldLabel);
            DefaultDownloadPath = EditorGUILayout.TextField("기본 저장 경로", DefaultDownloadPath);

            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

            // 일괄 선택
            EditorGUI.BeginChangeCheck();
            bool allSelected = _sheets.All(i => i.Selected);
            bool newSelectedAll = EditorGUILayout.ToggleLeft("일괄 선택", allSelected);
            if (EditorGUI.EndChangeCheck())
            {
                _selectAll = newSelectedAll;
                foreach (var sheet in _sheets)
                {
                    sheet.Selected = _selectAll;
                }
            }

            foreach (var sheet in _sheets)
            {
                EditorGUI.BeginChangeCheck();
                sheet.Selected = EditorGUILayout.ToggleLeft($"{sheet.Title} (GID: {sheet.Gid})", sheet.Selected);
                sheet.OutputFileName = EditorGUILayout.TextField("→ 파일 이름", sheet.OutputFileName);
                EditorGUILayout.Space();
            }

            EditorGUILayout.EndScrollView();

            if (GUILayout.Button("선택한 시트들을 JSON으로 저장"))
            {
                DownloadSelectedSheetsAsync();
            }
            SaveSheetSettings();
        }
        else if (_isSheetLoaded)
        {
            EditorGUILayout.HelpBox("시트를 불러오지 못했습니다. 스프레드시트 ID나 API 키를 확인하세요.", MessageType.Warning);
        }
    }

    private async Task LoadSheetListAsync()
    {
        _isSheetLoaded = false;
        _sheets.Clear();

        string url = $"https://sheets.googleapis.com/v4/spreadsheets/{SheetId}?key={APIKey}";

        try
        {
            using HttpClient client = new();
            string response = await client.GetStringAsync(url);

            var wrapper = JsonUtility.FromJson<SheetMetadataWrapper>(response);
            foreach (var sheet in wrapper.sheets)
            {
                string title = sheet.properties.title;
                int gid = sheet.properties.sheetId;

                _sheets.Add(new SheetInfo
                {
                    Title = title,
                    Gid = gid.ToString(),
                    OutputFileName = $"{title}.json",
                    Selected = false
                });
            }

            _isSheetLoaded = true;
            LoadSheetSettings();
            Debug.Log($"✅ 시트 {_sheets.Count}개를 확인했습니다.");
        }
        catch (Exception e)
        {
            Debug.LogError($"❌ 시트 불러오기 실패: {e.Message}");
            _isSheetLoaded = true;
        }
    }

    private async Task DownloadSelectedSheetsAsync()
    {
        foreach (SheetInfo sheet in _sheets)
        {
            if (!sheet.Selected) continue;

            string url = $"https://docs.google.com/spreadsheets/d/{SheetId}/export?format=csv&gid={sheet.Gid}";

            try
            {
                using HttpClient client = new();
                string csvText = await client.GetStringAsync(url);
                string json = ConvertCsvToJson(csvText);

                string path = Path.Combine(Application.dataPath, DefaultDownloadPath, sheet.OutputFileName);
                string directory = Path.GetDirectoryName(path);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                File.WriteAllText(path, json);

                Debug.Log($"✅ {sheet.Title} 시트 저장 완료: {path}");
            }
            catch (Exception e)
            {
                Debug.LogError($"❌ {sheet.Title} 시트 저장 실패: {e.Message}");
            }
        }

        AssetDatabase.Refresh();
    }

    private static string ConvertCsvToJson(string csv)
    {
        List<Dictionary<string, string>> rows = new();
        string[] lines = Regex.Split(csv.Trim(), "\r?\n");

        if (lines.Length < 2) return "[]";

        List<string> headers = ParseCsvLine(lines[0]);

        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            var values = ParseCsvLine(lines[i]);
            Dictionary<string, string> row = new();

            for (int j = 0; j < headers.Count && j < values.Count; j++)
            {
                row[headers[j]] = values[j];
            }

            rows.Add(row);
        }

        return JsonConvert.SerializeObject(rows, Formatting.Indented);
    }

    private static List<string> ParseCsvLine(string line)
    {
        List<string> fields = new();
        StringBuilder field = new();
        bool inQuotes = false;

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];

            if (c == '"' && (i == 0 || line[i - 1] != '\\'))
            {
                inQuotes = !inQuotes;
            }
            else if (c == ',' && !inQuotes)
            {
                fields.Add(field.ToString().Trim('"'));
                field.Clear();
            }
            else
            {
                field.Append(c);
            }
        }

        fields.Add(field.ToString().Trim('"'));
        return fields;
    }

    [Serializable]
    private class SheetSettings
    {
        public string Gid;
        public bool Selected;
        public string OutputFileName;
    }

    [Serializable]
    private class SheetInfo
    {
        public string Title;
        public string Gid;
        public string OutputFileName;
        public bool Selected;
    }

    [Serializable]
    private class SheetMetadataWrapper
    {
        public List<SheetMetadata> sheets;
    }

    [Serializable]
    private class SheetMetadata
    {
        public SheetProperties properties;
    }

    [Serializable]
    private class SheetProperties
    {
        public string title;
        public int sheetId;
    }
}
#endif