//TODO : 수정 필요할듯
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