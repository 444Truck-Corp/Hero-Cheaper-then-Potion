using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartSceneBtns : MonoBehaviour
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