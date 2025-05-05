using UnityEngine;
using UnityEngine.EventSystems;

public class MonoButtonHandler : MonoBehaviour
{
    [SerializeField] private Camera _camera;

    private void Awake()
    {
        _camera ??= Camera.main;
    }

    private void Update()
    {
        // UI 오브젝트 감지
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        var worldPosition = _camera.ScreenToWorldPoint(Input.mousePosition);

        // 현재는 가장 위에 있는 것으로 가져오지만 수정필요
        var collider = Physics2D.OverlapPoint(worldPosition);
        if (collider != null && collider.TryGetComponent(out MonoButton button))
        {
            Debug.Log(collider.gameObject.name);

            if (Input.GetMouseButtonDown(0))
            {
                button.OnLeftClick();
            }
            else if (Input.GetMouseButtonDown(1))
            {
                button.OnRightClick();
            }
        }
    }
}