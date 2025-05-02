using System.Collections.Generic;
using UnityEngine;

public class TileMapEventLocationController
{
    private readonly Dictionary<GuildLocationEventType, List<EventLocation>> _locations = new();
    private readonly List<EventLocation> _usingLocation = new();

    public Vector2Int EntranceTilePosition { get; private set; }
    public Vector3 EntrancePosition { get; private set; }

    public void Initialize(List<EventLocation> locations)
    {
        foreach (EventLocation location in locations)
        {
            if (!_locations.TryGetValue(location.EventType, out List<EventLocation> list))
            {
                list = new List<EventLocation>();
            }
            list.Add(location);
        }
        EventLocation entrance = locations.Find(location => location.EventType.Equals(GuildLocationEventType.Entrance));
        EntrancePosition = entrance.transform.localPosition;
        EntranceTilePosition = new Vector2Int((int)EntrancePosition.x, -(int)EntrancePosition.y);
    }

    public void Clear()
    {
    }

    public EventLocation GetEmptyEventLocationByType(GuildLocationEventType type)
    {
        if (_locations.TryGetValue(type, out var eventLocations))
        {
            int count = eventLocations.Count;
            if (count > 0)
            {
                int random = Random.Range(0, count);
                EventLocation location = eventLocations[random];
                eventLocations.RemoveAt(random);
                _usingLocation.Add(location);
                return location;
            }
        }
        return null;
    }

    public void ReturnLocation(EventLocation location)
    {
        _usingLocation.Remove(location);
        _locations[location.EventType].Add(location);
    }
}