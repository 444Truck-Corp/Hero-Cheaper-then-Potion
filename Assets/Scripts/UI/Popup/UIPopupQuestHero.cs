using System;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupQuestHero : UIBase
{
    [SerializeField] private Transform listParent;
    [SerializeField] private GameObject slotPrefab;

    private Action<int> onSelected;

    public override void Opened(object[] param)
    {
        onSelected = param.Length > 0 && param[0] is Action<int> cb ? cb : null;
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

        var allHeros = SaveManager.Instance.MySaveData.ownedHeros;

        foreach (var (id, hero) in allHeros)
        {
            if (hero.state != eHeroState.FREE) continue;

            var go = Instantiate(slotPrefab, listParent);
            var slot = go.GetComponent<SlotHeroList>();
            slot.InitHeroSlot(id, hero);
            slot.BindSelection(() =>
            {
                onSelected?.Invoke(hero.classData.id);
                SetActive<UIPopupQuestHero>(false);
            });
        }
    }
}