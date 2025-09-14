using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AudioManager : Singleton<AudioManager>
{
    [SerializeField] private AudioMixer myMixer;
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;

    [SerializeField] private AudioClip[] bgmClips;
    [SerializeField] private AudioClip[] sfxClips;

    private int lastPlayedIndex = -1;

    private readonly string bgmVolumeKey = "BGMVolume";
    private readonly string sfxVolumeKey = "SFXVolume";

    private const string MainSceneName = "MainScene";

    private void Start()
    {
        float bgmVol = PlayerPrefs.GetFloat(bgmVolumeKey, 0.6f);
        float sfxVol = PlayerPrefs.GetFloat(sfxVolumeKey, 0.6f);
        SetVolume(bgmVol, true);
        SetVolume(sfxVol, false);

        if (SceneManager.GetActiveScene().name == MainSceneName)
        {
            PlayRandomBGM();
            StartCoroutine(LoopRandomBGM());
        }
        else if (bgmClips.Length > 0)
        {
            PlayBGM(0);
        }
    }

    private IEnumerator LoopRandomBGM()
    {
        WaitForSeconds wait = new(0.5f);
        while (true)
        {
            if (!bgmSource.isPlaying)
                PlayRandomBGM();
            yield return wait;
        }
    }

    public void SetVolume(float volume, bool isBGM)
    {
        if (isBGM)
        {
            PlayerPrefs.SetFloat(bgmVolumeKey, volume);
            volume = Mathf.Clamp(volume, 0.0001f, 1f);
            myMixer.SetFloat("BGM", Mathf.Log10(volume) * 20);
        }
        else
        {
            PlayerPrefs.SetFloat(sfxVolumeKey, volume);
            volume = Mathf.Clamp(volume, 0.0001f, 1f);
            myMixer.SetFloat("SFX", Mathf.Log10(volume) * 20);
        }
    }

    public void PlayBGM(int clipIndex)
    {
        if (clipIndex < 0 || clipIndex >= bgmClips.Length)
        {
            Debug.LogWarning("배경음악 인덱스가 범위를 벗어났습니다!");
            return;
        }

        bgmSource.clip = bgmClips[clipIndex];
        bgmSource.loop = true;
        bgmSource.Play();
    }

    private void PlayRandomBGM()
    {
        int len = bgmClips?.Length ?? 0;
        if (len == 0) return;

        int next = 0;
        if (len == 1)
        {
            next = 0;
        }
        else
        {
            do { next = Random.Range(0, len); }
            while (next == lastPlayedIndex);
        }

        bgmSource.loop = false;
        bgmSource.clip = bgmClips[next];
        bgmSource.Play();

        lastPlayedIndex = next;
    }

    public void PlaySFX(int clipIndex)
    {
        if (clipIndex < 0 || clipIndex >= sfxClips.Length)
        {
            Debug.LogWarning("효과음 인덱스가 범위를 벗어났습니다!");
            return;
        }

        sfxSource.PlayOneShot(sfxClips[clipIndex]);
    }
}