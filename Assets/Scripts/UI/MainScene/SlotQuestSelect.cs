//using TMPro;
//using UnityEngine;

//public class SlotQuestSelect : MonoBehaviour
//{
//    [SerializeField] private PopupQuest popupQuest;

//    [SerializeField] private TextMeshProUGUI nameTxt;
//    [SerializeField] private TextMeshProUGUI statTxt;
//    [SerializeField] private TextMeshProUGUI timeTxt;
//    [SerializeField] private TextMeshProUGUI rewardTxt;

//    private int myIdx = -1;

//    public void SetQSlot(QuestData quest)
//    {
//        nameTxt.text = quest.QuestName;
//        statTxt.text =
//            $"<size=18>STR </size>{quest.needSpecs[0]}, " +
//            $"<size=18>DEF </size>{quest.needSpecs[1]}, " +
//            $"<size=18>INT </size>{quest.needSpecs[2]}, " +
//            $"<size=18>LUK </size>{quest.needSpecs[3]}";

//        timeTxt.text = $"{quest.needTime}일";

//        rewardTxt.text =
//            $"<size=18><color=#AC7500>보상 : </color></size>";
//        if (quest.rewardValues[0] != 0)
//        {
//            rewardTxt.text += $"{quest.rewardValues[0]} 골드";
//            if (quest.rewardValues[1] != 0)
//            {
//                rewardTxt.text += $", {quest.rewardValues[1]} EXP";
//            }
//        }
//        else
//        {
//            rewardTxt.text += $"{quest.rewardValues[1]} EXP";
//        }

//        myIdx = quest.id;
//    }

//    public void OnSlotClicked()
//    {
//        if (popupQuest != null)
//        {
//            //popupQuest.OnSelectSlot(myIdx);
//        }
//    }
//}