using TMPro;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIMainClock : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI dayTxt;
    [SerializeField] private Image clockFill;

    private Coroutine updateTimeCoroutine;
    
    private int day;
    private float time;
    private float lastSavedTime = -1f;
    private const float saveThreshold = 0.1f;
    private const float maxTime = 60f * 3; //3분.

    #region Unity Life Cycle
    private void Awake()
    {
        day = SaveManager.Instance.MySaveData.day;
        time = SaveManager.Instance.MySaveData.time;

        dayTxt.text = $"{day}일"; //TODO : Localization
        clockFill.fillAmount = time / maxTime;

        updateTimeCoroutine = StartCoroutine(UpdateTime());
    }
    #endregion

    #region Main Methods

    #endregion

    #region Sub Methods
    private IEnumerator UpdateTime()
    {
        while (time < maxTime)
        {
            //시간 업데이트
            time += Time.deltaTime;
            clockFill.fillAmount = time / maxTime;

            //시간 저장 (0.1초마다)
            if (Mathf.Abs(time - lastSavedTime) >= saveThreshold)
            {
                SaveManager.Instance.SetSaveData(nameof(time), time);
                lastSavedTime = time;
            }

            yield return null;
        }

        //하루 종료
        yield return StartCoroutine(NextDayRoutine());
        updateTimeCoroutine = StartCoroutine(UpdateTime());
    }

    private IEnumerator NextDayRoutine()
    {
        day--;
        SaveManager.Instance.SetSaveData(nameof(day), day);
        UIManager.Show<UIOverrideDayResult>();

        yield return new WaitUntil(() => !UIManager.IsActive<UIOverrideDayResult>());
    }
    #endregion
}