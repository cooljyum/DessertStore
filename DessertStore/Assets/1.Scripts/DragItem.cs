using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragItem : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    private bool isDragging = false;
    private Vector2 originalPosition;
    private GridManager gridManager;

    public bool isPlacedCorrectly = false;  // �̹����� �ùٸ��� ��ġ���� �� ����� ��

    // �������� �����ϴ� �׸��� ũ�� (1x2)
    public int width = 1;
    public int height = 1;

    private void Start()
    {
        gridManager = FindObjectOfType<GridManager>();
        originalPosition = transform.position;  // ���� ��ġ ����
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isDragging = true;
        isPlacedCorrectly = false;  // �巡�� �����ϸ� �ʱ�ȭ
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;

        // �׸��� �Ŵ����� ���� ��ġ�� �������� ũ�⸦ ���� ���� ����� 1x2 �׸��� ��ġ�� ������
        Vector2 closestGridPosition = gridManager.FindClosestGridPosition(this.gameObject, width, height);

        // �������� ��ġ�� �׸��忡 ���� ����
        transform.position = new Vector3(closestGridPosition.x, closestGridPosition.y, transform.position.z);

        // �������� �ùٸ� ��ġ�� ��ġ�Ǿ����� Ȯ��
        if (CheckIfPlacedCorrectly(closestGridPosition))
        {
            isPlacedCorrectly = true;  // �ùٸ��� ��ġ�Ǹ� bool �� ����
        }
    }



    public void OnDrag(PointerEventData eventData)
    {
        if (isDragging)
        {
            // ���콺 ��ġ�� ����ٴϰ� ��
            Vector2 pos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                transform.parent.GetComponent<RectTransform>(),
                eventData.position,
                eventData.pressEventCamera,
                out pos);
            transform.localPosition = pos;
        }
    }

    // �ùٸ� ��ġ�� ��ġ�Ǿ����� Ȯ���ϴ� �Լ�
    private bool CheckIfPlacedCorrectly(Vector2 snappedPosition)
    {
        // 1x2 ũ���� �������� Ư�� ������ ��ġ�Ǿ����� Ȯ��
        // ����: (2, 2)�� (2, 3)�� �����ؾ� �Ѵٰ� ����
        Vector2 gridPosition1 = new Vector2(2, 2);
        Vector2 gridPosition2 = new Vector2(2, 3);

        return snappedPosition == gridPosition1 || snappedPosition == gridPosition2;
    }

    // �������� ũ�⸦ �������� �Լ�
    public Vector2 GetItemSize()
    {
        return new Vector2(width, height);  // 1x2 ũ�� ��ȯ
    }
}
