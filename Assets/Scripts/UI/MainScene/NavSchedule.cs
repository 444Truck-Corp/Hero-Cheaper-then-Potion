//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using UnityEngine;
//using UnityEngine.UI;

//public class NavSchedule : MonoBehaviour
//{
//    private int clickedSlotIdx = -1; //HeroSelect 이벤트를 적용할 칸.

//    [SerializeField] PopupHeroSelect popupHeroSelect; //용사 선택창
//    [SerializeField] PopupQuest popupQuest; //퀘스트 선택창

//    [SerializeField] SlotScheduleHero[] scheduleHeroSlots = new SlotScheduleHero[3];
//    [SerializeField] SlotScheduleQuest scheduledQuestSlot;
//    [SerializeField] Button questStartBtn;

//    [SerializeField] PotionToggleGroup toggleGroup;

//    public int[] QuestSlotIdx { get; private set; } = new int[4];

//    private bool[] isSelected = new bool[4];

//    private void OnEnable()
//    {
//        //GameManager.Instance.HeroSelectAction = SetHeroSlot;
//        //GameManager.Instance.QuestSelectAction = SetQuestSlot;

//        clickedSlotIdx = -1;
//        questStartBtn.interactable = false;

//        for (int i = 0; i < 3; i++)
//        {
//            scheduleHeroSlots[i].InitSlot();
//            isSelected[i] = false;
//        }

//        scheduledQuestSlot.InitSlot();
//        isSelected[3] = false;

//        //for (int i = 0; i < GameManager.Instance.TodayQuests.Length; i++)
//        //{
//        //    QuestSlotIdx[i] = GameManager.Instance.TodayQuests[i];
//        //}

//        //if (!GameManager.Instance.FirstQuest)
//        //{
//        //    Debug.Log("지원금 수령 퀘");
//        //    QuestSlotIdx[0] = 0;
//        //}

//        StartCoroutine(EnableQuestStart());
//    }

//    #region Select Hero
//    public void OnHeroSelectBtn(int idx)
//    {
//        clickedSlotIdx = idx;
//        popupHeroSelect.gameObject.SetActive(true);
//    }

//    public void SetHeroSlot(int heroIdx)
//    {
//        if (clickedSlotIdx == -1)
//        {//방어코드
//            Debug.LogWarning("슬롯 idx 이상함.");
//            return;
//        }

//        HeroData heroData = null;
//        //HeroData heroData = HeroManager.Instance.GetHero(heroIdx);

//        isSelected[clickedSlotIdx] = true;
//        scheduleHeroSlots[clickedSlotIdx].SetScheduleSlot(heroData);

//        scheduledQuestSlot.HeroSelected(clickedSlotIdx, heroData);
//        scheduledQuestSlot.CalculateSuccessRate();
//    }
//    #endregion

//    #region Select Quest
//    public void OnQuestSelectBtn()
//    {
//        popupQuest.gameObject.SetActive(true);
//    }

//    public void SetQuestSlot(int questIdx)
//    {
//        QuestData questData = DataManager.Instance.GetData<QuestData>(nameof(QuestData), questIdx);

//        isSelected[3] = true;
//        scheduledQuestSlot.SetScheduleSlot(questData);
//    }
//    #endregion

//    #region Quest Start Btn
//    private IEnumerator EnableQuestStart()
//    {
//        yield return new WaitUntil(() => isSelected[0] || isSelected[1] || isSelected[2]);
//        yield return new WaitUntil(() => isSelected[3]);
//        questStartBtn.interactable = true;
//    }

//    public void OnQuestStartBtn()
//    {
//        GameManager.Instance.UsePotion(toggleGroup.GetSelectedIndices());
//        GameManager.Instance.OnPotionActionEvent();

//        scheduledQuestSlot.ReturnScheduleInfo(out List<int> heroIdx, out QuestData qData, out int successRate);

//        HeroManager.Instance.AddQuestSchedule(heroIdx, qData, successRate);

//        if (qData.id == 0) { GameManager.Instance.FirstQuest = true; }
//        // TODO : 여기에서 퀘스트가 셔플되어야 함
//        gameObject.SetActive(false);
//    }
//    #endregion

//    public void OnBackBtn()
//    {
//        scheduledQuestSlot.ReturnHeroStates();
//        gameObject.SetActive(false);
//    }
//}