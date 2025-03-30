using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class EndScene : MonoBehaviour
{
    [SerializeField] Image[] BGs;
    [SerializeField] TextMeshProUGUI DescTxt;

    string[] End1 = new string[] {
        "모든 것은 한 순간의 선택에서 시작되었다.",
        "자금이 고갈되고, 용사의 훈련은 중단되었다.",
        "결국, 마왕은 부활에 성공했고 이 땅을 파멸로 이끌었다.",
        "길드는 폐허가 되고, 당신은 국가에서 추방당했다.",
        "그러나 실패는 끝이 아니다.",
        "모든 것은 다시 시작될 수 있다.",
        "다음에는 더 나은 결정을 내릴 수 있을 것이다.",
        "...",
        "END 1 : 당신은 경영자로서의 책임을 다하지 못하였습니다"
    };

    string[] End2 = new string[] {
        "마왕은 전설 속의 악몽 그대로 이 땅에 파멸을 가져왔다",
        "도시들은 불길 속에 휩싸였고, 대지는 어둠으로 물들었다",
        "사람들은 희망을 잃고 흩어져 도망쳤으며, 이 세계는 절망의 시대에 접어들었다",
        "그러나 실패는 끝이 아니다.",
        "모든 것은 다시 시작될 수 있다.",
        "다음에는 더 나은 결정을 내릴 수 있을 것이다.",
        "...",
        "END 2 : 당신은 마왕의 부활을 막지 못했습니다"
    };

    string[] End3 = new string[] {
        "긴 여정의 끝에서, 용사 일행은 마왕을 쓰러뜨리고 이 땅에 다시 평화를 가져왔다",
        "당신의 지휘 아래, 용사들은 훌륭히 성장하여 마왕에 맞설 힘을 얻었다",
        "사람들은 당신과 용사들을 찬양하며, 당신의 이름은 영원히 이 세계의 전설로 남게 되었다",
        "...",
        "END 3 : 축하합니다! 당신은 마왕을 물리치고 게임을 클리어했습니다!"
    };

    private string[] currentEnding;
    private int currentLineIndex = 0;
    private bool isDisplaying = false;
    private string fullText = "";
    public float displaySpeed = 0.05f;

    private void Start()
    {
        //switch (GameManager.Instance.Ending)
        //{
        //    case eEnding.Bankrupt:
        //        BGs[0].gameObject.SetActive(true);
        //        currentEnding = End1;
        //        AudioManager.Instance.PlayBGM(6);
        //        break;
        //    case eEnding.Lose:
        //        BGs[1].gameObject.SetActive(true);
        //        currentEnding = End2;
        //        AudioManager.Instance.PlayBGM(6);
        //        break;
        //    case eEnding.Win:
        //        BGs[2].gameObject.SetActive(true);
        //        currentEnding = End3;
        //        AudioManager.Instance.PlayBGM(5);
        //        break;
        //}

        currentLineIndex = 0;
        StartDisplayingText();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (isDisplaying)
            {
                StopAllCoroutines();
                DescTxt.text = fullText;
                isDisplaying = false;
            }
            else
            {
                currentLineIndex++;
                if (currentLineIndex < currentEnding.Length)
                {
                    StartDisplayingText();
                }
            }
        }
    }

    private void StartDisplayingText()
    {
        if (currentLineIndex < currentEnding.Length)
        {
            fullText = currentEnding[currentLineIndex];
            DescTxt.text = ""; 
            StartCoroutine(DisplayText());
        }
    }

    private System.Collections.IEnumerator DisplayText()
    {
        isDisplaying = true;
        for (int i = 0; i < fullText.Length; i++)
        {
            DescTxt.text += fullText[i];
            yield return new WaitForSeconds(displaySpeed);
        }
        isDisplaying = false;
    }

    public void EndGame()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
    Application.Quit(); // 실제 빌드에서 애플리케이션 종료
#endif
    }
}
