using System.Collections.Generic;
using UnityEngine;

public class PoolManager : Singleton<PoolManager>
{
    private readonly Dictionary<string, Stack<GameObject>> pools = new Dictionary<string, Stack<GameObject>>();
    private readonly Dictionary<string, GameObject> prefabs = new Dictionary<string, GameObject>();
    private readonly Dictionary<int, GameObject> aliveItems = new Dictionary<int, GameObject>();
    private int poolId = 0;

    public T Get<T>(GameObject prefab, Transform parent, Vector3 position = default) where T : Poolable
    {
        string text = prefab.GetHashCode().ToString();
        prefabs[text] = prefab;
        return Get<T>(text, parent, position);
    }

    /// <summary>
    /// 풀에서 객체를 획득하는 메서드
    /// </summary>
    public T Get<T>(string path, Transform parent, Vector3 position = default) where T : Poolable
    {
        GameObject gameObject = null;
        // 경로의 풀에서 객체 획득
        if (pools.TryGetValue(path, out Stack<GameObject> stack) && stack.Count > 0)
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
            aliveItems.Add(component.PoolID, component.gameObject);
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
            if (!item.IsUsing) return;
            GetStack(item.ResourcePath).Push(item.gameObject);
            item.IsUsing = false;
            item.gameObject.SetActive(false);
        }
        aliveItems.Remove(item.PoolID);
    }

    public void Clear()
    {
        foreach (Stack<GameObject> stack in pools.Values)
        {
            stack.Clear();
        }
        pools.Clear();
        prefabs.Clear();
        aliveItems.Clear();
    }

    private GameObject CreateObject(string path)
    {
        // path 경로의 프리팹 캐싱
        if (!prefabs.TryGetValue(path, out GameObject gameObject) || gameObject == null)
        {
            gameObject = Resources.Load<GameObject>(path);
            prefabs[path] = gameObject;
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

        item.PoolID = poolId++;
        item.IsUsing = true;
        item.Pool = this;
    }

    /// <summary>
    /// 경로에 해당하는 스택을 반환.
    /// 없다면 생성 후 반환
    /// </summary>
    private Stack<GameObject> GetStack(string path)
    {
        if (!pools.TryGetValue(path, out Stack<GameObject> stack))
        {
            stack = new Stack<GameObject>();
            pools[path] = stack;
        }
        return stack;
    }
}