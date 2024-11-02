using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeliveryManager : MonoBehaviour
{
    [SerializeField] private Transform _gridTransform;       // 맵 그리드 오브젝트
    [SerializeField] private Slider _staminaBar;             // 체력바 UI
    [SerializeField] private Button _eraseButton;            // 길 지우기 버튼
    [SerializeField] private Button _packageButton;          // 포장가기 버튼
    public MapGrid MapGrid;                                  // MapGrid 클래스 참조
    private GameObject _homePoint;                           // 출발 및 도착 지점
    private List<Vector3> _pathPoints = new List<Vector3>(); // 현재 경로를 저장하는 리스트
    private LineRenderer _lineRenderer;                      // 경로를 그리는 라인 렌더러
    private bool _isDragging;                                // 드래그 상태를 확인하는 플래그
    private float _currentStamina;                           // 현재 남은 체력
    private float _maxStamina = 100f;                        // 최대 체력
    private float _staminaCostPerCell = 5f;                  // 셀 하나 이동 시 소모되는 체력
    private Dictionary<GameObject, int> _cellVisitCount = new Dictionary<GameObject, int>(); // 셀 방문 횟수를 저장용

    private void Start()
    {
        _eraseButton.onClick.AddListener(OnEraseButtonClicked);
        _packageButton.onClick.AddListener(OnPackageButtonClicked);

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

        InvokeRepeating("LogStatus", 1f, 1f); // 1초마다 체력과 셀 방문 횟수 출력
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
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

    private void StartDrag()
    {
        if (_homePoint != null)
        {
            _pathPoints.Add(_homePoint.transform.position);
        }

        _isDragging = true;
    }

    private void ContinueDrag()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;

        Collider2D hitCollider = Physics2D.OverlapPoint(mousePosition);
        if (hitCollider != null)
        {
            if (hitCollider.CompareTag("BlockCell"))
            {
                // BlockCell에 충돌하면 드래그 중단
                _isDragging = false;
                return;
            }
            else if (hitCollider.CompareTag("PathCell"))
            {
                Vector3 lastPoint = _pathPoints[_pathPoints.Count - 1];

                // 현재 셀이 직전 셀과 이웃한지 확인 (한붓그리기)
                if (Vector3.Distance(mousePosition, lastPoint) > 1.08f)
                {
                    return; // 인접하지 않으면 무시
                }

                // 이미 경로에 추가된 마지막 위치와 충분히 떨어져 있어야 추가
                if (_pathPoints.Count == 1 || Vector3.Distance(mousePosition, _pathPoints[_pathPoints.Count - 1]) >= 1f)
                {
                    _pathPoints.Add(mousePosition);
                    _lineRenderer.positionCount = _pathPoints.Count;
                    _lineRenderer.SetPositions(_pathPoints.ToArray());

                    // 방문 횟수 증가
                    if (_cellVisitCount.ContainsKey(hitCollider.gameObject))
                    {
                        _cellVisitCount[hitCollider.gameObject]++;
                    }
                    else
                    {
                        _cellVisitCount[hitCollider.gameObject] = 1;
                    }

                    // 방문 횟수에 따라 색상 조정
                    int visitCount = _cellVisitCount[hitCollider.gameObject];
                    var spriteRenderer = hitCollider.GetComponent<SpriteRenderer>();
                    if (spriteRenderer != null)
                    {
                        float colorValue = Mathf.Clamp01(1f - (visitCount * 0.2f)); // 방문 횟수에 따라 점점 어두워짐
                        spriteRenderer.color = new Color(colorValue, colorValue, 0f); // 노란색으로 시작하여 점점 어두워짐
                    }

                    _currentStamina -= _staminaCostPerCell;
                    if (_currentStamina <= 0)
                    {
                        EndDrag(); // 체력 소진 시 드래그 종료
                    }
                }
            }
        }
    }

    private void EndDrag()
    {
        _isDragging = false;

        bool hasReturnedHome = false;

        if (_homePoint != null)
        {
            // 마우스 위치 가져오기
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;

            // 홈 포인트와의 거리 계산
            float distanceToHome = Vector3.Distance(mousePosition, _homePoint.transform.position);

            // 거리가 0.5f 이내라면 홈 포인트에 도착한 것으로 간주
            hasReturnedHome = distanceToHome <= 0.5f;
        }

        if (hasReturnedHome)
        {
            Debug.Log("성공적으로 돌아왔습니다!");
        }
        else
        {
            Debug.Log("집에 돌아가지 못했습니다.");
        }

        ResetPath();
    }

    private void ResetPath()
    {
        _pathPoints.Clear();
        _lineRenderer.positionCount = 0;
        _currentStamina = _maxStamina;
        _cellVisitCount.Clear(); // 방문 횟수 초기화

        // 모든 PathCell의 색상 초기화
        foreach (var cell in MapGrid.MapCells)
        {
            if (cell.tag == "PathCell" && cell != _homePoint)
            {
                var spriteRenderer = cell.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null)
                    spriteRenderer.color = Color.gray;
            }
        }
    }

    private void LogStatus()
    {
        Debug.Log($"현재 체력: {_currentStamina}");
        foreach (var cell in _cellVisitCount)
        {
            Debug.Log($"{cell.Key.name}\n셀 방문 횟수: {cell.Value}");
        }
    }

    public void OnEraseButtonClicked()
    {
        ResetPath();
    }

    public void OnPackageButtonClicked()
    {
        SceneManager.LoadScene("MainScene");
    }
}