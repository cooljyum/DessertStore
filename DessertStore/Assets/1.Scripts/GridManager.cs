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
    private GameObject[,] _grid; // 2���� �迭�� �׸��� �� ����

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
       // _grid = new Cell[_gridSize.x, _gridSize.x]; // �׸��� �ʱ�ȭ
      //  CreateGrid();
    }

    private void CreateGrid()
    {
        for (int x = 0; x < _gridSize.x; x++)
        {
            for (int y = 0; y < _gridSize.x; y++)
            {
                // �� ���� Instantiate�ϰ� �׸��忡 �߰�
                GameObject cellObj = Instantiate(_cellPrefab, transform);
                cellObj.name = $"Cell_{x}_{y}";

                // Cell ��ũ��Ʈ ��������
                Cell cellScript = cellObj.GetComponent<Cell>();
                if (cellScript != null)
                {
                    cellScript.Initialize(this, x, y); // xIndex�� yIndex�� �ʱ�ȭ
                 //   _grid[x, y] = cellScript; // �׸��忡 �� �߰�
                }

                // ���� RectTransform ����
                RectTransform rectTransform = cellObj.GetComponent<RectTransform>();
                rectTransform.sizeDelta = _cellHalf; // �� ũ�� ����
                rectTransform.anchoredPosition = new Vector2(x * _cellHalf.x, y * _cellHalf.y); // �׸��忡 �°� ��ġ ����
            }
        }
    }

    // Ư�� ��ǥ�� ���� �����Ǿ����� Ȯ���ϴ� �Լ�
    public bool IsCellOccupied(int x, int y)
    {
        if (x >= 0 && x < _gridSize.x && y >= 0 && y < _gridSize.x)
        {
          //  return _grid[x, y].IsOccupied();
        }
        return false;
    }

    // �׸��忡�� ���� ����� �� ���� ã���ִ� �Լ�
    public Vector2 FindClosestGridPosition(GameObject item, int itemWidth, int itemHeight)
    {
        int closestX = -1;
        int closestY = -1;
        float minDistance = float.MaxValue;
        Vector2 closetGridCellPosition = new Vector2(0, 0);

        Vector3 itemWorldPosition;

        // �������� UI ���(RectTransform)�� ����ϴ��� Ȯ��
        RectTransform itemRectTransform = item.GetComponent<RectTransform>();
        itemWorldPosition = item.transform.position;


        // �׸����� ��� ���� ��ȸ�Ͽ� ���� ����� �� ���� ã��
        for (int x = 0; x < _gridSize.x; x++) // ������ ���� ����Ͽ� ���� ����
        {
            for (int y = 0; y < _gridSize.x; y++) // ������ ���̸� ����Ͽ� ���� ����
            {

                // �׸��� ���� RectTransform�� �����ͼ� ��ġ�� ���
                RectTransform rectTransform = _grid[x, y].GetComponent<RectTransform>();

                // anchoredPosition�� ���� ��ǥ�� ��ȯ
                Vector3 gridCellWorldPosition = rectTransform.TransformPoint(rectTransform.anchoredPosition);

                // �׸��� ���� �������� ���� ��ǥ ���� �Ÿ� ���
                float distance = Vector2.Distance(itemWorldPosition, gridCellWorldPosition);

                // ���� ����� ���� ����
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestX = x; // �ε��� ����
                    closestY = y; // �ε��� ����
                    closetGridCellPosition = gridCellWorldPosition;
                }
            }
        }

        Debug.Log($"Closest Cell Index: ({closestX}, {closestY})");

        Vector2 gridCellCenterOffset = new Vector2(-1*itemWidth*100 / 2.0f, itemHeight*100 / 2.0f);        
        
        // ���� ����� ���� �ε��� ��ȯ
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

            // ��ġ�� ������ Rect ���
            float xMin = Mathf.Max(boundsA.min.x, boundsB.min.x);
            float xMax = Mathf.Min(boundsA.max.x, boundsB.max.x);
            float yMin = Mathf.Max(boundsA.min.y, boundsB.min.y);
            float yMax = Mathf.Min(boundsA.max.y, boundsB.max.y);

            // ��ġ�� �κ��� �ִ��� Ȯ��
            if (xMin < xMax && yMin < yMax)
            {
                // ��ġ�� ������ ũ��� ���� ���
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

    // Ư�� ��ǥ�� �������� ��ġ�ϴ� �Լ�
    public void PlaceItemAtGridPosition(int x, int y, DragItem item)
    {
        if (x >= 0 && x < _gridSize && y >= 0 && y < _gridSize)
        {
            _grid[x, y].SetItem(item); // �ش� ���� ������ ��ġ
        }
    }

    // Ư�� ��ǥ���� �������� �����ϴ� �Լ�
    public void RemoveItemFromGrid(int x, int y)
    {
        if (x >= 0 && x < _gridSize && y >= 0 && y < _gridSize)
        {
            _grid[x, y].ClearItem(); // �ش� ������ ������ ����
        }
    }*/
}
