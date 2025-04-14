using UnityEngine;

public class UIOverridePause : UIBase
{
    public override void Closed(object[] param)
    {
        Time.timeScale = 1;
    }

    public void OnClicked()
    {
        SetActive<UIOverridePause>(false);
    }
}