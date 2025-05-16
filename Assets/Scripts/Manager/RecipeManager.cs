using System.Collections.Generic;
using UnityEngine;

public class RecipeManager : Singleton<RecipeManager>
{
    private readonly Dictionary<int, RecipeData> _recipes = new();

    protected override void Awake()
    {
        base.Awake();

        List<RecipeData> recipeList = DataManager.Instance.GetObjList<RecipeData>(nameof(RecipeData));
        foreach (RecipeData recipe in recipeList)
        {
            recipe.Parse();
            _recipes[recipe.id] = recipe;
        }
    }

    private void DebugRecipe(RecipeData recipe)
    {
        string debug = $"레시피 {recipe.id}: {recipe.name}\n";

        if (recipe.ingredientList == null)
        {
            debug += "아이템 불필요";
        }
        else
        {
            foreach (var ingredient in recipe.ingredientList)
            {
                debug += $"아이템 \"{ingredient.id}\" x{ingredient.amount}\n";
            }
        }

        Debug.Log(debug);
    }

    public RecipeData GetRandomOwnedRecipe()
    {
        //int rank = SaveManager.Instance.MySaveData.rank;
        int count = SaveManager.Instance.MySaveData.ownedRecipes.Count;

        // 보유 레시피가 없을 때
        if (count == 0)
        {
            return null;
        }
        int index = Random.Range(0, count);
        int recipeId = SaveManager.Instance.MySaveData.ownedRecipes[index];
        return _recipes[recipeId];
    }

    public bool TryCook(int recipeId)
    {
        if (!_recipes.TryGetValue(recipeId, out RecipeData recipe)) return false;
        // 개수 확인
        foreach (RecipeIngredient ingredient in recipe.ingredientList)
        {
            if (!SaveManager.Instance.MySaveData.items.TryGetValue(ingredient.id, out int itemAmount))
            {
                return false;
            }
            if (itemAmount < ingredient.amount) return false;
        }

        // 개수 만큼 감소
        foreach (RecipeIngredient ingredient in recipe.ingredientList)
        {
            SaveManager.Instance.MySaveData.items[ingredient.id] -= ingredient.amount;
        }
        return true;
    }
}