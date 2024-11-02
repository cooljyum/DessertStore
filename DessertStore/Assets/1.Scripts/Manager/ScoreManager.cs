using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    private float _currentStamina; // 현재 남은 체력
    private float _maxStamina = 100f; // 최대 체력
    private int _staminaIncrementThreshold = 5; // 스테미나 증가를 위한 포장 갯수 기준
    private int _packagingCount; // 포장 갯수 변수 추가

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

    // 현재 점수 반환
    public int CurrentScore => _score;

    // 포장 갯수 반환
    public int PackagingCount => _packagingCount;

    // 점수 추가
    public void AddScore(int amount)
    {
        _score += amount;
    }

    // 포장 갯수 추가 및 스테미나 체크
    public void AddPackagingCount(int amount)
    {
        _packagingCount += amount;

        IncreaseStamina();

        Debug.Log($"현재 포장 개수 = {_packagingCount}");
    }

    // 스테미나 증가
    private void IncreaseStamina()
    {
        _currentStamina += 0.2f;
        if (_currentStamina > _maxStamina) // 최대 체력 초과 방지
        {
            _currentStamina = _maxStamina;
        }
        Debug.Log($"현재 스테미나 = {_currentStamina}");

        GameManager.Instance.PlayWitchAction(CharacterState.Happy);
    }

    // 점수 초기화
    public void ResetScore()
    {
        _score = 0;
        _packagingCount = 0; // 포장 갯수도 초기화
        _currentStamina = 0; // 스테미나도 초기화
    }

    //스테미나 설정
    public float GetCurStaminaGauge()
    {
        return _currentStamina;
    }
    public void SetCurStaminaGauge(float curStamina)
    {
        _currentStamina = curStamina;
    }
}
