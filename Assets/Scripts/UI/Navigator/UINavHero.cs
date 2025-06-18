using System.Collections.Generic;
using UnityEngine;

public class UINavHero : UIBase
{
    [Header("list obj")]
    [SerializeField] private Transform ListParent;
    [SerializeField] private GameObject ListPrefab;

    private Dictionary<int, SlotHeroList> heroSlots = new();
    private Dictionary<int, HeroData> heroInfos = new();

    private readonly int summonPrice = 100;

    protected override void Awake()
    {
        base.Awake();
        EventManager.Instance.AddSaveDataListener(nameof(SaveData.ownedHeroes), FetchHeroList);
    }

    public override void Opened(object[] param)
    {
        FetchHeroList();
    }

    private void OnDestroy()
    {
        EventManager.Instance.RemoveSaveDataListener(nameof(SaveData.ownedHeroes), FetchHeroList);
    }

    public void OnHeroSlotSelected(int listIdx)
    {
        UIManager.Show<UIPopupHeroDetail>(heroInfos[listIdx]);
    }

    public void OnSummonHeroClicked()
    {
        int gold = SaveManager.Instance.MySaveData.gold;

        if (gold >= summonPrice)
        {
            gold -= summonPrice;
            SaveManager.Instance.SetSaveData(nameof(SaveManager.Instance.MySaveData.gold), gold);
            HeroManager.Instance.SpawnNewHero();
            FetchHeroList();
        }
        else
        {
            //TODO : 경고 Popup.
        }
    }

    #region Sub Methods
    private void FetchHeroList()
    {
        Dictionary<int, HeroData> heros = SaveManager.Instance.MySaveData.ownedHeroes;
        heroInfos = new Dictionary<int, HeroData>(heros);

        heroSlots.Clear();
        foreach (Transform child in ListParent)
        {
            Destroy(child.gameObject);
        }

        foreach (var (key, heroData) in heros)
        {
            var slotObj = Instantiate(ListPrefab, ListParent);
            var slot = slotObj.GetComponent<SlotHeroList>();

            heroSlots.Add(key, slot);
            slot.InitHeroSlot(key, heroData);
        }
    }
    #endregion
}