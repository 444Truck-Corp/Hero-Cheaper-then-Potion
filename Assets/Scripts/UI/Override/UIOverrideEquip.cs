using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIOverrideEquip : UIBase
{
    [SerializeField] private TextMeshProUGUI equipTitle;
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private Transform itemParent;
    
    private EEquipType curType;
    private Action<int> selectedEquipmentId;
    private HeroData curHero;

    public override void Opened(object[] param)
    {
        curType = (param.Length > 0) ? (EEquipType)param[0] : default;
        selectedEquipmentId = (param.Length > 1) ? (Action<int>)param[1] : null;
        curHero = (param.Length > 2) ? (HeroData)param[2] : null;

        equipTitle.text = curType switch
        {
            EEquipType.Head => "투구",
            EEquipType.Chest => "갑옷",
            EEquipType.Leg => "바지",
            EEquipType.Accessory => "악세사리",
            EEquipType.Weapon => "무기",
            _ => ""
        };

        Dictionary<int, EquipmentData> equipDics = SaveManager.Instance.MySaveData.ownedEquips;

        foreach (KeyValuePair<int, EquipmentData> pair in equipDics)
        {
            if (pair.Value.parts == curType)
            {
                if (curHero.id == pair.Value.equippedHero)
                    continue;

                GameObject slot = Instantiate(itemPrefab, itemParent);

                Button slotBtn = slot.GetComponent<Button>();
                slotBtn.onClick.RemoveAllListeners();
                slotBtn.onClick.AddListener(() => selectedEquipmentId?.Invoke(pair.Key));

                Image slotImg = slot.transform.GetChild(0).GetComponent<Image>();
                slotImg.sprite = ResourceManager.Instance.LoadAsset<Sprite>(ResourceManager.textureDir, pair.Value.icon);
            }
        }
    }

    public void CloseTab()
    {
        UIManager.Hide<UIOverrideEquip>();
    }
}
