using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    [SerializeField]
    public Text timerText;  // 타이머text
    private float timeRemaining = 30f; // test 30초
   // private float timeRemaining = 300f; // 5분 = 300초
    private bool timerIsRunning = false;

    private void Start()
    {
        // 타이머 시작
        timerIsRunning = true;
    }

    private void Update()
    {
        if (timerIsRunning)
        {
            if (timeRemaining > 0)
            {
                // 타이머 감소
                timeRemaining -= Time.deltaTime;
                DisplayTime(timeRemaining);
            }
            else
            {
                // 타이머 종료 시
                Debug.Log("타이머 끝");
                timeRemaining = 0;
                timerIsRunning = false;
            }
        }
    }

    // 남은 시간 표시
    private void DisplayTime(float timeToDisplay)
    {
        timeToDisplay += 1; // 시간 반올림

        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        // UI 텍스트 업데이트
        if (timerText != null)
        {
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }
}
