using System.Collections;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class EndScene : MonoBehaviour
{
    [SerializeField] private Image[] _backgroundImages;
    [SerializeField] private TextMeshProUGUI _descriptionText;
    [SerializeField] private float _textDisplaySpeed = 0.05f;

    private readonly int[] BGMS = { 3, 3, 4 };
    private readonly string[][] TEXTS = {
        new string[] {
            "모든 것은 한 순간의 선택에서 시작되었다.",
            "자금이 고갈되고, 용사의 훈련은 중단되었다.",
            "결국, 마왕은 부활에 성공했고 이 땅을 파멸로 이끌었다.",
            "길드는 폐허가 되고, 당신은 국가에서 추방당했다.",
            "그러나 실패는 끝이 아니다.",
            "모든 것은 다시 시작될 수 있다.",
            "다음에는 더 나은 결정을 내릴 수 있을 것이다.",
            "...",
            "END 1 : 당신은 경영자로서의 책임을 다하지 못하였습니다"
        },
        new string[] {
            "마왕은 전설 속의 악몽 그대로 이 땅에 파멸을 가져왔다",
            "도시들은 불길 속에 휩싸였고, 대지는 어둠으로 물들었다",
            "사람들은 희망을 잃고 흩어져 도망쳤으며, 이 세계는 절망의 시대에 접어들었다",
            "그러나 실패는 끝이 아니다.",
            "모든 것은 다시 시작될 수 있다.",
            "다음에는 더 나은 결정을 내릴 수 있을 것이다.",
            "...",
            "END 2 : 당신은 마왕의 부활을 막지 못했습니다"
        },
        new string[] {
            "긴 여정의 끝에서, 용사 일행은 마왕을 쓰러뜨리고 이 땅에 다시 평화를 가져왔다",
            "당신의 지휘 아래, 용사들은 훌륭히 성장하여 마왕에 맞설 힘을 얻었다",
            "사람들은 당신과 용사들을 찬양하며, 당신의 이름은 영원히 이 세계의 전설로 남게 되었다",
            "...",
            "END 3 : 축하합니다! 당신은 마왕을 물리치고 게임을 클리어했습니다! \n 정식 출시를 기대해주세요."
        },
    };

    private bool _isDisplaying;
    private int _currentLineIndex = 0;
    private int _endingType;
    private string _currentText = "";

    private void Start()
    {
        _endingType = (int)GameManager.Instance.Ending;
        _backgroundImages[_endingType].gameObject.SetActive(true);
        AudioManager.Instance.PlayBGM(BGMS[_endingType]);

        _currentLineIndex = 0;
        StartDisplayingText();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (_isDisplaying)
            {
                StopAllCoroutines();
                _descriptionText.text = _currentText;
                _isDisplaying = false;
            }
            else
            {
                _currentLineIndex++;
                if (_currentLineIndex < TEXTS[_endingType].Length)
                {
                    StartDisplayingText();
                }
            }
        }
    }

    private void StartDisplayingText()
    {
        if (_currentLineIndex < TEXTS[_endingType].Length)
        {
            _currentText = TEXTS[_endingType][_currentLineIndex];
            _descriptionText.text = "";
            StartCoroutine(DisplayText());
        }
    }

    private IEnumerator DisplayText()
    {
        _isDisplaying = true;
        for (int i = 0; i < _currentText.Length; i++)
        {
            _descriptionText.text += _currentText[i];
            yield return new WaitForSeconds(_textDisplaySpeed);
        }
        _isDisplaying = false;
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