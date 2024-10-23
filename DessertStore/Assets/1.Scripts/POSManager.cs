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
    private HashSet<ItemData> _usedFoodItems = new HashSet<ItemData>(); // 중복 방지

    private ColorBlock _defaultButtonColors;
    private ColorBlock _disabledButtonColors;
    private float _roundTime = 60f;
    private float _remainingTime;
    private bool _isRoundActive = false;
    private int _currentOrderIndex = 0; // 현재 처리 중인 주문서 인덱스
    private int _currentRoundNumber = 1; // 현재 라운드 번호

    private void Start()
    {
        _defaultButtonColors = _readyButton.colors;

        _disabledButtonColors = _defaultButtonColors;
        _disabledButtonColors.normalColor = new Color(
            _defaultButtonColors.normalColor.r * 0.5f,
            _defaultButtonColors.normalColor.g * 0.5f,
            _defaultButtonColors.normalColor.b * 0.5f
        );

        StartNewRound();
    }

    private void Update()
    {
        if (_isRoundActive)
        {
            _remainingTime -= Time.deltaTime;

            if (_remainingTime <= 0)
            {
                // 제한 시간 초과 시 라운드 실패
                Debug.Log($"라운드 {_currentRoundNumber} 실패!");
                RestartRound();
            }
        }

        UpdateReadyButton();
    }

    // (처음, 성공 후 다음) 라운드 시작 //
    private void StartNewRound()
    {
        Debug.Log($"라운드 {_currentRoundNumber} 시작!");
        _isRoundActive = true;
        _remainingTime = _roundTime;

        _currentOrderIndex = 0;  // 첫 주문서부터 시작
        _usedFoodItems.Clear();  // 중복 방지 초기화
        CreateNewOrder();        // 첫 주문서 생성
        SetReadyButtonInteractable(false);
    }

    // (실패 후) 라운드 재시작 //
    private void RestartRound()
    {
        _isRoundActive = false;
        _currentRoundNumber++;  // 라운드 실패 시 라운드 번호 증가
        StartNewRound();
    }

    // 새로운 주문서 생성 //
    private void CreateNewOrder()
    {
        if (_currentOrderIndex >= 10)
        {
            // 모든 주문서 처리 완료 시 라운드 성공
            Debug.Log($"라운드 {_currentRoundNumber} 성공!");
            _isRoundActive = false;
            _currentRoundNumber++;
            StartNewRound(); // 새로운 라운드 시작
            return;
        }

        // 기존 주문서 초기화
        foreach (Transform child in _orderPanel.transform)
        {
            Destroy(child.gameObject);
        }
        _activeOrders.Clear();

        ItemData randomFood;
        do
        {
            randomFood = _currentRound.availableMenu[Random.Range(0, _currentRound.availableMenu.Length)];
        } while (_usedFoodItems.Contains(randomFood)); // 중복 방지

        _usedFoodItems.Add(randomFood);

        GameObject orderItemObject = Instantiate(_orderItemPrefab, _orderPanel.transform);
        OrderItem orderItem = orderItemObject.GetComponent<OrderItem>();
        orderItem.SetupOrder(randomFood, Random.Range(1, randomFood.maxAmount));

        RectTransform rectTransform = orderItemObject.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(0, 200); // 주문서 위치 조정

        // 아이템 카운트 위치 이동
        orderItem.ItemCount.rectTransform.anchoredPosition = new Vector2(300, orderItem.ItemCount.rectTransform.anchoredPosition.y);

        _activeOrders.Add(orderItem);

        _currentOrderIndex++; // 주문서 인덱스 증가
    }

    // 준비 버튼 상태 업데이트 //
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

        // 준비 버튼이 유효한지 확인
        if (_readyButton != null)
        {
            SetReadyButtonInteractable(allOrdersCompleted);
        }
        else
        {
            Debug.LogWarning("ReadyButton이 유효하지 않습니다. 버튼이 이미 파괴된 상태입니다.");
        }
    }

    // 준비 버튼 클릭 가능 여부 세팅 //
    private void SetReadyButtonInteractable(bool interactable)
    {
        if (_readyButton != null) // 버튼이 유효한지 다시 한 번 체크
        {
            _readyButton.gameObject.SetActive(true); // 버튼 활성화
            _readyButton.interactable = interactable;

            if (interactable)
            {
                _readyButton.colors = _defaultButtonColors;
                _readyButton.onClick.RemoveAllListeners();
                _readyButton.onClick.AddListener(CreateNewOrder); // Ready 버튼을 누르면 새로운 주문서 생성
            }
            else
            {
                _readyButton.colors = _disabledButtonColors;
            }
        }
        else
        {
            Debug.LogError("ReadyButton이 null 상태입니다. SetReadyButtonInteractable에서 호출되었습니다.");
        }
    }

    // 현재 활성화된 주문서 리스트 반환 //
    public List<OrderItem> GetActiveOrders()
    {
        return _activeOrders;
    }

    // Ready 버튼이 눌렸을 때 처리 //
    public void OnReadyButtonClicked()
    {
        // 다음 주문서 생성
        CreateNewOrder();

        // 선택한 음식 초기화
        UIManager.Instance.GetComponent<MenuManager>().ResetSelection();
    }
}