using System;
using System.Collections.Generic;

[Serializable]
public class RecipeIngredient
{
    public int id;
    public int amount;
}

[Serializable]
public class RecipeData
{
    public int id;
    public int itemId;
    public string name;
    public string rawIngredients;
    public bool isActive;

    [NonSerialized] public List<RecipeIngredient> ingredientList;

    public void Parse()
    {
        if (string.IsNullOrWhiteSpace(rawIngredients)) return;
        ingredientList = new List<RecipeIngredient>();
        string[] ingredients = rawIngredients.Split(',');
        foreach (string ingredient in ingredients)
        {
            string[] parts = ingredient.Trim().Split(':');
            if (parts.Length != 2)
            {
                continue;
            }
            if (int.TryParse(parts[0], out int itemId) &&
                int.TryParse(parts[1], out int amount))
            {
                ingredientList.Add(new RecipeIngredient { id = itemId, amount = amount });
            }
        }
    }
}