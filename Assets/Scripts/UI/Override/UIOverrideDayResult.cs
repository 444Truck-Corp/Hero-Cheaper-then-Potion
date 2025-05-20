using TMPro;
using UnityEngine;

public class UIOverrideDayResult : UIBase
{
    [SerializeField] private TextMeshProUGUI foodProfitTxt;
    [SerializeField] private TextMeshProUGUI questProfitTxt;
    [SerializeField] private TextMeshProUGUI dailyCostTxt;
    [SerializeField] private TextMeshProUGUI totalProfitTxt; // 총 평가 금액 텍스트

    public override void Opened(object[] param)
    {
        ProcessDayTransition();
        SetActive<UIOverrideDayResult>(false);
    }

    public void OnBtnClicked()
    {
        SetActive<UIOverrideDayResult>(false);
    }

    private void ProcessDayTransition()
    {
        UpdateDayResultUI();
    }

    private void UpdateDayResultUI()
    {
        int foodProfit = CalculateFoodProfit();
        foodProfitTxt.text = $"길드 식당 수익 : {foodProfit} G";

        int questProfit = ProcessCompletedQuests();
        questProfitTxt.text = $"퀘스트 수익 : {questProfit} G";

        int dailyCost = CalculateDailyCost();
        dailyCostTxt.text = $"길드 유지비 : {dailyCost} G";

        SaveManager.Instance.SetSaveData(nameof(SaveData.gold), -dailyCost);

        int totalProfit = foodProfit + questProfit - dailyCost;
        UpdateTotalProfitText(totalProfit);
    }

    private int CalculateFoodProfit()
    {
        int currentGold = SaveManager.Instance.MySaveData.gold;
        int startGold = SaveManager.Instance.MySaveData.dayStartGold;
        int profit = currentGold - startGold;

        SaveManager.Instance.MySaveData.dayStartGold = currentGold;
        return profit;
    }

    private int ProcessCompletedQuests()
    {
        int currentDay = SaveManager.Instance.MySaveData.day;
        var quests = SaveManager.Instance.MySaveData.processingQuests;
        var completedQuests = quests.FindAll(q => q.returnDay == currentDay);

        int totalQuestReward = 0;

        foreach (var quest in completedQuests)
        {
            RemoveCompletedQuest(quest);
            var data = QuestManager.Instance.GetQuestDataById(quest.questId);
            totalQuestReward += data.reward;
        }

        SaveManager.Instance.SetSaveData(nameof(SaveData.gold), SaveManager.Instance.MySaveData.gold + totalQuestReward);
        SaveManager.Instance.MySaveData.dayStartGold = SaveManager.Instance.MySaveData.gold;

        return totalQuestReward;
    }

    private void RemoveCompletedQuest(QuestProcessInfo quest)
    {
        SaveManager.Instance.MySaveData.processingQuests.Remove(quest);
        SaveManager.Instance.SetSaveData(nameof(SaveData.processingQuests), SaveManager.Instance.MySaveData.processingQuests);

        foreach (var heroId in quest.heroIds)
        {
            SaveManager.Instance.MySaveData.ownedHeroes[heroId].state = EHeroState.FREE;
        }

        // TODO: 아이템 확률적으로 획득
    }

    private int CalculateDailyCost()
    {
        int cost = SaveManager.Instance.MySaveData.rank * 150;

        foreach (var hero in SaveManager.Instance.MySaveData.ownedHeroes.Values)
        {
            cost += hero.level * 150;
        }

        return cost;
    }

    private void UpdateTotalProfitText(int totalProfit)
    {
        totalProfitTxt.text = $"평가 금액 : {totalProfit} G";

        string colorHex = totalProfit < 0 ? "#FF6464" : "#64C8FF";
        totalProfitTxt.color = HexToColor(colorHex);
    }

    private Color HexToColor(string hex)
    {
        Color color;
        if (ColorUtility.TryParseHtmlString(hex, out color))
            return color;

        return Color.white;
    }
}