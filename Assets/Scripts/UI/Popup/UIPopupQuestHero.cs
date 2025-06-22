using System;
using System.Collections.Generic;
using UnityEngine;

public class UIPopupQuestHero : UIBase
{
    [SerializeField] private Transform listParent;
    [SerializeField] private GameObject slotPrefab;

    private UINavQuest navQuest;
    private Action<int> onSelected;

    public override void Opened(object[] param)
    {
        navQuest = param.Length > 0 && param[0] is UINavQuest nav ? nav : null;
        onSelected = param.Length > 1 && param[1] is Action<int> cb ? cb : null;
        Refresh();
    }

    public void OnCancelButtonClicked()
    {
        onSelected?.Invoke(-1);
        SetActive<UIPopupQuestHero>(false);
    }

    private void Refresh()
    {
        foreach (Transform child in listParent)
            Destroy(child.gameObject);

        Dictionary<int, HeroData> allHeros = new(SaveManager.Instance.MySaveData.ownedHeroes);
        foreach (int key in navQuest.selectedHeros)
        {
            allHeros.Remove(key);
        }

        foreach (var (id, hero) in allHeros)
        {
            if (hero.state != EHeroState.FREE) continue;

            var go = Instantiate(slotPrefab, listParent);
            var slot = go.GetComponent<SlotHeroList>();
            slot.InitHeroSlot(id, hero);
            slot.BindSelection(() =>
            {
                onSelected?.Invoke(hero.id);
                SetActive<UIPopupQuestHero>(false);
            });
        }
    }
}