using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // UI 네임스페이스 추가

public class Cell : MonoBehaviour
{
    [SerializeField] private int _xIndex;
    public int xIndex
    { get { return _xIndex; } }

    [SerializeField] private int _yIndex;
    public int yIndex
    { get { return _yIndex; } }

    [SerializeField] private bool _isActive;
    public bool isActive
    { get { return _isActive; } }

    [SerializeField] private DragBlock _occupyingItem; // 해당 셀을 차지하는 아이템 (없으면 null)

    private SpriteRenderer _cellImg; // Image 컴포넌트 추가

    // 포장 아이템이 차지했는지 여부
    [SerializeField] private bool _isOccupiedByPackaging;

    private void Start()
    {
        // 셀의 Image 컴포넌트를 가져옵니다.
        _cellImg = GetComponent<SpriteRenderer>();
    }

    public void Initialize(GridManager manager, int x, int y)
    {
        _xIndex = x;
        _yIndex = y;
    }

    // 셀이 아이템을 차지했는지 확인
    public bool IsOccupied()
    {
        return _occupyingItem != null;
    }

    public bool CanPlacePackaging()
    {
        // 포장 아이템이 차지하지 않는 경우에만 True 반환
        return !_isOccupiedByPackaging;
    }

    public void SetOccupyingItem(DragBlock item)
    {
       // _occupyingItem = item;
        _isActive = item != null;

        if (item != null && item.ItemData.itemType == 0) // 포장 아이템인 경우
        {
            _isOccupiedByPackaging = true; // 포장 아이템이 차지함
            _cellImg.color = Color.green; // 아이템이 배치되면 초록색 변경
        }
        else
        {
            _isOccupiedByPackaging = false; // 비포장 아이템은 차지하지 않음
            _cellImg.color = Color.yellow; // 아이템이 배치되면 노란색으로 변경
        }

        
    }

    // 셀에 아이템 배치
    public void SetItem(DragBlock item)
    {
        SetOccupyingItem(item);
    }

    // 셀에서 아이템 제거
    public void ClearItem()
    {
        if (_occupyingItem != null && _occupyingItem.ItemData.itemType == 0) // 포장 아이템이 제거될 경우
        {
            _isOccupiedByPackaging = false; // 포장 아이템 점유 해제
        }

        _occupyingItem = null;
        _isActive = false;

        // 셀 색상을 원래대로 변경 (비어 있는 상태)
        if (_cellImg != null)
        {
            _cellImg.color = Color.white;
        }
    }

    public DragBlock GetOccupyingItem()
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
        Debug.Log($"xIndex: {_xIndex}, yIndex: {_yIndex}");
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

    public void ChangeColor(Color color)
    {
        _cellImg.color = color;
    }

    public bool HasPackagingItem()
    {
        return _isOccupiedByPackaging;
    }
}
