using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIMainIcons : MonoBehaviour
{
    [SerializeField] private Outline pauseOutline;
    [SerializeField] private Outline speedOutline;

    private bool isPaused = false;
    private bool isFast;
    private int curSpeed = 1;

    public void OnPauseClicked()
    {
        if (isPaused)
        {
            isPaused = false;
            pauseOutline.enabled = false;
            Time.timeScale = curSpeed;
        }
        else
        {
            isPaused = true;
            pauseOutline.enabled = true;
            Time.timeScale = 0;
        }
    }

    public void OnSpeedClicked()
    {
        isFast = !isFast;
        speedOutline.enabled = !speedOutline.enabled;

        curSpeed = isFast ? 2 : 1;
        if (!isPaused) Time.timeScale = curSpeed;
    }

    public void OnSettingClicked()
    {
        UIManager.Show<UIOverrideSetting>();
    }
}