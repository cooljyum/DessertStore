using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class POSManager : MonoBehaviour
{
    public RoundData currentRound;        // 현재 라운드 데이터
    public GameObject orderPanel;         // 주문서 UI가 표시될 패널
    public GameObject orderItemPrefab;    // 주문 항목 UI 프리팹

    private List<OrderItem> activeOrders = new List<OrderItem>();  // 현재 활성화된 주문들
    private bool allOrdersComplete = false;

    void Start()
    {
        // 게임 시작 시 첫 주문서 생성
        CreateNewOrder();
    }

    void Update()
    {
        // 모든 주문이 완료되었을 때 새로운 주문서 생성
        if (allOrdersComplete)
        {
            CreateNewOrder();
            allOrdersComplete = false;
        }
    }

    // 새로운 주문서 생성 //
    void CreateNewOrder()
    {
        // 기존 주문서 초기화
        foreach (Transform child in orderPanel.transform)
        {
            Destroy(child.gameObject);
        }
        activeOrders.Clear();

        // 랜덤한 개수의 음식을 선택해 주문서 생성
        int orderCount = Random.Range(1, currentRound.maxOrders + 1);  // 1 ~ maxOrders 사이의 랜덤 값
        float itemHeight = orderItemPrefab.GetComponent<RectTransform>().rect.height; // 주문 항목 높이
        Vector3 startPosition = new Vector2(-660, 200); // 시작 위치

        for (int i = 0; i < orderCount; i++)
        {
            // 랜덤한 음식 메뉴 선택
            FoodItem randomFood = currentRound.availableMenu[Random.Range(0, currentRound.availableMenu.Length)];

            // 주문 항목 UI 생성
            GameObject orderItemObject = Instantiate(orderItemPrefab, orderPanel.transform);

            // OrderItem UI 컴포넌트 설정
            OrderItem orderItem = orderItemObject.GetComponent<OrderItem>();
            orderItem.SetupOrder(randomFood, Random.Range(1, 5), OnOrderCompleted);

            // 위치 조정
            RectTransform rectTransform = orderItemObject.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(0, startPosition.y - (itemHeight * i)); // 위치 조정

            activeOrders.Add(orderItem);
        }
    }

    // 주문 완료 콜백 //
    void OnOrderCompleted(OrderItem orderItem)
    {
        // 주문 항목이 완료되면 리스트에서 제거
        activeOrders.Remove(orderItem);

        // 모든 주문이 완료되었는지 확인
        if (activeOrders.Count == 0)
        {
            allOrdersComplete = true;
        }
    }
}