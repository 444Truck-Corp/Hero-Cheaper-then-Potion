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
    public float reward;
    public string rawNeedSpecs; // "str, dex, int, hp"
    public int time; // 소요 일수
    public string rawDropItems; // "itemId:probability,itemId:probability,..."

    [NonSerialized] public int[] needSpecs;
    [NonSerialized] public List<DropItemInfo> parsedDropItems;

    public void Parse()
    {
        // parsing neepSpecs 
        if (!string.IsNullOrWhiteSpace(rawNeedSpecs))
        {
            var parts = rawNeedSpecs.Split(',');
            var values = new List<int>();
            foreach (var part in parts)
            {
                if (int.TryParse(part.Trim(), out var num))
                    values.Add(num);
            }
            needSpecs = values.ToArray();
        }
        
        //parsing dropItems
        parsedDropItems = new List<DropItemInfo>();
        if (string.IsNullOrWhiteSpace(rawDropItems))
            return;

        var entries = rawDropItems.Split(',');
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
