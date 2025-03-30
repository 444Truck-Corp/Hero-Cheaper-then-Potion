using System.Collections.Generic;
using UnityEngine;

public class PopupHeroSelect : MonoBehaviour
{
    [Header("list obj")]
    [SerializeField] private Transform ListParent;
    [SerializeField] private GameObject ListPrefab;

    public List<SlotHeroList> HeroSlots { get; private set; } = new();//영웅 슬롯 캐싱.
    private bool isEnabled; //남은 영웅 찾기

    private void OnEnable()
    {
        //isEnabled = false;

        //for (int i = 0; i < HeroManager.Instance.heroStates.Count; i++)
        //{
        //    if (i >= HeroSlots.Count)
        //    {//신규 slot
        //        SlotHeroListBtn newSlot = Instantiate(ListPrefab, ListParent).GetComponent<SlotHeroListBtn>();
        //        HeroSlots.Add(newSlot);
        //        newSlot.InitHeroSlot(i, this);
        //    }
        //    HeroSlots[i].SetHeroSlot(HeroManager.Instance.GetHero(i), ref isEnabled);
        //}

        //if (HeroManager.Instance.heroStates.Count == 0)
        //{
        //    GameManager.Instance.InvokeWarning("아직 용사가 없습니다...");
        //    gameObject.SetActive(false);
        //}
        //else if (!isEnabled)
        //{
        //    GameManager.Instance.InvokeWarning("남은 용사가 없습니다...");
        //    gameObject.SetActive(false);
        //}
    }

    public void OnSelectSlot(int listIdx)
    {
        //GameManager.Instance.OnHeroSelectEvent(listIdx);
        //gameObject.SetActive(false);
    }
}