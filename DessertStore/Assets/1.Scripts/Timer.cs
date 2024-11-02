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

    void Start()
    {
        InvokeRepeating("UpdateTime", 0f, timeSpeed);
    }

    void UpdateTime()
    {
        currentTime += 1f;

        // 시간 텍스트 형식으로 변환
        int hours = (int)currentTime;
        string hourString = hours < 10 ? "0" + hours : hours.ToString();
        string timeString = hourString + ":00";
        timeText.text = timeString;

        // 시간에 따라 이미지 변경
        if (currentTime < 14f)
        {
            timeImage.sprite = sunSprite;
        }
        else
        {
            timeImage.sprite = moonSprite;
        }

        // 현재 시간을 디버그로 표시
        Debug.Log("현재 시간: " + timeString);

        // 오후 6시(18시) 이후 게임 종료
        if (currentTime >= 18f)
        {
            Debug.Log("게임 종료");
            GameManager.Instance.EndGame();
            CancelInvoke("UpdateTime");
        }
    }
}
