using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeliveryManager : MonoBehaviour
{
    [SerializeField] private Transform _gridTransform;      // 맵 그리드
    [SerializeField] private Slider _staminaBar;            // 체력바
    public MapGrid MapGrid;
    private GameObject _homePoint;                          // 출발 및 도착 지점
    private List<Vector3> _pathPoints = new List<Vector3>();
    private LineRenderer _lineRenderer;
    private bool _isDragging;
    private float _currentStamina;
    private float _maxStamina = 100f;
    private float _staminaCostPerCell = 5f;

    void Start()
    {
        // MapGrid의 홈 포인트를 가져와 설정
        if (MapGrid != null)
        {
            _homePoint = MapGrid.GetHomePoint();
        }
        else
        {
            Debug.LogError("MapGrid가 설정되지 않았습니다!");
            return;
        }

        _lineRenderer = gameObject.AddComponent<LineRenderer>();
        _lineRenderer.positionCount = 0;
        _lineRenderer.startWidth = 0.1f;
        _lineRenderer.endWidth = 0.1f;
        _currentStamina = _maxStamina;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // 마우스 클릭 위치가 홈 포인트와 일치하면 드래그 시작
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;

            if (_homePoint != null && Vector3.Distance(mousePosition, _homePoint.transform.position) < 0.5f)
            {
                StartDrag();
            }
        }

        if (_isDragging)
        {
            ContinueDrag();
        }

        if (Input.GetMouseButtonUp(0))
        {
            EndDrag();
        }
    }

    void StartDrag()
    {
        _pathPoints.Clear();
        _lineRenderer.positionCount = 0;

        if (_homePoint != null)
        {
            _pathPoints.Add(_homePoint.transform.position);
        }

        _isDragging = true;
    }

    void ContinueDrag()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;

        Collider2D hitCollider = Physics2D.OverlapPoint(mousePosition);
        if (hitCollider != null && hitCollider.CompareTag("PathCell")) // "PathCell" 태그 확인
        {
            if (_pathPoints.Count == 1 || Vector3.Distance(mousePosition, _pathPoints[_pathPoints.Count - 1]) >= 1f)
            {
                _pathPoints.Add(mousePosition);
                _lineRenderer.positionCount = _pathPoints.Count;
                _lineRenderer.SetPositions(_pathPoints.ToArray());

                var spriteRenderer = hitCollider.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null)
                {
                    spriteRenderer.color = Color.yellow; // 경로 따라 색 변경
                }

                _currentStamina -= _staminaCostPerCell;
                if (_currentStamina <= 0)
                {
                    ResetPath();
                }
            }
        }
    }

    void EndDrag()
    {
        _isDragging = false;

        // 드래그를 끝낼 때 집으로 돌아왔는지 확인
        if (_homePoint != null && Vector3.Distance(_homePoint.transform.position, _pathPoints[_pathPoints.Count - 1]) < 1f)
        {
            Debug.Log("성공적으로 돌아왔습니다!");
        }
        else
        {
            Debug.Log("집에 돌아가지 못했습니다.");
            ResetPath();
        }
    }

    void ResetPath()
    {
        _pathPoints.Clear();
        _lineRenderer.positionCount = 0;
        _currentStamina = _maxStamina;

        // 모든 PathCell의 색상 초기화
        foreach (var point in _pathPoints)
        {
            Collider2D collider = Physics2D.OverlapPoint(point);
            if (collider != null && collider.CompareTag("PathCell"))
            {
                var spriteRenderer = collider.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null)
                {
                    spriteRenderer.color = Color.white;
                }
            }
        }
    }
}