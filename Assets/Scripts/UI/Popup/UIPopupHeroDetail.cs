
using TMPro;
using UnityEngine;

public class UIPopupHeroDetail : UIBase
{
    [SerializeField] private TextMeshProUGUI nameTxt;
    [SerializeField] private TextMeshProUGUI lvTxt;
    [SerializeField] private TextMeshProUGUI classTxt;

    public override void Opened(object[] param)
    {
        if (param.Length == 1)
        {
            if (param[0] is HeroData data)
            {
                nameTxt.text = data.name;
                lvTxt.text = data.level.ToString();
                classTxt.text = data.classData.className.ToString();

                //TODO : Status 추가
            }
            else
            {
                Debug.LogError("[UIPopupDetail] : Show에서 올바른 인자가 전달되지 못했습니다.");
            }
        }
        else
        {
            Debug.LogError("[UIPopupDetail] : Show에서 올바른 인자가 전달되지 못했습니다.");
        }
    }
}