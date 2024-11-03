using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StoryManager : MonoBehaviour
{
    [SerializeField] private DialogueManager _dialogueManager;
    [SerializeField] private GameObject _storyPanel;
    [SerializeField] private Image _catWing;
    [SerializeField] private Image _catFace;
    [SerializeField] private Image _witchFace;
    [SerializeField] private Button _skipButton;
    [SerializeField] private TextMeshProUGUI _dialogueText;

    private int _currentIndex = 0;
    private Coroutine _wingCoroutine;
    private Coroutine _typingCoroutine;

    private void Start()
    {
        _skipButton.onClick.AddListener(OnSkipButtonClicked);
        ShowDialogue(_currentIndex);
        SoundManager.Instance.PlayBG("StoryBGM1");
    }

    private void Update()
    {
        // 우클릭으로 대사 진행
        if (Input.GetMouseButtonDown(1))
        {
            _currentIndex++;
            ShowDialogue(_currentIndex);
        }
    }

    private void ShowDialogue(int index)
    {
        var entry = _dialogueManager.GetDialogueEntry(index);

        if (entry != null)
        {
            // 타이핑 효과를 위한 코루틴 시작
            if (_typingCoroutine != null)
            {
                StopCoroutine(_typingCoroutine);
            }
            _typingCoroutine = StartCoroutine(TypeDialogue(entry.Dialogue));

            // 캐릭터별 표정 업데이트
            UpdateCharacterEmotion(entry.Speaker, entry.CatEmotion, entry.WitchEmotion);
        }
        else
        {
            // 대사가 끝났을 때
            EndDialogue();
        }
    }

    private IEnumerator TypeDialogue(string dialogue)
    {
        _dialogueText.text = "";
        foreach (char letter in dialogue.ToCharArray())
        {
            _dialogueText.text += letter;
            yield return new WaitForSeconds(0.05f); // 글자 간의 딜레이 (조정 가능)
        }
    }

    private void UpdateCharacterEmotion(string speaker, string catEmotion, string witchEmotion)
    {
        // 고양이 표정 업데이트
        _catFace.sprite = GetEmotionSprite("Cat", catEmotion);

        // 고양이 대사일 때만 날개 애니메이션 시작
        if (speaker == "Cat")
        {
            if (_wingCoroutine == null)
            {
                _wingCoroutine = StartCoroutine(WingAnimation());
            }
        }
        else
        {
            // 마녀가 대사 중일 때는 날개 애니메이션 중지
            if (_wingCoroutine != null)
            {
                StopCoroutine(_wingCoroutine);
                _wingCoroutine = null;
                _catWing.sprite = Resources.Load<Sprite>("Sprite/Character/Cat/Cat_WingDown"); // 기본 상태로 설정
            }
        }

        // 마녀 표정 업데이트
        _witchFace.sprite = GetEmotionSprite("Witch", witchEmotion);
    }

    private Sprite GetEmotionSprite(string speaker, string emotion)
    {
        if (speaker == "Cat")
        {
            return Resources.Load<Sprite>($"Sprite/Character/Cat/Cat_{emotion}");
        }
        else
        {
            return Resources.Load<Sprite>($"Sprite/Character/Witch/Witch_{emotion}");
        }
    }

    private IEnumerator WingAnimation()
    {
        Sprite wingDown = Resources.Load<Sprite>("Sprite/Character/Cat/Cat_WingDown");
        Sprite wingUp = Resources.Load<Sprite>("Sprite/Character/Cat/Cat_WingUp");

        while (true)
        {
            _catWing.sprite = wingUp;
            yield return new WaitForSeconds(1f);
            _catWing.sprite = wingDown;
            yield return new WaitForSeconds(1f);
        }
    }

    public void OnSkipButtonClicked()
    {
        EndDialogue();
    }

    private void EndDialogue()
    {
        // 날개 애니메이션 중지
        if (_wingCoroutine != null)
        {
            StopCoroutine(_wingCoroutine);
            _wingCoroutine = null;
        }

        // 타이핑 효과 중지
        if (_typingCoroutine != null)
        {
            StopCoroutine(_typingCoroutine);
        }

        SceneManager.LoadScene("MainScene");
    }
}