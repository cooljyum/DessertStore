using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartManager : MonoBehaviour
{
    [SerializeField] private Image _titleImage;
    [SerializeField] private Button _startButton;

    private float _blinkSpeed = 0.5f; // ±ôºýÀÌ´Â ¼Óµµ

    private void Start()
    {
        _startButton.onClick.AddListener(OnStartButtonClicked);
        SoundManager.Instance.PlayBG("StartBGM");
    }

    private void Update()
    {
        BlinkButton();
    }

    private void BlinkButton()
    {
        float alpha = Mathf.PingPong(Time.time * _blinkSpeed, 1f);
        Color buttonColor = _startButton.image.color;
        buttonColor.a = alpha;
        _startButton.image.color = buttonColor;
    }

    public void OnStartButtonClicked()
    {
        SceneManager.LoadScene("StoryScene");
    }    
}
