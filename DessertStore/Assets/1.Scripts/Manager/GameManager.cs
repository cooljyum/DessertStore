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

        // HeartManager�� ScoreManager �ν��Ͻ��� ã�ų� ���� �Ҵ�
        _scoreManager = _scoreManager ?? FindObjectOfType<ScoreManager>();
        _heartManager = _heartManager ?? FindObjectOfType<HeartManager>();

        IsGamePlay = true;

        // ������ �׼��� ������ �������� �����ϴ� �ڷ�ƾ ����
        StartCoroutine(WitchActionRoutine());
    }


    // ���� �߰� �� ��Ʈ ������Ʈ
    public void AddScore(int amount)
    {
        _scoreManager.AddScore(amount);
        _heartManager.OnOrderSuccess();
    }

    // ���� ������ �θ��� �Լ�
    public void OnPackagingSuccess()
    {
        _scoreManager.AddPackagingCount(1);
        _heartManager.OnOrderSuccess();
    }

    // ���� ������ ��Ʈ �� ��ȯ
    public int GetCurrentScore() => _scoreManager.CurrentScore;
    public int GetCurrentHearts() => _heartManager.CalculateHearts();

    // �������� ���� �� �ʱ�ȭ
    public void ResetStage()
    {
        _scoreManager.ResetScore();
        _heartManager.ResetHearts();
    }

    // ���� �׼�����
    public void PlayWitchAction(CharacterState state)
    {
        _witchManager.PerformCharacterAction(state);
    }

    //  �׼�����
    public void PlayCatAction(CharacterState state)
    {
        _catManager.PerformCharacterAction(state);
    }

    // ���� �׼��� ������ �������� �����ϴ� �ڷ�ƾ
    private IEnumerator WitchActionRoutine()
    {
        while (true) // ���� ����
        {
            // ������ �ð�(3�� ~ 10��) ��ٸ���
            float waitTime = Random.Range(7f, 10f);
            yield return new WaitForSeconds(waitTime);

            // ������ ĳ���� ���� ���� (���⼭�� Neutral, Happy)
            //CharacterState randomState = (CharacterState)Random.Range(0, 2);
            CharacterState randomState = CharacterState.Basic;
            PlayWitchAction(randomState);
        }
    }

    public void EndGame() 
    {
        Debug.Log("���� ��");
        IsGamePlay= false;

        //HeartManager ����
         int hearts = HeartManager.Instance.CalculateHearts();
        Debug.Log("���� ����. ���� ��Ʈ: " + hearts + "��");
    }
}
