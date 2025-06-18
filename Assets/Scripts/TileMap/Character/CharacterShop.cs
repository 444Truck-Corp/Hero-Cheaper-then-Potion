using System.Collections.Generic;

public class CharacterShop : TileMapCharacterCore
{
    public new GuildLocationEventType TargetType = GuildLocationEventType.Shop;

    protected override void OnClickOrder()
    {
        UIManager.Show<UIPopupShop>(new List<int>() { 1, 2, 3, 4, 5, 6, 7 });
        TileMapManager.Instance.OnDinerCharacterExited(this);
        base.OnClickOrder();
    }
}