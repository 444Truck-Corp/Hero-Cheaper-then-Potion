using System.Collections.Generic;

public class CharacterShop : TileMapCharacterCore
{
    public new GuildLocationEventType TargetType = GuildLocationEventType.Shop;

    protected override void OnClickOrder()
    {
        UIManager.Show<UIPopupShop>(new List<int>() { 41, 42, 43, 44, 45, 46, 47, 48, 49, 50 });
        TileMapManager.Instance.OnDinerCharacterExited(this);
        base.OnClickOrder();
    }
}