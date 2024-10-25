using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class GridManager : MonoBehaviour
{
    [SerializeField] private GameObject _cellPrefab; // Cell prefab
    private Cell[,] grid; // 2차원 배열로 그리드 셀 관리
    [SerializeField] private int _gridSize = 5; // 그리드 크기
    [SerializeField] private Vector2 cellSize = new Vector2(100, 100); // 셀 크기

    private void Start()
    {
        grid = new Cell[_gridSize, _gridSize]; // 그리드 초기화
        CreateGrid();
    }

    private void CreateGrid()
    {
        for (int x = 0; x < _gridSize; x++)
        {
            for (int y = 0; y < _gridSize; y++)
            {
                // 각 셀을 Instantiate하고 그리드에 추가
                GameObject cellObj = Instantiate(_cellPrefab, transform);
                cellObj.name = $"Cell_{x}_{y}";

                // Cell 스크립트 가져오기
                Cell cellScript = cellObj.GetComponent<Cell>();
                if (cellScript != null)
                {
                    cellScript.Initialize(this, x, y); // xIndex와 yIndex를 초기화
                    grid[x, y] = cellScript; // 그리드에 셀 추가
                }

                // 셀의 RectTransform 설정
                RectTransform rectTransform = cellObj.GetComponent<RectTransform>();
                rectTransform.sizeDelta = cellSize; // 셀 크기 설정
                rectTransform.anchoredPosition = new Vector2(x * cellSize.x, y * cellSize.y); // 그리드에 맞게 위치 설정
            }
        }
    }

    // 특정 좌표의 셀이 차지되었는지 확인하는 함수
    public bool IsCellOccupied(int x, int y)
    {
        if (x >= 0 && x < _gridSize && y >= 0 && y < _gridSize)
        {
            return grid[x, y].IsOccupied();
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

        //if (itemRectTransform != null)
        //{
        //    // UI의 RectTransform을 월드 좌표로 변환
        //    itemWorldPosition = itemRectTransform.TransformPoint(itemRectTransform.anchoredPosition);
        //}
        //else
        //{
        //    // UI 요소가 아니면 Transform의 월드 좌표를 그대로 사용
        //    itemWorldPosition = item.transform.position;
        //}

        // 그리드의 모든 셀을 순회하여 가장 가까운 빈 셀을 찾음
        for (int x = 0; x < _gridSize; x++) // 아이템 폭을 고려하여 범위 제한
        {
            for (int y = 0; y < _gridSize; y++) // 아이템 높이를 고려하여 범위 제한
            {
                //// 아이템이 차지할 셀의 범위를 계산
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

                // 배치 가능한 셀이면 거리 계산
                if (true)
                {
                    // 그리드 셀의 RectTransform을 가져와서 위치를 계산
                    RectTransform rectTransform = grid[x, y].GetComponent<RectTransform>();

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
        }

        Debug.Log($"Closest Cell Index: ({closestX}, {closestY})");

        Vector2 gridCellCenterOffset = new Vector2(-1*itemWidth*100 / 2.0f, itemHeight*100 / 2.0f);

        // 가장 가까운 셀의 인덱스 반환
        return closetGridCellPosition + gridCellCenterOffset;
    }





    // 특정 좌표에 아이템을 배치하는 함수
    public void PlaceItemAtGridPosition(int x, int y, DragItem item)
    {
        if (x >= 0 && x < _gridSize && y >= 0 && y < _gridSize)
        {
            grid[x, y].SetItem(item); // 해당 셀에 아이템 배치
        }
    }

    // 특정 좌표에서 아이템을 제거하는 함수
    public void RemoveItemFromGrid(int x, int y)
    {
        if (x >= 0 && x < _gridSize && y >= 0 && y < _gridSize)
        {
            grid[x, y].ClearItem(); // 해당 셀에서 아이템 제거
        }
    }
}
