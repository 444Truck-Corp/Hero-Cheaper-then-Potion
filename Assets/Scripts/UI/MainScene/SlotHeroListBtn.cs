using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlotHeroListBtn : MonoBehaviour
{
    public PopupHeroSelect popupHeroSelect;
    [SerializeField] private Button myBtn;
    public int listIdx;

    [SerializeField] private Image thumbnail;
    [SerializeField] private TextMeshProUGUI nameTxt;
    [SerializeField] private TextMeshProUGUI strTxt;
    [SerializeField] private TextMeshProUGUI dexTxt;
    [SerializeField] private TextMeshProUGUI intTxt;
    [SerializeField] private TextMeshProUGUI lukTxt;

    public void InitHeroSlot(int idx, PopupHeroSelect p)
    {
        popupHeroSelect = p;
        listIdx = idx;
    }

    public void SetHeroSlot(HeroData hero, ref bool isEnd)
    {
        //if (myBtn != null)
        //{
        //    if (HeroManager.Instance.heroStates[hero.id] != eHeroState.FREE)
        //        myBtn.interactable = false;
        //    else
        //    {
        //        myBtn.interactable = true;
        //        isEnd = true;
        //    }
        //}
        //else isEnd = true;
        
        ///*ui info*/
        //thumbnail.sprite = DataManager.Instance.GetSprites(false, hero.spriteIdx);
        //nameTxt.text = hero.name;
        //strTxt.text = $"STR : {hero.status.STR}";
        //dexTxt.text = $"DEX : {hero.status.DEX}";
        //intTxt.text = $"INT : {hero.status.INT}";
        //lukTxt.text = $"LUK : {hero.status.LUK}";
    }

    public void OnSlotClicked()
    {
        if (popupHeroSelect != null)
        {
            popupHeroSelect.OnSelectSlot(listIdx);
        }
    }
}