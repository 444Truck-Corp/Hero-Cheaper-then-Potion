using System;
using System.Collections.Generic;

[Serializable]
public class RecipeData
{
    public int id;
    public string name;
    public List<ItemData> ingredients;
    public bool isActive;
}