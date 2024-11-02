using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageManager : MonoBehaviour
{
    [SerializeField] private GameObject _stagePanel;

    private GameObject _stageIconPrefab;
    private Dictionary<int, int> _starPointByStage = new Dictionary<int, int>(); // 스테이지별 별점 저장용
    public Dictionary<int, int> StarPointByStage => _starPointByStage;
    private int _totalStarPoint = 0;                                              // 총 별점
    private int _maxStarPoint;                                                    // 최대 별점(엔딩 조건)

    private void Start()
    {
        //CreateStageIcons();

    }

    // 스테이지 자동 생성 //
    private void CreateStageIcons()
    {
        // 최대 별점 개수 세팅
        _maxStarPoint = Resources.LoadAll<ScriptableObject>("ScriptableObject/Level").Length * 5;

        // 레벨 데이터 개수에 따라 스테이지 생성
        var levelDataObjects = Resources.LoadAll<ScriptableObject>("ScriptableObject/Level");

        foreach (var levelData in levelDataObjects)
        {
            GameObject stageIcon = Instantiate(_stageIconPrefab, _stagePanel.transform);
            stageIcon.name = "Day" + levelData.name.Substring(5);

            Transform stageTxtTransform = stageIcon.transform.GetChild(0);
            TextMeshProUGUI stageTxtTMP = stageTxtTransform.GetComponent<TextMeshProUGUI>();
            stageTxtTMP.text = levelData.name.Substring(5);

            TextMeshProUGUI pointTxtTMP = _stagePanel.GetComponentInChildren<TextMeshProUGUI>();
            pointTxtTMP.text = $"{_totalStarPoint} / {_maxStarPoint}";
        }
    }

    // 현재 총 별점 상황 업데이트
    private void UpdateStarPoint(int points)
    {
        if (!_starPointByStage.ContainsKey(UIManager.Instance.CurrentDayNumber))
        {
            _starPointByStage[UIManager.Instance.CurrentDayNumber] = 0;
        }

        _starPointByStage[UIManager.Instance.CurrentDayNumber] += points;
        _totalStarPoint += points;

        // 엔딩 씬으로 자동 전환 체크
        if (_totalStarPoint >= _maxStarPoint)
        {
            SceneManager.LoadScene("EndingScene");
        }
    }
}
