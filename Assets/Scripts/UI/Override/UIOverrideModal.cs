using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using DG.Tweening;

public class UIOverrideModal : UIBase
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;

    private Action onConfirm;

    /// <summary>
    /// 사용 예 : UIManager.Show&lt;UIOverrideModal&gt;("삭제", "정말 삭제하시겠습니까?", true, true, () =&gt; DeleteData());
    /// </summary>
    /// <param name="param">제목(string), 내용(string), 확인 버튼 표시(bool), 취소 버튼 표시(bool), 확인 시 실행할 Action</param>
    public override void Opened(object[] param)
    {
        //parameter receive
        string title = param.Length > 0 ? param[0] as string : "";
        string desc = param.Length > 1 ? param[1] as string : "";
        bool showConfirm = param.Length > 2 && param[2] is bool b1 && b1;
        bool showCancel = param.Length > 3 && param[3] is bool b2 && b2;
        onConfirm = param.Length > 4 && param[4] is Action act ? act : null;

        //제목, 설명 설정
        titleText.text = title;
        descriptionText.text = desc;

        //확인 버튼
        confirmButton.gameObject.SetActive(showConfirm);
        confirmButton.onClick.RemoveAllListeners();
        if (showConfirm)
            confirmButton.onClick.AddListener(OnConfirmClicked);

        //취소 버튼
        cancelButton.gameObject.SetActive(showCancel);
        cancelButton.onClick.RemoveAllListeners();
        if (showCancel)
            cancelButton.onClick.AddListener(() => SetActive<UIOverrideModal>(false));

        //애니메이션
        transform.DOLocalMoveY(0, 0.25f).From().SetEase(Ease.OutBack);
    }

    private void OnConfirmClicked()
    {
        onConfirm?.Invoke();
        SetActive<UIOverrideModal>(false);
    }
}
