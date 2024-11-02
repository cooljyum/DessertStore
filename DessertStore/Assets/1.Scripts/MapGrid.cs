using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGrid : MonoBehaviour
{
    [SerializeField] private GameObject _cellPrefab;
    private GameObject _homePoint;                   // 출발/도착 지점
    private int _columns = 13;                       // 가로 셀 개수
    private int _rows = 7;                           // 세로 셀 개수
    private float _cellSize = 1.0f;                  // 셀 간격
    private List<GameObject> _mapCells = new List<GameObject>();
    public List<GameObject> MapCells => _mapCells;

    private void Start()
    {
        GenerateGrid();
    }

    public void GenerateGrid()
    {
        // 왼쪽 아래 기준으로 셀이 생성되도록 시작 위치를 계산
        Vector3 startPosition = transform.position - new Vector3(_columns * _cellSize / 1.6f, _rows * _cellSize / 2.25f, 0);

        for (int x = 0; x < _columns; x++)
        {
            for (int y = 0; y < _rows; y++)
            {
                // 각 셀의 위치를 왼쪽 아래 기준으로 조정
                Vector3 position = startPosition + new Vector3(x * _cellSize, y * _cellSize, 0);
                GameObject cell = Instantiate(_cellPrefab, position, Quaternion.identity, transform);

                if (x % 2 == 1 || y % 2 == 1)
                {
                    // 회색 PathCell
                    cell.tag = "PathCell";
                    cell.AddComponent<BoxCollider2D>();

                    var spriteRenderer = cell.GetComponent<SpriteRenderer>();
                    if (spriteRenderer != null)
                        spriteRenderer.color = Color.gray;
                }
                else
                {
                    // 흰색 BlockCell
                    cell.tag = "BlockCell";

                    var spriteRenderer = cell.GetComponent<SpriteRenderer>();
                    if (spriteRenderer != null)
                        spriteRenderer.color = Color.white;
                }

                _mapCells.Add(cell);
            }
        }
        // 맨 아래 오른쪽 셀을 홈 포인트로 지정
        GameObject homePoint = transform.GetChild(84).gameObject;
        _homePoint = homePoint;

        var homeSpriteRenderer = homePoint.GetComponent<SpriteRenderer>();
        if (homeSpriteRenderer != null)
            homeSpriteRenderer.color = Color.red;
    }

    public int GetCellIndex(GameObject cell)
    {
        if (_mapCells.Contains(cell))
        {
            return _mapCells.IndexOf(cell);
        }
        else
        {
            Debug.LogWarning("해당 셀은 맵에 존재하지 않습니다.");
            return -1;
        }
    }

    public GameObject GetHomePoint()
    {
        return _homePoint;
    }
}