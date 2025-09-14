using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIOverrideSetting : UIBase
{
    [SerializeField] private Slider bgmSldr;
    [SerializeField] private Slider sfxSldr;
    [SerializeField] private TMP_Dropdown resolutionDropdown;

    private const string PrefVideoPreset = "VideoPreset";

    private struct VideoPreset
    {
        public string label;
        public int w, h;
        public FullScreenMode mode;
        public bool lockWindow;
        public VideoPreset(string label, int w, int h, FullScreenMode mode, bool lockWindow)
        { this.label = label; this.w = w; this.h = h; this.mode = mode; this.lockWindow = lockWindow; }
    }

    private static readonly VideoPreset[] presets =
    {
        new("2560×1440 전체화면", 2560, 1440, FullScreenMode.ExclusiveFullScreen, false),
        new("1920×1080 전체화면", 1920, 1080, FullScreenMode.ExclusiveFullScreen, false),
        new("1920×1080 창모드", 1920, 1080, FullScreenMode.Windowed, true),
        new("1600×900 창모드", 1600,  900, FullScreenMode.Windowed, true),
        new("1280×720 창모드", 1280,   720, FullScreenMode.Windowed, true),
    };

    private Coroutine lockRoutine;

    public override void Opened(object[] param)
    {
        bgmSldr.onValueChanged.AddListener(OnBGMChanged);
        sfxSldr.onValueChanged.AddListener(OnSFXChanged);

        float bgmVol = PlayerPrefs.GetFloat("BGMVolume", 0.6f);
        float sfxVol = PlayerPrefs.GetFloat("SFXVolume", 0.6f);
        bgmSldr.value = bgmVol;
        sfxSldr.value = sfxVol;

        InitResolutionDropdown();
    }

    public override void Closed(object[] param)
    {
        bgmSldr.onValueChanged.RemoveListener(OnBGMChanged);
        sfxSldr.onValueChanged.RemoveListener(OnSFXChanged);
        if (resolutionDropdown != null)
            resolutionDropdown.onValueChanged.RemoveListener(OnResolutionChanged);

        if (lockRoutine != null)
        {
            StopCoroutine(lockRoutine);
            lockRoutine = null;
        }
    }

    private void OnBGMChanged(float value) => AudioManager.Instance.SetVolume(value, true);
    private void OnSFXChanged(float value) => AudioManager.Instance.SetVolume(value, false);

    private void InitResolutionDropdown()
    {
        if (resolutionDropdown == null) return;

        var labels = new List<string>(presets.Length);
        for (int i = 0; i < presets.Length; i++) labels.Add(presets[i].label);

        resolutionDropdown.ClearOptions();
        resolutionDropdown.AddOptions(labels);

        int saved = Mathf.Clamp(PlayerPrefs.GetInt(PrefVideoPreset, 2), 0, presets.Length - 1);
        resolutionDropdown.SetValueWithoutNotify(saved);
        ApplyPreset(saved);

        resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);
    }

    private void OnResolutionChanged(int index)
    {
        ApplyPreset(index);
        PlayerPrefs.SetInt(PrefVideoPreset, index);
        PlayerPrefs.Save();
    }

    private void ApplyPreset(int index)
    {
        if (index < 0 || index >= presets.Length) return;
        var p = presets[index];

        if (lockRoutine != null)
        {
            StopCoroutine(lockRoutine);
            lockRoutine = null;
        }

        Screen.fullScreenMode = p.mode;
        Screen.SetResolution(p.w, p.h, p.mode);

        if (p.mode == FullScreenMode.Windowed && p.lockWindow)
            lockRoutine = StartCoroutine(LockWindowSize(p.w, p.h));
    }

    private IEnumerator LockWindowSize(int targetW, int targetH)
    {
        var wait = new WaitForSecondsRealtime(0.1f);
        while (true)
        {
            if (Screen.fullScreenMode != FullScreenMode.Windowed) yield break;
            if (Screen.width != targetW || Screen.height != targetH)
                Screen.SetResolution(targetW, targetH, FullScreenMode.Windowed);
            yield return wait;
        }
    }

    public void OnBackBtnClicked()
    {
        SetActive<UIOverrideSetting>(false);
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