using System.Collections.Generic;
using UnityEngine;

public class PoolManager : Singleton<PoolManager>
{
    private readonly Dictionary<string, Stack<GameObject>> _pools = new();
    private readonly Dictionary<string, GameObject> _prefabs = new();

    public T Get<T>(GameObject prefab, Transform parent, Vector3 position = default) where T : Poolable
    {
        string text = prefab.GetHashCode().ToString();
        _prefabs[text] = prefab;
        return Get<T>(text, parent, position);
    }

    /// <summary>
    /// 풀에서 객체를 획득하는 메서드
    /// </summary>
    public T Get<T>(string path, Transform parent, Vector3 position = default) where T : Poolable
    {
        GameObject gameObject = null;
        // 경로의 풀에서 객체 획득
        if (_pools.TryGetValue(path, out Stack<GameObject> stack) && stack.Count > 0)
        {
            gameObject = stack.Pop();
        }
        // 풀에 객체가 없을 때, 객체 생성
        if (gameObject == null)
        {
            gameObject = CreateObject(path);
        }
        if (gameObject != null)
        {
            T component = gameObject.GetComponent<T>();
            SetReady(gameObject, component, parent, position);
            return component;
        }
        Debug.LogError($"프리팹이 없습니다 : {path}");
        return default;
    }

    /// <summary>
    /// 풀에 객체를 반환하는 메서드
    /// </summary>
    public void Return(Poolable item)
    {
        if (item == null) return;
        // 경로가 없으면 객체 제거
        if (string.IsNullOrEmpty(item.ResourcePath))
        {
            Destroy(item.gameObject);
        }
        else
        {
            // 사용 해제 후 반환
            GetStack(item.ResourcePath).Push(item.gameObject);
            item.gameObject.SetActive(false);
        }
    }

    public void Clear()
    {
        foreach (Stack<GameObject> stack in _pools.Values)
        {
            stack.Clear();
        }
        _pools.Clear();
        _prefabs.Clear();
    }

    private GameObject CreateObject(string path)
    {
        // path 경로의 프리팹 캐싱
        if (!_prefabs.TryGetValue(path, out GameObject gameObject) || gameObject == null)
        {
            gameObject = Resources.Load<GameObject>(path);
            _prefabs[path] = gameObject;
        }
        if (gameObject == null) return null;

        GameObject createdObject = Instantiate(gameObject);
        if (createdObject == null) return null;
        Poolable poolable = createdObject.GetComponent<Poolable>();
        poolable.ResourcePath = path;
        return createdObject;
    }

    private void SetReady(GameObject itemObject, Poolable item, Transform parent, Vector3 position)
    {
        // 부모 객체 지정
        if (parent != null)
        {
            itemObject.transform.SetParent(parent);
        }

        itemObject.transform.localPosition = position;
        itemObject.transform.localScale = Vector3.one;
        itemObject.SetActive(true);

        item.Pool = this;
    }

    /// <summary>
    /// 경로에 해당하는 스택을 반환.
    /// 없다면 생성 후 반환
    /// </summary>
    private Stack<GameObject> GetStack(string path)
    {
        if (!_pools.TryGetValue(path, out Stack<GameObject> stack))
        {
            stack = new Stack<GameObject>();
            _pools[path] = stack;
        }
        return stack;
    }
}