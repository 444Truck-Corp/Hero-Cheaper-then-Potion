using UnityEditor;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(RectTransform))]
public class CustomContentSizeFitter : MonoBehaviour
{
    private RectTransform rect;
    [SerializeField] private RectTransform targetRect;

    [Header("Padding")]
    public float top = 0f;
    public float bottom = 0f;
    public float left = 0f;
    public float right = 0f;

    private void OnEnable()
    {
        rect = GetComponent<RectTransform>();
        if (targetRect == null)
        {
            targetRect = GetComponent<RectTransform>();
        }
        ApplyPadding();
    }

    private void OnValidate()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            EditorApplication.delayCall += () =>
            {
                if (this == null || gameObject == null) return;
                rect = GetComponent<RectTransform>();
                ApplyPadding();
            };
            return;
        }
#endif
        ApplyPadding();
    }

    public void ApplyPadding()
    {
        if (rect == null || targetRect == null) return;

        Vector2 targetSize = targetRect.rect.size;

        float width = targetSize.x + left + right;
        float height = targetSize.y + top + bottom;
        
        rect.sizeDelta = new Vector2(width, height);

        Vector2 offset = new Vector2(
            (right - left) * 0.5f,
            (top - bottom) * 0.5f
        );
        rect.anchoredPosition = targetRect.anchoredPosition + offset;
    }
}