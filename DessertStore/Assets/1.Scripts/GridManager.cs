using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridManager : MonoBehaviour
{
    [SerializeField] private GameObject _cellPrefab; // Cell prefab
    private Cell[,] grid; // 2���� �迭�� �׸��� �� ����
    [SerializeField] private int _gridSize = 5; // �׸��� ũ��
    [SerializeField] private Vector2 cellSize = new Vector2(100, 100); // �� ũ��

    private void Start()
    {
        grid = new Cell[_gridSize, _gridSize]; // �׸��� �ʱ�ȭ
        CreateGrid();
    }

    private void CreateGrid()
    {
        for (int x = 0; x < _gridSize; x++)
        {
            for (int y = 0; y < _gridSize; y++)
            {
                // �� ���� Instantiate�ϰ� �׸��忡 �߰�
                GameObject cellObj = Instantiate(_cellPrefab, transform);
                cellObj.name = $"Cell_{x}_{y}";

                // Cell ��ũ��Ʈ ��������
                Cell cellScript = cellObj.GetComponent<Cell>();
                if (cellScript != null)
                {
                    cellScript.Initialize(this, x, y); // xIndex�� yIndex�� �ʱ�ȭ
                    grid[x, y] = cellScript; // �׸��忡 �� �߰�
                }

                // ���� RectTransform ����
                RectTransform rectTransform = cellObj.GetComponent<RectTransform>();
                rectTransform.sizeDelta = cellSize; // �� ũ�� ����
                rectTransform.anchoredPosition = new Vector2(x * cellSize.x, y * cellSize.y); // �׸��忡 �°� ��ġ ����
            }
        }
    }

    // Ư�� ��ǥ�� ���� �����Ǿ����� Ȯ���ϴ� �Լ�
    public bool IsCellOccupied(int x, int y)
    {
        if (x >= 0 && x < _gridSize && y >= 0 && y < _gridSize)
        {
            return grid[x, y].IsOccupied();
        }
        return false;
    }

    // �׸��忡�� ���� ����� �� ���� ã���ִ� �Լ�
    public Vector2Int FindClosestGridPosition(Vector2 itemPosition, int itemWidth, int itemHeight)
    {
        int closestX = -1;
        int closestY = -1;
        float minDistance = float.MaxValue;

        // �׸����� ��� ���� ��ȸ�Ͽ� ���� ����� �� ���� ã��
        for (int x = 0; x <= _gridSize - itemWidth; x++) // ������ ���� ����Ͽ� ���� ����
        {
            for (int y = 0; y <= _gridSize - itemHeight; y++) // ������ ���̸� ����Ͽ� ���� ����
            {
                // �������� ������ ���� ������ ���
                bool canPlaceItem = true;
                for (int dx = 0; dx < itemWidth; dx++)
                {
                    for (int dy = 0; dy < itemHeight; dy++)
                    {
                        // ���� �ε��� Ȯ��
                        int checkX = x + dx;
                        int checkY = y + dy;

                        // �� ���� ��ġ�� �� ���� ���
                        if (IsCellOccupied(checkX, checkY))
                        {
                            canPlaceItem = false; // �� ���� ��ġ�� �� ���� ���
                            break;
                        }
                    }
                    if (!canPlaceItem) break;
                }

                // ��ġ ������ ���̶�� �Ÿ� ���
                if (canPlaceItem)
                {
                    // �ش� ���� RectTransform�� �����ͼ� ��ġ�� ���
                    Vector2 gridCellPosition = grid[x, y].GetComponent<RectTransform>().anchoredPosition;

                    // �׸��� ���� ������ ��ġ ���� �Ÿ� ���
                    float distance = Vector2.Distance(itemPosition, gridCellPosition);

                    // ���� ����� ���� ����
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        closestX = x; // �ε��� ����
                        closestY = y; // �ε��� ����
                    }
                }
            }
        }

        Debug.Log($"Closest Cell Index: ({closestX}, {closestY})");

        // ���� ����� ���� �ε��� ��ȯ
        return new Vector2Int(closestX, closestY);
    }



    // Ư�� ��ǥ�� �������� ��ġ�ϴ� �Լ�
    public void PlaceItemAtGridPosition(int x, int y, DragItem item)
    {
        if (x >= 0 && x < _gridSize && y >= 0 && y < _gridSize)
        {
            grid[x, y].SetItem(item); // �ش� ���� ������ ��ġ
        }
    }

    // Ư�� ��ǥ���� �������� �����ϴ� �Լ�
    public void RemoveItemFromGrid(int x, int y)
    {
        if (x >= 0 && x < _gridSize && y >= 0 && y < _gridSize)
        {
            grid[x, y].ClearItem(); // �ش� ������ ������ ����
        }
    }
}
