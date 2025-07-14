using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UINavQuest : UIBase
{
    private enum eTab
    {
        Accepted = 0,
        Onboard,
        Always
    }

    [Header("탭")]
    [SerializeField] private Button[] questBtns;
    [SerializeField] private GameObject[] questScrollview;
    [SerializeField] private Transform[] questContent;

    [Header("슬롯")]
    [SerializeField] private GameObject slotPrefab;

    eTab curTab = eTab.Accepted;

    public void OnClickTabAccepted() => SwitchTab(eTab.Accepted);
    public void OnClickTabOnboard() => SwitchTab(eTab.Onboard);
    public void OnClickTabAlways() => SwitchTab(eTab.Always);

    [HideInInspector]
    public List<int> selectedHeros;

    public override void Opened(object[] param)
    {
        SwitchTab(curTab);
        selectedHeros = new();
    }

    private void SwitchTab(eTab tab)
    {
        curTab = tab;
        int idx = (int)tab;

        for (int i = 0; i < questScrollview.Length; i++)
        {
            questScrollview[i].SetActive(i == idx);
        }

        switch (tab)
        {
            case eTab.Accepted:
                RefreshAccepted();
                break;
            case eTab.Onboard:
                RefreshOnboard();
                break;
            case eTab.Always:
                RefreshAlways();
                break;
        }
    }

    private void RefreshAccepted()
    {
        ClearContent((int)eTab.Accepted);

        var receivedList = SaveManager.Instance.MySaveData.receivedQuests;
        foreach (int questId in receivedList)
        {
            var quest = QuestManager.Instance.GetQuestDataById(questId);
            if (quest == null) continue;

            var go = Instantiate(slotPrefab, questContent[(int)eTab.Accepted]);
            var slot = go.GetComponent<SlotQuest>();
            slot.SetData(this, quest, true);
        }
    }

    private void RefreshOnboard()
    {
        ClearContent((int)eTab.Onboard);

        var ongoingList = SaveManager.Instance.MySaveData.processingQuests;
        foreach (var questInfo in ongoingList)
        {
            var quest = QuestManager.Instance.GetQuestDataById(questInfo.questId);
            if (quest == null) continue;

            var go = Instantiate(slotPrefab, questContent[(int)eTab.Onboard]);
            var slot = go.GetComponent<SlotQuest>();
            slot.SetData(this, quest, false);
            slot.SetHeroes(questInfo.heroIds);
        }
    }

    private void RefreshAlways()
    {
        ClearContent((int)eTab.Always);
    }

    private void ClearContent(int idx)
    {
        foreach (Transform child in questContent[idx])
            Destroy(child.gameObject);
    }
}
