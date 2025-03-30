using UnityEngine;
using UnityEngine.UI;

public class UINavQuest : UIBase
{
    [SerializeField] private Toggle toggleQuestAccepted;
    [SerializeField] private Toggle toggleQuestOnboard;
    [SerializeField] private Toggle toggleQuestAlways;

    private void Start()
    {
        toggleQuestAccepted.isOn = true;
    }
}