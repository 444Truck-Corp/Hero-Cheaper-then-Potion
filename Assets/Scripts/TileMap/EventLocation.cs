using UnityEngine;

public enum GuildLocationEventType
{
    None,
    Entrance,
    QuestBoard,
    Chair,
    Food,
    Shop
}

public class EventLocation : MonoBehaviour
{
    public Direction Direction;
    public GuildLocationEventType EventType;
}