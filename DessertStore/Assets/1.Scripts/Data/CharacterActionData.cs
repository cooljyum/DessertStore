using UnityEngine;

public enum CharacterState
{
    Basic,
    Happy,
    Angry,
    Sad,
    Fail
}

[CreateAssetMenu(fileName = "New CharacterActionData", menuName = "Character/ActionData")]
public class CharacterActionData : ScriptableObject
{
    public CharacterState characterState;   // 캐릭터 상태
    public Sprite expression;               // 변경할 표정 이미지
    public string[] speechText;               // 말풍선에 표시할 텍스트
    public Sprite speechBalloonImg;   // 사용할 말풍선 프리팹
    public float displayDuration = 3f;      // 표정을 유지할 시간
}
