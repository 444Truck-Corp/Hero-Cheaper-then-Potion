using UnityEngine;

public class BtnsStartScene : MonoBehaviour
{
    public void OnStartBtnClicked()
    {
        SaveManager.Instance.CreateSaveData();
        GameManager.Instance.StartGame();
    }

    public void OnLoadBtnClicked()
    {
        UIManager.Show<UIPopupLoad>(false);
    }

    public void OnQuitBtnClicked()
    {
        Application.Quit();
    }
}