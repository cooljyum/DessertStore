using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragItem : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [SerializeField] private bool _isDragging = false; //�巡�� ������
    
    [SerializeField] private int _width = 1; //������ ũ�� (����)
    [SerializeField] private int _height = 1; //������ ũ�� (����)

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
        _originalPosition = transform.position;  // ���� ��ġ ����
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _isDragging = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _isDragging = false;

        // �׸��� �Ŵ����� ���� ��ġ�� �������� ũ�⸦ ���� ���� ����� �׸��� ��ġ�� ������
        Vector2 closestGridPosition = _gridManager.FindClosestGridPosition(this.gameObject, _width, _height);

        // �������� ��ġ�� �׸��忡 ���� ����
        transform.position = new Vector3(closestGridPosition.x, closestGridPosition.y, transform.position.z);

    }



    public void OnDrag(PointerEventData eventData)
    {
        if (_isDragging)
        {
            // ���콺 ��ġ�� ����ٴϰ� ��
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

    // �������� ũ�⸦ �������� �Լ�
    public Vector2 GetItemSize()
    {
        return new Vector2(_width, _height);  // 1x2 ũ�� ��ȯ
    }
}
