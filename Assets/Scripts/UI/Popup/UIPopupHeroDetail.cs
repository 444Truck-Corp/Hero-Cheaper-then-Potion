using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupHeroDetail : UIBase
{
    private HeroData curHeroData;

    [Header("Hero Detail")]
    [SerializeField] private Image standImg;
    [SerializeField] private TextMeshProUGUI nameTxt;
    [SerializeField] private TextMeshProUGUI classTxt;

    [Header("Hero Level")]
    [SerializeField] private TextMeshProUGUI lvTxt;
    [SerializeField] private Slider lvSlider;
    [SerializeField] private TextMeshProUGUI lvPriceTxt;
    [SerializeField] private TextMeshProUGUI slideValueTxt;
    private const int maxLv = 20;
    private int curPrice = 0;
    private int incLv => (int)lvSlider.value;

    [Header("Hero Status")]
    [SerializeField] private TextMeshProUGUI strTxt;
    [SerializeField] private TextMeshProUGUI dexTxt;
    [SerializeField] private TextMeshProUGUI intTxt;
    [SerializeField] private TextMeshProUGUI hpTxt;
    
    [Header("Equips")]
    [SerializeField] private Image[] equipImages;
    [SerializeField] private Sprite[] defaultEquips;
    [SerializeField] private Color defaultColor;
    private int[] equippedItem = new int[5];
    private EEquipType lastChosenEquipType = default;

    [Header("Potion")]
    [SerializeField] private Transform potionTab;
    [SerializeField] private Toggle potionTabToggle;
    [SerializeField] private ToggleGroup potionToggleGroup;
    [SerializeField] private GameObject potionTogglePrefab;
    [SerializeField] private List<SlotPotion> potionToggles;
    private SlotPotion selectedPotion;

    private const string COL_BASE = "#DACC00";
    private const string COL_EQUIP = "#2FDA00";
    private const string COL_POTION = "#00D8FF";

    public override void Opened(object[] param)
    {
        if (param.Length == 0 || param[0] is not HeroData curHero || curHero == null) return;
        curHeroData = curHero;

        standImg.sprite = ResourceManager.Instance.LoadAsset<Sprite>(ResourceManager.standDir, curHero.classData.id.ToString());

        nameTxt.text = curHero.name;
        classTxt.text = curHero.classData.className.ToString();

        lvSlider.onValueChanged.RemoveAllListeners();
        ResetLvSldr();
        lvSlider.onValueChanged.AddListener(OnExpValueChanged);

        SetStatus();

        equippedItem = curHero.equipList;
        int n = Mathf.Min(equipImages.Length, defaultEquips.Length);
        for (int i  = 0; i < n; i++)
        {
            if (equippedItem.Length <= i || equipImages[i] == null) continue;

            if (equippedItem[i] == 0)
            {
                equipImages[i].sprite = defaultEquips[i];
                equipImages[i].color = defaultColor;
            }
            else
            {
                if (SaveManager.Instance.MySaveData.ownedEquips.TryGetValue(equippedItem[i], out var equipData))
                {
                    equipImages[i].sprite = ResourceManager.Instance.LoadAsset<Sprite>(ResourceManager.textureDir, equipData.icon);
                    equipImages[i].color = Color.white;
                }
                else
                {
                    equipImages[i].sprite = defaultEquips[i];
                    equipImages[i].color = defaultColor;
                }
            }
        }

        ClearPotionUI();
        foreach (var kv in SaveManager.Instance.MySaveData.items)
        {
            int itemId = kv.Key;
            ItemData itemData = ItemManager.Instance.ItemList[itemId];
            if (itemData == null || itemData.category != EItemCategory.Potion) continue;

            SlotPotion slot = Instantiate(potionTogglePrefab, potionToggleGroup.transform).GetComponent<SlotPotion>();
            potionToggles.Add(slot);

            slot.SetSlot(itemData);
            slot.toggle.group = potionToggleGroup;
            slot.toggle.isOn = false;
            slot.toggle.onValueChanged.AddListener(isOn =>
            {
                if (isOn) selectedPotion = slot;
                else selectedPotion = null;
            });
        }
    }

    public override void Closed(object[] param)
    {
        ClearPotionUI();
    }

    private void OnExpValueChanged(float _)
    {
        int curLv = curHeroData.level;
        int targetLv = Mathf.Clamp(incLv, curLv, maxLv);
        int price = 0;
        for (int i = curLv; i < targetLv; i++)
        {
            price += HeroManager.Instance.LevelList[i].characExp;
        }
        curPrice = price;
        lvPriceTxt.text = $"{curPrice} G";
        slideValueTxt.text = $"+ {targetLv - curLv}Lv";
    }

    public void OnExpBtn()
    {
        if (incLv <= curHeroData.level) return;
        if (curPrice <= SaveManager.Instance.MySaveData.gold)
        {
            SaveManager.Instance.SetSaveData(nameof(SaveData.gold), SaveManager.Instance.MySaveData.gold - curPrice);
            curHeroData.GetLevel(incLv - curHeroData.level);
            ResetLvSldr();
            SetStatus();
        }
        else
        {
            // 부족 알림 처리
        }
    }

    public void OnEquipBtn(int type)
    {
        if (type < 0 || type >= equippedItem.Length) return;

        //장착중인 아이템이 있다면 장착해제
        if (equippedItem[type] != 0)
        {
            curHeroData.Equip(equippedItem[type]);
            equippedItem[type] = 0;
            if (type < equipImages.Length)
            {
                equipImages[type].sprite = defaultEquips[type];
                equipImages[type].color = defaultColor;
            }
            SetStatus();
            return;
        }

        lastChosenEquipType = (EEquipType)type;
        Action<int> selectedEquipmentId = SetEquipOnSlot;
        UIManager.Show<UIOverrideEquip>(lastChosenEquipType, selectedEquipmentId, curHeroData);
    }

    public void OnPotionTabToggle(bool isOpened)
    {
        potionTabToggle.interactable = false;
        var rt = potionTab as RectTransform;
        if (rt == null) return;
        Vector2 next = rt.anchoredPosition;
        next.x += isOpened ? -300f : 300f;
        rt.DOAnchorPos(next, 1f).OnComplete(() => potionTabToggle.interactable = true);
    }

    public void OnUsePotionBtn()
    {
        if (selectedPotion == null) return;
        int potionId = selectedPotion.ItemData.id;
        if (!SaveManager.Instance.MySaveData.items.TryGetValue(potionId, out int count) || count <= 0) return;

        curHeroData.UsePotion(selectedPotion.ItemData);
        SetStatus();

        if (count - 1 == 0)
        {
            Destroy(selectedPotion.gameObject);
            potionToggles.Remove(selectedPotion);
            selectedPotion = null;
        }
        else
        {
            selectedPotion.SetPotionCount(count - 1);
        }
    }

    private void ResetLvSldr()
    {
        int curLv = curHeroData.level;
        lvTxt.text = curLv.ToString();

        lvSlider.wholeNumbers = true;
        lvSlider.minValue = curLv;
        lvSlider.maxValue = maxLv;
        lvSlider.value = curLv;
        lvSlider.interactable = curLv < maxLv;

        curPrice = 0;
        lvPriceTxt.text = "0 G";
        slideValueTxt.text = "+ 0Lv";
    }

    private void SetStatus()
    {
        hpTxt.text = Utils.BuildHpBar(curHeroData);

        strTxt.text = BuildStatLine(curHeroData.status.STR, curHeroData.equipStatus.STR, curHeroData.potionStatus.STR);
        Utils.RefreshHorizontalRow(strTxt.transform.parent, strTxt);
        dexTxt.text = BuildStatLine(curHeroData.status.DEX, curHeroData.equipStatus.DEX, curHeroData.potionStatus.DEX);
        Utils.RefreshHorizontalRow(dexTxt.transform.parent, dexTxt);
        intTxt.text = BuildStatLine(curHeroData.status.INT, curHeroData.equipStatus.INT, curHeroData.potionStatus.INT);
        Utils.RefreshHorizontalRow(intTxt.transform.parent, intTxt);
    }

    private string BuildStatLine(int baseVal, int equipVal, int potionVal)
    {
        int total = baseVal + equipVal + potionVal;

        string basePart = $"<color={COL_BASE}>{baseVal}</color>";
        if (equipVal == 0 && potionVal == 0) return $"{total}";

        string equipPart = equipVal != 0 ? $" + <color={COL_EQUIP}>{equipVal}</color>" : "";
        string potionPart = potionVal != 0 ? $" + <color={COL_POTION}>{potionVal}</color>" : "";

        return $"{total} ({basePart}{equipPart}{potionPart})";
    }

    private void SetEquipOnSlot(int equipId)
    {
        UIManager.Hide<UIOverrideEquip>();

        int type = (int)lastChosenEquipType;
        if (type < 0 || type >= equipImages.Length) return;

        if (equipId != 0 && SaveManager.Instance.MySaveData.ownedEquips.TryGetValue(equipId, out var equipData))
        {
            equipImages[type].sprite = ResourceManager.Instance.LoadAsset<Sprite>(ResourceManager.textureDir, equipData.icon);
            equipImages[type].color = Color.white;
            curHeroData.Equip(equipId);
            if (type < equippedItem.Length) equippedItem[type] = equipId;
        }
        else
        {
            equipImages[type].sprite = defaultEquips[type];
            equipImages[type].color = defaultColor;
            if (type < equippedItem.Length) equippedItem[type] = 0;
        }

        SetStatus();
    }

    private void ClearPotionUI()
    {
        selectedPotion = null;
        foreach (Transform t in potionToggleGroup.transform) Destroy(t.gameObject);
        potionToggles.Clear();
    }
}