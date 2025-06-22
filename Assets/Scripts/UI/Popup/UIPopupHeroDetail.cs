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

    public override void Opened(object[] param)
    {
        HeroData data = param.Length > 0 && param[0] is HeroData heroData ? heroData : null;

        standImg.sprite = ResourceManager.Instance.LoadAsset<Sprite>(ResourceManager.standDir, data.classData.id.ToString());

        nameTxt.text = data.name;
        lvTxt.text = data.level.ToString();
        classTxt.text = data.classData.className.ToString();

        strTxt.text = data.status.STR.ToString();
        dexTxt.text = data.status.DEX.ToString();
        intTxt.text = data.status.INT.ToString();
        maxHpTxt.text = data.status.HP.ToString();
        curHpTxt.text = data.curHP.ToString();
    }

    public void OnExpBtn()
    {
        if (lvSlider.gameObject.activeSelf)
        {
            lvSlider.gameObject.SetActive(false);
        }
        else
        {
            lvSlider.gameObject.SetActive(true);
            lvPriceTxt.text = "0 G";

            lvSlider.onValueChanged.RemoveAllListeners();
            lvSlider.onValueChanged.AddListener(OnExpValueChanged);
        }
    }

    private void OnExpValueChanged(float value)
    {
    }
}