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
        _witchManager.PerformCharacterAction(state);
    }

    //  액션제어
    public void PlayCatAction(CharacterState state)
    {
        _catManager.PerformCharacterAction(state);
    }

    // 마녀 액션을 무작위 간격으로 실행하는 코루틴
    private IEnumerator WitchActionRoutine()
    {
        while (true) // 무한 루프
        {
            // 무작위 시간(3초 ~ 10초) 기다리기
            float waitTime = Random.Range(7f, 10f);
            yield return new WaitForSeconds(waitTime);

            // 무작위 캐릭터 상태 선택 (여기서는 Neutral, Happy)
            //CharacterState randomState = (CharacterState)Random.Range(0, 2);
            CharacterState randomState = CharacterState.Basic;
            PlayWitchAction(randomState);
        }
    }

    public void EndGame() 
    {
        Debug.Log("게임 끝");
        IsGamePlay= false;

        //HeartManager 종료
         int hearts = HeartManager.Instance.CalculateHearts();
        Debug.Log("게임 종료. 얻은 하트: " + hearts + "개");
    }
}
