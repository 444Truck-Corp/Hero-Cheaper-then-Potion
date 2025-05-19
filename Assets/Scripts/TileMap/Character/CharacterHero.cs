public class CharacterHero : TileMapCharacterCore
{
    public new TileMapCharacterType CharacterType = TileMapCharacterType.Hero;
    public new GuildLocationEventType TargetType =
        GuildLocationEventType.QuestBoard |
        GuildLocationEventType.Food |
        GuildLocationEventType.Chair;

    public override void Initialize(string textureName)
    {
        _movement.Initialize(textureName, TargetType, true);
    }
}