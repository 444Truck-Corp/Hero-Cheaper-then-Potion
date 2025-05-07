using System.Collections.Generic;
using UnityEngine;

public class TileMapEventLocationController
{
    private readonly Dictionary<GuildLocationEventType, List<EventLocation>> _locations = new();
    private readonly List<EventLocation> _usingLocation = new();

    public EventLocation Entrance { get; private set; }

    public void Initialize(List<EventLocation> locations)
    {
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
    }

    private readonly List<EventLocation> candidates = new();

    public EventLocation GetEmptyEventLocationByType(GuildLocationEventType type)
    {
        candidates.Clear();
        foreach (GuildLocationEventType key in _locations.Keys)
        {
            if (type.HasFlag(key))
            {
                var locations = _locations[key];
                if (locations != null && locations.Count > 0)
                {
                    candidates.AddRange(locations);
                }
            }
        }

        if (candidates.Count > 0)
        {
            int index = Random.Range(0, candidates.Count);
            EventLocation chosen = candidates[index];

            // 해당 EventLocation이 속해 있는 key에서 제거
            foreach (GuildLocationEventType key in _locations.Keys)
            {
                if (type.HasFlag(key) && _locations[key].Remove(chosen))
                {
                    break;
                }
            }

            _usingLocation.Add(chosen);
            return chosen;
        }

        return null;
    }

    public void ReturnLocation(EventLocation location)
    {
        _usingLocation.Remove(location);
        _locations[location.EventType].Add(location);
    }

    private void DebugLocations()
    {
        string str = "";
        foreach (var location in _locations)
        {
            var list = location.Value;
            foreach(var element in list)
            {
                str += $"{element.EventType} = {list.Count}개,\n";
            }
        }
        Debug.Log(str);
    }
}