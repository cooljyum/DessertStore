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

    private void Start()
    {
        _skipButton.onClick.AddListener(OnSkipButtonClicked);
        ShowDialogue(_currentIndex);
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
            _dialogueText.text = entry.Dialogue;

            // 캐릭터별 표정 업데이트
            UpdateCharacterEmotion(entry.Speaker, entry.CatEmotion, entry.WitchEmotion);
        }
        else
        {
            // 대사가 끝났을 때
            EndDialogue();
        }
    }

    private void UpdateCharacterEmotion(string speaker, string catEmotion, string witchEmotion)
    {
        // Cat의 표정과 날개 상태를 설정
        if (speaker == "Cat")
        {
            _catFace.sprite = GetEmotionSprite(speaker, catEmotion);

            // Cat의 대사일 때만 날개 애니메이션 시작
            if (_wingCoroutine == null)
            {
                _wingCoroutine = StartCoroutine(WingAnimation());
            }
        }
        else if (speaker == "Witch")
        {
            _witchFace.sprite = GetEmotionSprite(speaker, witchEmotion);

            // Witch의 대사일 때는 날개 애니메이션 중지
            if (_wingCoroutine != null)
            {
                StopCoroutine(_wingCoroutine);
                _wingCoroutine = null;
                _catWing.sprite = Resources.Load<Sprite>("Sprite/Character/Cat/Cat_WingDown"); // 기본 상태로 설정
            }
        }
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

        SceneManager.LoadScene("MainScene");
    }
}