using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

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

        //if (itemRectTransform != null)
        //{
        //    // UI�� RectTransform�� ���� ��ǥ�� ��ȯ
        //    itemWorldPosition = itemRectTransform.TransformPoint(itemRectTransform.anchoredPosition);
        //}
        //else
        //{
        //    // UI ��Ұ� �ƴϸ� Transform�� ���� ��ǥ�� �״�� ���
        //    itemWorldPosition = item.transform.position;
        //}

        // �׸����� ��� ���� ��ȸ�Ͽ� ���� ����� �� ���� ã��
        for (int x = 0; x < _gridSize; x++) // ������ ���� ����Ͽ� ���� ����
        {
            for (int y = 0; y < _gridSize; y++) // ������ ���̸� ����Ͽ� ���� ����
            {
                //// �������� ������ ���� ������ ���
                //bool canPlaceItem = true;
                //for (int dx = 0; dx < itemWidth; dx++)
                //{
                //    for (int dy = 0; dy < itemHeight; dy++)
                //    {
                //        int checkX = x + dx;
                //        int checkY = y + dy;

                //        if (IsCellOccupied(checkX, checkY))
                //        {
                //            canPlaceItem = false;
                //            break;
                //        }
                //    }
                //    if (!canPlaceItem) break;
                //}

                // ��ġ ������ ���̸� �Ÿ� ���
                if (true)
                {
                    // �׸��� ���� RectTransform�� �����ͼ� ��ġ�� ���
                    RectTransform rectTransform = grid[x, y].GetComponent<RectTransform>();

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
        }

        Debug.Log($"Closest Cell Index: ({closestX}, {closestY})");

        Vector2 gridCellCenterOffset = new Vector2(-1*itemWidth*100 / 2.0f, itemHeight*100 / 2.0f);

        // ���� ����� ���� �ε��� ��ȯ
        return closetGridCellPosition + gridCellCenterOffset;
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
