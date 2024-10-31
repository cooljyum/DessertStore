using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class GridManager : MonoBehaviour
{
    // �̱��� �ν��Ͻ�
    public static GridManager Instance { get; private set; }

    [SerializeField] private GameObject _cellPrefab; // Cell������
    [SerializeField] private int _cellOrderInLayer; // �׷����� ����

    
    private Vector2 _gridSize = new Vector2(5, 5); // ���� �� ����
    private Vector2 _cellHalf = new Vector2(0.5f, 0.5f); // �� ũ��
    [SerializeField] private Cell[,] _grid; // 2���� �迭�� �׸��� �� ����

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // �ʿ��� ��� �� ��ȯ �� ��ü ����
        }
        else
        {
            Destroy(gameObject); // �̹� �ν��Ͻ��� �����ϸ� ���� ��ü �ı�
        }

        _grid = new Cell[(int)_gridSize.x, (int)_gridSize.y];  // _grid �迭 �ʱ�ȭ

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
                    cellScript.Initialize(this, x, y); // xIndex�� yIndex�� �ʱ�ȭ
                    _grid[x, y] = cellScript;
                }
            }
        }
    }
    private void Start()
    {
       // _grid = new Cell[_gridSize.x, _gridSize.x]; // �׸��� �ʱ�ȭ
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
                            // ���� �̹� ���� �������� �ִ��� Ȯ��
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
