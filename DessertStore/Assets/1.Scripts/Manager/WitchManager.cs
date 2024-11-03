using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WitchManager : MonoBehaviour
{
    [SerializeField] private CharacterManager _characterManager; // 하나의 캐릭터 매니저
    [SerializeField] private List<CharacterActionData> _characterActionDataList; // 캐릭터 행동 데이터 리스트

    private void Awake()
    {
        // CharacterManager 컴포넌트 초기화 (이 경우, 같은 GameObject에 있다고 가정)
        _characterManager = GetComponent<CharacterManager>();

        // CharacterActionData 리스트 초기화
        // 예를 들어, Resources 폴더에서 로드하는 방식
         _characterActionDataList = new List<CharacterActionData>(Resources.LoadAll<CharacterActionData>("ScriptableObject/CharacterAction/Main/Witch"));
    }

    public void PerformCharacterAction(CharacterState state)
    {
        // 상태에 해당하는 액션 데이터 가져오기
        CharacterActionData actionData = _characterActionDataList.Find(data => data.characterState == state);

        if (actionData != null)
        {
            _characterManager.PerformAction(actionData); // 캐릭터 액션 실행

            // 말한 뒤 고양이의 말하기를 실행하는 코루틴 시작
            StartCoroutine(CatSpeakAfterWitch(actionData));
        }
        else
        {
            Debug.LogWarning("해당 상태에 대한 액션 데이터가 없습니다.");
        }
    }

    private IEnumerator CatSpeakAfterWitch(CharacterActionData actionData)
    {
        // 마녀의 말한 시간(지속 시간) 이후 무작위 대기
        yield return new WaitForSeconds(actionData.displayDuration + Random.Range(1f, 2f));

        // 고양이의 상태에 따른 행동 설정
        switch (actionData.characterState)
        {
            case CharacterState.Happy:
                GameManager.Instance.PlayCatAction(CharacterState.Happy); // 기쁨 상태로 고양이 말하기
                break;

            case CharacterState.Sad:
                GameManager.Instance.PlayCatAction(CharacterState.Angry); // 화난 상태로 고양이 말하기
                break;
            case CharacterState.Fail:
                break;
            default:
                GameManager.Instance.PlayCatAction(CharacterState.Basic); // 기본 상태로 고양이 말하기
                break;
        }
    }
}
