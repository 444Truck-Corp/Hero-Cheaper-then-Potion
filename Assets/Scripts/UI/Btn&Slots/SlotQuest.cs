using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

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
    [SerializeField] private Image[] heroImgs;
    [SerializeField] private Button[] heroBtns;
    private List<int> heroIds = new() { -1, -1, -1 };

    [Header("보내기 버튼")]
    [SerializeField] private GameObject sendBtnObj;

    private bool isInteractive;

    private void Awake()
    {
        for (int i = 0; i < heroBtns.Length; i++)
        {
            int idx = i;
            heroBtns[i].onClick.AddListener(() => { if (isInteractive) _ = OnHeroSlotClicked(idx); });
        }
    }

    public void SetData(QuestData questData, bool isPending = false)
    {
        questId = questData.id;
        isInteractive = isPending;

        titleTxt.text = questData.title;
        timeTxt.text = $"{questData.time}일";
        rewardTxt.text = $"{questData.reward} P";
        rankTxt.text = Utils.ChangeToStars(questData.rank);

        sendBtnObj.SetActive(isPending);
        SetHeroButtonsInteractable(isPending);

        if (isPending)
        {
            heroIds = new() { -1, -1, -1 };
        }

        ResetHeroIcons();
    }

    public void SetHeroes(List<int> ids)
    {
        for (int i = 0; i < heroIds.Count; i++)
            heroIds[i] = (i < ids.Count) ? ids[i] : -1;

        ResetHeroIcons();
    }

    private void SetHeroButtonsInteractable(bool interactable)
    {
        foreach (var btn in heroBtns)
            btn.interactable = interactable;
    }

    private async Task OnHeroSlotClicked(int slotIdx)
    {
        int selectedId = await RequestHeroSelection();
        heroIds[slotIdx] = selectedId;
        ResetHeroIcons();
    }

    public void OnSendButtonClicked()
    {
        List<int> selectedHeroIds = new();
        foreach (var id in heroIds)
        {
            if (id != -1) selectedHeroIds.Add(id);
        }

        if (selectedHeroIds.Count == 0)
        {
            Debug.LogWarning("용사가 선택되지 않았습니다.");
            return;
        }

        QuestManager.Instance.SendQuest(questId, selectedHeroIds);
        Destroy(gameObject); // 완료 후 제거
    }

    private void ResetHeroIcons()
    {
        for (int i = 0; i < heroIds.Count; i++)
        {
            int id = heroIds[i];

            if (id == -1)
            {
                SetHeroImageVisible(i, false);
            }
            else if (SaveManager.Instance.MySaveData.ownedHeros.TryGetValue(id, out var heroData))
            {
                string spriteName = heroData.classData.id.ToString();
                heroImgs[i].sprite = ResourceManager.Instance.LoadAsset<Sprite>(thumbnailPath, spriteName);
                SetHeroImageVisible(i, true);
            }
        }
    }

    private void SetHeroImageVisible(int idx, bool visible)
    {
        if (idx < 0 || idx >= heroImgs.Length) return;

        Color c = heroImgs[idx].color;
        c.a = visible ? 1f : 0f;
        heroImgs[idx].color = c;
    }

    private Task<int> RequestHeroSelection()
    {
        var tcs = new TaskCompletionSource<int>();
        UIManager.Show<UIPopupQuestHero>((Action<int>)((int selectedId) =>
        {
            tcs.SetResult(selectedId);
        }));
        return tcs.Task;
    }
}