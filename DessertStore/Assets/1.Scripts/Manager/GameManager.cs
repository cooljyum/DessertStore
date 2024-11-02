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

    // ���� �߰� �� ��Ʈ ������Ʈ
    public void AddScore(int amount)
    {
        _scoreManager.AddScore(amount);
        _heartManager.UpdateHearts(_scoreManager.CurrentScore);
    }

    // ���� ������ ��Ʈ �� ��ȯ
    public int GetCurrentScore() => _scoreManager.CurrentScore;
    public int GetCurrentHearts() => _heartManager.CurrentHearts;

    // �������� ���� �� �ʱ�ȭ
    public void ResetStage()
    {
        _scoreManager.ResetScore();
        _heartManager.ResetHearts();
    }
}
