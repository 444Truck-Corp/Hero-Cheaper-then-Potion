using System.Collections.Generic;

public class CharacterShop : TileMapCharacterCore
{
    public new GuildLocationEventType TargetType = GuildLocationEventType.Shop;

    protected override void OnClickOrder()
    {
        UIManager.Show<UIPopupShop>(new List<int>() { 140041, 140042, 140043, 140044, 140045, 140046, 140047, 140048, 140049, 140050 });
        TileMapManager.Instance.OnDinerCharacterExited(this);
        base.OnClickOrder();
    }
}