using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlotRecipe : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private TextMeshProUGUI _recipeNameText;
    [SerializeField] private Transform _ingredientsContainer;
    [SerializeField] private Image _thumbnail;
    [SerializeField] private GameObject _ingredientSlotPrefab;
    [SerializeField] private GameObject _completeMark;
    [SerializeField] private Button _button;
    [SerializeField] private Outline _outline;

    private bool _complete;

    public void Init(RecipeData recipeData)
    {
        if (_recipeNameText != null)
        {
            _recipeNameText.text = recipeData.name;
            var item = ItemManager.Instance.ItemList[recipeData.itemId];
            _thumbnail.sprite = ResourceManager.Instance.LoadAsset<Sprite>(ResourceManager.textureDir, item.icon); 
        }

        foreach (Transform child in _ingredientsContainer)
        {
            Destroy(child.gameObject);
        }

        if (recipeData.ingredientList != null && _ingredientSlotPrefab != null)
        {
            foreach (var ingredient in recipeData.ingredientList)
            {
                if (ItemManager.Instance.ItemList.TryGetValue(ingredient.id, out ItemData itemData))
                {
                    GameObject slotObj = Instantiate(_ingredientSlotPrefab, _ingredientsContainer);
                    if (slotObj.TryGetComponent<UIIngredientSlot>(out var slot))
                    {
                        SaveManager.Instance.MySaveData.items.TryGetValue(ingredient.id, out int itemAmount);
                        slot.SetIngredient(itemData, ItemManager.Instance.ItemList[ingredient.id].name, itemAmount, ingredient.amount);
                    }
                }
            }
        }

        if (SaveManager.Instance.MySaveData.ownedRecipeId.Contains(recipeData.id))
        {
            SetComplete(true);
        }
    }

    public void SetSelected(bool value)
    {
        if (!_complete)
        {
            _button.interactable = !value;
        }

        _outline.enabled = value;
    }

    public void SetComplete(bool value)
    {
        _completeMark.SetActive(value);
        _complete = value;
        if (value) SetSelected(false);
    }
}