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
                GameObject cell = Instantiate(_cellPrefab, transform); // �θ� GridManager�� ����
                cell.name = $"Cell_{x}_{z}";

                // Cell ��ũ��Ʈ ��������
                Cell cellScript = cell.GetComponent<Cell>();
                if (cellScript != null)
                {
                    cellScript.Initialize(this, x, z); // xIndex�� zIndex�� �ʱ�ȭ
                }

                // ��ư�� RectTransform�� �����Ϸ��� �Ʒ� �ڵ� �߰�
                RectTransform rectTransform = cell.GetComponent<RectTransform>();
                rectTransform.sizeDelta = new Vector2(100, 100); // ���ϴ� ũ��� ����
            }
        }
    }
}
