using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SlotCartItem : Poolable, IPointerClickHandler
{
    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _amountText;

    private int _itemId;
    private int _amount;
    private UIPopupShop _shop;

    public int ItemId => _itemId;
    public int Amount => _amount;

    public void Initialize(UIPopupShop shop, int itemId, Sprite icon)
    {
        _shop = shop;
        _itemId = itemId;
        _icon.sprite = icon;
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
            _shop.RemoveFromCart(_itemId, 1);
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            _shop.RemoveFromCart(_itemId, _amount);
        }
    }
}