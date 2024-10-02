using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    [SerializeField]
    private Text _timerTxt;  // 타이머text
    private float _timeRemaining = 30f; // test 30초
   // private float timeRemaining = 300f; // 5분 = 300초
    private bool _timerIsRunning = false;

    public bool TimerIsRunning
    {
        get { return _timerIsRunning; }
    }

    public Action OnTimerStart { get; internal set; }

    private void Start()
    {
        // 타이머 시작
        _timerIsRunning = true;
    }

    private void Update()
    {
        if (_timerIsRunning)
        {
            if (_timeRemaining > 0)
            {
                // 타이머 감소
                _timeRemaining -= Time.deltaTime;
                DisplayTime(_timeRemaining);
            }
            else
            {
                // 타이머 종료 시
                Debug.Log("타이머 끝");
                _timeRemaining = 0;
                _timerIsRunning = false;
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
        if (_timerTxt != null)
        {
            _timerTxt.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }
}
