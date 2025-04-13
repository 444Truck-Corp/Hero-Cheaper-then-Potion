//TODO : 수정 필요할듯
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum EEnding
{
    Bankrupt,
    Lose,
    Win
}

public class GameManager : Singleton<GameManager>
{
    #region Unity Life Cycles
    protected override void Awake()
    {
        base.Awake();
        InitManagers();
    }
    #endregion

    #region Main Methods
    public async void StartGame()
    {
        UIManager.Instance.ToBlack();
        await Task.Delay(1000);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("MainScene");
        while (!asyncLoad.isDone) await Task.Yield();

        await Task.Delay(1000);
        UIManager.Instance.ToTransparent();
    }

    #endregion

    #region Sub Methods
    private void InitManagers()
    {
        ResourceManager.Instance.Init();
        DataManager.Instance.Init();
        SaveManager.Instance.Init();
        SpawnManager.Instance.Init();
    }
    #endregion
}