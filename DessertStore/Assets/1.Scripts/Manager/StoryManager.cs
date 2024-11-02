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
        // ��Ŭ������ ��� ����
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

            // ĳ���ͺ� ǥ�� ������Ʈ
            UpdateCharacterEmotion(entry.Speaker, entry.CatEmotion, entry.WitchEmotion);
        }
        else
        {
            // ��簡 ������ ��
            EndDialogue();
        }
    }

    private void UpdateCharacterEmotion(string speaker, string catEmotion, string witchEmotion)
    {
        // Cat�� ǥ���� ���� ���¸� ����
        if (speaker == "Cat")
        {
            _catFace.sprite = GetEmotionSprite(speaker, catEmotion);

            // Cat�� ����� ���� ���� �ִϸ��̼� ����
            if (_wingCoroutine == null)
            {
                _wingCoroutine = StartCoroutine(WingAnimation());
            }
        }
        else if (speaker == "Witch")
        {
            _witchFace.sprite = GetEmotionSprite(speaker, witchEmotion);

            // Witch�� ����� ���� ���� �ִϸ��̼� ����
            if (_wingCoroutine != null)
            {
                StopCoroutine(_wingCoroutine);
                _wingCoroutine = null;
                _catWing.sprite = Resources.Load<Sprite>("Sprite/Character/Cat/Cat_WingDown"); // �⺻ ���·� ����
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
        // ���� �ִϸ��̼� ����
        if (_wingCoroutine != null)
        {
            StopCoroutine(_wingCoroutine);
            _wingCoroutine = null;
        }

        SceneManager.LoadScene("MainScene");
    }
}