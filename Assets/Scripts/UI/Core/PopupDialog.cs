using System;
using TMPro;
using UnityEngine;

public enum eDialogResult
{
    Cancle,
    Yes,
    No,
}

public class PopupDialog : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    private Action<eDialogResult> action;

    public void SetTextAndAction(string text, Action<eDialogResult> action)
    {
        this.text.text = text;
        this.action = action;
    }

    public void OnClickButton(int number)
    {
        if (number == 1)
        {
            action.Invoke(eDialogResult.Yes);
        }
        else if (number == 0)
        {
            action.Invoke(eDialogResult.No);
        }
        else
        {
            action.Invoke(eDialogResult.Cancle);
        }
    }

    private void OnDisable()
    {
        Destroy(this);
    }
}