using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainManager : MonoBehaviour
{
    public static MainManager Instance;

    [SerializeField] private Button _deliveryBtn;
    [SerializeField] private Text scoreText;                // 호감도 표시용 텍스트 UI

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        SoundManager.Instance.PlayBG("MainBGM");
        _deliveryBtn.onClick.AddListener(OnStartButtonClicked);

    }

    public void OnStartButtonClicked()
    {
        if (ScoreManager.Instance.PackagingCount > 0)
        {
            SceneManager.LoadScene("DeliveryScene");
        }
        else
        {
            GameManager.Instance.PlayWitchAction(CharacterState.Fail);
        }
    }
    public void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text =  HeartManager.Instance.GetCurScore();
        }
    }


}
