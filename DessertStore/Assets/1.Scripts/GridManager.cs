using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridManager : MonoBehaviour
{
    [SerializeField] private GameObject _cellPrefab; // Cell prefab
    [SerializeField] private bool[,] grid; // 5x5 grid
    [SerializeField] private int _gridSize = 5;

    private void Start()
    {
        grid = new bool[_gridSize, _gridSize];
        CreateGrid();
    }

    private void CreateGrid()
    {
        for (int x = 0; x < _gridSize; x++)
        {
            for (int z = 0; z < _gridSize; z++)
            {
                GameObject cell = Instantiate(_cellPrefab, transform); // 부모를 GridManager로 설정
                cell.name = $"Cell_{x}_{z}";

                // Cell 스크립트 가져오기
                Cell cellScript = cell.GetComponent<Cell>();
                if (cellScript != null)
                {
                    cellScript.Initialize(this, x, z); // xIndex와 zIndex를 초기화
                }

                // 버튼의 RectTransform을 조정하려면 아래 코드 추가
                RectTransform rectTransform = cell.GetComponent<RectTransform>();
                rectTransform.sizeDelta = new Vector2(100, 100); // 원하는 크기로 조정
            }
        }
    }
}
