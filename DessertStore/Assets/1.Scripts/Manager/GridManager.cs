using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Progress;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }            // �̱��� �ν��Ͻ�

    [SerializeField] private GameObject _cellPrefab;                    // Cell ������
    [SerializeField] private Vector2 _cellsSize = new Vector2(5, 5);     // �׸��� ũ��

    private Cell[,] _cells;                                             // 2���� �迭�� �׸��� �� ����
    public Cell[,] cells 
    {
        get {return _cells;}
    }

    // ���� ����Ʈ�� ������ ������ ����
    private List<ItemData>[] _itemDataArray;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // �� ��ȯ �� ��ü ����
        }
        else
        {
            Destroy(gameObject); // �̹� �ν��Ͻ��� �����ϸ� ���� ��ü �ı�
        }

        InitializeGrid();
    }

    // �׸��带 �ʱ�ȭ�ϴ� �޼���
    private void InitializeGrid()
    {
        _cells = new Cell[(int)_cellsSize.x, (int)_cellsSize.y];          // _grid �迭 �ʱ�ȭ
        Vector2 cellHalf = new Vector2(0.5f, 0.5f);                     // �� ũ��
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

    // ���� ����Ʈ �迭 �ʱ�ȭ
    private void InitializeItemDataArray()
    {
        _itemDataArray = new List<ItemData>[(int)(_cellsSize.x * _cellsSize.y)]; // �� ������ŭ ����Ʈ �迭 �ʱ�ȭ

        for (int i = 0; i < _itemDataArray.Length; i++)
        {
            _itemDataArray[i] = new List<ItemData>();
        }
    }

    // ���� �����ϴ� �޼���
    private void CreateCell(Vector3 position, int x, int y)
    {
        GameObject clone = Instantiate(_cellPrefab, position, Quaternion.identity, transform);
        Cell cellScript = clone.GetComponent<Cell>();

        if (cellScript != null)
        {
            cellScript.Initialize(this, x, y); // xIndex�� yIndex �ʱ�ȭ
            _cells[x, y] = cellScript;
        }
    }

    public void OnClearButtonClicked()
    {
        ClearAllItems(); // ���� �������� �����ϴ� �޼��带 ȣ��
    }


    // ��� ���� �������� �����ϴ� �޼���
    public void ClearAllItems()
    {
        foreach (Cell cell in _cells)
        {
            if (cell.IsOccupied())
            {
                cell.ClearItems(); // ������ ����
            }
        }
        Debug.Log("��� ���� �������� ���ŵǾ����ϴ�.");
    }



    public void ClearCollidingCells(DragBlock targetItem)
    {
        foreach (Cell cell in _cells) // ��� ���� ��ȸ
        {
            if (!cell.IsOccupied()) continue;
            
            if (cell.GetOccupyingItems().Contains(targetItem)) // Ư�� �����۰� �浹�ϴ� ���̸�
            {
                cell.ClearItems(true); // �ش� ���� ������ ����
                cell.UpdateCellColor();
            }
        }
    }

    // ���õ� ������ �߽� ��ġ�� ����ϴ� �޼���
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

    // �����۰� ���� �浹�� üũ�ϴ� �޼���
    public List<Cell> CheckCellOverlap(BoxCollider2D itemCollider, int width, int height, int itemOrderIndex)
    {
        List<Cell> selectedCells = new List<Cell>();
        Cell selectedCell = null;
        float maxArea = 0.0f;

        foreach (Cell cell in _cells)
        {
            cell.ChangeColor(Color.white); // �ʱ� ���� ����

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

    // �� ���� �ٿ�� ���� ��ġ�� ������ ����ϴ� �޼���
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

    // ���õ� ���� �������� �߰��ϴ� �޼���
    private void AddCellsToSelection(List<Cell> selectedCells, Cell selectedCell, int width, int height, int itemOrderIndex)
    {
        int x = selectedCell.xIndex;
        int y = selectedCell.yIndex;
        int startX = x - (width - 1) / 2; // ������ �ʺ� �ݿ�
        int startY = y + (height - 1) / 2; // ������ ���̸� �ݿ�

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                int cellX = startX + j; // ���� X ��ǥ�� j�� ����
                int cellY = startY + i; // ���� Y ��ǥ���� i�� ��

                if (cellX >= 0 && cellX < _cells.GetLength(0) && cellY >= 0 && cellY < _cells.GetLength(1))
                {
                    Cell cell = _cells[cellX, cellY];

                    if (itemOrderIndex > 0)
                    {
                        cell.ChangeColor(Color.blue);
                        if (!selectedCells.Contains(cell) && cell.IsOccupied() && itemOrderIndex > cell.GetLastOccupyingItemOrderIndex())
                        {
                            cell.ChangeColor(Color.cyan);
                            selectedCells.Add(cell);
                        }
                    }
                    else
                    {
                        // ���� �̹� ���� �������� �ִ��� Ȯ��
                        if (!selectedCells.Contains(cell) && !cell.IsOccupied())
                        {
                            cell.ChangeColor(Color.blue);
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
                var dragBlockListCopy = new List<DragBlock>(dragBlockList); // ���纻 ����

                foreach (var item in dragBlockListCopy) // ���纻�� �ݺ�
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