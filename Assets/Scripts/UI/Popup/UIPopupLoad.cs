using Newtonsoft.Json;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupLoad : UIBase
{
    [SerializeField] private SlotSave[] saveSlots;
    private Button[] saveBtns = new Button[4];

    public override void Opened(object[] param)
    {
        bool isSaveSlot = param .Length > 0 && param[0] is bool b && b;

        for (int i = 0; i < saveSlots.Length; i++)
        {
            saveSlots[i].SetSlotData(null);
            saveBtns[i] = saveSlots[i].GetComponent<Button>();
            saveBtns[i].interactable = isSaveSlot;

            saveBtns[i].onClick.RemoveAllListeners();
            int idx = i;
            if (isSaveSlot)
                saveBtns[i].onClick.AddListener(() => OnSaveClicked(idx));
            else
                saveBtns[i].onClick.AddListener(() => OnLoadClicked(idx));
        }

        LoadSlotData();
    }

    #region Main Methods
    public void OnBackBtnClicked()
    {
        UIManager.Hide<UIPopupLoad>();
    }

    private void OnSaveClicked(int slotNum)
    {
        switch (slotNum)
        {
            case 0:
                SaveManager.Instance.SaveSlot(ESaveSlot.Auto);
                break;
            case 1:
                SaveManager.Instance.SaveSlot(ESaveSlot.Slot1);
                break;
            case 2:
                SaveManager.Instance.SaveSlot(ESaveSlot.Slot2);
                break;
            case 3:
                SaveManager.Instance.SaveSlot(ESaveSlot.Slot3);
                break;
            default:
                Debug.LogError($"잘못된 슬롯 번호: {slotNum}");
                break;
        }
        LoadSlotData();
    }

    private void OnLoadClicked(int slotNum)
    {
        switch (slotNum)
        {
            case 0:
                SaveManager.Instance.LoadSlot(ESaveSlot.Auto);
                break;
            case 1:
                SaveManager.Instance.LoadSlot(ESaveSlot.Slot1);
                break;
            case 2:
                SaveManager.Instance.LoadSlot(ESaveSlot.Slot2);
                break;
            case 3:
                SaveManager.Instance.LoadSlot(ESaveSlot.Slot3);
                break;
            default:
                Debug.LogError($"잘못된 슬롯 번호: {slotNum}");
                break;
        }

        GameManager.Instance.StartGame();
    }
    #endregion

    #region Sub Methods
    private void LoadSlotData()
    {
        string dir = Path.Combine(Application.persistentDataPath, "Save");
        string[] files = Directory.GetFiles(dir, "*.json");

        for (int i = 0; i < files.Length; i++)
        {
            string filePath = files[i];
            SaveData data = null;
            try
            {
                string json = File.ReadAllText(filePath);
                data = JsonConvert.DeserializeObject<SaveData>(json);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"{filePath} 파싱 실패: {e.Message}");
            }

            string fileName = Path.GetFileNameWithoutExtension(filePath);
            switch (fileName)
            {
                case nameof(ESaveSlot.Auto):
                    saveSlots[0].SetSlotData(data);
                    saveBtns[0].interactable = true;
                    break;
                case nameof(ESaveSlot.Slot1):
                    saveSlots[1].SetSlotData(data);
                    saveBtns[1].interactable = true;
                    break;
                case nameof(ESaveSlot.Slot2):
                    saveSlots[2].SetSlotData(data);
                    saveBtns[2].interactable = true;
                    break;
                case nameof(ESaveSlot.Slot3):
                    saveSlots[3].SetSlotData(data);
                    saveBtns[3].interactable = true;
                    break;
                default:
                    Debug.LogError($"[UIPopupLoad] 알 수 없는 파일 이름: {fileName}");
                    break;
            }
        }
    }
    #endregion
}