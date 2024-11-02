using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CatManager : MonoBehaviour
{
    [SerializeField] private CharacterManager _characterManager; // 하나의 캐릭터 매니저
    [SerializeField] private List<CharacterActionData> _characterActionDataList; // 캐릭터 행동 데이터 리스트
    [SerializeField] private Image _catWing;// 캣윙이미지

    private void Awake()
    {
        // CharacterManager 컴포넌트 초기화 (이 경우, 같은 GameObject에 있다고 가정)
        _characterManager = GetComponent<CharacterManager>();

        // CharacterActionData 리스트 초기화
        // 예를 들어, Resources 폴더에서 로드하는 방식
         _characterActionDataList = new List<CharacterActionData>(Resources.LoadAll<CharacterActionData>("ScriptableObject/CharacterAction/Main/Cat"));

        StartCoroutine(WingAnimation());
    }

    public void PerformCharacterAction(CharacterState state)
    {
        // 상태에 해당하는 액션 데이터 가져오기
        CharacterActionData actionData = _characterActionDataList.Find(data => data.characterState == state);

        if (actionData != null)
        {
            _characterManager.PerformAction(actionData); // 캐릭터 액션 실행
        }
        else
        {
            Debug.LogWarning("해당 상태에 대한 액션 데이터가 없습니다.");
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
}
