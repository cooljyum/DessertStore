using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class GridManager : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static GridManager Instance { get; private set; }

    [SerializeField] private GameObject _cellPrefab; // Cell프리팹
    [SerializeField] private int _cellOrderInLayer; // 그려지는 순서

    
    private Vector2 _gridSize = new Vector2(5, 5); // 생성 블럭 개수
    private Vector2 _cellHalf = new Vector2(0.5f, 0.5f); // 셀 크기
    private GameObject[,] _grid; // 2차원 배열로 그리드 셀 관리

    private void Awake()
    {
        for (int y = 0; y < _gridSize.y; ++y) 
        {
            for (int x = 0; x < _gridSize.x; ++x)
            {
                float px = -_gridSize.x * 0.5f + _cellHalf.x + x;
                float py = _gridSize.y * 0.5f + _cellHalf.y - y;

                Vector3 position = new Vector3(px, py, 0);
                GameObject clone = Instantiate(_cellPrefab, position, Quaternion.identity, transform);
                clone.GetComponent<SpriteRenderer>().sortingOrder = _cellOrderInLayer;
                _grid[x, y] = clone; 
            }
        }
    }
    private void Start()
    {
       // _grid = new Cell[_gridSize.x, _gridSize.x]; // 그리드 초기화
      //  CreateGrid();
    }

    private void CreateGrid()
    {
        for (int x = 0; x < _gridSize.x; x++)
        {
            for (int y = 0; y < _gridSize.x; y++)
            {
                // 각 셀을 Instantiate하고 그리드에 추가
                GameObject cellObj = Instantiate(_cellPrefab, transform);
                cellObj.name = $"Cell_{x}_{y}";

                // Cell 스크립트 가져오기
                Cell cellScript = cellObj.GetComponent<Cell>();
                if (cellScript != null)
                {
                    cellScript.Initialize(this, x, y); // xIndex와 yIndex를 초기화
                 //   _grid[x, y] = cellScript; // 그리드에 셀 추가
                }

                // 셀의 RectTransform 설정
                RectTransform rectTransform = cellObj.GetComponent<RectTransform>();
                rectTransform.sizeDelta = _cellHalf; // 셀 크기 설정
                rectTransform.anchoredPosition = new Vector2(x * _cellHalf.x, y * _cellHalf.y); // 그리드에 맞게 위치 설정
            }
        }
    }

    // 특정 좌표의 셀이 차지되었는지 확인하는 함수
    public bool IsCellOccupied(int x, int y)
    {
        if (x >= 0 && x < _gridSize.x && y >= 0 && y < _gridSize.x)
        {
          //  return _grid[x, y].IsOccupied();
        }
        return false;
    }

    // 그리드에서 가장 가까운 빈 셀을 찾아주는 함수
    public Vector2 FindClosestGridPosition(GameObject item, int itemWidth, int itemHeight)
    {
        int closestX = -1;
        int closestY = -1;
        float minDistance = float.MaxValue;
        Vector2 closetGridCellPosition = new Vector2(0, 0);

        Vector3 itemWorldPosition;

        // 아이템이 UI 요소(RectTransform)를 사용하는지 확인
        RectTransform itemRectTransform = item.GetComponent<RectTransform>();
        itemWorldPosition = item.transform.position;


        // 그리드의 모든 셀을 순회하여 가장 가까운 빈 셀을 찾음
        for (int x = 0; x < _gridSize.x; x++) // 아이템 폭을 고려하여 범위 제한
        {
            for (int y = 0; y < _gridSize.x; y++) // 아이템 높이를 고려하여 범위 제한
            {

                // 그리드 셀의 RectTransform을 가져와서 위치를 계산
                RectTransform rectTransform = _grid[x, y].GetComponent<RectTransform>();

                // anchoredPosition을 월드 좌표로 변환
                Vector3 gridCellWorldPosition = rectTransform.TransformPoint(rectTransform.anchoredPosition);

                // 그리드 셀과 아이템의 월드 좌표 간의 거리 계산
                float distance = Vector2.Distance(itemWorldPosition, gridCellWorldPosition);

                // 가장 가까운 셀을 저장
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestX = x; // 인덱스 저장
                    closestY = y; // 인덱스 저장
                    closetGridCellPosition = gridCellWorldPosition;
                }
            }
        }

        Debug.Log($"Closest Cell Index: ({closestX}, {closestY})");

        Vector2 gridCellCenterOffset = new Vector2(-1*itemWidth*100 / 2.0f, itemHeight*100 / 2.0f);        
        
        // 가장 가까운 셀의 인덱스 반환
        return closetGridCellPosition + gridCellCenterOffset;
    }

    public void CheckCellOverlap(BoxCollider2D itemCollider, int width, int height)
    {
        float maxArea = 0.0f;
        GameObject selectSell = null;

        foreach(GameObject cell in _grid)
        {
            cell.ChangeColor(Color.white);

            BoxCollider2D cellCollider = cell.GetComponent<BoxCollider2D>();

            Bounds boundsA = itemCollider.bounds;
            Bounds boundsB = cellCollider.bounds;

            // 겹치는 영역의 Rect 계산
            float xMin = Mathf.Max(boundsA.min.x, boundsB.min.x);
            float xMax = Mathf.Min(boundsA.max.x, boundsB.max.x);
            float yMin = Mathf.Max(boundsA.min.y, boundsB.min.y);
            float yMax = Mathf.Min(boundsA.max.y, boundsB.max.y);

            // 겹치는 부분이 있는지 확인
            if (xMin < xMax && yMin < yMax)
            {
                // 겹치는 영역의 크기와 넓이 계산
                float overlapWidth = xMax - xMin;
                float overlapHeight = yMax - yMin;
                float overlapArea = overlapWidth * overlapHeight;

                if (overlapArea > maxArea)
                {
                    maxArea = overlapArea;
                    selectSell = cell;
                }
            }            
        }

        int x = selectSell.xIndex;
        int y = selectSell.yIndex;
        //selectSell.ChangeColor(Color.blue);
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                Cell cell = _grid[x - i, y + j];
                cell.ChangeColor(Color.blue);
            }
        }
    }

/*

    // 특정 좌표에 아이템을 배치하는 함수
    public void PlaceItemAtGridPosition(int x, int y, DragItem item)
    {
        if (x >= 0 && x < _gridSize && y >= 0 && y < _gridSize)
        {
            _grid[x, y].SetItem(item); // 해당 셀에 아이템 배치
        }
    }

    // 특정 좌표에서 아이템을 제거하는 함수
    public void RemoveItemFromGrid(int x, int y)
    {
        if (x >= 0 && x < _gridSize && y >= 0 && y < _gridSize)
        {
            _grid[x, y].ClearItem(); // 해당 셀에서 아이템 제거
        }
    }*/
}
