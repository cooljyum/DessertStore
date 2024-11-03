using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HeartManager : MonoBehaviour
{
    public static HeartManager Instance;

    [SerializeField] private int _currentScore = 50;       // 초기 호감도
    [SerializeField] private float _successIncrement = 10f; // 성공 시 호감도 증가량
    [SerializeField] private float _failureDecrement;       // 실패 시 호감도 감소량 (성공의 2/3)
    [SerializeField] private int _fullHeartThreshold = 80;  // 만점 기준

    private int _maxScore = 100;           // 최대 호감도
    private int _minScore = 0;             // 최소 호감도

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
   

    void Start()
    {
        // 실패 시 감소량을 성공 시 증가량의 2/3로 설정
        _failureDecrement = _successIncrement * 2 / 3;

        // 1초마다 호감도 감소
        InvokeRepeating("UpdateScoreOverTime", 1.5f, 1.5f);

        // 초기 호감도 표시
        MainManager.Instance.UpdateScoreUI();
    }

    public string GetCurScore() 
    {
        return _currentScore.ToString();
    }

    // 주문 성공 시 호출
    public void OnOrderSuccess()
    {
        IncreaseScore(_successIncrement);
    }

    // 주문 실패 시 호출
    public void OnOrderFailure()
    {
        DecreaseScore(_failureDecrement);
    }

    // 배달 성공 시 호출
    public void OnDeliverySuccess()
    {
        IncreaseScore(_successIncrement);
    }

    // 배달 실패 시 호출
    public void OnDeliveryFailure()
    {
        DecreaseScore(_failureDecrement);
    }

    // 1초마다 호출되어 호감도 1씩 감소
    private void UpdateScoreOverTime()
    {
        DecreaseScore(1f);
    }

    // 호감도 증가 함수
    private void IncreaseScore(float amount)
    {
        _currentScore = Mathf.Clamp(_currentScore + (int)amount, _minScore, _maxScore);
        MainManager.Instance.UpdateScoreUI();
    }

    // 호감도 감소 함수
    private void DecreaseScore(float amount)
    {
        _currentScore = Mathf.Clamp(_currentScore - (int)amount, _minScore, _maxScore);
        MainManager.Instance.UpdateScoreUI();
    }

    // UI 업데이트 함수

    // 하트 수 초기화 함수
    public void ResetHearts()
    {
        _currentScore = 50; // 초기값으로 설정 (여기서 초기값은 필요에 따라 수정 가능)
        MainManager.Instance.UpdateScoreUI();    // UI 업데이트
    }

    // 게임 종료시 1초 마다 깎이는 호감도 종료
    public void EndCalculateHeart()
    {
        CancelInvoke("UpdateScoreOverTime"); // 게임 종료 시 호출 멈춤
    }

    // 하트 개수 계산
    public int CalculateHearts()
    {
        if (_currentScore >= _fullHeartThreshold)
            return 3;
        else if (_currentScore >= _fullHeartThreshold * 2 / 3)
            return 2;
        else if (_currentScore >= _fullHeartThreshold / 3)
            return 1;
        else
            return 0;
    }
}
