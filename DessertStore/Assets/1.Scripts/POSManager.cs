using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class POSManager : MonoBehaviour
{
    [SerializeField] private RoundData _currentRound;        // 현재 라운드 데이터
    [SerializeField] private GameObject _orderPanel;         // 주문서 UI가 표시될 패널
    [SerializeField] private GameObject _orderItemPrefab;    // 주문 항목 UI 프리팹
    [SerializeField] private Button _readyButton;            // 주문 준비 완료 버튼

    private List<OrderItem> _activeOrders = new List<OrderItem>();  // 현재 활성화된 주문들
    private bool _allOrdersComplete = false;

    private ColorBlock _defaultButtonColors;   // 버튼의 기본 색상
    private ColorBlock _disabledButtonColors;  // 버튼이 비활성화된 상태의 색상

    private void Start()
    {
        // Ready 버튼 기본 색상 저장
        _defaultButtonColors = _readyButton.colors;

        // 비활성화 상태에서의 색상 설정 (밝기를 30% 어둡게)
        _disabledButtonColors = _defaultButtonColors;
        _disabledButtonColors.normalColor = new Color(
            _defaultButtonColors.normalColor.r * 0.7f,
            _defaultButtonColors.normalColor.g * 0.7f,
            _defaultButtonColors.normalColor.b * 0.7f
        );

        // 게임 시작 시 첫 주문서 생성
        CreateNewOrder();

        // 처음에는 주문이 완료되지 않았으므로 버튼 비활성화 상태로 시작
        SetReadyButtonInteractable(false);
    }

    private void Update()
    {
        // Ready 버튼 상태를 갱신
        UpdateReadyButton();
    }

    // 새로운 주문서 생성 //
    private void CreateNewOrder()
    {
        // 기존 주문서 초기화
        foreach (Transform child in _orderPanel.transform)
        {
            Destroy(child.gameObject);
        }
        _activeOrders.Clear();

        // 랜덤한 개수의 음식을 선택해 주문서 생성
        int orderCount = Random.Range(1, _currentRound.maxOrders + 1);  // 1 ~ maxOrders 사이의 랜덤 값
        float itemHeight = _orderItemPrefab.GetComponent<RectTransform>().rect.height; // 주문 항목 높이
        Vector3 startPosition = new Vector2(-660, 200); // 시작 위치

        for (int i = 0; i < orderCount; i++)
        {
            // 랜덤한 음식 메뉴 선택
            FoodItem randomFood = _currentRound.availableMenu[Random.Range(0, _currentRound.availableMenu.Length)];

            // 주문 항목 UI 생성
            GameObject orderItemObject = Instantiate(_orderItemPrefab, _orderPanel.transform);

            // OrderItem UI 컴포넌트 설정
            OrderItem orderItem = orderItemObject.GetComponent<OrderItem>();
            orderItem.SetupOrder(randomFood, Random.Range(1, randomFood.maxAmount));

            // 위치 조정
            RectTransform rectTransform = orderItemObject.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(0, startPosition.y - (itemHeight * i)); // 위치 조정

            _activeOrders.Add(orderItem);
        }

        // Ready 버튼을 비활성화 상태로 초기화
        SetReadyButtonInteractable(false);
    }

    // Ready 버튼 상태 갱신
    private void UpdateReadyButton()
    {
        // 모든 주문이 완료되었는지 확인
        bool allOrdersCompleted = true;

        foreach (var order in _activeOrders)
        {
            if (!order.IsCompleted())
            {
                allOrdersCompleted = false;
                break;
            }
        }

        // 모든 주문이 완료되면 Ready 버튼을 활성화
        SetReadyButtonInteractable(allOrdersCompleted);
    }

    // Ready 버튼 상호작용 가능 여부와 색상 변경
    private void SetReadyButtonInteractable(bool interactable)
    {
        _readyButton.interactable = interactable;

        // 상호작용 가능 여부에 따라 버튼 색상 변경
        if (interactable)
        {
            _readyButton.colors = _defaultButtonColors;   // 기본 색상으로 복귀
        }
        else
        {
            _readyButton.colors = _disabledButtonColors;  // 어두운 색상으로 변경
        }
    }

    // 주문 완료 콜백 //
    private void OnOrderCompleted(OrderItem orderItem)
    {
        // 주문 항목 완료 처리
        orderItem.CompleteOrder();

        // Ready 버튼 상태 업데이트
        UpdateReadyButton();
    }
}