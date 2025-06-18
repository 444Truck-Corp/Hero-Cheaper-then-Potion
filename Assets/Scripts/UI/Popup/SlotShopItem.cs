using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SlotShopItem : Poolable, IPointerClickHandler
{
    [SerializeField] private Image _icon;

    private int _itemId;
    private Sprite _iconSprite;
    private UIPopupShop _shop;

    private static readonly string Path = "";

    public void Initialize(UIPopupShop shop, int itemId, string iconPath)
    {
        _shop = shop;
        _itemId = itemId;
        _iconSprite = Resources.Load<Sprite>(Path + iconPath);
        if (_iconSprite == null) return;
        _icon.sprite = _iconSprite;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            _shop.AddToCart(_itemId, 1, _iconSprite);
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            _shop.RemoveFromCart(_itemId, 1);
        }
    }
}