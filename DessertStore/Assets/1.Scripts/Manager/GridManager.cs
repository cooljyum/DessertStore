using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Progress;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }            // 싱글톤 인스턴스

    [SerializeField] private GameObject _cellPrefab;                    // Cell 프리팹
    [SerializeField] private Vector2 _cellsSize = new Vector2(5, 5);     // 그리드 크기

    private Cell[,] _cells;                                             // 2차원 배열로 그리드 셀 관리
    public Cell[,] cells 
    {
        get {return _cells;}
    }

    // 이중 리스트로 아이템 데이터 저장
    private List<ItemData>[] _itemDataArray;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // 이미 인스턴스가 존재하면 현재 객체 파괴
        }

        InitializeGrid();
    }

    // 그리드를 초기화하는 메서드
    private void InitializeGrid()
    {
        _cells = new Cell[(int)_cellsSize.x, (int)_cellsSize.y];          // _grid 배열 초기화
        Vector2 cellHalf = new Vector2(0.5f, 0.5f);                       // 셀 크기
        Vector2 cellOffset = new Vector2(.38f, -0.1f);
        for (int y = 0; y < _cellsSize.y; ++y)
        {
            for (int x = 0; x < _cellsSize.x; ++x)
            {
                Vector3 position = new Vector3(-_cellsSize.x * 0.5f + cellHalf.x + x + cellOffset.x,
                                                _cellsSize.y * 0.5f + cellHalf.y - y + cellOffset.y, 0);
                CreateCell(position, x, y);
            }
        }
    }

    // 단일 리스트 배열 초기화
    private void InitializeItemDataArray()
    {
        _itemDataArray = new List<ItemData>[(int)(_cellsSize.x * _cellsSize.y)]; // 셀 개수만큼 리스트 배열 초기화

        for (int i = 0; i < _itemDataArray.Length; i++)
        {
            _itemDataArray[i] = new List<ItemData>();
        }
    }

    // 셀을 생성하는 메서드
    private void CreateCell(Vector3 position, int x, int y)
    {
        // 오프셋 추가
        Vector3 cellPosition = position + new Vector3(.36f, -0.26f, 0f);

        GameObject clone = Instantiate(_cellPrefab, cellPosition, Quaternion.identity, transform);
        Cell cellScript = clone.GetComponent<Cell>();

        if (cellScript != null)
        {
            cellScript.Initialize(this, x, y); // xIndex와 yIndex 초기화
            _cells[x, y] = cellScript;
        }
    }

    public void OnClearButtonClicked()
    {
        ClearAllItems(); // 실제 아이템을 제거하는 메서드를 호출
    }


    // 모든 셀의 아이템을 제거하는 메서드
    public void ClearAllItems()
    {
        foreach (Cell cell in _cells)
        {
            if (cell.IsOccupied())
            {
                cell.ClearItems(); // 아이템 제거
            }
        }
        Debug.Log("모든 셀의 아이템이 제거되었습니다.");
    }



    public void ClearCollidingCells(DragBlock targetItem)
    {
        foreach (Cell cell in _cells) // 모든 셀을 순회
        {
            if (!cell.IsOccupied()) continue;
            
            if (cell.GetOccupyingItems().Contains(targetItem)) // 특정 아이템과 충돌하는 셀이면
            {
                cell.ClearItems(true); // 해당 셀의 아이템 제거
                cell.UpdateCellColor();
            }
        }
    }

    // 선택된 셀들의 중심 위치를 계산하는 메서드
    public Vector2 GetCellsCenterPosition(List<Cell> selectedCells)
    {
        if (selectedCells.Count == 0) return Vector2.zero;

        Vector2 centerPosition = Vector2.zero;
        foreach (Cell cell in selectedCells)
        {
            centerPosition += (Vector2)cell.transform.position;
        }

        centerPosition /= selectedCells.Count;
        return centerPosition;
    }

    // 아이템과 셀의 충돌을 체크하는 메서드
    public List<Cell> CheckCellOverlap(BoxCollider2D itemCollider, int width, int height, int itemOrderIndex)
    {
        List<Cell> selectedCells = new List<Cell>();
        Cell selectedCell = null;
        float maxArea = 0.0f;

        foreach (Cell cell in _cells)
        {
            BoxCollider2D cellCollider = cell.GetComponent<BoxCollider2D>();
            float overlapArea = GetOverlapArea(itemCollider.bounds, cellCollider.bounds, out float overlapWidth, out float overlapHeight);

            if (overlapArea > maxArea)
            {
                maxArea = overlapArea;
                selectedCell = cell;
            }
        }

        if (selectedCell != null)
        {
            AddCellsToSelection(selectedCells, selectedCell, width, height, itemOrderIndex);
        }

        return selectedCells;
    }

    // 두 개의 바운드 간의 겹치는 영역을 계산하는 메서드
    private float GetOverlapArea(Bounds boundsA, Bounds boundsB, out float overlapWidth, out float overlapHeight)
    {
        float xMin = Mathf.Max(boundsA.min.x, boundsB.min.x);
        float xMax = Mathf.Min(boundsA.max.x, boundsB.max.x);
        float yMin = Mathf.Max(boundsA.min.y, boundsB.min.y);
        float yMax = Mathf.Min(boundsA.max.y, boundsB.max.y);

        overlapWidth = xMax - xMin;
        overlapHeight = yMax - yMin;

        return overlapWidth > 0 && overlapHeight > 0 ? overlapWidth * overlapHeight : 0;
    }

    // 선택된 셀에 아이템을 추가하는 메서드
    private void AddCellsToSelection(List<Cell> selectedCells, Cell selectedCell, int width, int height, int itemOrderIndex)
    {
        int x = selectedCell.xIndex;
        int y = selectedCell.yIndex;
        

        int startX = 0;
        int startY = 0;

        // 아이템 크기에 따라 시작 좌표 계산
        //  x : + -> 오른 /- -> 왼 //y : + -> 아래 /- -> 위 
        switch (width)
        {
            case 1: // 1x1, 1x2, 1x3
                startX = x;
                startY = y + (height > 1 ? 1 : 0); // 높이에 맞게 아래쪽으로 이동;

                if (y == 0 && height > 2)
                {
                    startY += 1;
                }

                if (y == 4 && height >= 2)
                {
                    startY -= 1;
                }

                break;

            case 2: // 2x1, 2x2
                startX = x - 1; // 2칸일 때 왼쪽으로 1칸 이동
                startY = y + (height > 1 ? 1 : 0); // 높이에 맞게  위쪽으로 이동

                if (x == 0)
                {
                    startX += 1;
                }

                if (y == 4 && height > 1)
                {
                    startY -= 1;
                }

                break;

            case 3: // 3x1, 3x3
                startX = x - 1; // 3칸일 때 왼쪽으로 1칸 이동
                startY = y + (height == 1 ? 0 : (height == 2 ? 1 : 2)); // 높이에 맞게 위쪽으로 이동
                
                if (x == 0)
                {
                    startX += 1;
                }

                if (x == 4)
                {
                    startX -= 1;
                }

                if (y >= 3 && height > 1)
                {
                    startY -= (y-2);
                }
                break;

            case 4: // 4x4
                startX = x - 2; // 4칸일 때 왼쪽으로 2칸 이동
                startY = y + 2; // 높이에 맞게 위쪽으로 이동

                if (x == 0)
                {
                    startX += 2;
                }
                if (x == 1)
                {
                    startX += 1;
                }

                if (x == 4)
                {
                    startX -= 1;
                }

                if (y == 0)
                {
                    startY += 1;
                }

                if (y >= 3 && height > 1)
                {
                    startY -= (y - 2);
                }

                break;

            default:
                startX = x - (width - 1) / 2; // 기본적인 경우 (가운데 정렬)
                startY = y - (height - 1) / 2; // 기본적인 경우 (가운데 정렬)
                break;
        }

        // 선택할 셀 좌표
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                int cellX = startX + j; // 시작 X 좌표에 j를 더함
                int cellY = startY - i; // 시작 Y 좌표에서 i를 뺌 (Y축 반전)

                if (cellX >= 0 && cellX < _cells.GetLength(0) && cellY >= 0 && cellY < _cells.GetLength(1))
                {
                    Cell cell = _cells[cellX, cellY];

                    if (itemOrderIndex > 0)
                    {
                        if (!selectedCells.Contains(cell) && cell.IsOccupied() && itemOrderIndex > cell.GetLastOccupyingItemOrderIndex())
                        {
                            selectedCells.Add(cell);
                        }
                    }
                    else
                    {
                        // 셀에 이미 포장 아이템이 있는지 확인
                        if (!selectedCells.Contains(cell) && !cell.IsOccupied())
                        {
                            selectedCells.Add(cell);
                        }
                    }
                }
            }
        }
    }




    public List<ItemData>[] GetCellItemData()
    {
        InitializeItemDataArray();
        int itemCount = 0;

        foreach (Cell cell in _cells)
        {
            if (cell.IsOccupied())
            {
                List<DragBlock> dragBlockList = cell.GetOccupyingItems();
                var dragBlockListCopy = new List<DragBlock>(dragBlockList); // 복사본 생성

                foreach (var item in dragBlockListCopy) // 복사본을 반복
                {
                    if (itemCount < _itemDataArray.Length)
                    {
                        _itemDataArray[itemCount].Add(item.ItemData);
                        itemCount++; 
                        ClearCollidingCells(item);
                    }
                }
            }
        }

        return _itemDataArray;
    }

}