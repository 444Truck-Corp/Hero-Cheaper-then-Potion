using System.Collections.Generic;

public class ItemManager : Singleton<ItemManager>
{
    public Dictionary<int, ItemData> ItemList => _itemList; 
    public Dictionary<int, EquipmentData> EquipmentList => _equipmentList; 

    private readonly Dictionary<int, ItemData> _itemList = new();
    private readonly Dictionary<int, EquipmentData> _equipmentList = new();

    protected override void Awake()
    {
        base.Awake();

        // 아이템 초기화
        List<ItemData> itemList = DataManager.Instance.GetObjList<ItemData>(nameof(ItemData));
        foreach (ItemData item in itemList)
        {
            _itemList[item.id] = item;
        }
        
        // 장비 목록 초기화
        List<EquipmentData> equipmentList = DataManager.Instance.GetObjList<EquipmentData>(nameof(EquipmentData));
        foreach (EquipmentData equipment in equipmentList)
        {
            _equipmentList[equipment.id] = equipment;
        }
    }
}