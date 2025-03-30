using UnityEngine;
using UnityEngine.UI;

public enum NavUI
{
    None = 0,
    Hero,
    Food,
    Quest,
    Inven
}

public class UIMainBtns : MonoBehaviour
{
    #region Component
    [SerializeField] private Button btnHero;
    [SerializeField] private Button btnFood;
    [SerializeField] private Button btnQuest;
    [SerializeField] private Button btnInven;
    #endregion

    private NavUI currentOpenUI = NavUI.None;

    #region Main Method
    public void OnClickHeroBtn() => OpenNavUI(NavUI.Hero);
    public void OnClickFoodBtn() => OpenNavUI(NavUI.Food);
    public void OnClickQuestBtn() => OpenNavUI(NavUI.Quest);
    public void OnClickInvenBtn() => OpenNavUI(NavUI.Inven);
    #endregion

    #region Sub Method
    private void OpenNavUI(NavUI type)
    {
        if (currentOpenUI == type) return;

        // 이전 UI 끄기
        switch (currentOpenUI)
        {
            case NavUI.Hero: UIManager.Hide<UINavHero>(); break;
            case NavUI.Food: UIManager.Hide<UINavFood>(); break;
            case NavUI.Quest: UIManager.Hide<UINavQuest>(); break;
            case NavUI.Inven: UIManager.Hide<UINavInven>(); break;
        }

        // 새로운 UI 켜기
        switch (type)
        {
            case NavUI.Hero: UIManager.Show<UINavHero>(); break;
            case NavUI.Food: UIManager.Show<UINavFood>(); break;
            case NavUI.Quest: UIManager.Show<UINavQuest>(); break;
            case NavUI.Inven: UIManager.Show<UINavInven>(); break;
        }

        currentOpenUI = type;
    }
    #endregion
}
