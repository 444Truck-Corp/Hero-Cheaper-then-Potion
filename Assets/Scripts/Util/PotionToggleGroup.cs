using System.Collections.Generic;
using UnityEngine;

public class PotionToggleGroup : MonoBehaviour
{
    [Header("Image Toggles")]
    public List<ImageToggle> toggles = new(); 
    public List<int> selectedIndices = new();

    private void OnEnable()
    {
        for (int i = 0; i < toggles.Count; i++)
        {
            //if (GameManager.Instance.Potions[i] == 0)
            //{
            //    toggles[i].transform.parent.gameObject.SetActive(false);
            //}
        }
    }

    private void OnDisable()
    {
        for (int i = 0; i < toggles.Count; i++)
        {
            if (toggles[i].isOn) 
            {
                toggles[i].Toggle();
                selectedIndices.Clear();
            }
        }
    }

    public void OnToggleClicked(int index)
    {
        var toggle = toggles[index];

        if (toggle.isOn)
        {
            if (!selectedIndices.Contains(index))
            {
                selectedIndices.Add(index);
            }
        }
        else
        {
            if (selectedIndices.Contains(index))
            {
                selectedIndices.Remove(index);
            }
        }
    }

    public int[] GetSelectedIndices()
    {
        return selectedIndices.ToArray(); 
    }

    public List<ImageToggle> GetSelectedItems()
    {
        List<ImageToggle> selectedItems = new List<ImageToggle>();

        foreach (int index in selectedIndices)
        {
            selectedItems.Add(toggles[index]);
        }

        return selectedItems;
    }
}
