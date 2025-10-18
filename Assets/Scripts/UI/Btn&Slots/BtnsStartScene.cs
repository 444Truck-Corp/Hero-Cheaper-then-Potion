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
        UIManager.Show<UIPopupLoad>();
    }

    public void OnQuitBtnClicked()
    {
        Application.Quit();
    }
}