using UnityEngine;

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
        // 골드 증가
        // TODO: 주문 완료 저장 필요
        // 재화 획득하고 다시 불러와서 악용 가능
        SaveManager.Instance.MySaveData.gold += ItemManager.Instance.ItemList[_recipeId].value;
        SaveManager.Instance.SetSaveData(nameof(SaveManager.Instance.MySaveData.gold), SaveManager.Instance.MySaveData.gold);
        TileMapManager.Instance.OnDinerCharacterExited(this);
        base.OnClickOrder();
    }
}