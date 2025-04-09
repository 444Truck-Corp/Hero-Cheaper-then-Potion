using UnityEngine;

public class Emotion : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _iconSpriteRenderer;

    public void SetIcon(string path)
    {
        var sprites = Resources.LoadAll<Sprite>(path);
        if (sprites == null) return;
        _iconSpriteRenderer.sprite = sprites[0];
    }
}