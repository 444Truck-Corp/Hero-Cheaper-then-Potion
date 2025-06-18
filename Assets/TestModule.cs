using System.Collections.Generic;
using UnityEngine;

public class TestModule : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            UIManager.Show<UIPopupShop>(new List<int>() { 1, 2, 3, 4 });
        }
    }
}