using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // UI 네임스페이스 추가

public class Cell : MonoBehaviour
{
    [SerializeField] private int _xIndex;
    [SerializeField] private int _yIndex;
    [SerializeField] private bool _isActive;

    [SerializeField] private DragItem _occupyingItem; // 해당 셀을 차지하는 아이템 (없으면 null)
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

    // 셀이 아이템을 차지했는지 확인
    public bool IsOccupied()
    {
        return _occupyingItem != null;
    }

    // 셀에 아이템 배치
    public void SetItem(DragItem item)
    {
        _occupyingItem = item;
        _isActive = true;

        // 셀이 차지된 상태이므로 색상 변경
        if (_cellImg != null)
        {
            _cellImg.color = Color.yellow; // 아이템이 배치되면 노란색으로 변경
        }
    }

    // 셀에서 아이템 제거
    public void ClearItem()
    {
        _occupyingItem = null;
        _isActive = false;

        // 셀 색상을 원래대로 변경 (비어 있는 상태)
        if (_cellImg != null)
        {
            _cellImg.color = Color.white;
        }
    }

    public DragItem GetOccupyingItem()
    {
        return _occupyingItem;
    }

    // 셀을 클릭할 때 호출
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

        // 셀의 색깔을 변경
        if (_cellImg != null)
        {
            _cellImg.color = Color.yellow; // 노란색으로 설정
        }
    }
}
