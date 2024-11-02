using UnityEngine;

public class HeartManager : MonoBehaviour
{
    private int _hearts;

    // ��Ʈ ȹ�� �ִ� ��
    private const int MaxHearts = 3;
    private const int ScorePerHeart = 3; // ��Ʈ�� ��� ���� ���� ����

    // ���� ��Ʈ �� ��ȯ
    public int CurrentHearts => _hearts;

    // ������ ���� ��Ʈ ���� ���
    public void UpdateHearts(int score)
    {
        _hearts = Mathf.Min(score / ScorePerHeart, MaxHearts);
    }

    // ��Ʈ �ʱ�ȭ
    public void ResetHearts()
    {
        _hearts = 0;
    }
}
