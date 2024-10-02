using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    [SerializeField]
    private Text _timerTxt;  // Ÿ�̸�text
    private float _timeRemaining = 30f; // test 30��
   // private float timeRemaining = 300f; // 5�� = 300��
    private bool _timerIsRunning = false;

    public bool TimerIsRunning
    {
        get { return _timerIsRunning; }
    }

    public Action OnTimerStart { get; internal set; }

    private void Start()
    {
        // Ÿ�̸� ����
        _timerIsRunning = true;
    }

    private void Update()
    {
        if (_timerIsRunning)
        {
            if (_timeRemaining > 0)
            {
                // Ÿ�̸� ����
                _timeRemaining -= Time.deltaTime;
                DisplayTime(_timeRemaining);
            }
            else
            {
                // Ÿ�̸� ���� ��
                Debug.Log("Ÿ�̸� ��");
                _timeRemaining = 0;
                _timerIsRunning = false;
            }
        }
    }

    // ���� �ð� ǥ��
    private void DisplayTime(float timeToDisplay)
    {
        timeToDisplay += 1; // �ð� �ݿø�

        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        // UI �ؽ�Ʈ ������Ʈ
        if (_timerTxt != null)
        {
            _timerTxt.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }
}
