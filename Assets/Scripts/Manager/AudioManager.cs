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
    [SerializeField] protected AudioClip[] sfxClips;

    private Slider volumeSlider;

    private int lastPlayedIndex = -1;

    private void Start()
    {
        SetVolume();

        if (bgmClips.Length > 0)
        {
            PlayBGM(0);
        }
        StartCoroutine(WaitForMainSceneAndPlayBGM());
    }

    private IEnumerator WaitForMainSceneAndPlayBGM()
    {
        WaitForSeconds wait = new(0.5f);

        yield return new WaitUntil(() => SceneManager.GetActiveScene().name == "MainScene");
        PlayRandomBGM();

        while (true)
        {
            if (!bgmSource.isPlaying)
            {
                PlayRandomBGM();
            }
            yield return wait;
        }
    }

    public void SetVolume()
    {
        float volume = (volumeSlider == null) ? 1f : volumeSlider.value;
        volume = Mathf.Clamp(volume, 0.0001f, 1f);
        myMixer.SetFloat("Master", Mathf.Log10(volume) * 20);
    }

    public void RegisterSlider(Slider slider)
    {
        volumeSlider = slider;
        volumeSlider.onValueChanged.AddListener(delegate { SetVolume(); });
        volumeSlider.value = 1.0f;
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
        if (bgmClips.Length == 0) return;

        int randomIndex;

        do
        {
            randomIndex = UnityEngine.Random.Range(1, 5);
        } while (randomIndex == lastPlayedIndex);

        bgmSource.clip = bgmClips[randomIndex];
        bgmSource.Play();

        lastPlayedIndex = randomIndex;
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