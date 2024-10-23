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
    private HashSet<ItemData> _usedFoodItems = new HashSet<ItemData>(); // �ߺ� ����

    private ColorBlock _defaultButtonColors;
    private ColorBlock _disabledButtonColors;
    private float _roundTime = 60f;
    private float _remainingTime;
    private bool _isRoundActive = false;
    private int _currentOrderIndex = 0; // ���� ó�� ���� �ֹ��� �ε���
    private int _currentRoundNumber = 1; // ���� ���� ��ȣ

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
                // ���� �ð� �ʰ� �� ���� ����
                Debug.Log($"���� {_currentRoundNumber} ����!");
                RestartRound();
            }
        }

        UpdateReadyButton();
    }

    // (ó��, ���� �� ����) ���� ���� //
    private void StartNewRound()
    {
        Debug.Log($"���� {_currentRoundNumber} ����!");
        _isRoundActive = true;
        _remainingTime = _roundTime;

        _currentOrderIndex = 0;  // ù �ֹ������� ����
        _usedFoodItems.Clear();  // �ߺ� ���� �ʱ�ȭ
        CreateNewOrder();        // ù �ֹ��� ����
        SetReadyButtonInteractable(false);
    }

    // (���� ��) ���� ����� //
    private void RestartRound()
    {
        _isRoundActive = false;
        _currentRoundNumber++;  // ���� ���� �� ���� ��ȣ ����
        StartNewRound();
    }

    // ���ο� �ֹ��� ���� //
    private void CreateNewOrder()
    {
        if (_currentOrderIndex >= 10)
        {
            // ��� �ֹ��� ó�� �Ϸ� �� ���� ����
            Debug.Log($"���� {_currentRoundNumber} ����!");
            _isRoundActive = false;
            _currentRoundNumber++;
            StartNewRound(); // ���ο� ���� ����
            return;
        }

        // ���� �ֹ��� �ʱ�ȭ
        foreach (Transform child in _orderPanel.transform)
        {
            Destroy(child.gameObject);
        }
        _activeOrders.Clear();

        ItemData randomFood;
        do
        {
            randomFood = _currentRound.availableMenu[Random.Range(0, _currentRound.availableMenu.Length)];
        } while (_usedFoodItems.Contains(randomFood)); // �ߺ� ����

        _usedFoodItems.Add(randomFood);

        GameObject orderItemObject = Instantiate(_orderItemPrefab, _orderPanel.transform);
        OrderItem orderItem = orderItemObject.GetComponent<OrderItem>();
        orderItem.SetupOrder(randomFood, Random.Range(1, randomFood.maxAmount));

        RectTransform rectTransform = orderItemObject.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(0, 200); // �ֹ��� ��ġ ����

        // ������ ī��Ʈ ��ġ �̵�
        orderItem.ItemCount.rectTransform.anchoredPosition = new Vector2(300, orderItem.ItemCount.rectTransform.anchoredPosition.y);

        _activeOrders.Add(orderItem);

        _currentOrderIndex++; // �ֹ��� �ε��� ����
    }

    // �غ� ��ư ���� ������Ʈ //
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

        // �غ� ��ư�� ��ȿ���� Ȯ��
        if (_readyButton != null)
        {
            SetReadyButtonInteractable(allOrdersCompleted);
        }
        else
        {
            Debug.LogWarning("ReadyButton�� ��ȿ���� �ʽ��ϴ�. ��ư�� �̹� �ı��� �����Դϴ�.");
        }
    }

    // �غ� ��ư Ŭ�� ���� ���� ���� //
    private void SetReadyButtonInteractable(bool interactable)
    {
        if (_readyButton != null) // ��ư�� ��ȿ���� �ٽ� �� �� üũ
        {
            _readyButton.gameObject.SetActive(true); // ��ư Ȱ��ȭ
            _readyButton.interactable = interactable;

            if (interactable)
            {
                _readyButton.colors = _defaultButtonColors;
                _readyButton.onClick.RemoveAllListeners();
                _readyButton.onClick.AddListener(CreateNewOrder); // Ready ��ư�� ������ ���ο� �ֹ��� ����
            }
            else
            {
                _readyButton.colors = _disabledButtonColors;
            }
        }
        else
        {
            Debug.LogError("ReadyButton�� null �����Դϴ�. SetReadyButtonInteractable���� ȣ��Ǿ����ϴ�.");
        }
    }

    // ���� Ȱ��ȭ�� �ֹ��� ����Ʈ ��ȯ //
    public List<OrderItem> GetActiveOrders()
    {
        return _activeOrders;
    }

    // Ready ��ư�� ������ �� ó�� //
    public void OnReadyButtonClicked()
    {
        // ���� �ֹ��� ����
        CreateNewOrder();

        // ������ ���� �ʱ�ȭ
        UIManager.Instance.GetComponent<MenuManager>().ResetSelection();
    }
}