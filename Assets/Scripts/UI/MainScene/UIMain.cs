using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIMain : MonoBehaviour
{
    [Header("Warning")]
    [SerializeField] Transform warningParent;

    [Header("UIs")]
    [SerializeField] GameObject navHeroList;
    [SerializeField] GameObject navSchedule;
    [SerializeField] GameObject navTraining;

    [Header("Volume Setting")]
    [SerializeField] private Slider volumeSlider;

    [Header("Status")]
    [SerializeField] private TextMeshProUGUI curGoldTxt;
    [SerializeField] private TextMeshProUGUI FoodTxt;
    [SerializeField] private TextMeshProUGUI[] potionTxts;
    private int foodCost = 0;

    [Header("Time Skip")]
    [SerializeField] private TextMeshProUGUI skipTxt;
    [SerializeField] private Button decreaseBtn;
    [SerializeField] private Button increaseBtn;
    [SerializeField] private Button skipBtn;
    private int skipCount = 1;

    [Header("Date")]
    [SerializeField] private TextMeshProUGUI dDayTxt;

    //private void Awake()
    //{
    //    GameManager.Instance.GoldChangeAction += OnGoldChange;
    //    GameManager.Instance.FoodChangeAction += OnFoodChange;
    //    GameManager.Instance.DayChangeAction += OnDayChange;
    //    GameManager.Instance.PotionAction += OnPotionChange;

    //    GameManager.Instance.SetWarnParent(warningParent);
    //    GameManager.Instance.SetDialogParent(warningParent);

    //    // AudioManager에 Slider 등록
    //    if (AudioManager.Instance != null)
    //    {
    //        AudioManager.Instance.RegisterSlider(volumeSlider);
    //    }
    //}

    //private void Start()
    //{
    //    OnGoldChange(GameManager.Instance.Gold);
    //    OnFoodChange();
    //    OnDayChange(GameManager.Instance.Day);

    //    // 초기 용사 한 명 소환
    //    HeroManager.Instance.MakeNewHero();
    //}

    

    //#region Status
    //private void OnGoldChange(int curGold)
    //{//골드 변화
    //    curGoldTxt.text = curGold.ToString() + " 골드";
    //}

    //private void OnFoodChange()
    //{//히어로 스케줄 
    //    foodCost = 0;
    //    for (int i = 0; i < HeroManager.Instance.heroStates.Count; i++)
    //    {
    //        if (HeroManager.Instance.heroStates[i] != eHeroState.QUEST)
    //            foodCost += 50 * HeroManager.Instance.heroList[i].level;
    //        else
    //        {
    //            foodCost += 10 * HeroManager.Instance.heroList[i].level;
    //        }
    //    }
    //    FoodTxt.text = foodCost.ToString() + " 골드";
    //}

    //private void OnPotionChange()
    //{
    //    for (int i = 0; i < potionTxts.Length; i++)
    //    {
    //        potionTxts[i].text = GameManager.Instance.Potions[i] + "개";
    //    }
    //}
    //#endregion

    //#region Time Skip
    //public void IncreaseDay()
    //{
    //    decreaseBtn.interactable = true;
    //    skipTxt.text = $"{++skipCount}일";
    //    if (skipCount + GameManager.Instance.Day == 0)
    //    {
    //        increaseBtn.interactable = false;
    //    }
    //}

    //public void DecreaseDay()
    //{
    //    increaseBtn.interactable = true;
    //    skipTxt.text = $"{--skipCount}일";
    //    if (skipCount == 1)
    //    {
    //        decreaseBtn.interactable = false;
    //    }
    //}

    //// 시간을 바꾸는 곳
    //public void ChangeDate()
    //{
    //    GameManager.Instance.OnDayChangeButtonEvent?.Invoke(skipCount);
    //    skipCount = 1;
    //    skipTxt.text = $"{skipCount}일";
    //    decreaseBtn.interactable = false;
    //    skipBtn.interactable = false;
    //    increaseBtn.interactable = true;
    //}

    //public void SetSkipButtonInteraction(bool value)
    //{
    //    skipBtn.interactable = value;
    //}

    //public void ChangeOneDay()
    //{
    //    // 정산
    //    if (foodCost != 0) GameManager.Instance.OnHeroFeedEvent?.Invoke(foodCost);
    //    GameManager.Instance.OnGoldChangeEvent(-foodCost);

    //    int trainCount = 0;
    //    foreach (var h in HeroManager.Instance.heroStates)
    //    {
    //        if (h == eHeroState.TRAINING) trainCount++;
    //    }
    //    trainCount *= 20;
    //    if (trainCount != 0) GameManager.Instance.OnHeroTrainingGoldEvent?.Invoke(trainCount);
    //    GameManager.Instance.OnGoldChangeEvent(-trainCount);

    //    GameManager.Instance.OnDayChangeEvent(1);

    //    // 정산 후 파산 확인
    //    if (GameManager.Instance.Gold < 0)
    //    {
    //        GameManager.Instance.Ending = eEnding.Bankrupt;
    //        SceneManager.LoadScene(2);
    //    }
    //}
    //#endregion

    //#region Date
    //private void OnDayChange(int today)
    //{
    //    dDayTxt.text = $"{-today}일";
    //}
    //#endregion
}