using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIIngredientSlot : MonoBehaviour
{
    [SerializeField] private Image _ingredientIcon;
    [SerializeField] private TextMeshProUGUI _text;

    public void SetIngredient(ItemData item, string name, int itemAmount, int ingredientAmount)
    {
        if (_ingredientIcon != null)
        {
            _ingredientIcon.sprite = ResourceManager.Instance.LoadAsset<Sprite>(ResourceManager.textureDir, item.icon);
        }

        if (_text != null)
        {
            _text.text = $"{name} {itemAmount} / {ingredientAmount}";
        }
    }
}