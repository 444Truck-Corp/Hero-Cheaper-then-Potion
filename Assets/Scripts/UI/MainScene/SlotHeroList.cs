using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlotHeroList : MonoBehaviour
{
    [SerializeField] private Button myBtn;
    public BigInteger listKey;

    [SerializeField] private Image thumbnail;
    [SerializeField] private TextMeshProUGUI nameTxt;
    [SerializeField] private TextMeshProUGUI lvTxt;
    [SerializeField] private TextMeshProUGUI classTxt;

    public void InitHeroSlot(BigInteger idx, HeroData hero)
    {
        listKey = idx;

        /*ui info*/
        string className = hero.classData.className;
        thumbnail.sprite = ResourceManager.Instance.LoadAsset<Sprite>("Thumbnails", className);
        nameTxt.text = hero.name;
        lvTxt.text = $"Lv : {hero.level}";

        //TODO : Localization 시 설정 필요
        classTxt.text = className.ToString();
    }

    public void OnSlotClicked()
    {
        UINavHero ui = UIManager.Get<UINavHero>();
        if (ui != null)
        {
            ui.OnHeroSlotSelected(listKey);
        }
    }
}