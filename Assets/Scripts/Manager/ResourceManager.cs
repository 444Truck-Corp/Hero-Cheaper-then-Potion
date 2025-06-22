using UnityEngine;
using System.Collections.Generic;

public class ResourceManager : Singleton<ResourceManager>
{
    private readonly Dictionary<string, Object> assetPools = new();

    public static readonly string thumbnailDir = "Thumbnails";
    public static readonly string standDir = "Stands";

    /// <summary> 
    /// Awake 대용 : Manager 실행순서 관리하기 위함. 
    /// </summary>
    public void Init()
    {

    }

    /// <summary>
    /// 지정한 폴더/파일명으로 Resources에서 프리팹을 로드하고, 인스턴스를 생성하여 반환합니다.
    /// </summary>
    public T Instantiate<T>(string folderName, string fileName, Transform parent = null) where T : Object
    {
        var prefab = LoadAsset<T>(folderName, fileName);
        if (prefab == null) return null;

        return Instantiate(prefab, parent);
    }

    /// <summary>
    /// 지정한 폴더/파일명으로 Resources에서 리소스를 불러오고, assetPools에 캐싱하여 반환합니다.
    /// key 형식: <paramref name="folderName"/>/<paramref name="fileName"/>
    /// </summary>
    public T LoadAsset<T>(string folderName, string fileName) where T : Object
    {
        string key = $"{folderName}/{fileName}";
        if (assetPools.TryGetValue(key, out var cachedAsset))
        {
            return cachedAsset as T;
        }

        T asset = Resources.Load<T>($"{folderName}/{fileName}");
        if (asset == null)
        {
            Debug.LogError($"[ResourceManager] {key} 리소스를 찾을 수 없습니다.");
            return null;
        }

        assetPools[key] = asset;
        return asset;
    }

    /// <summary>
    /// 지정한 폴더 내의 모든 <typeparamref name="T"/> 타입 리소스를 리스트로 반환합니다.
    /// assetPools에는 캐싱하지 않습니다.
    /// </summary>
    public List<T> LoadAssetList<T>(string folderName) where T : Object
    {
        var assets = Resources.LoadAll<T>(folderName);
        if (assets == null || assets.Length == 0)
        {
            Debug.LogWarning($"[ResourceManager] {folderName} 폴더에 {typeof(T).Name}가 없습니다.");
            return new List<T>();
        }

        return new List<T>(assets);
    }

    /// <summary>
    /// 지정한 폴더에 해당하는 모든 캐시된 리소스를 언로드하고, assetPools에서 제거합니다.
    /// </summary>
    public void UnloadFolder(string folderName)
    {
        var keysToRemove = new List<string>();

        foreach (var kvp in assetPools)
        {
            if (kvp.Key.StartsWith($"{folderName}/"))
            {
                Resources.UnloadAsset(kvp.Value);
                keysToRemove.Add(kvp.Key);
            }
        }

        foreach (var key in keysToRemove)
        {
            assetPools.Remove(key);
        }
    }
}