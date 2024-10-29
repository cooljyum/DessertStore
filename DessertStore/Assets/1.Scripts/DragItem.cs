using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragItem : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [SerializeField] private bool _isDragging = false; //드래그 중인지
    
    [SerializeField] private int _width = 1; //아이템 크기 (가로)
    [SerializeField] private int _height = 1; //아이템 크기 (세로)

    private Vector2 _originalPosition;
    private GridManager _gridManager;
    private BoxCollider2D _collider;

    private void Awake()
    {
        _collider = GetComponent<BoxCollider2D>();
    }

    private void Start()
    {
        _gridManager = FindObjectOfType<GridManager>();
        _originalPosition = transform.position;  // 시작 위치 저장
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _isDragging = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _isDragging = false;

        // 그리드 매니저에 현재 위치와 아이템의 크기를 보내 가장 가까운 그리드 위치를 가져옴
        Vector2 closestGridPosition = _gridManager.FindClosestGridPosition(this.gameObject, _width, _height);

        // 아이템의 위치를 그리드에 맞춰 조정
        transform.position = new Vector3(closestGridPosition.x, closestGridPosition.y, transform.position.z);

    }



    public void OnDrag(PointerEventData eventData)
    {
        if (_isDragging)
        {
            // 마우스 위치를 따라다니게 함
            Vector2 pos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                transform.parent.GetComponent<RectTransform>(),
                eventData.position,
                eventData.pressEventCamera,
                out pos);
            transform.localPosition = pos;

            _gridManager.CheckCellOverlap(_collider, _width, _height);
        }
    }

    // 아이템의 크기를 가져오는 함수
    public Vector2 GetItemSize()
    {
        return new Vector2(_width, _height);  // 1x2 크기 반환
    }
}
