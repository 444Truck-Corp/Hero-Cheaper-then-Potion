using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIMainIcons : MonoBehaviour
{
    public void OnPauseClicked()
    {
        UIManager.Show<UIOverridePause>();
        Time.timeScale = 0;
    }

    public void OnSettingClicked()
    {
        UIManager.Show<UIOverrideSetting>();
    }

    public void OnHomeClicked()
    {
        UIManager.Show<UIOverrideModal>(
            "메인 메뉴로",
            "정말 메인 메뉴로 돌아가시겠습니까?",
            true,
            true,
            (Action)BackToMenu
        );

    }

    private void BackToMenu()
    {
        SaveManager.Instance.SaveSlot(0);
        SceneManager.LoadScene("StartScene");
    }
}