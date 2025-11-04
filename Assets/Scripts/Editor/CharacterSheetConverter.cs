using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class CharacterSheetConverter : EditorWindow
{
    private readonly List<string> _selectedImagePaths = new();

    [MenuItem("Tools/CharacterSheetConverter")]
    public static void ShowWindow()
    {
        GetWindow<CharacterSheetConverter>("Character Sheet Converter");
    }

    private void OnGUI()
    {
        GUILayout.Label("Select Folder To Convert", EditorStyles.boldLabel);

        if (GUILayout.Button("Select Folder with PNG Images"))
        {
            SelectFolder();
        }

        if (_selectedImagePaths.Count > 0)
        {
            GUILayout.Label($"Selected Images: {_selectedImagePaths.Count}");
            if (GUILayout.Button("Convert Selected Images"))
            {
                ConvertAllImages();
            }
        }
    }

    private void SelectFolder()
    {
        string path = EditorUtility.OpenFolderPanel("Select Folder To Convert", "", "");
        if (!string.IsNullOrEmpty(path))
        {
            _selectedImagePaths.Clear();
            string[] files = Directory.GetFiles(path, "*.png", SearchOption.TopDirectoryOnly);
            _selectedImagePaths.AddRange(files);
        }
    }

    private Texture2D LoadTextureFromFile(string path)
    {
        byte[] fileData = File.ReadAllBytes(path);
        Texture2D texture = new(2, 2);
        return texture.LoadImage(fileData) ? texture : null;
    }

    private void ConvertAllImages()
    {
        foreach (string path in _selectedImagePaths)
        {
            Texture2D originalTexture = LoadTextureFromFile(path);
            if (originalTexture != null)
            {
                ConvertImage(originalTexture, path);
                DestroyImmediate(originalTexture);
            }
        }
        _selectedImagePaths.Clear();
        AssetDatabase.Refresh();
        Debug.Log("[CharacterSheetConverter] 모든 이미지 변환 완료");
    }

    private void ConvertImage(Texture2D original, string originalPath)
    {
        int segmentWidth = original.width / 3;
        int height = original.height;

        // 새 텍스처 생성 (세로 4배)
        Texture2D newTexture = new(segmentWidth, height * 4);

        CopySegment(original, newTexture,
            sourceX: segmentWidth, sourceY: 0,
            destX: 0, destY: height * 3,
            segmentWidth, height);

        CopySegmentFlipped(original, newTexture,
            sourceX: segmentWidth * 2, sourceY: 0,
            destX: 0, destY: height * 2,
            segmentWidth, height);

        CopySegment(original, newTexture,
            sourceX: 0, sourceY: 0,
            destX: 0, destY: height,
            segmentWidth, height);

        CopySegment(original, newTexture,
            sourceX: segmentWidth * 2, sourceY: 0,
            destX: 0, destY: 0,
            segmentWidth, height);

        SaveTexture(newTexture, originalPath);
    }

    private void CopySegment(Texture2D src, Texture2D dst, int sourceX, int sourceY, int destX, int destY, int width, int height)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Color pixel = src.GetPixel(sourceX + x, sourceY + y);
                dst.SetPixel(destX + x, destY + y, pixel);
            }
        }
    }

    private void CopySegmentFlipped(Texture2D src, Texture2D dst, int sourceX, int sourceY, int destX, int destY, int width, int height)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Color pixel = src.GetPixel(sourceX + (width - 1 - x), sourceY + y);
                dst.SetPixel(destX + x, destY + y, pixel);
            }
        }
    }

    private void SaveTexture(Texture2D texture, string originalPath)
    {
        string directory = Path.GetDirectoryName(originalPath);
        string filename = Path.GetFileNameWithoutExtension(originalPath);
        string newPath = Path.Combine(directory, filename + ".png");

        byte[] pngData = texture.EncodeToPNG();
        File.WriteAllBytes(newPath, pngData);
        DestroyImmediate(texture);

        Debug.Log($"[CharacterSheetConverter] 이미지를 저장했습니다: {newPath}");
    }
}