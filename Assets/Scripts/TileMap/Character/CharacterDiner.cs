using UnityEngine;

public class CharacterDiner : TileMapCharacterCore
{
    public override TileMapCharacterType CharacterType => TileMapCharacterType.Diner;
    public override GuildLocationEventType TargetType => 
        GuildLocationEventType.Food | 
        GuildLocationEventType.Chair;

    private int _recipeId;

    public override void Initialize(string textureName)
    {
        base.Initialize(textureName);

        SetRecipe();
    }

    public override void SetOrder()
    {
        base.SetOrder();
    }

    private void SetRecipe()
    {
        _recipeId = RecipeManager.Instance.GetRandomOwnedRecipeId();
        var recipe = RecipeManager.Instance.Recipes[_recipeId];
        var item = ItemManager.Instance.ItemList[recipe.itemId];
        var sprite = ResourceManager.Instance.LoadAsset<Sprite>(ResourceManager.textureDir, item.icon);
        _emotion.SetIcon(sprite);
    }

    protected override void OnClickOrder()
    {
        // TODO: 주문 완료 저장 필요
        // 재화 획득하고 다시 불러와서 악용 가능
        int itemId = RecipeManager.Instance.Recipes[_recipeId].itemId;

        // 골드 증가
        if (SaveManager.Instance.MySaveData.foodProfits.TryGetValue(itemId, out int count))
        {
            SaveManager.Instance.MySaveData.foodProfits[itemId] = count + 1;
        }
        else
        {
            SaveManager.Instance.MySaveData.foodProfits.Add(itemId, 1);
        }

        // 경험치 증가
        int exp = SaveManager.Instance.MySaveData.exp += 1;
        SaveManager.Instance.SetSaveData(nameof(SaveManager.Instance.MySaveData.exp), exp);

                TileMapManager.Instance.OnCharacterExit(this);
        base.OnClickOrder();
    }
}