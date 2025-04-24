using System;
using System.Collections;
using System.Collections.Generic;
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

    private Dictionary<NavUI, Button> navButtons;
    private NavUI currentOpenUI;

    private readonly WaitForSeconds transitionDelay = new(0.2f);
    private readonly Color normalColor = Color.white;
    private readonly Color activeColor = new(0.7f, 0.7f, 0.7f);

    #region Unity Methods
    private void Awake()
    {
        navButtons = new Dictionary<NavUI, Button>
        {
            { NavUI.Hero, btnHero },
            { NavUI.Food, btnFood },
            { NavUI.Quest, btnQuest },
            { NavUI.Inven, btnInven }
        };

        currentOpenUI = NavUI.None;
        UpdateButtonColor(currentOpenUI);
    }

    private void Update()
    {
        if (currentOpenUI == NavUI.None) return;

        bool isActive = currentOpenUI switch
        {
            NavUI.Hero => UIManager.IsActive<UINavHero>(),
            NavUI.Food => UIManager.IsActive<UINavFood>(),
            NavUI.Quest => UIManager.IsActive<UINavQuest>(),
            NavUI.Inven => UIManager.IsActive<UINavInven>(),
            _ => false
        };

        if (!isActive)
        {
            currentOpenUI = NavUI.None;
            UpdateButtonColor(currentOpenUI);
        }
    }
    #endregion

    #region Main Methods
    public void OnClickHeroBtn() => StartCoroutine(OpenNavUI(NavUI.Hero));
    public void OnClickFoodBtn() => StartCoroutine(OpenNavUI(NavUI.Food));
    public void OnClickQuestBtn() => StartCoroutine(OpenNavUI(NavUI.Quest));
    public void OnClickInvenBtn() => StartCoroutine(OpenNavUI(NavUI.Inven));
    #endregion

    #region Sub Methods
    private IEnumerator OpenNavUI(NavUI type)
    {
        if (currentOpenUI == type) yield break;

        SetAllButtonsInteractable(false);

        try
        {
            switch (currentOpenUI)
            {
                case NavUI.Hero: UIManager.Hide<UINavHero>(); break;
                case NavUI.Food: UIManager.Hide<UINavFood>(); break;
                case NavUI.Quest: UIManager.Hide<UINavQuest>(); break;
                case NavUI.Inven: UIManager.Hide<UINavInven>(); break;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Hide 실패: {e.Message}");
        }

        try
        {
            switch (type)
            {
                case NavUI.Hero: UIManager.Show<UINavHero>(); break;
                case NavUI.Food: UIManager.Show<UINavFood>(); break;
                case NavUI.Quest: UIManager.Show<UINavQuest>(); break;
                case NavUI.Inven: UIManager.Show<UINavInven>(); break;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Show 실패: {e.Message}");
        }

        currentOpenUI = type;
        UpdateButtonColor(type);

        yield return transitionDelay;
        SetAllButtonsInteractable(true);
    }

    private void SetAllButtonsInteractable(bool interactable)
    {
        foreach (var btn in navButtons.Values)
            btn.interactable = interactable;
    }

    private void UpdateButtonColor(NavUI active)
    {
        foreach (var pair in navButtons)
        {
            var cb = pair.Value.colors;
            cb.normalColor = (active != NavUI.None && pair.Key == active) ? activeColor : normalColor;
            pair.Value.colors = cb;
        }
    }
    #endregion
}