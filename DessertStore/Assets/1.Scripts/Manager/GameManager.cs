using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private ScoreManager _scoreManager;
    private HeartManager _heartManager;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        _scoreManager = GetComponent<ScoreManager>();
        _heartManager = GetComponent<HeartManager>();
    }

    // 점수 추가 및 하트 업데이트
    public void AddScore(int amount)
    {
        _scoreManager.AddScore(amount);
        _heartManager.UpdateHearts(_scoreManager.CurrentScore);
    }

    // 현재 점수와 하트 수 반환
    public int GetCurrentScore() => _scoreManager.CurrentScore;
    public int GetCurrentHearts() => _heartManager.CurrentHearts;

    // 스테이지 시작 시 초기화
    public void ResetStage()
    {
        _scoreManager.ResetScore();
        _heartManager.ResetHearts();
    }
}
