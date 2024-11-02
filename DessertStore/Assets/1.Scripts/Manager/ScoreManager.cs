using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    private float _currentStamina;                           // 현재 남은 체력

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
    }

    private int _score;
    private int _packagingCount; // 포장 갯수 변수 추가

    // 현재 점수 반환
    public int CurrentScore => _score;

    // 포장 갯수 반환
    public int PackagingCount => _packagingCount;

    // 점수 추가
    public void AddScore(int amount)
    {
        _score += amount;
    }

    // 포장 갯수 추가
    public void AddPackagingCount(int amount)
    {
        _packagingCount += amount;

        Debug.Log($"현재 포장 개수 = {_packagingCount}");
    }

    // 점수 초기화
    public void ResetScore()
    {
        _score = 0;
        _packagingCount = 0; // 포장 갯수도 초기화
    }

    private void UpdateCurStaminaGauge(float currentStamina)
    {
        _currentStamina = currentStamina;
    }
    public float GetCurStaminaGauge()
    {
        return _currentStamina;
    }
}
