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

    private int curHeroIdx;

    private int maxExp = 0;
    private int incExp = 0;
    private float curSldrValue = 0;

    public override void Opened(object[] param)
    {
        HeroData data = param.Length > 0 && param[0] is HeroData heroData ? heroData : null;
        curHeroIdx = data.id;

        standImg.sprite = ResourceManager.Instance.LoadAsset<Sprite>(ResourceManager.standDir, data.classData.id.ToString());

        nameTxt.text = data.name;
        lvTxt.text = data.level.ToString();
        classTxt.text = data.classData.className.ToString();

        strTxt.text = data.status.STR.ToString();
        dexTxt.text = data.status.DEX.ToString();
        intTxt.text = data.status.INT.ToString();
        maxHpTxt.text = data.status.HP.ToString();
        curHpTxt.text = data.curHP.ToString();

        curSldrValue = 0;
        lvSlider.value = 0;
        lvSlider.onValueChanged.RemoveAllListeners();
        lvSlider.onValueChanged.AddListener(OnExpValueChanged);

        lvPriceTxt.text = "0 G";

        maxExp = HeroManager.Instance.lvList[data.level].characExp;
        maxExp -= data.exp;
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
            maxHpTxt.text = curHero.status.HP.ToString();
            curHpTxt.text = curHero.curHP.ToString();
        }
        else
        {
            //TODO : 보유 금액 부족 알림.
        }
    }

    private void OnExpValueChanged(float value)
    {
        curSldrValue = value;
        incExp = (int)(value * maxExp);
        lvPriceTxt.text = $"{incExp} G";
    }
}