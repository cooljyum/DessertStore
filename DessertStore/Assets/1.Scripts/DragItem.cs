using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragItem : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    private bool isDragging = false;
    private Vector2 originalPosition;
    private GridManager gridManager;

    public bool isPlacedCorrectly = false;  // 이미지를 올바르게 배치했을 때 변경될 값

    // 아이템이 차지하는 그리드 크기 (1x2)
    public int width = 1;
    public int height = 1;

    private void Start()
    {
        gridManager = FindObjectOfType<GridManager>();
        originalPosition = transform.position;  // 시작 위치 저장
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isDragging = true;
        isPlacedCorrectly = false;  // 드래그 시작하면 초기화
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;

        // 그리드 매니저에 현재 위치와 아이템의 크기를 보내 가장 가까운 1x2 그리드 위치를 가져옴
        Vector2 closestGridPosition = gridManager.FindClosestGridPosition(this.gameObject, width, height);

        // 아이템의 위치를 그리드에 맞춰 조정
        transform.position = new Vector3(closestGridPosition.x, closestGridPosition.y, transform.position.z);

        // 아이템이 올바른 위치에 배치되었는지 확인
        if (CheckIfPlacedCorrectly(closestGridPosition))
        {
            isPlacedCorrectly = true;  // 올바르게 배치되면 bool 값 변경
        }
    }



    public void OnDrag(PointerEventData eventData)
    {
        if (isDragging)
        {
            // 마우스 위치를 따라다니게 함
            Vector2 pos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                transform.parent.GetComponent<RectTransform>(),
                eventData.position,
                eventData.pressEventCamera,
                out pos);
            transform.localPosition = pos;
        }
    }

    // 올바른 위치에 배치되었는지 확인하는 함수
    private bool CheckIfPlacedCorrectly(Vector2 snappedPosition)
    {
        // 1x2 크기의 아이템이 특정 영역에 배치되었는지 확인
        // 예시: (2, 2)와 (2, 3)을 차지해야 한다고 가정
        Vector2 gridPosition1 = new Vector2(2, 2);
        Vector2 gridPosition2 = new Vector2(2, 3);

        return snappedPosition == gridPosition1 || snappedPosition == gridPosition2;
    }

    // 아이템의 크기를 가져오는 함수
    public Vector2 GetItemSize()
    {
        return new Vector2(width, height);  // 1x2 크기 반환
    }
}
