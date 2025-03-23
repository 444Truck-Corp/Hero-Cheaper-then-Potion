using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ToastMessage : Poolable
{
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI text;

    private float time;
    private bool isInitialized;

    public void Initialize(string message)
    {
        text.text = message;
        time = 15.0f;
        isInitialized = true;
        Color color = image.color;
        color.a = 1.0f;
        image.DOColor(color, 0.0f);
        color = text.color;
        color.a = 1.0f;
        text.DOColor(color, 0.0f);
    }

    private void FixedUpdate()
    {
        if (!isInitialized) return;

        if (time > 0.0f)
        {
            time -= Time.fixedDeltaTime;
            return;
        }

        Color color = image.color;
        color.a = 0.0f;
        image.DOColor(color, 0.5f);

        color = text.color;
        color.a = 0.0f;
        text.DOColor(color, 0.5f);

        isInitialized = false;
        Pool.Return(this);
    }
}