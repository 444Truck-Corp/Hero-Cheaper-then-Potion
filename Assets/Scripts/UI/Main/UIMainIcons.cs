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
        SaveManager.Instance.SaveSlot(0);
        SceneManager.LoadScene("StartScene");
    }
}