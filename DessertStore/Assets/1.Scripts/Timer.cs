using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public Text timeText;       // 시간 텍스트를 표시할 UI Text 컴포넌트
    public Image timeImage;     // 해/달 이미지를 변경할 이미지
    public Sprite sunSprite;    // 해 이미지
    public Sprite moonSprite;   // 달 이미지

    private float currentTime = 8f; // 시작 시간을 9시로 설정
    private float timeSpeed = 2.5f; // 2.5초당 1시간 추가

    void Update()
    {
        if (GameManager.Instance != null)
        {
            // 시간 텍스트 업데이트
            timeText.text = GameManager.Instance.GetFormattedTime();

            // 해/달 이미지 업데이트
            timeImage.sprite = GameManager.Instance.GetTimeSprite(sunSprite, moonSprite);
        }
    }
}
