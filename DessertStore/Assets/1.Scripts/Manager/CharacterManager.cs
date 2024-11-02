using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class CharacterManager : MonoBehaviour
{
    [SerializeField] private Image _faceImage; // 캐릭터 얼굴 이미지
    [SerializeField] private GameObject _balloon; // 말풍선 위치를 위한 부모 객체

    private Sprite _originalFace; // 원래 표정 저장

    private void Awake()
    {
        _originalFace = _faceImage.sprite; // 초기 얼굴 이미지 저장
    }

    // 캐릭터의 액션을 설정하는 메소드
    public void PerformAction(CharacterActionData actionData)
    {
        // 상태와 표정 변경
        _faceImage.sprite = actionData.expression;
        CharacterState currentState = actionData.characterState;

        // 말풍선 인스턴스화 및 텍스트 설정
        _balloon.GetComponent<Image>().sprite = actionData.speechBalloonImg;
         TextMeshProUGUI speechText = _balloon.GetComponentInChildren<TextMeshProUGUI>();

        // speechText 배열에서 랜덤으로 텍스트 선택
        string randomText = actionData.speechText[Random.Range(0, actionData.speechText.Length)];
        speechText.text = randomText;

        // 상태에 따른 추가 행동 구현 가능 (예: 상태에 따른 사운드 효과)
        HandleStateBehavior(currentState);

        //말풍선 활성화
        _balloon.SetActive(true);

        // 코루틴으로 일정 시간 후 원래 상태로 복구
        StartCoroutine(RevertToOriginalState(actionData.displayDuration));
    }

    // 상태에 따른 행동 처리
    private void HandleStateBehavior(CharacterState state)
    {
        switch (state)
        {
            case CharacterState.Happy:
                Debug.Log("캐릭터가 기뻐합니다!");
                // 기쁨에 따른 추가 효과 (예: 이모티콘 표시) 구현 가능
                break;
            case CharacterState.Sad:
                Debug.Log("캐릭터가 슬퍼합니다.");
                // 슬픔에 따른 추가 효과 (예: 우는 소리) 구현 가능
                break;
            case CharacterState.Angry:
                Debug.Log("캐릭터가 화났습니다!");
                // 놀람에 따른 추가 효과 (예: 점프 애니메이션) 구현 가능
                break;
            default:
               // Debug.Log("기본 상태입니다.");
                break;
        }
    }

    // 원래 상태로 복구하는 코루틴
    private IEnumerator RevertToOriginalState(float delay)
    {
        yield return new WaitForSeconds(delay);

        // 원래 표정과 상태로 복구
        _faceImage.sprite = _originalFace;

        // 말풍선 비활성화
        _balloon.SetActive(false);
    }
}
