using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class POSManager : MonoBehaviour
{
    [SerializeField] private RoundData _currentRound;
    [SerializeField] private GameObject _orderPanel;
    [SerializeField] private GameObject _orderItemPrefab;
    [SerializeField] private Button _readyButton;
    public Button ReadyButton => _readyButton;

    private List<OrderItem> _activeOrders = new List<OrderItem>();
    private HashSet<ItemData> _usedFoodItems = new HashSet<ItemData>();

    private float _roundTime = 60f;
    private float _remainingTime;
    private bool _isRoundActive = false;
    private int _currentOrderIndex = 0;
    private int _currentRoundNumber = 1;

    private void Start()
    {
        StartNewRound();
    }

    private void Update()
    {
        if (_isRoundActive)
        {
            _remainingTime -= Time.deltaTime;

            if (_remainingTime <= 0)
            {
                Debug.Log($"라운드 {_currentRoundNumber} 실패!");
                RestartRound();
            }
        }

        UpdateReadyButton();
    }

    // 새로운 라운드 시작 //
    private void StartNewRound()
    {
        Debug.Log($"라운드 {_currentRoundNumber} 시작!");
        _isRoundActive = true;
        _remainingTime = _roundTime;

        _currentOrderIndex = 0;
        _usedFoodItems.Clear();
        CreateNewOrder();
    }

    // 라운드 재시작 (실패 시 호출) //
    private void RestartRound()
    {
        _isRoundActive = false;
        _currentRoundNumber++;
        StartNewRound();
    }

    // 새로운 주문서 생성 //
    private void CreateNewOrder()
    {
        if (_currentOrderIndex >= 10)
        {
            Debug.Log($"라운드 {_currentRoundNumber} 성공!");
            _isRoundActive = false;
            _currentRoundNumber++;
            StartNewRound();
            return;
        }

        // 기존 주문 패널의 모든 주문 항목 제거
        foreach (Transform child in _orderPanel.transform)
        {
            Destroy(child.gameObject);
        }
        _activeOrders.Clear();

        // 사용되지 않은 음식 항목 중에서 무작위로 하나 선택
        ItemData randomFood;
        do
        {
            randomFood = _currentRound.availableMenu[Random.Range(0, _currentRound.availableMenu.Length)];
        } while (_usedFoodItems.Contains(randomFood));

        _usedFoodItems.Add(randomFood);

        // 새로운 주문 항목 생성 및 설정
        GameObject orderItemObject = Instantiate(_orderItemPrefab, _orderPanel.transform);
        OrderItem orderItem = orderItemObject.GetComponent<OrderItem>();
        orderItem.SetupOrder(randomFood, Random.Range(1, randomFood.maxAmount));
        _activeOrders.Add(orderItem);

        _currentOrderIndex++;
        SetReadyButtonInteractable(false);
    }

    // Ready버튼 상태 업데이트 //
    private void UpdateReadyButton()
    {
        bool allOrdersCompleted = true;

        // 모든 주문이 완료되었는지 확인
        foreach (var order in _activeOrders)
        {
            if (!order.IsCompleted())
            {
                allOrdersCompleted = false;
                break;
            }
        }

        // Ready 버튼 활성화 여부 설정
        SetReadyButtonInteractable(allOrdersCompleted);
    }

    // Ready 버튼의 상호작용 가능 여부 설정 //
    private void SetReadyButtonInteractable(bool interactable)
    {
        _readyButton.interactable = interactable;

        ColorBlock buttonColors = _readyButton.colors;
        buttonColors.normalColor = interactable
            ? new Color(69f / 255f, 122f / 255f, 228f / 255f)
            : new Color(80f / 255f, 88f / 255f, 101f / 255f);

        _readyButton.colors = buttonColors;
    }

    // 모든 주문을 완료 처리 후 새로운 주문 생성 (Ready 버튼 클릭 시 호출) //
    public void OnReadyButtonClicked()
    {
        // 모든 주문을 완료로 설정
        foreach (var order in _activeOrders)
        {
            order.CompleteOrder();
        }

        // 메뉴 선택 초기화 및 새로운 주문 생성
        UIManager.Instance.GetComponent<MenuManager>().ResetSelection();
        CreateNewOrder();
    }

    // 현재 활성화된 주문 항목 리스트 반환 //
    public List<OrderItem> GetActiveOrders()
    {
        return _activeOrders;
    }
}