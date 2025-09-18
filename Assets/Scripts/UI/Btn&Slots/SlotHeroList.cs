using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlotHeroList : MonoBehaviour
{
    private Action onClickCallback;

    [SerializeField] private Button myBtn;
    [NonSerialized] public int listKey;

    [SerializeField] private TextMeshProUGUI nameTxt;
    [SerializeField] private TextMeshProUGUI lvTxt;
    [SerializeField] private TextMeshProUGUI classTxt;
    [SerializeField] private Image thumbnail;
    [SerializeField] private TextMeshProUGUI hpTxt;

    public void InitHeroSlot(int idx, HeroData hero)
    {
        listKey = idx;

        nameTxt.text = hero.name;
        lvTxt.text = $"{hero.level}";

        ClassData classData = hero.classData;
        classTxt.text = classData.className;
        thumbnail.sprite = ResourceManager.Instance.LoadAsset<Sprite>(ResourceManager.thumbnailDir, classData.id.ToString());

        if (hpTxt != null) hpTxt.text = Utils.BuildHpBar(hero);
    }

    public void BindSelection(Action callback)
    {
        onClickCallback = callback;
    }

    public void OnSlotClicked()
    {
        onClickCallback?.Invoke();
    }

}