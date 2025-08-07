using NUnit.Framework.Interfaces;
using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupHeroDetail : UIBase
{
    [Header("Hero Detail")]
    [SerializeField] private Image standImg;
    [SerializeField] private TextMeshProUGUI nameTxt;
    [SerializeField] private TextMeshProUGUI classTxt;

    [Header("Hero Level")]
    [SerializeField] private TextMeshProUGUI lvTxt;
    [SerializeField] private Slider lvSlider;
    [SerializeField] private TextMeshProUGUI lvPriceTxt;

    [Header("Hero Status")]
    [SerializeField] private TextMeshProUGUI strTxt;
    [SerializeField] private TextMeshProUGUI dexTxt;
    [SerializeField] private TextMeshProUGUI intTxt;
    [SerializeField] private TextMeshProUGUI maxHpTxt;
    [SerializeField] private TextMeshProUGUI curHpTxt;

    [Header("Equips")]
    [SerializeField] private Image[] equipImages;
    [SerializeField] private Sprite[] defaultEquips;
    [SerializeField] private Color defaultColor;

    private int[] equippedItem = new int[5];
    private EEquipType lastChosenEquipType = default;

    private int curHeroIdx;
    private int maxExp = 0;
    private int incExp = 0;
    private float curSldrValue = 0;

    public override void Opened(object[] param)
    {
        HeroData curHero = param.Length > 0 && param[0] is HeroData heroData ? heroData : null;
        curHeroIdx = curHero.id;

        standImg.sprite = ResourceManager.Instance.LoadAsset<Sprite>(ResourceManager.standDir, curHero.classData.id.ToString());

        nameTxt.text = curHero.name;
        lvTxt.text = curHero.level.ToString();
        classTxt.text = curHero.classData.className.ToString();

        strTxt.text = curHero.status.STR.ToString();
        dexTxt.text = curHero.status.DEX.ToString();
        intTxt.text = curHero.status.INT.ToString();
        maxHpTxt.text = curHero.maxHP.ToString();
        curHpTxt.text = curHero.curHP.ToString();

        curSldrValue = 0;
        lvSlider.value = 0;
        lvSlider.onValueChanged.RemoveAllListeners();
        lvSlider.onValueChanged.AddListener(OnExpValueChanged);

        lvPriceTxt.text = "0 G";

        maxExp = HeroManager.Instance.lvList[curHero.level].characExp;
        maxExp -= curHero.exp;

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
    }

    public void OnExpBtn()
    {
        if (incExp <= SaveManager.Instance.MySaveData.gold)
        {
            HeroData curHero = SaveManager.Instance.MySaveData.ownedHeroes[curHeroIdx];
            curHero.GetExp(incExp);
            SaveManager.Instance.SetSaveData(nameof(SaveData.gold), SaveManager.Instance.MySaveData.gold - incExp);

            maxExp = HeroManager.Instance.lvList[curHero.level].characExp;
            incExp = (int)(curSldrValue * maxExp);
            lvPriceTxt.text = $"{incExp} G";

            lvTxt.text = curHero.level.ToString();
            strTxt.text = curHero.status.STR.ToString();
            dexTxt.text = curHero.status.DEX.ToString();
            intTxt.text = curHero.status.INT.ToString();
            maxHpTxt.text = curHero.maxHP.ToString();
            curHpTxt.text = curHero.curHP.ToString();
        }
        else
        {
            //TODO : 보유 금액 부족 알림.
        }
    }

    public void OnEquipBtn(int type)
    {
        HeroData heroData = SaveManager.Instance.MySaveData.ownedHeroes[curHeroIdx];

        //장착중인 아이템이 있다면 장착해제
        if (equippedItem[type] != 0)
        {
            heroData.Equip(equippedItem[type]);
            equippedItem[type] = 0;
            equipImages[type].sprite = defaultEquips[type];
            equipImages[type].color = defaultColor;
            return;
        }

        lastChosenEquipType = (EEquipType)type;
        Action<int> selectedEquipmentId = SetEquipOnSlot;
        UIManager.Show<UIOverrideEquip>(lastChosenEquipType, selectedEquipmentId, heroData);
    }

    private void OnExpValueChanged(float value)
    {
        curSldrValue = value;
        incExp = (int)(value * maxExp);
        lvPriceTxt.text = $"{incExp} G";
    }

    private void SetEquipOnSlot(int equipId)
    {
        UIManager.Hide<UIOverrideEquip>();

        int type = (int)lastChosenEquipType;
        equippedItem[type] = equipId;

        if (equipId != 0)
        {   
            EquipmentData equipData = SaveManager.Instance.MySaveData.ownedEquips[equipId];
            equipImages[type].sprite = ResourceManager.Instance.LoadAsset<Sprite>(ResourceManager.textureDir, equipData.icon);
            equipImages[type].color = Color.white;

            HeroData heroData = SaveManager.Instance.MySaveData.ownedHeroes[curHeroIdx];
            heroData.Equip(equipId);
        }
        else
        {
            equipImages[type].sprite = defaultEquips[type];
            equipImages[type].color = defaultColor;
        }
    }
}