using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    [SerializeField]
    public Text timerText;  // Ÿ�̸�text
    private float timeRemaining = 30f; // test 30��
   // private float timeRemaining = 300f; // 5�� = 300��
    private bool timerIsRunning = false;

    private void Start()
    {
        // Ÿ�̸� ����
        timerIsRunning = true;
    }

    private void Update()
    {
        if (timerIsRunning)
        {
            if (timeRemaining > 0)
            {
                // Ÿ�̸� ����
                timeRemaining -= Time.deltaTime;
                DisplayTime(timeRemaining);
            }
            else
            {
                // Ÿ�̸� ���� ��
                Debug.Log("Ÿ�̸� ��");
                timeRemaining = 0;
                timerIsRunning = false;
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
        if (timerText != null)
        {
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }
}
