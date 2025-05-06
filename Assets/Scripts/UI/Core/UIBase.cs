using UnityEngine;
using UnityEngine.Events;

public class UIBase : MonoBehaviour
{
    public bool isActiveInCreated = true;
    public bool isDestroyAtClosed = true;
    public eUIPosition uiPosition;
    public UnityAction<object[]> opened;
    public UnityAction<object[]> closed;

    protected virtual void Awake()
    {
        opened += Opened;
        closed += Closed;
    }

    public void SetActive<T>(bool isActive) where T : UIBase
    {
        if (isActive) UIManager.Show<T>();
        else UIManager.Hide<T>();
    }

    public void SetActive(bool isActive, params object[] param)
    {
        if (isActive)
        {
            opened?.Invoke(param);
            gameObject.SetActive(true);
        }
        else
        {
            closed?.Invoke(param);

            if (isDestroyAtClosed)
            {
                if (UIManager.Instance != null)
                    UIManager.RemoveUI(this);

                Destroy(gameObject);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }


    public virtual void Opened(object[] param) { }
    public virtual void Closed(object[] param) { }
}
