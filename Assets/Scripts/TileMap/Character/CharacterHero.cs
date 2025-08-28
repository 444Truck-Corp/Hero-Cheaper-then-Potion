public class CharacterHero : TileMapCharacterCore
{
    public override TileMapCharacterType CharacterType => TileMapCharacterType.Hero;
    public override GuildLocationEventType TargetType =>
        GuildLocationEventType.QuestBoard |
        GuildLocationEventType.Food |
        GuildLocationEventType.Chair;

    public override void Initialize(string textureName)
    {
        _movement.Initialize(textureName, TargetType, true);
    }
}