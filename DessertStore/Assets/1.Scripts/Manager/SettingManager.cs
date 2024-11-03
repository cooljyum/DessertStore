using UnityEngine;

public class SettingManager : MonoBehaviour
{

    [SerializeField] private GameObject _settingsBtn; // 씬에 존재하는 세팅 버튼 오브젝트
    [SerializeField] private GameObject _settingsPanel;  // 씬에 존재하는 세팅 패널 오브젝트

    [SerializeField] private GameObject bgmOnObj; // BGM 토글의 On 상태 오브젝트
    [SerializeField] private GameObject bgmOffObj; // BGM 토글의 Off 상태 오브젝트
    [SerializeField] private GameObject effectOnObj; // Effect 토글의 On 상태 오브젝트
    [SerializeField] private GameObject effectOffObj; // Effect 토글의 Off 상태 오브젝트

    private bool isBGMOn = true; // 초기 BGM 상태
    private bool isEffectOn = true; // 초기 Effect 상태

    private void Start()
    {
        InitializeSettingsUI();
    }

    // 설정 UI 초기화 함수
    private void InitializeSettingsUI()
    {
        _settingsBtn.SetActive(true); // 세팅 버튼 활성화
        _settingsPanel.SetActive(false); // 세팅 패널 비활성화

        // SoundManager의 볼륨 상태를 받아와 초기값 설정
        isBGMOn = SoundManager.Instance.GetBGVolume() > 0;
        isEffectOn = SoundManager.Instance.GetFXVolume() > 0;

        UpdateBGMButtonState();
        UpdateEffectButtonState();
    }

    // 세팅 패널을 활성화 및 비활성화하는 함수
    public void ToggleSettingsPanel()
    {
        if (_settingsPanel != null)
        {
            _settingsPanel.SetActive(!_settingsPanel.activeSelf);
        }
        else
        {
            Debug.LogWarning("Settings panel not initialized.");
        }
    }

    // BGM 토글 버튼
    public void ToggleBGM()
    {
        isBGMOn = !isBGMOn;
        SoundManager.Instance.SetBGVolume(isBGMOn ? 1f : 0f);
        UpdateBGMButtonState();
    }

    // Effect 토글 버튼
    public void ToggleEffect()
    {
        isEffectOn = !isEffectOn;
        SoundManager.Instance.SetFXVolume(isEffectOn ? 1f : 0f);
        UpdateEffectButtonState();
    }

    // BGM 버튼 상태 업데이트
    private void UpdateBGMButtonState()
    {
        bgmOnObj.SetActive(isBGMOn);
        bgmOffObj.SetActive(!isBGMOn);
    }

    // Effect 버튼 상태 업데이트
    private void UpdateEffectButtonState()
    {
        effectOnObj.SetActive(isEffectOn);
        effectOffObj.SetActive(!isEffectOn);
    }
}
