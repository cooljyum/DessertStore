using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public Text timeText;       // �ð� �ؽ�Ʈ�� ǥ���� UI Text ������Ʈ
    public Image timeImage;     // ��/�� �̹����� ������ �̹���
    public Sprite sunSprite;    // �� �̹���
    public Sprite moonSprite;   // �� �̹���

    private float currentTime = 8f; // ���� �ð��� 9�÷� ����
    private float timeSpeed = 2.5f; // 2.5�ʴ� 1�ð� �߰�

    void Start()
    {
        InvokeRepeating("UpdateTime", 0f, timeSpeed);
    }

    void UpdateTime()
    {
        currentTime += 1f;

        // �ð� �ؽ�Ʈ �������� ��ȯ
        int hours = (int)currentTime;
        string hourString = hours < 10 ? "0" + hours : hours.ToString();
        string timeString = hourString + ":00";
        timeText.text = timeString;

        // �ð��� ���� �̹��� ����
        if (currentTime < 14f)
        {
            timeImage.sprite = sunSprite;
        }
        else
        {
            timeImage.sprite = moonSprite;
        }

        // ���� �ð��� ����׷� ǥ��
        Debug.Log("���� �ð�: " + timeString);

        // ���� 6��(18��) ���� ���� ����
        if (currentTime >= 18f)
        {
            Debug.Log("���� ����");
            GameManager.Instance.EndGame();
            CancelInvoke("UpdateTime");
        }
    }
}
