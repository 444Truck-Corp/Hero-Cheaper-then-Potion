using UnityEngine;

public class Emotion : MonoBehaviour
{
    private const string PATH = "";

    [SerializeField] private SpriteRenderer _iconSpriteRenderer;

    public void SetIcon(string path)
    {
        var sprite = Resources.Load<Sprite>(PATH + path);
        if (sprite == null) return;
        _iconSpriteRenderer.sprite = sprite;
    }
}