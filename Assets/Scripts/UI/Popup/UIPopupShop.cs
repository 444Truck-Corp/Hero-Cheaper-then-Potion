using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class UIPopupShop : UIBase
{
    [SerializeField] private GameObject _itemPrefab;
    [SerializeField] private GameObject _cartPrefab;
    [SerializeField] private Transform _itemParent;
    [SerializeField] private Transform _cartParent;
    [SerializeField] private TextMeshProUGUI _priceText;

    private readonly List<SlotShopItem> _items = new();
    private readonly Dictionary<int, SlotCartItem> _cartItems = new();

    private int _price;

    public override void Opened(object[] param)
    {
        RefreshPrice();
        if (param.Length > 0 && param[0] is IList<int> items)
        {
            foreach (var itemId in items)
            {
                var itemData = ItemManager.Instance.ItemList[itemId];
                var slot = PoolManager.Instance.Get<SlotShopItem>(_itemPrefab, _itemParent);
                slot.Initialize(this, itemData);
                _items.Add(slot);
            }
        }
    }

    private void Update()
    {
        // TODO: Gold 변화 이벤트로 대체
        if (SaveManager.Instance.MySaveData.gold < _price)
        {
            _priceText.color = Color.red;
        }
        else
        {
            _priceText.color = Color.white;
        }
    }

    public void OnClickBuyButton()
    {
        var save = SaveManager.Instance.MySaveData;
        if (save.gold < _price) return;

        save.gold -= _price;

        foreach (var (id, slotItem) in _cartItems)
        {
            save.ModifyItem(id, slotItem.Amount);
            //Debug.Log($"{ItemManager.Instance.ItemList[id].name}을 {slotItem.Amount}개 구입 -> {SaveManager.Instance.MySaveData.items[id]}");
        }
        foreach (var id in _cartItems.Keys.ToList())
        {
            RemoveFromCart(id);
        }
        _cartItems.Clear();

        SaveManager.Instance.SetSaveData(nameof(save.gold), save.gold);
        SaveManager.Instance.SetSaveData(nameof(save.items), save.items);
    }

    public void AddToCart(int itemId, int amount, Sprite icon)
    {
        var itemData = ItemManager.Instance.ItemList[itemId];
        if (!_cartItems.TryGetValue(itemId, out var cartItem))
        {
            cartItem = PoolManager.Instance.Get<SlotCartItem>(_cartPrefab, _cartParent);
            cartItem.Initialize(this, itemData);
            _cartItems[itemId] = cartItem;
        }

        cartItem.ModifyAmount(amount);
        _price += itemData.value * amount;

        RefreshPrice();
    }

    public void RemoveFromCart(int itemId)
    {
        if (_cartItems.TryGetValue(itemId, out var cartItem))
        {
            RemoveFromCart(itemId, cartItem.Amount);
        }
    }

    public void RemoveFromCart(int itemId, int amount)
    {
        if (!_cartItems.TryGetValue(itemId, out var cartItem)) return;

        amount = Mathf.Min(amount, cartItem.Amount);
        cartItem.ModifyAmount(-amount);

        _price -= ItemManager.Instance.ItemList[itemId].value * amount;
        _price = Mathf.Max(0, _price);

        if (cartItem.Amount <= 0)
        {
            _cartItems.Remove(itemId);
        }

        RefreshPrice();
    }

    private void RefreshPrice()
    {
        _priceText.SetText(_price.ToString());
    }
}