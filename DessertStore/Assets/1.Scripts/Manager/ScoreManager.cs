using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private int _score;

    // 현재 점수 반환
    public int CurrentScore => _score;

    // 점수 추가
    public void AddScore(int amount)
    {
        _score += amount;
    }

    // 점수 초기화
    public void ResetScore()
    {
        _score = 0;
    }
}
