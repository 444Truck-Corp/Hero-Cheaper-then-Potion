using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : Singleton<ResourceManager>
{
    [SerializeField] private List<Sprite> thumbnails; //StartScene에서 수동캐싱
    [SerializeField] private List<Sprite> standUIs; //StartScene에서 수동캐싱
    [SerializeField] private List<string> characterSheetPaths; //StartScene에서 수동캐싱
    [SerializeField] private List<string> damageVFXPaths; //StartScene에서 수동캐싱

    public Sprite GetSprites(bool isStand, int idx)
    {
        if (isStand) return standUIs[idx];
        else return thumbnails[idx];
    }

    public string GetCharacterSheetPath(int idx)
    {
        return characterSheetPaths[idx];
    }

    public string GetDamageVFXPath(int idx)
    {
        return damageVFXPaths[idx];
    }
}