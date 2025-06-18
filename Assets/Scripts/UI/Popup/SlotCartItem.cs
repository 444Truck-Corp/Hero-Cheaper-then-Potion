using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SlotCartItem : Poolable, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _amountText;

    private int _amount;
    private ItemData _itemData;
    private UIPopupShop _shop;
    private const string Path = "";

    public int ItemId => _itemData.id;
    public int Amount => _amount;

    public void Initialize(UIPopupShop shop, ItemData itemData)
    {
        _shop = shop;
        _itemData = itemData;

        Sprite sprite = Resources.Load<Sprite>(Path + itemData.icon);
        if (sprite != null)
        {
            _icon.sprite = sprite;
        }
        _amount = 0;
        UpdateUI();
    }

    public void ModifyAmount(int amount)
    {
        _amount += amount;
        if (_amount <= 0)
        {
            PoolManager.Instance.Return(this);
        }
        UpdateUI();
    }

    private void UpdateUI()
    {
        _amountText.text = (_amount > 1) ? _amount.ToString() : "";
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            _shop.RemoveFromCart(_itemData.id, 1);
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            _shop.RemoveFromCart(_itemData.id, _amount);
        }
        if (_amount < 0) UIManager.Hide<UIItemTooltip>();
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