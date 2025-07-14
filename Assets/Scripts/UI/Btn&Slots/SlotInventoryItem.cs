using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SlotInventoryItem : Poolable, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _amountText;

    private ItemData _data;

    public void Initialize(ItemData itemData)
    {
        _data = itemData;

        Sprite sprite = ResourceManager.Instance.LoadAsset<Sprite>(ResourceManager.textureDir, itemData.icon);
        if (sprite != null)
        {
            _icon.sprite = sprite;
        }

        int amount = 1;
        if (itemData.category != EItemCategory.Equipment &&
            SaveManager.Instance.MySaveData.items.TryGetValue(itemData.id, out var savedAmount))
        {
            amount = savedAmount;
        }

        UpdateText(amount);
    }

    private void UpdateText(int amount)
    {
        _amountText.SetText(amount > 1 ? amount.ToString() : "");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        UIManager.Show<UIItemTooltip>(_data);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UIManager.Hide<UIItemTooltip>();
    }
}