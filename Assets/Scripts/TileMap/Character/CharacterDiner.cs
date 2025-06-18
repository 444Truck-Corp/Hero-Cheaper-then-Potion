public class CharacterDiner : TileMapCharacterCore
{
    public new GuildLocationEventType TargetType =
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
        string iconPath = ItemManager.Instance.ItemList[_recipeId].icon;
        _emotion.SetIcon(iconPath);
    }

    protected override void OnClickOrder()
    {
        // TODO: 주문 완료 저장 필요
        // 재화 획득하고 다시 불러와서 악용 가능

        // 골드 증가
        if (SaveManager.Instance.MySaveData.foodProfits.TryGetValue(_recipeId, out int count))
        {
            SaveManager.Instance.MySaveData.foodProfits[_recipeId] = count + 1;
        }
        else
        {
            SaveManager.Instance.MySaveData.foodProfits.Add(_recipeId, 1);
        }

        // 경험치 증가
        int exp = SaveManager.Instance.MySaveData.exp += 1;
        SaveManager.Instance.SetSaveData(nameof(SaveManager.Instance.MySaveData.exp), exp);

        TileMapManager.Instance.OnDinerCharacterExited(this);
        base.OnClickOrder();
    }
}