using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SlotShopItem : Poolable, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image _icon;

    private Sprite _iconSprite;
    private ItemData _itemData;
    private UIPopupShop _shop;

    private static readonly string Path = "";

    public void Initialize(UIPopupShop shop, ItemData itemData)
    {
        _shop = shop;
        _itemData = itemData;
        _iconSprite = Resources.Load<Sprite>(Path + itemData.icon);
        if (_iconSprite == null) return;
        _icon.sprite = _iconSprite;

        Sprite sprite = Resources.Load<Sprite>(Path + itemData.icon);
        if (sprite != null)
        {
            _icon.sprite = sprite;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            _shop.AddToCart(_itemData.id, 1, _iconSprite);
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            _shop.RemoveFromCart(_itemData.id, 1);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        UIManager.Show<UIItemTooltip>(_itemData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UIManager.Hide<UIItemTooltip>();
    }
}