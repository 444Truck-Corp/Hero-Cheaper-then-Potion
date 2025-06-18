using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UINavInven : UIBase
{
    [SerializeField] private GameObject _itemPrefab;
    [SerializeField] private Transform _itemParent;

    private static readonly Dictionary<int, (EItemCategory category, EEquipType? equip)> _categoryMap = new()
    {
        { 0, (EItemCategory.Equipment, EEquipType.Weapon) },
        { 1, (EItemCategory.Equipment, EEquipType.Head) },
        { 2, (EItemCategory.Equipment, EEquipType.Chest) },
        { 3, (EItemCategory.Equipment, EEquipType.Leg) },
        { 4, (EItemCategory.Equipment, EEquipType.Accessory) },
        { 5, (EItemCategory.Material, null) },
        { 6, (EItemCategory.Potion, null) }
    };

    [SerializeField] private EItemCategory _selectedCategory;
    [SerializeField] private EEquipType _selectedEquipType;
    [SerializeField] private int _selectedIndex;
    [SerializeField] private List<SlotInventoryItem> _slots = new();

    public override void Opened(object[] param)
    {
        SetCategory(_selectedIndex);
        FetchItemList();
    }

    public void SetCategory(int index)
    {
        _selectedIndex = index;
        FetchItemList();
    }

    private void FetchItemList()
    {
        UpdateCategory();

        var items = FilterItems();
        InitializeSlots(items);
    }

    private List<KeyValuePair<int, int>> FilterItems()
    {
        var allItems = SaveManager.Instance.MySaveData.items;

        if (_selectedCategory == EItemCategory.Equipment)
        {
            return allItems
                .Where(item => ItemManager.Instance.EquipmentList[item.Key].parts == _selectedEquipType)
                .ToList();
        }

        return allItems
            .Where(item => ItemManager.Instance.ItemList[item.Key].category == _selectedCategory)
            .ToList();
    }

    private void InitializeSlots(List<KeyValuePair<int, int>> items)
    {
        int totalItemCount = _selectedCategory == EItemCategory.Equipment
            ? items.Sum(item => item.Value)
            : items.Count;

        UpdateSlotCount(totalItemCount);

        int index = 0;
        foreach (var item in items)
        {
            for (int i = 0; i < (_selectedCategory == EItemCategory.Equipment ? item.Value : 1); i++)
            {
                var data = _selectedCategory == EItemCategory.Equipment
                    ? ItemManager.Instance.EquipmentList[item.Key]
                    : ItemManager.Instance.ItemList[item.Key];

                _slots[index++].Initialize(data);
            }
        }
    }

    private void UpdateSlotCount(int targetCount)
    {
        if (targetCount < _slots.Count)
        {
            for (int i = targetCount; i < _slots.Count; i++)
            {
                PoolManager.Instance.Return(_slots[i]);
            }
            _slots.RemoveRange(targetCount, _slots.Count - targetCount);
        }

        while (_slots.Count < targetCount)
        {
            var slot = PoolManager.Instance.Get<SlotInventoryItem>(_itemPrefab, _itemParent);
            _slots.Add(slot);
        }
    }

    private void UpdateCategory()
    {
        if (_categoryMap.TryGetValue(_selectedIndex, out var result))
        {
            _selectedCategory = result.category;
            _selectedEquipType = result.equip ?? _selectedEquipType;
        }
    }
}