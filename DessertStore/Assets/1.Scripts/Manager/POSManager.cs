using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class POSManager : MonoBehaviour
{
    [SerializeField] private LevelData _currentLevel;
    [SerializeField] private GameObject _orderPanel;
    [SerializeField] private GameObject _orderItemPrefab;
    [SerializeField] private Button _readyButton;
    public Button ReadyButton => _readyButton;
    [SerializeField] private Button _returnButton;

    private List<OrderItem> _activeOrders = new List<OrderItem>();
    private HashSet<ItemData> _usedFoodItems = new HashSet<ItemData>();

    private void Start()
    {
        CreateNewOrder();
        _readyButton.onClick.AddListener(OnReadyButtonClicked);
        //_returnButton.onClick.AddListener(OnReturnButtonClicked);
    }

    // 새로운 주문서 생성 // (버튼 클릭 호출)
    public void CreateNewOrder()
    {
        foreach (Transform child in _orderPanel.transform)
        {
            Destroy(child.gameObject);
        }
        _activeOrders.Clear();
        _usedFoodItems.Clear();

        ItemData randomFood;
        do
        {
            randomFood = _currentLevel.availableMenu[Random.Range(0, _currentLevel.availableMenu.Length)];
        } while (_usedFoodItems.Contains(randomFood));

        _usedFoodItems.Add(randomFood);

        GameObject orderItemObject = Instantiate(_orderItemPrefab, _orderPanel.transform);
        OrderItem orderItem = orderItemObject.GetComponent<OrderItem>();
        orderItem.SetupOrder(randomFood, Random.Range(1, randomFood.maxAmount));
        _activeOrders.Add(orderItem);

        SetReadyButtonInteractable(false);
    }

    // 레디 버튼 클릭 가능/불가능 설정 //
    public void SetReadyButtonInteractable(bool interactable)
    {
        _readyButton.interactable = interactable;
    }

    public void OnReadyButtonClicked()
    {
        foreach (var order in _activeOrders)
        {
            order.CompleteOrder();
        }

        UIManager.Instance.MenuPanel.ResetSelection();
        UIManager.Instance.SetOrderStatus(UIManager.Instance.AllOrdersReady);
    }

    public List<OrderItem> GetActiveOrders()
    {
        return _activeOrders;
    }

    private void OnReturnButtonClicked()
    {
        UIManager.Instance.MenuPanel.ResetSelection();
        Debug.Log("모든 선택된 음식 수량 초기화 완료.");
    }
}