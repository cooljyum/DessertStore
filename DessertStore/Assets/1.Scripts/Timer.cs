using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    [SerializeField] private Text _timerTxt;  // 타이머text
    [SerializeField] private GameObject _panel;  // 타이머가 끝났을 때 표시할 패널
    [SerializeField] private bool _timerIsRunning = false; //타이머 bool

    private float _timeRemaining = 60f;
    public float TimeRemaining => _timeRemaining;
   // private float timeRemaining = 300f; // 5분 = 300초

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

                // 타이머가 끝나면 패널 표시
                if (_panel != null)
                {
                    _panel.SetActive(true); // 패널 활성화
                }
                else
                {
                    Debug.LogError("패널이 null입니다!");
                }
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
