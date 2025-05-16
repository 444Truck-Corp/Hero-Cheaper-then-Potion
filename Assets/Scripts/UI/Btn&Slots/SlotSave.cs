using System;
using TMPro;
using UnityEngine;

public class SlotSave : MonoBehaviour
{
    [SerializeField] private GameObject fields;
    [SerializeField] private GameObject emptyTxt;

    [SerializeField] private TextMeshProUGUI playTime;
    [SerializeField] private TextMeshProUGUI leftDays;
    [SerializeField] private TextMeshProUGUI guildLv;
    [SerializeField] private TextMeshProUGUI gold;
    [SerializeField] private TextMeshProUGUI heros;

    public void SetSlotData(SaveData saveData)
    {
        if (saveData == null)
        {
            fields.SetActive(false);
            emptyTxt.SetActive(true);
        }
        else
        {
            fields.SetActive(true);
            emptyTxt.SetActive(false);

            DateTime time = new(saveData.lastSaveTime);
            string formatted = time.ToString("yyyy-MM-dd HH:mm:ss");
            playTime.text = formatted;
            leftDays.text = saveData.day.ToString();
            guildLv.text = Utils.ChangeToRomanNumeral(saveData.rank);
            gold.text = saveData.gold.ToString() + "P";
            heros.text = saveData.ownedHeroes.Count.ToString();
        }
    }
}