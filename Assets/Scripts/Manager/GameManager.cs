// TODO : 수정 필요
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum EndingType
{
    Bankrupt,
    Lose,
    Win,
}

public class GameManager : Singleton<GameManager>
{
    public EndingType Ending { get; set; }

    private void Start()
    {
        InitManagers();
    }

    public async void StartGame()
    {
        UIManager.Instance.ToBlack();
        await Task.Delay(1000);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("MainScene");
        while (!asyncLoad.isDone) await Task.Yield();

        await Task.Delay(1000);
        UIManager.Instance.ToTransparent();

        // TODO : REMOVE
        SaveManager.Instance.MySaveData.AcquireItem(120001);
        SaveManager.Instance.MySaveData.AcquireItem(120002);
        SaveManager.Instance.MySaveData.AcquireItem(120003);
        SaveManager.Instance.MySaveData.AcquireItem(120004);
        SaveManager.Instance.MySaveData.AcquireItem(120005);
        SaveManager.Instance.MySaveData.AcquireItem(140001);
        SaveManager.Instance.MySaveData.AcquireItem(140001);
        SaveManager.Instance.MySaveData.AcquireItem(140002);
    }

    private void InitManagers()
    {
        ResourceManager.Instance.Init();
        DataManager.Instance.Init();
        SaveManager.Instance.Init();
    }
}