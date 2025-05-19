public class CharacterShop : TileMapCharacterCore
{
    public new GuildLocationEventType TargetType = GuildLocationEventType.Shop;

    protected override void OnClickOrder()
    {
        // 상점 열기
        TileMapManager.Instance.OnDinerCharacterExited(this);
        base.OnClickOrder();
    }
}