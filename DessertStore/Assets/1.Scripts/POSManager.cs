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
    [SerializeField] private Button _returnButton;

    private List<OrderItem> _activeOrders = new List<OrderItem>();
    private HashSet<ItemData> _usedFoodItems = new HashSet<ItemData>();

    private float _roundTime = 60f;
    private float _remainingTime;
    private bool _isRoundActive = false;
    private int _currentOrderIndex = 0;
    private int _currentRoundNumber = 1;

    private void Start()
    {
        _returnButton.onClick.AddListener(OnReturnButtonClicked);
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

    private void StartNewRound()
    {
        Debug.Log($"라운드 {_currentRoundNumber} 시작!");
        _isRoundActive = true;
        _remainingTime = _roundTime;

        _currentOrderIndex = 0;
        _usedFoodItems.Clear();
        CreateNewOrder();
    }

    private void RestartRound()
    {
        _isRoundActive = false;
        _currentRoundNumber++;
        StartNewRound();
    }

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

        foreach (Transform child in _orderPanel.transform)
        {
            Destroy(child.gameObject);
        }
        _activeOrders.Clear();

        ItemData randomFood;
        do
        {
            randomFood = _currentRound.availableMenu[Random.Range(0, _currentRound.availableMenu.Length)];
        } while (_usedFoodItems.Contains(randomFood));

        _usedFoodItems.Add(randomFood);

        GameObject orderItemObject = Instantiate(_orderItemPrefab, _orderPanel.transform);
        OrderItem orderItem = orderItemObject.GetComponent<OrderItem>();
        orderItem.SetupOrder(randomFood, Random.Range(1, randomFood.maxAmount));
        _activeOrders.Add(orderItem);

        _currentOrderIndex++;
        SetReadyButtonInteractable(false);
    }

    private void UpdateReadyButton()
    {
        bool allOrdersCompleted = true;

        foreach (var order in _activeOrders)
        {
            if (!order.IsCompleted())
            {
                allOrdersCompleted = false;
                break;
            }
        }

        SetReadyButtonInteractable(allOrdersCompleted);
    }

    private void SetReadyButtonInteractable(bool interactable)
    {
        _readyButton.interactable = interactable;

        ColorBlock buttonColors = _readyButton.colors;
        buttonColors.normalColor = interactable
            ? new Color(69f / 255f, 122f / 255f, 228f / 255f)
            : new Color(80f / 255f, 88f / 255f, 101f / 255f);

        _readyButton.colors = buttonColors;
    }

    public void OnReadyButtonClicked()
    {
        foreach (var order in _activeOrders)
        {
            order.CompleteOrder();
        }

        UIManager.Instance.GetComponent<MenuManager>().ResetSelection();
        CreateNewOrder();
    }

    public List<OrderItem> GetActiveOrders()
    {
        return _activeOrders;
    }

    private void OnReturnButtonClicked()
    {
        // UIManager의 _menuPanel을 통해 MenuManager 인스턴스를 참조
        var menuManager = UIManager.Instance.MenuPanel;
        if (menuManager != null)
        {
            menuManager.ResetSelection();
            Debug.Log("모든 선택된 음식 수량 초기화 완료.");
        }
        else
        {
            Debug.LogError("MenuManager가 초기화되지 않았습니다.");
        }
    }
}