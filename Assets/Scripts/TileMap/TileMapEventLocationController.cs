using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileMapEventLocationController
{
    private readonly Dictionary<GuildLocationEventType, List<EventLocation>> _locations = new();
    private readonly List<EventLocation> _usingLocation = new();

    public EventLocation Entrance { get; private set; }

    public void Initialize(List<EventLocation> locations)
    {
        _locations.Clear();
        _usingLocation.Clear();

        foreach (EventLocation location in locations)
        {
            if (!_locations.TryGetValue(location.EventType, out List<EventLocation> list))
            {
                list = new List<EventLocation>();
                _locations[location.EventType] = list;
            }
            list.Add(location);
        }
        Entrance = locations.Find(location => location.EventType.Equals(GuildLocationEventType.Entrance));
    }

    public void Clear()
    {
        _usingLocation.Clear();
    }

    public EventLocation GetEmptyEventLocationByType(GuildLocationEventType type)
    {
        var possibleLocations = new List<EventLocation>();

        foreach (var kvp in _locations)
        {
            if ((kvp.Key & type) != 0 && kvp.Value.Count > 0)
            {
                possibleLocations.AddRange(kvp.Value);
            }
        }

        var availableLocations = possibleLocations.Where(loc => !_usingLocation.Contains(loc)).ToList();

        if (availableLocations.Count > 0)
        {
            EventLocation chosen;
            if (type == GuildLocationEventType.Waiting)
            {
                availableLocations.Sort((a, b) =>
                {
                    if (a is WaitingLocation wa && b is WaitingLocation wb)
                    {
                        return wa.Index.CompareTo(wb.Index);
                    }
                    return 0;
                });
                chosen = availableLocations[0];
            }
            else
            {
                int index = Random.Range(0, availableLocations.Count);
                chosen = availableLocations[index];
            }

            _usingLocation.Add(chosen);
            return chosen;
        }

        return null;
    }

    public void ReturnLocation(EventLocation location)
    {
        if (location != null)
        {
            _usingLocation.Remove(location);
        }
    }
}
