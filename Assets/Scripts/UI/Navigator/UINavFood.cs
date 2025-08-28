using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UINavFood : UIBase
{
    [SerializeField] private Transform _listParent;
    [SerializeField] private GameObject _recipeSlotPrefab;
    [SerializeField] private Button _cookButton;

    private readonly Dictionary<int, SlotRecipe> _recipeSlots = new();
    private int _selectedRecipeId = -1;

    protected override void Awake()
    {
        base.Awake();
        EventManager.Instance.AddSaveDataListener(nameof(SaveData.ownedRecipeId), FetchRecipeList);
    }

    public override void Opened(object[] param)
    {
        base.Opened(param);
        FetchRecipeList();
    }

    private void OnDestroy()
    {
        EventManager.Instance.RemoveSaveDataListener(nameof(SaveData.ownedRecipeId), FetchRecipeList);
    }

    private void FetchRecipeList()
    {
        foreach ((int id, var child) in _recipeSlots)
        {
            Destroy(child.gameObject);
        }
        _recipeSlots.Clear();

        List<int> ownedRecipeIds = SaveManager.Instance.MySaveData.ownedRecipeId;

        foreach ((int recipeId, RecipeData recipeData) in RecipeManager.Instance.Recipes)
        {
            if (recipeData != null)
            {
                GameObject slotObj = Instantiate(_recipeSlotPrefab, _listParent);
                if (slotObj.TryGetComponent<SlotRecipe>(out var slot))
                {
                    _recipeSlots.Add(recipeId, slot);
                    slot.Init(recipeData);

                    if (slotObj.TryGetComponent<Button>(out var btn))
                    {
                        btn.onClick.AddListener(() => { OnRecipeSelected(recipeId); slot.SetSelected(true); });
                    }
                }
                else
                {
                    Debug.LogError($"SlotRecipe 컴포넌트가 {slotObj.name}에 없습니다.");
                }
            }
        }
    }

    private void OnRecipeSelected(int recipeId)
    {
        List<int> ownedRecipeIds = SaveManager.Instance.MySaveData.ownedRecipeId;
        if (ownedRecipeIds.Contains(recipeId)) return;
        if (_selectedRecipeId > 0)
        {
            _recipeSlots[_selectedRecipeId].SetSelected(false);
        }

        _selectedRecipeId = recipeId;
        _cookButton.interactable = true;
    }

    public void OnClickedCookButton()
    {
        if (_selectedRecipeId < 0) return;
        if (RecipeManager.Instance.TryCook(_selectedRecipeId))
        {
            _recipeSlots[_selectedRecipeId].SetComplete(true);
            SaveManager.Instance.MySaveData.ownedRecipeId.Add(_selectedRecipeId);
            _selectedRecipeId = -1;
            _cookButton.interactable = false;
        }
        else
        {
            // TODO: 만들 수 없다는 메시지 띄우기
        }
    }
}