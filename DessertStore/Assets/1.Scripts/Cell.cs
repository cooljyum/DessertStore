using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // UI 네임스페이스 추가

public class Cell : MonoBehaviour
{
    [SerializeField] private int _xIndex;
    [SerializeField] private int _yIndex;
    [SerializeField] private bool _isActive;
    private GridManager _gridManager;
    private Image _cellImg; // Image 컴포넌트 추가

    private void Start()
    {
        // 셀의 Image 컴포넌트를 가져옵니다.
        _cellImg = GetComponent<Image>();
    }

    public void Initialize(GridManager manager, int x, int y)
    {
        _gridManager = manager;
        _xIndex = x;
        _yIndex = y;
    }

    public void ClickCell()
    {
        PrintIndex(); // 인덱스 출력
        ChangeColor(); // 색깔 변경
    }

    private void PrintIndex()
    {
        Debug.Log($"xIndex: {_xIndex}, zIndex: {_yIndex}");
    }

    private void ChangeColor()
    {
        _isActive = true;

        // 셀의 색깔을 랜덤으로 변경
        if (_cellImg != null)
        {
            _cellImg.color = Color.yellow; // 노란색으로 설정
        }
    }
}
