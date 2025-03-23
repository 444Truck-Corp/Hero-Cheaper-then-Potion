using System.Collections.Generic;
using UnityEngine;

public class UICanvas : MonoBehaviour
{
    [SerializeField] List<Transform> uiParents;

    private void Awake()
    {
        UIManager.SetParents(uiParents);
    }
}