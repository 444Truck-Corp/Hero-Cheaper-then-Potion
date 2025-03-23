using TMPro;
using UnityEngine;

public class PopupWarning : MonoBehaviour
{
    public TextMeshProUGUI titleTxt;
    public TextMeshProUGUI warnTxt;

    public void SetWarnTxt(string msg, string title)
    {
        warnTxt.text = msg;
        if (string.IsNullOrEmpty(title))
        {
            titleTxt.text = "경고";
        }
        else
        {
            titleTxt.text = title;
        }
    }

    private void OnDisable()
    {
        Destroy(this);
    }
}