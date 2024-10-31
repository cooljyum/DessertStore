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
    [SerializeField] private Cell[,] _grid; // 2차원 배열로 그리드 셀 관리

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 필요한 경우 씬 전환 시 객체 유지
        }
        else
        {
            Destroy(gameObject); // 이미 인스턴스가 존재하면 현재 객체 파괴
        }

        _grid = new Cell[(int)_gridSize.x, (int)_gridSize.y];  // _grid 배열 초기화

        for (int y = 0; y < _gridSize.y; ++y) 
        {
            for (int x = 0; x < _gridSize.x; ++x)
            {
                float px = -_gridSize.x * 0.5f + _cellHalf.x + x;
                float py = _gridSize.y * 0.5f + _cellHalf.y - y;

                Vector3 position = new Vector3(px, py, 0);
                GameObject clone = Instantiate(_cellPrefab, position, Quaternion.identity, transform);

                Cell cellScript = clone.GetComponent<Cell>();
                if (cellScript != null)
                {
                    cellScript.Initialize(this, x, y); // xIndex와 yIndex를 초기화
                    _grid[x, y] = cellScript;
                }
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
    }

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


    public List<Cell> CheckCellOverlap(BoxCollider2D itemCollider, int width, int height,bool isPackagingType)
    {
        List<Cell> selectedCells = new List<Cell>();
        float maxArea = 0.0f;
        Cell selectCell = null;

        foreach (Cell cell in _grid)
        {
            if (cell.isActive && !isPackagingType) continue;

            cell.ChangeColor(Color.white);

            BoxCollider2D cellCollider = cell.GetComponent<BoxCollider2D>();
            Bounds boundsA = itemCollider.bounds;
            Bounds boundsB = cellCollider.bounds;

            float xMin = Mathf.Max(boundsA.min.x, boundsB.min.x);
            float xMax = Mathf.Min(boundsA.max.x, boundsB.max.x);
            float yMin = Mathf.Max(boundsA.min.y, boundsB.min.y);
            float yMax = Mathf.Min(boundsA.max.y, boundsB.max.y);

            if (xMin < xMax && yMin < yMax)
            {
                float overlapWidth = xMax - xMin;
                float overlapHeight = yMax - yMin;
                float overlapArea = overlapWidth * overlapHeight;

                if (overlapArea > maxArea)
                {
                    maxArea = overlapArea;
                    selectCell = cell;
                }
            }
        }

        if (selectCell != null)
        {
            int x = selectCell.xIndex;
            int y = selectCell.yIndex;

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    int cellX = x - i;
                    int cellY = y + j;

                    if (cellX >= 0 && cellX < _grid.GetLength(0) && cellY >= 0 && cellY < _grid.GetLength(1))
                    {
                        Cell cell = _grid[cellX, cellY];

                        if (isPackagingType)
                        {
                            cell.ChangeColor(Color.blue);
                            if (!selectedCells.Contains(cell) && cell.isActive && !cell.HasPackagingItem())
                            {
                                cell.ChangeColor(Color.cyan);
                                selectedCells.Add(cell);
                            }
                        }
                        else 
                        {
                            // 셀에 이미 포장 아이템이 있는지 확인
                            if (!selectedCells.Contains(cell) && !cell.isActive)
                            {
                                cell.ChangeColor(Color.blue);
                                selectedCells.Add(cell);
                            }
                        }
                    }
                }
            }
        }
        return selectedCells;
    }

}
