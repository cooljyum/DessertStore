using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private int _score;

    // ���� ���� ��ȯ
    public int CurrentScore => _score;

    // ���� �߰�
    public void AddScore(int amount)
    {
        _score += amount;
    }

    // ���� �ʱ�ȭ
    public void ResetScore()
    {
        _score = 0;
    }
}
