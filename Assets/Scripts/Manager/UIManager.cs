using DG.Tweening;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public enum eUIPosition
{
    Main,
    Navigator,
    Popup,
    Override
}

public class UIManager : Singleton<UIManager>
{
    [SerializeField] private List<Transform> parents;
    [SerializeField] private List<UIBase> uiList = new();
    [SerializeField] private Image fadeBG;

    private static readonly string resourceFolder = "UIPrefabs";

    #region static methods
    public static void SetParents(List<Transform> parents)
    {
        Instance.parents = parents;
        Instance.parents.Add(Instance.transform);
    }

    /// <summary> UI를 생성하고 isActiveInCreated에 따라 활성화하는 메서드 </summary>
    /// <typeparam name="T">UIBase를 상속받은 클래스</typeparam>
    /// <param name="param">원하는 변수를 원하는 개수만큼!</param>
    public static T Show<T>(params object[] param) where T : UIBase
    {
        string uiName = typeof(T).Name;
        var ui = Instance.uiList.Find(obj => obj.name == uiName);

        if (ui == null)
        {
            var prefab = ResourceManager.Instance.LoadAsset<T>(resourceFolder, uiName);
            if (prefab == null)
            {
                Debug.LogError($"[UIManager] {uiName} 프리팹을 찾을 수 없습니다.");
                return null;
            }

            ui = Instantiate(prefab, Instance.parents[(int)prefab.uiPosition]);
            ui.name = uiName;
            Instance.uiList.Add(ui);
        }
        ui.opened.Invoke(param);
        ui.gameObject.SetActive(ui.isActiveInCreated);
        ui.isActiveInCreated = true;

        return (T)ui;
    }

    /// <summary> Scene에 생성된 UI 반환 </summary>
    /// <typeparam name="T">UIBase를 상속받은 클래스</typeparam>
    public static T Get<T>() where T : UIBase
    {
        return (T)Instance.uiList.Find(obj => obj.name == typeof(T).ToString());
    }

    /// <summary>
    /// UI를 isDestroyAtClosed에 따라 숨기거나 파괴
    /// </summary>
    /// <typeparam name="T">UIBase를 상속받은 클래스명</typeparam>
    /// <param name="param">원하는 변수를 원하는 개수만큼!</param>
    public static void Hide<T>(params object[] param) where T : UIBase
    {
        var ui = Instance.uiList.Find(obj => obj.name == typeof(T).ToString());
        if (ui != null)
        {
            ui.closed.Invoke(param);

            if (ui.isDestroyAtClosed)
            {
                Instance.uiList.Remove(ui);
                Destroy(ui.gameObject);
            }
            else
            {
                ui.SetActive(false);
            }
        }
    }

    /// <summary> Scene에 특정 UI가 생성된 상태인지 확인 </summary>
    /// <typeparam name="T">UIBase를 상속받은 클래스명</typeparam>
    public static bool IsOpened<T>() where T : UIBase
    {
        return Instance.uiList.Exists(obj => obj.name == typeof(T).ToString());
    }

    /// <summary>
    /// 특정 UI가 활성화된 상태인지 확인
    /// </summary>
    public static bool IsActive<T>() where T : UIBase
    {
        return Instance.uiList.Exists(obj => obj.name == typeof(T).ToString() && obj.gameObject.activeSelf);
    }
    #endregion

    #region fades
    public void ToBlack()
    {
        fadeBG.DOFade(1f, 1f).SetUpdate(true);
    }

    public void ToTransparent()
    {
        fadeBG.DOFade(0f, 1f).SetUpdate(true);
    }
    #endregion
}