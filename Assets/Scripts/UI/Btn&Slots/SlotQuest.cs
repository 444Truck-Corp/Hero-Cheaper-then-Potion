using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class SlotQuest : MonoBehaviour
{
    private int questId;
    private readonly string thumbnailPath = "Thumbnails";

    [Header("퀘스트 기본 정보")]
    [SerializeField] private TextMeshProUGUI titleTxt;
    [SerializeField] private TextMeshProUGUI timeTxt;
    [SerializeField] private TextMeshProUGUI rewardTxt;
    [SerializeField] private TextMeshProUGUI rankTxt;

    [Header("용사 아이콘 영역")]
    [SerializeField] private Image[] heroImgs;   // 직접 Image를 연결
    [SerializeField] private Button[] heroBtns;  // 직접 Button을 연결
    private List<int> heroIds = new List<int> { -1, -1, -1 };

    [Header("보내기 버튼")]
    [SerializeField] private GameObject sendBtnObj;

    #region 초기화
    public void SetData(QuestData questData, bool isPending = false)
    {
        questId = questData.id;

        titleTxt.text = questData.title;
        timeTxt.text = $"{questData.time}일";
        rewardTxt.text = $"{questData.reward} P";
        rankTxt.text = Utils.ChangeToStars(questData.rank);

        sendBtnObj.SetActive(isPending);
        ResetHeroIcons();
    }
    #endregion

    #region 상호작용
    public void OnHeroSlotClicked(int slotIdx)
    {
        Debug.Log($"Hero Slot {slotIdx} 클릭됨");

        // TODO:
        //Task 활용
        // HeroList UI를 띄워 선택한 HeroId를 받아오고
        // heroIds[slotIdx] = 선택된 HeroId
        // heroImgs[slotIdx]의 Sprite를 변경
    }

    public void OnSendButtonClicked()
    {
        List<int> selectedHeroIds = new List<int>();
        foreach (var id in heroIds)
        {
            if (id != -1)
                selectedHeroIds.Add(id);
        }

        if (selectedHeroIds.Count == 0)
        {
            Debug.LogWarning("용사가 선택되지 않았습니다.");
            return;
        }

        QuestManager.Instance.SendQuest(questId, selectedHeroIds);

        Destroy(gameObject); // 슬롯 삭제 (퀘스트가 진행중으로 이동했기 때문)
    }
    #endregion

    #region 보조 메서드
    private void ResetHeroIcons()
    {
        for (int i = 0; i < heroIds.Count; i++)
        {
            int id = heroIds[i];

            if (id == -1)
            {
                heroImgs[i].sprite = null; // 빈 슬롯
            }
            else
            {
                var heroData = SaveManager.Instance.MySaveData.ownedHeros[id];
                string spriteName = heroData.classData.id.ToString();
                heroImgs[i].sprite = ResourceManager.Instance.LoadAsset<Sprite>(thumbnailPath, spriteName);
            }
        }
    }
    #endregion
}
