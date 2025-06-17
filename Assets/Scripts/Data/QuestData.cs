using System;
using System.Collections.Generic;

[Serializable]
public class DropItemInfo
{
    public int id;
    public float probability; // 0.0f ~ 1.0f
}

[Serializable]
public class QuestProcessInfo
{
    public int questId; // 진행 중인 퀘스트 ID
    public List<int> heroIds; // 참여 중인 용사 ID 리스트
    public int returnDay;  //복귀 예정일
}

[Serializable]
public class QuestData
{
    public int id;
    public int rank;
    public string title;
    public int goldReward;
    public string needSpec; // "str, dex, int, hp"
    public int time; // 소요 일수
    public string dropItems; // "itemId:probability,itemId:probability,..."

    [NonSerialized] public StatusData needSpecs;
    [NonSerialized] public List<DropItemInfo> parsedDropItems;

    public void Parse()
    {
        // parsing neepSpecs 
        if (!string.IsNullOrWhiteSpace(needSpec))
        {
            var parts = needSpec.Split(',');
            var values = new List<int>();
            
            for (int i = 0; i < parts.Length; i++)
            {
                if (int.TryParse(parts[i].Trim(), out var value))
                {
                    values.Add(value);
                }
            }

            needSpecs.STR = values.Count > 0 ? values[0] : 0;
            needSpecs.DEX = values.Count > 1 ? values[1] : 0;
            needSpecs.INT = values.Count > 2 ? values[2] : 0;
            needSpecs.HP = values.Count > 3 ? values[3] : 0;
        }
        
        //parsing dropItems
        parsedDropItems = new List<DropItemInfo>();
        if (string.IsNullOrWhiteSpace(dropItems))
            return;

        var entries = dropItems.Split(',');
        foreach (var entry in entries)
        {
            var parts = entry.Trim().Split(':');
            if (parts.Length != 2) continue;

            if (int.TryParse(parts[0], out var itemId) &&
                float.TryParse(parts[1], out var prob))
            {
                parsedDropItems.Add(new DropItemInfo { id = itemId, probability = prob });
            }
        }
    }
}
