using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI; // UI 네임스페이스 추가

public class Cell : MonoBehaviour
{
    [SerializeField] private int _xIndex;
    public int xIndex { get { return _xIndex; } }

    [SerializeField] private int _yIndex;
    public int yIndex { get { return _yIndex; } }

    private List<DragBlock> _occupyingItems = new List<DragBlock>(); // 해당 셀을 차지하는 아이템 리스트

    private SpriteRenderer _cellImg; // Image 컴포넌트 추가

    private void Start()
    {
        // 셀의 Image 컴포넌트를 가져옵니다.
        _cellImg = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        // 마우스 오른쪽 버튼 클릭 감지
        if (Input.GetMouseButtonDown(1))
        {
            if (!IsOccupied()) return;
            // 마우스 위치에서 Raycast를 쏴서 클릭된 셀을 찾습니다.
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null && hit.collider.gameObject == gameObject) // 자신이 클릭되었는지 확인
            {
                ClearItems(); // 아이템을 제거하는 메서드 호출
                Debug.Log("셀의 아이템이 제거되었습니다. 위치: (" + _xIndex + ", " + _yIndex + ")");
            }
        }
    }

    public List<ItemData> GetItemDataList() 
    {
        List<ItemData> itemDatas = new List<ItemData>();

        DragBlock data = new DragBlock();
        foreach (var item in _occupyingItems)
        {
            itemDatas.Add(item.ItemData);
            if (item.ItemData.itemType > 0) data = item;
        }

        GridManager.Instance.ClearCollidingCells(data);

        return itemDatas;
    }

    public void Initialize(GridManager manager, int x, int y)
    {
        _xIndex = x;
        _yIndex = y;
    }

    // 셀에 아이템을 추가
    public void AddOccupyingItem(DragBlock item)
    {
        // 아이템이 이미 리스트에 존재하는지 확인
        if (!_occupyingItems.Contains(item))
        {
            _occupyingItems.Add(item); // 리스트에 아이템 추가
            UpdateCellColor(); // 셀 색상 업데이트
        }
        else
        {
            Debug.LogWarning("아이템이 이미 셀에 존재합니다."); // 이미 존재하는 경우 경고 메시지
        }
    }

    // 셀이 아이템을 차지했는지 확인
    public bool IsOccupied()
    {
        return _occupyingItems.Count > 0; // 리스트에 아이템이 있으면 occupied
    }
    // 셀의 아이템들을 파괴
    public void ClearItems(bool isCheck = false)
    {
        foreach (Cell cell in _occupyingItems[0].SelectedCells)
        {
            if (cell.IsOccupied()) // 셀이 아이템을 가지고 있는 경우에만
            {
                cell.ClearOccupyingItems(); // 셀의 아이템들을 파괴하는 메서드 호출
            }
        }

        ClearOccupyingItems();
    }
    public void ClearOccupyingItems() 
    {
        foreach (var item in _occupyingItems)
        {
            if (item != null)
            {
                Destroy(item.gameObject); // 아이템 파괴
            }
        }
        _occupyingItems.Clear(); // 리스트 초기화

        UpdateCellColor();
        Debug.Log("셀의 아이템이 제거되었습니다. 위치: (" + _xIndex + ", " + _yIndex + ")");

    }

    public void UpdateCellColor()
    {
/*        if (_occupyingItems.Count > 0)
        {
            _cellImg.color = Color.green; // 아이템이 있을 때 초록색으로 변경
        }
        else
        {
            _cellImg.color = Color.white; // 비어 있으면 흰색으로 변경
        }*/
    }

    public List<DragBlock> GetOccupyingItems()
    {
        return _occupyingItems;
    }

    // 셀의 마지막 아이템의 orderIndex를 반환
    public int GetLastOccupyingItemOrderIndex()
    {
        if (_occupyingItems.Count > 0)
        {
            return _occupyingItems[_occupyingItems.Count - 1].ItemData.orderIndex; // 마지막 아이템의 orderIndex 반환
        }
        return -1; // 아이템이 없으면 -1 반환 (또는 다른 기본값)
    }

    public void ChangeColor(Color color)
    {
        // 셀의 색깔을 변경
        if (_cellImg != null)
        {
            _cellImg.color = color;
        }
    }
}
