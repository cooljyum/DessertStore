using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    private float _currentStamina;                           // ���� ���� ü��

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
    private int _packagingCount; // ���� ���� ���� �߰�

    // ���� ���� ��ȯ
    public int CurrentScore => _score;

    // ���� ���� ��ȯ
    public int PackagingCount => _packagingCount;

    // ���� �߰�
    public void AddScore(int amount)
    {
        _score += amount;
    }

    // ���� ���� �߰�
    public void AddPackagingCount(int amount)
    {
        _packagingCount += amount;

        Debug.Log($"���� ���� ���� = {_packagingCount}");
    }

    // ���� �ʱ�ȭ
    public void ResetScore()
    {
        _score = 0;
        _packagingCount = 0; // ���� ������ �ʱ�ȭ
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
