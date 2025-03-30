using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SlotScheduleQuest : MonoBehaviour
{
    [SerializeField] private PotionToggleGroup p;

    [SerializeField] private GameObject nullBtn;
    [SerializeField] private GameObject info;

    [SerializeField] private TextMeshProUGUI questNameTxt;
    [SerializeField] private TextMeshProUGUI strTxt;
    [SerializeField] private TextMeshProUGUI dexTxt;
    [SerializeField] private TextMeshProUGUI intTxt;
    [SerializeField] private TextMeshProUGUI lukTxt;
    [SerializeField] private TextMeshProUGUI successRateTxt;

    /*스케줄 정보*/
    private HeroData[] selectedHero = new HeroData[3]; 
    private QuestData selectedQuest;
    private int successRate;

    public void InitSlot()
    {//초기화 (NavSChedule OnEnable)
        nullBtn.SetActive(false); //빈 퀘스트 선택 버튼.
        info.SetActive(false); //내용UI + 퀘스트 선택 버튼.

        for (int i = 0; i < selectedHero.Length; i++)
        {//데이터 초기화.
            selectedHero[i] = null;
            selectedQuest = null;
            successRate = 0;
        }
    }

    public void HeroSelected(int idx, HeroData hero)
    { //스케줄표에 선택된 히어로 정보 저장.
        //if (!info.activeSelf)
        //    nullBtn.SetActive(true);
        
        //if (selectedHero[idx] != null)
        //{//현재 스케줄에서 선택된 캐릭터 풀어주기
        //    HeroManager.Instance.heroStates[selectedHero[idx].id] = eHeroState.FREE;
        //}
        
        //selectedHero[idx] = hero;
        //HeroManager.Instance.heroStates[selectedHero[idx].id] = eHeroState.QUEST;
    }

    public void SetScheduleSlot(QuestData quest)
    {
        nullBtn.SetActive(false);
        info.SetActive(true);

        selectedQuest = quest;

        questNameTxt.text = selectedQuest.QuestName;
        strTxt.text = $"<size=18>STR </size>{quest.needSpecs[0]}";
        dexTxt.text = $"<size=18>DEX </size>{quest.needSpecs[1]}";
        intTxt.text = $"<size=18>INT </size>{quest.needSpecs[2]}";
        lukTxt.text = $"<size=18>LUK </size>{quest.needSpecs[3]}";

        CalculateSuccessRate();
    }

    public void CalculateSuccessRate()
    {
        if (selectedQuest != null)
        {
            float baseRate = 0;
            float rateWithPotion = 0;
            float _str = 0, _dex = 0, _int = 0, _luk = 0;

            // 선택된 영웅의 스탯 합산
            for (int i = 0; i < selectedHero.Length; i++)
            {
                if (selectedHero[i] != null)
                {
                    _str += selectedHero[i].status.STR;
                    _dex += selectedHero[i].status.DEX;
                    _int += selectedHero[i].status.INT;
                    _luk += selectedHero[i].status.LUK;
                }
            }

            // 포션 효과 전 기본 성공률 계산
            float baseStr = Mathf.Min(_str / selectedQuest.needSpecs[0], 1);
            float baseDex = Mathf.Min(_dex / selectedQuest.needSpecs[1], 1);
            float baseInt = Mathf.Min(_int / selectedQuest.needSpecs[2], 1);
            float baseLuk = Mathf.Min(_luk / selectedQuest.needSpecs[3], 1);

            baseRate = baseStr * baseDex * baseInt * baseLuk;
            baseRate = Mathf.RoundToInt(baseRate * 100);

            // 포션 효과 적용
            float pStr = _str, pDex = _dex, pInt = _int, pLuk = _luk;
            int[] potionIdxs = p.GetSelectedIndices();

            for (int i = 0; i < potionIdxs.Length; i++)
            {
                switch (potionIdxs[i])
                {
                    case 0:
                        pStr += 5;
                        break;
                    case 1:
                        pDex += 5;
                        break;
                    case 2:
                        pInt += 5;
                        break;
                    case 3:
                        pLuk += 5;
                        break;
                }
            }

            // 포션 효과 후 성공률 계산
            pStr = Mathf.Min(pStr / selectedQuest.needSpecs[0], 1);
            pDex = Mathf.Min(pDex / selectedQuest.needSpecs[1], 1);
            pInt = Mathf.Min(pInt / selectedQuest.needSpecs[2], 1);
            pLuk = Mathf.Min(pLuk / selectedQuest.needSpecs[3], 1);

            rateWithPotion = pStr * pDex * pInt * pLuk;
            rateWithPotion = Mathf.RoundToInt(rateWithPotion * 100);

            // 성공률 텍스트 출력
            successRateTxt.text = $"{baseRate}%";

            if (rateWithPotion > baseRate)
            {
                int bonusRate = Mathf.RoundToInt(rateWithPotion - baseRate);
                successRateTxt.text += $" <color=#0BFF00><size=18>+{bonusRate}%</size></color>";
            }
            successRate = Mathf.RoundToInt(rateWithPotion);
        }
    }

    public void ReturnScheduleInfo(out List<int> list, out QuestData qData, out int rate)
    {
        list = new();
        for (int i = 0; i < selectedHero.Length; i++)
        {
            if (selectedHero[i] == null) continue;
            list.Add(selectedHero[i].id);
        }
        qData = selectedQuest;
        rate = successRate;
    }

    public void ReturnHeroStates()
    {
        for (int i = 0;i < selectedHero.Length;i++)
        {
            if (selectedHero[i] != null)
            {
                //HeroManager.Instance.heroStates[selectedHero[i].id] = eHeroState.FREE;
            }
        }
    }
}