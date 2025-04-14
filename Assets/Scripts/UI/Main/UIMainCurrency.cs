using TMPro;
using UnityEngine;

public class UIMainCurrency : MonoBehaviour
{
    [Header("Status")]
    [SerializeField] private TextMeshProUGUI goldTxt;
    [SerializeField] private TextMeshProUGUI dailyConsumeTxt;

    #region Unity Life Cycles   
    private void OnEnable()
    {
        EventManager.Instance.AddSaveDataListener(nameof(SaveData.gold), UpdateGoldUI);
        EventManager.Instance.AddSaveDataListener(nameof(SaveData.dailyConsume), UpdateDailyConsumeUI);
    }

    private void Start()
    {
        UpdateGoldUI();
        UpdateDailyConsumeUI();
    }

    private void OnDisable()
    {
        EventManager.Instance.RemoveSaveDataListener(nameof(SaveData.gold), UpdateGoldUI);
        EventManager.Instance.RemoveSaveDataListener(nameof(SaveData.dailyConsume), UpdateDailyConsumeUI);
    }
    #endregion

    #region Sub Methods
    private void UpdateGoldUI()
    {
        int gold = SaveManager.Instance.MySaveData.gold;
        goldTxt.text = gold.ToString();
    }

    private void UpdateDailyConsumeUI()
    {
        int dailyConsume = SaveManager.Instance.MySaveData.dailyConsume;
        dailyConsumeTxt.text = dailyConsume.ToString();
    }
    #endregion
}