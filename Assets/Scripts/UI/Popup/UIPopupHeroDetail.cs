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
    [SerializeField] private TextMeshProUGUI strEquipTxt;
    [SerializeField] private TextMeshProUGUI strPotionTxt;
    [SerializeField] private TextMeshProUGUI dexTxt;
    [SerializeField] private TextMeshProUGUI dexEquipTxt;
    [SerializeField] private TextMeshProUGUI dexPotionTxt;
    [SerializeField] private TextMeshProUGUI intTxt;
    [SerializeField] private TextMeshProUGUI intEquipTxt;
    [SerializeField] private TextMeshProUGUI intPotionTxt;
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
    private int selectedPotionIdx = -1;

    public override void Opened(object[] param)
    {
        HeroData curHero = param.Length > 0 && param[0] is HeroData heroData ? heroData : null;
        curHeroData = curHero;

        standImg.sprite = ResourceManager.Instance.LoadAsset<Sprite>(ResourceManager.standDir, curHero.classData.id.ToString());

        nameTxt.text = curHero.name;
        classTxt.text = curHero.classData.className.ToString();

        ResetLvSldr();
        lvSlider.onValueChanged.RemoveAllListeners();
        lvSlider.onValueChanged.AddListener(OnExpValueChanged);

        SetStatus();

        equippedItem = curHero.equipList;
        for (int i  = 0; i < equippedItem.Length; i++)
        {
            if (equippedItem[i] == 0)
            {
                equipImages[i].sprite = defaultEquips[i];
                equipImages[i].color = defaultColor;
            }
            else
            {
                EquipmentData equipData = SaveManager.Instance.MySaveData.ownedEquips[equippedItem[i]];
                equipImages[i].sprite = ResourceManager.Instance.LoadAsset<Sprite>(ResourceManager.textureDir, equipData.icon);
                equipImages[i].color = Color.white;
            }
        }

        foreach (var item in SaveManager.Instance.MySaveData.items)
        {
            int itemIdx = item.Key;
            ItemData itemData = ItemManager.Instance.ItemList[itemIdx];
            if (itemData == null || itemData.category != EItemCategory.Potion) continue;

            SlotPotion slot = Instantiate(potionTogglePrefab, potionToggleGroup.transform).GetComponent<SlotPotion>();
            potionToggles.Add(slot);

            slot.SetSlot(itemData);
            slot.toggle.group = potionToggleGroup;
            slot.toggle.isOn = false;
            slot.toggle.onValueChanged.AddListener(isOn =>
            {
                if (isOn) selectedPotionIdx = slot.slotNum;
                else selectedPotionIdx = -1;
                Debug.Log(selectedPotionIdx);
            });
        }
    }

    public override void Closed(object[] param)
    {
        foreach (Transform item in potionToggleGroup.transform)
            Destroy(item.gameObject);
    }

    private void OnExpValueChanged(float value)
    {
        curPrice = 0;
        for (int i = curHeroData.level; i < incLv; i++)
        {
            if (i >= maxLv) break;
            curPrice += HeroManager.Instance.lvList[i].characExp;
        }
        lvPriceTxt.text = $"{curPrice} G";
        slideValueTxt.text = $"+ {incLv - curHeroData.level}Lv";
    }

    public void OnExpBtn()
    {
        if (curPrice <= SaveManager.Instance.MySaveData.gold)
        {
            SaveManager.Instance.SetSaveData(nameof(SaveData.gold), SaveManager.Instance.MySaveData.gold - curPrice);
            curHeroData.GetLevel(incLv - curHeroData.level);
            ResetLvSldr();
            SetStatus();
        }
        else
        {
            //TODO : 보유 금액 부족 알림.
        }
    }

    public void OnEquipBtn(int type)
    {
        //장착중인 아이템이 있다면 장착해제
        if (equippedItem[type] != 0)
        {
            curHeroData.Equip(equippedItem[type]);
            equippedItem[type] = 0;
            equipImages[type].sprite = defaultEquips[type];
            equipImages[type].color = defaultColor;
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
        Vector3 nextPos = potionTab.position;
        nextPos.x += isOpened ? -300 : 300;
        potionTab.DOMove(nextPos, 1f)
            .OnComplete(() => potionTabToggle.interactable = true);
    }

    public void OnUsePotionBtn()
    {
        if (selectedPotionIdx < 0 || selectedPotionIdx >= potionToggles.Count) return;
        int potionId = potionToggles[selectedPotionIdx].ItemData.id;
        if (potionId == 0) return; // 선택된 포션이 없는 경우.

        if (SaveManager.Instance.MySaveData.items.TryGetValue(potionId, out int count) && count > 0)
        {
            SlotPotion curPotion = potionToggles[selectedPotionIdx];
            curHeroData.UsePotion(curPotion.ItemData);
            SetStatus();

            if (count - 1 == 0)
            {
                potionToggles.Remove(curPotion);
                Destroy(curPotion.gameObject);
                selectedPotionIdx = -1;
            }
            else
            {
                curPotion.SetPotionCount(count - 1);
            }
        }
        else
        {
            //TODO : 포션 사용 불가 알림.
        }
    }

    private void ResetLvSldr()
    {
        lvTxt.text = curHeroData.level.ToString();
        lvSlider.minValue = curHeroData.level;
        lvSlider.value = 0;
        curPrice = 0;
        lvPriceTxt.text = $"{curPrice} G";
        slideValueTxt.text = $"+ {incLv - curHeroData.level}Lv";
    }

    private void SetStatus()
    {
        strTxt.text = curHeroData.status.STR.ToString();
        dexTxt.text = curHeroData.status.DEX.ToString();
        intTxt.text = curHeroData.status.INT.ToString();
        Utils.BuildHpBar(curHeroData);

        if (curHeroData.equipStatus.STR == 0) strEquipTxt.gameObject.SetActive(false);
        else
        {
            strEquipTxt.gameObject.SetActive(true);
            if (curHeroData.equipStatus.STR > 0)
                strEquipTxt.text = $"(+{curHeroData.equipStatus.STR})";
            else if (curHeroData.equipStatus.STR < 0)
                strEquipTxt.text = $"({curHeroData.equipStatus.STR})";

            Utils.RefreshHorizontalRow(strEquipTxt.transform.parent, strEquipTxt);
        }
        if (curHeroData.equipStatus.DEX == 0) dexEquipTxt.gameObject.SetActive(false);
        else
        {
            dexEquipTxt.gameObject.SetActive(true);
            if (curHeroData.equipStatus.DEX > 0)
                dexEquipTxt.text = $"(+{curHeroData.equipStatus.DEX})";
            else if (curHeroData.equipStatus.DEX < 0)
                dexEquipTxt.text = $"({curHeroData.equipStatus.DEX})";

            Utils.RefreshHorizontalRow(dexEquipTxt.transform.parent, dexEquipTxt);
        }
        if (curHeroData.equipStatus.INT == 0) intEquipTxt.gameObject.SetActive(false);
        else
        {
            intEquipTxt.gameObject.SetActive(true);
            if (curHeroData.equipStatus.INT > 0)
                intEquipTxt.text = $"(+{curHeroData.equipStatus.INT})";
            else if (curHeroData.equipStatus.INT < 0)
                intEquipTxt.text = $"({curHeroData.equipStatus.INT})";

            Utils.RefreshHorizontalRow(intEquipTxt.transform.parent, intEquipTxt);
        }

        if (curHeroData.potionStatus.STR == 0) strPotionTxt.gameObject.SetActive(false);
        else
        {
            strPotionTxt.gameObject.SetActive(true);
            if (curHeroData.potionStatus.STR > 0)
                strPotionTxt.text = $"(+{curHeroData.potionStatus.STR})";
            else if (curHeroData.potionStatus.STR < 0)
                strPotionTxt.text = $"({curHeroData.potionStatus.STR})";
            Utils.RefreshHorizontalRow(strPotionTxt.transform.parent, strPotionTxt);
        }
        if (curHeroData.potionStatus.DEX == 0) dexPotionTxt.gameObject.SetActive(false);
        else
        {
            dexPotionTxt.gameObject.SetActive(true);
            if (curHeroData.potionStatus.DEX > 0)
                dexPotionTxt.text = $"(+{curHeroData.potionStatus.DEX})";
            else if (curHeroData.potionStatus.DEX < 0)
                dexPotionTxt.text = $"({curHeroData.potionStatus.DEX})";
            Utils.RefreshHorizontalRow(dexPotionTxt.transform.parent, dexPotionTxt);
        }
        if (curHeroData.potionStatus.INT == 0) intPotionTxt.gameObject.SetActive(false);
        else
        {
            intPotionTxt.gameObject.SetActive(true);
            if (curHeroData.potionStatus.INT > 0)
                intPotionTxt.text = $"(+{curHeroData.potionStatus.INT})";
            else if (curHeroData.potionStatus.INT < 0)
                intPotionTxt.text = $"({curHeroData.potionStatus.INT})";
            Utils.RefreshHorizontalRow(intPotionTxt.transform.parent, intPotionTxt);
        }
    }

    private void SetEquipOnSlot(int equipId)
    {
        UIManager.Hide<UIOverrideEquip>();
        
        int type = (int)lastChosenEquipType;
        if (equipId != 0)
        {
            EquipmentData equipData = SaveManager.Instance.MySaveData.ownedEquips[equipId];
            Debug.Log(curHeroData.equipList[0]);
            equipImages[type].sprite = ResourceManager.Instance.LoadAsset<Sprite>(ResourceManager.textureDir, equipData.icon);
            equipImages[type].color = Color.white;

            curHeroData.Equip(equipId);
        }
        else
        {
            equipImages[type].sprite = defaultEquips[type];
            equipImages[type].color = defaultColor;
        }

        SetStatus();
    }
}