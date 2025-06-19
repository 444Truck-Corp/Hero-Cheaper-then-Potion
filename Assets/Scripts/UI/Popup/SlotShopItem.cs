using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SlotShopItem : Poolable, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image _icon;

    private Sprite _iconSprite;
    private ItemData _itemData;
    private UIPopupShop _shop;

    public void Initialize(UIPopupShop shop, ItemData itemData)
    {
        _shop = shop;
        _itemData = itemData;

        _iconSprite = ResourceManager.Instance.LoadAsset<Sprite>(ResourceManager.textureDir, itemData.icon);
        if (_iconSprite != null)
        {
            _icon.sprite = _iconSprite;
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