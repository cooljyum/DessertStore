using UnityEngine;

public class HeartManager : MonoBehaviour
{
    private int _hearts;

    // 하트 획득 최대 수
    private const int MaxHearts = 3;
    private const int ScorePerHeart = 3; // 하트를 얻기 위한 점수 기준

    // 현재 하트 수 반환
    public int CurrentHearts => _hearts;

    // 점수에 따라 하트 갯수 계산
    public void UpdateHearts(int score)
    {
        _hearts = Mathf.Min(score / ScorePerHeart, MaxHearts);
    }

    // 하트 초기화
    public void ResetHearts()
    {
        _hearts = 0;
    }
}
