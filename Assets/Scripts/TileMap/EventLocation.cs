using UnityEngine;

[System.Flags]
public enum GuildLocationEventType
{
    None = 0,
    Entrance = 1,
    QuestBoard = 2,
    Chair = 4,
    Food = 8,
    Shop = 16,
    Waiting = 32
}

public class EventLocation : MonoBehaviour
{
    public Direction Direction;
    public GuildLocationEventType EventType;
}