using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DragBlock : MonoBehaviour
{
    [SerializeField] private AnimationCurve _curveMovement; // 이동 제어 그래프
    [SerializeField] private AnimationCurve _curveScale;    // 크기 제어 그래프

    private bool _isOccupied = false;

    private float _returnTime = 0.1f;  // 블록 원래 위치 돌아갈 때 소요 시간

    [SerializeField] private ItemData _itemData; //아이템 데이터
    public ItemData ItemData // public 프로퍼티 추가
    {
        get { return _itemData; }
    }

    private Vector2 _parentPosition;

    private List<Cell> _selectedCells = new List<Cell>(); // DropBlock 전용 선택된 셀 리스트
    public List<Cell> SelectedCells
    {
        get { return _selectedCells; }
    }

    [field: SerializeField] public Vector2Int BlockCount { private set; get; } // 블럭카운트
    private BoxCollider2D _collider;
    private SpriteRenderer _spriteRenderer; // 스프라이트 렌더러 추가

    private bool _isMouseUp = false;

    private void Awake()
    {
        _collider = GetComponent<BoxCollider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>(); // 스프라이트 렌더러 초기화
    }

    private void Start()
    {
        MouseDown();
    }

    private void Update()
    {
        if (_isOccupied) return;
        if (!_isMouseUp) MouseDrag();

        if (Input.GetMouseButtonUp(0))
        {
            _isMouseUp= true;
            MouseUp();
        }
    }

    public void Setup(Vector3 parentPosition, ItemData itemData)
    {
        _parentPosition = parentPosition;
        _itemData = itemData;
      
        _collider = GetComponent<BoxCollider2D>();

        // 아이템의 스프라이트를 설정
        if (_spriteRenderer != null && _itemData != null)
        {
            _spriteRenderer.sprite = _itemData.itemImage; // 아이템 이미지로 설정
            _spriteRenderer.sortingOrder = _itemData.orderIndex + 3; // 인덱스를 기반으로 정렬 순서 설정
        }
    }

    private void MouseDown()
    {
        UIManager.Instance.SetScrollEnabled(false);

        // 코루틴 호출 시 문자열이 아닌 메서드를 직접 호출
        StopCoroutine(OnScaleTo(Vector3.one));
        StartCoroutine(OnScaleTo(Vector3.one * 1.3f));
    }

    private void MouseDrag()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.WorldToScreenPoint(transform.position).z;
        transform.position = Camera.main.ScreenToWorldPoint(mousePos);

        bool isPackagingItem = _itemData.itemType == 0;

        _selectedCells = GridManager.Instance.CheckCellOverlap(_collider, _itemData.itemSize.x, _itemData.itemSize.y, _itemData.orderIndex); // 업데이트된 리스트를 반환받음
    }

    private void MouseUp()
    {
        UIManager.Instance.SetScrollEnabled(true);

        // 아이템이 차지할 셀 수 계산
        int requiredCells = _itemData.itemSize.x * _itemData.itemSize.y;

        // 선택된 셀이 필요한 셀 수보다 적으면 원래 위치로 복귀
        if (_selectedCells.Count < requiredCells)
        {
            StartCoroutine(OnMoveTo(_parentPosition, _returnTime)); // 부모 위치로 이동
        }
        else
        {
            // 충분한 셀이 선택된 경우, 가장 가까운 그리드 위치로 스냅
            Vector2 closestGridPosition = GridManager.Instance.GetCellsCenterPosition(_selectedCells);
            transform.position = new Vector3(closestGridPosition.x, closestGridPosition.y, transform.position.z);

            // 각 선택된 셀에 이 아이템을 배치했다고 표시
            foreach (var cell in _selectedCells)
            {
                cell.AddOccupyingItem(this);
            }

            _isOccupied = true;
        }

        // 크기 조절 애니메이션 초기화
        StopCoroutine(OnScaleTo(Vector3.one * 1.3f));
        StartCoroutine(OnScaleTo(Vector3.one));
    }

    /// <summary>
    /// 현재 위치에서 end 위치까지 time 시간동안 이동
    /// </summary>
    private IEnumerator OnMoveTo(Vector3 end, float time)
    {
        Vector3 start = transform.position;
        float current = 0f;
        float percent = 0f;

        while (percent < 1f)
        {
            current += Time.deltaTime;
            percent = current / time;

            transform.position = Vector3.Lerp(start, end, _curveMovement.Evaluate(percent));

            yield return null;
        }

        // 목표 위치에 도달하면 오브젝트 삭제
        transform.position = end; // 최종 위치로 설정
        Destroy(gameObject);
    }

    private IEnumerator OnScaleTo(Vector3 end)
    {
        Vector3 start = transform.localScale;
        float current = 0f;
        float percent = 0f;

        while (percent < 1f)
        {
            current += Time.deltaTime;
            percent = current / _returnTime;

            transform.localScale = Vector3.Lerp(start, end, _curveScale.Evaluate(percent));
            yield return null;
        }
    }
}
