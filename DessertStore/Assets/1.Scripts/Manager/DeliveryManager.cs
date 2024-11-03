using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeliveryManager : MonoBehaviour
{
    [SerializeField] private Transform _gridTransform;       // 맵 그리드 오브젝트
    [SerializeField] private Slider _staminaBar;             // 체력바 UI
    [SerializeField] private Button _packageButton;          // 포장가기 버튼
    [SerializeField] private Image _catImage;                // 키리 이미지
    [SerializeField] private TextMeshProUGUI _catMessage;    // 키리 말 TMP
    [SerializeField] private List<MessageData> _deliveryTextDataList; // 상황별 메시지
    [SerializeField] private int orderQuantity = 2;                  // 필수 경유 셀의 수

    public MapGrid MapGrid;                                  // MapGrid 클래스 참조
    private GameObject _homePoint;                           // 출발 및 도착 지점
    private List<Vector3> _pathPoints = new List<Vector3>(); // 현재 경로를 저장하는 리스트
    private LineRenderer _lineRenderer;                      // 경로를 그리는 라인 렌더러
    private bool _isDragging;                                // 드래그 상태를 확인용
    private float _currentStamina;                           // 현재 남은 체력
    private float _maxStamina = 100f;                        // 최대 체력
    private float _staminaCostPerCell;                       // 셀 하나 이동 시 소모되는 체력
    private List<float> _staminaCostPerCellList = new List<float> { 7.6f, 5.8f, 4.7f, 4f, 3f }; // 주문 개수 별 체력 소모율 지정용
    private Dictionary<GameObject, int> _cellVisitCount = new Dictionary<GameObject, int>(); // 셀 방문 횟수를 저장용
    private Dictionary<MessageType, string> _deliveryTextDictionary;                         // 상황별 메시지

    private List<int> _mandatoryCells = new List<int> { 75, 57, 45, 33, 15 }; // 반드시 지나야 하는 셀 인덱스
    private HashSet<int> _visitedMandatoryCells = new HashSet<int>();         // 방문한 필수 셀을 추적할 Set

    private bool _isGoMain = false;

    private void Start()
    {
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
        SetCatMessage(MessageType.Default);
        SetCatImage("Basic");

        InvokeRepeating("LogStatus", 1f, 1f); // 1초마다 체력과 셀 방문 횟수 출력

        SoundManager.Instance.PlayBG("DeliveryBGM");
    }

    private void Update()
    {
        UpdateDrag();
        UpdateStaminaGauge();
    }

    private void StartDrag()
    {
        if (ScoreManager.Instance.PackagingCount > 0)
{
    _staminaCostPerCell = _staminaCostPerCellList[ScoreManager.Instance.PackagingCount - 1];
}
else
{
    // PackagingCount가 0일 때의 기본값 설정 (예: 0으로 초기화)
    _staminaCostPerCell = 0; // 또는 원하는 기본값
}

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

                    // 필수 경유 셀 방문 체크
                    int cellIndex = MapGrid.GetCellIndex(hitCollider.gameObject);
                    if (_mandatoryCells.Contains(cellIndex))
                    {
                        _visitedMandatoryCells.Add(cellIndex); // 필수 경유 셀 방문 기록
                    }

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
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;
            float distanceToHome = Vector3.Distance(mousePosition, _homePoint.transform.position);
            hasReturnedHome = distanceToHome <= 0.5f;
        }

        // 필수 경유 셀 방문 여부 확인
        bool visitedAllMandatoryCells = _visitedMandatoryCells.Count >= ScoreManager.Instance.PackagingCount;
        //bool visitedAllMandatoryCells = _visitedMandatoryCells.Count >= orderQuantity;

        if (hasReturnedHome && visitedAllMandatoryCells)
        {
            SetCatMessage(MessageType.Success);
            SetCatImage("Happy");
            Debug.Log("성공!");
        }
        else
        {
            if(_isGoMain) 
            SetCatMessage(MessageType.Failure);
            SetCatImage("Angry");
            Debug.Log("실패!");
        }

        ResetPath();
    }

    private void SetCatMessage(MessageType messageType)
    {
        MessageData messageData = _deliveryTextDataList.Find(data => data.messageType == messageType);

        if (messageData != null)
        {
            _catMessage.text = messageData.text; // 텍스트 출력
        }
        else
        {
            _catMessage.text = "메시지를 찾을 수 없습니다.";
        }
    }

    private void SetCatImage(string imageType)
    {
        Sprite newSprite = Resources.Load<Sprite>($"Sprite/Character/Cat/Cat_{imageType}");

        if (newSprite != null)
        {
            _catImage.sprite = newSprite;
        }
        else
        {
            Debug.LogWarning($"이미지를 찾을 수 없습니다: Sprite/Character/Cat/Cat_{imageType}");
        }
    }

    private void ResetPath()
    {
        _pathPoints.Clear();
        _lineRenderer.positionCount = 0;
        _currentStamina = _maxStamina;
        _cellVisitCount.Clear(); // 방문 횟수 초기화
        _visitedMandatoryCells.Clear(); // 필수 경유 셀 방문 기록 초기화

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

    private void UpdateDrag()
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

    private void UpdateStaminaGauge()
    {
        _staminaBar.value = _currentStamina / _maxStamina;
    }

    private void LogStatus()
    {
        Debug.Log($"현재 체력: {_currentStamina}");
    }

    public void OnPackageButtonClicked()
    {
        SceneManager.LoadScene("MainScene");
    }
}