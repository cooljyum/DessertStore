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
                Debug.Log($"���� {_currentRoundNumber} ����!");
                RestartRound();
            }
        }

        UpdateReadyButton();
    }

    // ���ο� ���� ���� //
    private void StartNewRound()
    {
        Debug.Log($"���� {_currentRoundNumber} ����!");
        _isRoundActive = true;
        _remainingTime = _roundTime;

        _currentOrderIndex = 0;
        _usedFoodItems.Clear();
        CreateNewOrder();
    }

    // ���� ����� (���� �� ȣ��) //
    private void RestartRound()
    {
        _isRoundActive = false;
        _currentRoundNumber++;
        StartNewRound();
    }

    // ���ο� �ֹ��� ���� //
    private void CreateNewOrder()
    {
        if (_currentOrderIndex >= 10)
        {
            Debug.Log($"���� {_currentRoundNumber} ����!");
            _isRoundActive = false;
            _currentRoundNumber++;
            StartNewRound();
            return;
        }

        // ���� �ֹ� �г��� ��� �ֹ� �׸� ����
        foreach (Transform child in _orderPanel.transform)
        {
            Destroy(child.gameObject);
        }
        _activeOrders.Clear();

        // ������ ���� ���� �׸� �߿��� �������� �ϳ� ����
        ItemData randomFood;
        do
        {
            randomFood = _currentRound.availableMenu[Random.Range(0, _currentRound.availableMenu.Length)];
        } while (_usedFoodItems.Contains(randomFood));

        _usedFoodItems.Add(randomFood);

        // ���ο� �ֹ� �׸� ���� �� ����
        GameObject orderItemObject = Instantiate(_orderItemPrefab, _orderPanel.transform);
        OrderItem orderItem = orderItemObject.GetComponent<OrderItem>();
        orderItem.SetupOrder(randomFood, Random.Range(1, randomFood.maxAmount));
        _activeOrders.Add(orderItem);

        _currentOrderIndex++;
        SetReadyButtonInteractable(false);
    }

    // Ready��ư ���� ������Ʈ //
    private void UpdateReadyButton()
    {
        bool allOrdersCompleted = true;

        // ��� �ֹ��� �Ϸ�Ǿ����� Ȯ��
        foreach (var order in _activeOrders)
        {
            if (!order.IsCompleted())
            {
                allOrdersCompleted = false;
                break;
            }
        }

        // Ready ��ư Ȱ��ȭ ���� ����
        SetReadyButtonInteractable(allOrdersCompleted);
    }

    // Ready ��ư�� ��ȣ�ۿ� ���� ���� ���� //
    private void SetReadyButtonInteractable(bool interactable)
    {
        _readyButton.interactable = interactable;

        ColorBlock buttonColors = _readyButton.colors;
        buttonColors.normalColor = interactable
            ? new Color(69f / 255f, 122f / 255f, 228f / 255f)
            : new Color(80f / 255f, 88f / 255f, 101f / 255f);

        _readyButton.colors = buttonColors;
    }

    // ��� �ֹ��� �Ϸ� ó�� �� ���ο� �ֹ� ���� (Ready ��ư Ŭ�� �� ȣ��) //
    public void OnReadyButtonClicked()
    {
        // ��� �ֹ��� �Ϸ�� ����
        foreach (var order in _activeOrders)
        {
            order.CompleteOrder();
        }

        // �޴� ���� �ʱ�ȭ �� ���ο� �ֹ� ����
        UIManager.Instance.GetComponent<MenuManager>().ResetSelection();
        CreateNewOrder();
    }

    // ���� Ȱ��ȭ�� �ֹ� �׸� ����Ʈ ��ȯ //
    public List<OrderItem> GetActiveOrders()
    {
        return _activeOrders;
    }
}