using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public bool IsGamePlay = false;

    [SerializeField] private ScoreManager _scoreManager;
    [SerializeField] private HeartManager _heartManager;

    [SerializeField] private WitchManager _witchManager;
    [SerializeField] private CatManager _catManager;

    private float _currentTime = 8f; // 시작 시간 (9시)
    private float _timeSpeed = 30f; // 2.5초당 1시간 증가

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

        // HeartManager와 ScoreManager 인스턴스를 찾거나 직접 할당
        _scoreManager = _scoreManager ?? FindObjectOfType<ScoreManager>();
        _heartManager = _heartManager ?? FindObjectOfType<HeartManager>();

        IsGamePlay = true;

        // 마녀의 액션을 무작위 간격으로 실행하는 코루틴 시작
        StartCoroutine(WitchActionRoutine());

        // 시간 업데이트 코루틴 시작
        StartCoroutine(TimeUpdateRoutine());
    }

    // 시간을 관리하는 코루틴
    private IEnumerator TimeUpdateRoutine()
    {
        while (IsGamePlay)
        {
            _currentTime += 1f; // 1시간 증가
            if (_currentTime >= 18f)
            {
                EndGame();
                break;
            }

            yield return new WaitForSeconds(_timeSpeed);
        }
    }

    // 시간을 텍스트 형식으로 반환하는 메소드
    public string GetFormattedTime()
    {
        int hours = (int)_currentTime;
        string hourString = hours < 10 ? "0" + hours : hours.ToString();
        return hourString + ":00";
    }

    // 현재 시간에 따라 해/달 상태 반환
    public Sprite GetTimeSprite(Sprite sunSprite, Sprite moonSprite)
    {
        return _currentTime < 14f ? sunSprite : moonSprite;
    }

    // 점수 추가 및 하트 업데이트
    public void AddScore(int amount)
    {
        _scoreManager.AddScore(amount);
        _heartManager.OnOrderSuccess();
    }

    // 포장 성공시 부르는 함수
    public void OnPackagingSuccess()
    {
        _scoreManager.AddPackagingCount(1);
        _heartManager.OnOrderSuccess();
    }

    // 현재 점수와 하트 수 반환
    public int GetCurrentScore() => _scoreManager.CurrentScore;
    public int GetCurrentHearts() => _heartManager.CalculateHearts();

    // 스테이지 시작 시 초기화
    public void ResetStage()
    {
        _scoreManager.ResetScore();
        _heartManager.ResetHearts();
    }

    // 마녀 액션제어
    public void PlayWitchAction(CharacterState state)
    {
        if (_witchManager != null)
        {
            _witchManager.PerformCharacterAction(state);
        }
    }

    // 액션 제어
    public void PlayCatAction(CharacterState state)
    {
        if (_catManager != null)
        {
            _catManager.PerformCharacterAction(state);
        }
    }

    // 마녀 액션을 무작위 간격으로 실행하는 코루틴
    private IEnumerator WitchActionRoutine()
    {
        while (true) // 무한 루프
        {
            float waitTime = Random.Range(7f, 10f);
            yield return new WaitForSeconds(waitTime);
            CharacterState randomState = CharacterState.Basic;
            PlayWitchAction(randomState);
        }
    }

    public void EndGame()
    {
        Debug.Log("게임 끝");
        IsGamePlay = false;
        int hearts = HeartManager.Instance.CalculateHearts();
        Debug.Log("게임 종료. 얻은 하트: " + hearts + "개");
        SceneManager.LoadScene("EndingScene");
    }
}
