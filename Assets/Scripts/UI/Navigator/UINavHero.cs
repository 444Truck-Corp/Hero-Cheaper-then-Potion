using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UINavHero : UIBase
{
    [Header("list obj")]
    [SerializeField] private Transform ListParent;
    [SerializeField] private GameObject ListPrefab;

    private Dictionary<int, SlotHeroList> heroSlots = new();
    private Dictionary<int, HeroData> heroInfos = new();

    private readonly int summonPrice = 100;

    public override void Opened(object[] param)
    {
        FetchHeroList();
    }

    public void OnHeroSlotSelected(int listIdx)
    {
        UIManager.Show<UIPopupHeroDetail>(heroInfos[listIdx]);
    }

    public void OnSummonHeroClicked()
    {
        SaveManager.Instance.MySaveData.gold = 9999;
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
        Dictionary<int, HeroData> heros = SaveManager.Instance.MySaveData.ownedHeros;
        if (!heroInfos.SequenceEqual(heros))
        {
            foreach (Transform child in ListParent)
            {
                Destroy(child.gameObject); //List 초기화
            }

            heroInfos = heros;

            foreach (var (key, heroData) in heros)
            {
                SlotHeroList newSlot = Instantiate(ListPrefab, ListParent).GetComponent<SlotHeroList>();
                heroSlots.Add(key, newSlot);
                newSlot.InitHeroSlot(key, heroData);
            }
        }
    }
    #endregion
}