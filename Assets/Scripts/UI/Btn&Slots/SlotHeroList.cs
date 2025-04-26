using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlotHeroList : MonoBehaviour
{
    [SerializeField] private Button myBtn;
    public int listKey;

    [SerializeField] private TextMeshProUGUI nameTxt;
    [SerializeField] private TextMeshProUGUI lvTxt;
    [SerializeField] private TextMeshProUGUI classTxt;
    [SerializeField] private Image thumbnail;

    public void InitHeroSlot(int idx, HeroData hero)
    {
        listKey = idx;

        nameTxt.text = hero.name;
        lvTxt.text = $"Lv : {hero.level}";

        ClassData classData = hero.classData;
        classTxt.text = classData.className;
        thumbnail.sprite = ResourceManager.Instance.LoadAsset<Sprite>("Thumbnails", classData.id.ToString());
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