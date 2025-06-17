using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIOverrideDayResult : UIBase
{
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private Transform ResultTxtParent;
    [SerializeField] private GameObject ResultTxtPrefab;

    private readonly Color blueTxtColor = new(0.39f, 0.78f, 1f, 1f); // #64C8FF
    private readonly Color redTxtColor = new(1f, 0.39f, 0.39f, 1f); // #FF6464

    public override void Opened(object[] param)
    {
        ProcessDayTransition();
    }

    public void OnBtnClicked()
    {
        SetActive<UIOverrideDayResult>(false);
    }

    private void ProcessDayTransition()
    {
        StartCoroutine(UpdateDayResultUI());
    }

    private IEnumerator UpdateDayResultUI()
    {
        int totalProfit = 0;

        // 음식 판매 수익 계산
        yield return StartCoroutine(HandleFoodProfits());
        AddLine("", Color.white);
        yield return new WaitForSeconds(0.2f);

        // 퀘스트 보상 처리
        yield return StartCoroutine(HandleQuestResults());
        AddLine("", Color.white);
        yield return new WaitForSeconds(0.2f);

        // 생활비 차감
        int dailyCost = CalculateDailyCost();
        AddLine($"생활비 : {dailyCost} G\n", redTxtColor);
        totalProfit += foodProfitSubtotal + questProfitSubtotal - dailyCost;
        yield return new WaitForSeconds(0.2f);

        // 구분선 및 최종 수익
        AddLine("========================================", Color.white);
        yield return new WaitForSeconds(0.5f);

        SaveManager.Instance.SetSaveData(nameof(SaveData.gold), SaveManager.Instance.MySaveData.gold + totalProfit);
        AddLine($"최종 수익 : {totalProfit} G", Color.yellow);
    }

    private int foodProfitSubtotal = 0;
    private IEnumerator HandleFoodProfits()
    {
        foodProfitSubtotal = 0;
        var foodProfits = SaveManager.Instance.MySaveData.foodProfits;

        foreach (var foodProfit in foodProfits)
        {
            ItemManager.Instance.ItemList.TryGetValue(foodProfit.Key, out var foodData);
            int value = foodData.value * foodProfit.Value;
            foodProfitSubtotal += value;

            AddLine($"{foodData.name} * {foodProfit.Value} = {value} G", blueTxtColor);
            yield return new WaitForSeconds(0.1f);
        }
    }

    private int questProfitSubtotal = 0;
    private IEnumerator HandleQuestResults()
    {
        questProfitSubtotal = 0;
        int currentDay = SaveManager.Instance.MySaveData.day;
        var quests = SaveManager.Instance.MySaveData.processingQuests;
        var completedQuests = quests.FindAll(q => q.returnDay == currentDay);

        foreach (var quest in completedQuests)
        {
            RemoveCompletedQuest(quest);
            var questData = QuestManager.Instance.GetQuestDataById(quest.questId);
            string questMsg = $"{questData.title} 결과: ";

            List<HeroData> questHeros = GetQuestHeroes(quest);
            StatusData heroSum = SumHeroStatus(questHeros);
            float successProb = CalculateSuccessProbability(heroSum, questData.needSpecs);
            bool isSuccess = Random.value < successProb;

            if (isSuccess)
            {
                questProfitSubtotal += questData.goldReward;
                questMsg += $"성공!\t+{questData.goldReward} G, ";
                questMsg += DistributeQuestRewards(questData.parsedDropItems);
            }
            else
            {
                questMsg += "실패...";
            }

            AddLine(questMsg, isSuccess ? blueTxtColor : redTxtColor);
            yield return new WaitForSeconds(0.1f);

            // 용사 데미지 계산 및 사망 처리
            List<HeroData> deadHeros = ApplyQuestDamage(questHeros, questData.rank, successProb, isSuccess);
            if (deadHeros.Count > 0)
            {
                string deathMsg = "RIP: ";
                foreach (var hero in deadHeros)
                {
                    deathMsg += $"{hero.name} ";
                    SaveManager.Instance.MySaveData.ownedHeroes.Remove(hero.id);
                }
                AddLine(deathMsg, redTxtColor);
                yield return new WaitForSeconds(0.1f);
            }
        }
    }

    private List<HeroData> GetQuestHeroes(QuestProcessInfo quest)
    {
        List<HeroData> result = new();
        foreach (int heroId in quest.heroIds)
        {
            if (SaveManager.Instance.MySaveData.ownedHeroes.TryGetValue(heroId, out HeroData hero))
            {
                result.Add(hero);
            }
        }
        return result;
    }

    private StatusData SumHeroStatus(List<HeroData> heros)
    {
        StatusData sum = new();
        foreach (var h in heros)
        {
            sum += h.status;
        }
        return sum;
    }

    private float CalculateSuccessProbability(StatusData sum, StatusData req)
    {
        float prob = 0;
        prob += 0.25f * Mathf.Min(sum.STR / (float)req.STR, 1f);
        prob += 0.25f * Mathf.Min(sum.DEX / (float)req.DEX, 1f);
        prob += 0.25f * Mathf.Min(sum.INT / (float)req.INT, 1f);
        prob += 0.25f * Mathf.Min(sum.HP / (float)req.HP, 1f);
        return prob;
    }

    private string DistributeQuestRewards(List<DropItemInfo> drops)
    {
        string rewardText = "";
        foreach (var drop in drops)
        {
            if (Random.value < drop.probability)
            {
                if (SaveManager.Instance.MySaveData.items.TryGetValue(drop.id, out int amount))
                {
                    SaveManager.Instance.MySaveData.items[drop.id] = amount + 1;
                }
                else
                {
                    SaveManager.Instance.MySaveData.items[drop.id] = 1;
                }
                ItemManager.Instance.ItemList.TryGetValue(drop.id, out var itemData);
                rewardText += $"{itemData.name} ";
            }
        }
        return rewardText;
    }

    private List<HeroData> ApplyQuestDamage(List<HeroData> heros, int rank, float successProb, bool isSuccess)
    {
        List<HeroData> dead = new();
        int damage = isSuccess ? rank * 10 : (int)(rank * 10 + (1 - successProb) * 15f);

        foreach (var hero in heros)
        {
            hero.status.HP = Mathf.Max(0, hero.status.HP - damage);
            if (hero.status.HP == 0)
            {
                dead.Add(hero);
            }
        }
        return dead;
    }

    private TextMeshProUGUI AddLine(string message, Color color)
    {
        var entry = Instantiate(ResultTxtPrefab, ResultTxtParent);
        var textComponent = entry.GetComponent<TextMeshProUGUI>();
        textComponent.text = message;
        textComponent.color = color;

        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f;

        return textComponent;
    }

    private void RemoveCompletedQuest(QuestProcessInfo quest)
    {
        SaveManager.Instance.MySaveData.processingQuests.Remove(quest);
        SaveManager.Instance.SetSaveData(nameof(SaveData.processingQuests), SaveManager.Instance.MySaveData.processingQuests);

        foreach (var heroId in quest.heroIds)
        {
            SaveManager.Instance.MySaveData.ownedHeroes[heroId].state = EHeroState.FREE;
        }
    }

    private int CalculateDailyCost()
    {
        int cost = 300 + SaveManager.Instance.MySaveData.rank * 150;
        foreach (var hero in SaveManager.Instance.MySaveData.ownedHeroes.Values)
        {
            cost += hero.level * 150;
        }
        return cost;
    }
}