using System.Collections.Generic;

public class UIOverrideDayResult : UIBase
{
    public override void Opened(object[] param)
    {
        NextDayEvents();
        SetActive<UIOverrideDayResult>(false);
    }

    private void NextDayEvents()
    {
        int today = SaveManager.Instance.MySaveData.day;
        UpdateQuestInfo(today);
    }

    private void UpdateQuestInfo(int today)
    {
        var completedQuests = new List<QuestProcessInfo>();

        foreach (var quest in SaveManager.Instance.MySaveData.processingQuests)
        {
            if (quest.returnDay == today)
            {
                completedQuests.Add(quest);
            }
        }

        foreach (var completed in completedQuests)
        {
            // 진행중 리스트에서 제거
            var processList = SaveManager.Instance.MySaveData.processingQuests;
            processList.Remove(completed);
            SaveManager.Instance.SetSaveData(nameof(SaveData.processingQuests), processList);

            // 용사 복귀
            foreach (var heroId in completed.heroIds)
            {
                var hero = SaveManager.Instance.MySaveData.ownedHeros[heroId];
                hero.state = eHeroState.FREE;
            }

            //골드 지급
            var questData = QuestManager.Instance.GetQuestData(completed.questId);
            SaveManager.Instance.SetSaveData(nameof(SaveData.gold), SaveManager.Instance.MySaveData.gold + questData.reward);
            
            //TODO : 아이템 확률적으로 획득
        }
    }
}