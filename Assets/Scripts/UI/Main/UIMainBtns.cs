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
    [SerializeField] private Button btnHero;
    [SerializeField] private Button btnFood;
    [SerializeField] private Button btnQuest;
    [SerializeField] private Button btnInven;

    private NavUI _openedUI;
    private Dictionary<NavUI, Button> _navButtons;
    private Dictionary<NavUI, Action> _showActions;
    private Dictionary<NavUI, Action> _hideActions;
    private Dictionary<NavUI, Func<bool>> _isActiveChecks;

    private readonly WaitForSeconds _transitionDelay = new(0.2f);
    private readonly Color normalColor = Color.white;
    private readonly Color activeColor = new(0.7f, 0.7f, 0.7f);

    private void Awake()
    {
        _navButtons = new()
        {
            { NavUI.Hero, btnHero },
            { NavUI.Food, btnFood },
            { NavUI.Quest, btnQuest },
            { NavUI.Inven, btnInven }
        };

        _showActions = new()
        {
            { NavUI.Hero, () => UIManager.Show<UINavHero>() },
            { NavUI.Food, () => UIManager.Show<UINavFood>() },
            { NavUI.Quest, () => UIManager.Show<UINavQuest>() },
            { NavUI.Inven, () => UIManager.Show<UINavInven>() }
        };

        _hideActions = new()
        {
            { NavUI.Hero, () => UIManager.Hide<UINavHero>() },
            { NavUI.Food, () => UIManager.Hide<UINavFood>() },
            { NavUI.Quest, () => UIManager.Hide<UINavQuest>() },
            { NavUI.Inven, () => UIManager.Hide<UINavInven>() }
        };

        _isActiveChecks = new()
        {
            { NavUI.Hero, () => UIManager.IsActive<UINavHero>() },
            { NavUI.Food, () => UIManager.IsActive<UINavFood>() },
            { NavUI.Quest, () => UIManager.IsActive<UINavQuest>() },
            { NavUI.Inven, () => UIManager.IsActive<UINavInven>() }
        };

        _openedUI = NavUI.None;
        UpdateButtonColor(_openedUI);

        btnHero.onClick.AddListener(() => OpenNavUI(NavUI.Hero));
        btnFood.onClick.AddListener(() => OpenNavUI(NavUI.Food));
        btnQuest.onClick.AddListener(() => OpenNavUI(NavUI.Quest));
        btnInven.onClick.AddListener(() => OpenNavUI(NavUI.Inven));
    }

    private void Update()
    {
        if (_openedUI != NavUI.None && !_isActiveChecks[_openedUI]())
        {
            _openedUI = NavUI.None;
            UpdateButtonColor(NavUI.None);
        }
    }

    private void OpenNavUI(NavUI target)
    {
        if (_openedUI == target) return;

        SetInteractable(false);

        if (_openedUI != NavUI.None) _hideActions[_openedUI]?.Invoke();
        _showActions[target]?.Invoke();

        _openedUI = target;
        UpdateButtonColor(target);

        SetInteractable(true);
    }

    private void SetInteractable(bool enabled)
    {
        foreach (var button in _navButtons.Values)
        {
            button.interactable = enabled;
        }
    }

    private void UpdateButtonColor(NavUI active)
    {
        foreach (var (nav, button) in _navButtons)
        {
            var colors = button.colors;
            colors.normalColor = nav == active ? activeColor : normalColor;
            button.colors = colors;
        }
    }
}