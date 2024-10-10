using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class POSManager : MonoBehaviour
{
    [SerializeField] private RoundData _currentRound;        // ���� ���� ������
    [SerializeField] private GameObject _orderPanel;         // �ֹ��� UI�� ǥ�õ� �г�
    [SerializeField] private GameObject _orderItemPrefab;    // �ֹ� �׸� UI ������
    [SerializeField] private Button _readyButton;            // �ֹ� �غ� �Ϸ� ��ư

    private List<OrderItem> _activeOrders = new List<OrderItem>();  // ���� Ȱ��ȭ�� �ֹ���
    private bool _allOrdersComplete = false;

    private ColorBlock _defaultButtonColors;   // ��ư�� �⺻ ����
    private ColorBlock _disabledButtonColors;  // ��ư�� ��Ȱ��ȭ�� ������ ����

    private void Start()
    {
        // Ready ��ư �⺻ ���� ����
        _defaultButtonColors = _readyButton.colors;

        // ��Ȱ��ȭ ���¿����� ���� ���� (��⸦ 30% ��Ӱ�)
        _disabledButtonColors = _defaultButtonColors;
        _disabledButtonColors.normalColor = new Color(
            _defaultButtonColors.normalColor.r * 0.7f,
            _defaultButtonColors.normalColor.g * 0.7f,
            _defaultButtonColors.normalColor.b * 0.7f
        );

        // ���� ���� �� ù �ֹ��� ����
        CreateNewOrder();

        // ó������ �ֹ��� �Ϸ���� �ʾ����Ƿ� ��ư ��Ȱ��ȭ ���·� ����
        SetReadyButtonInteractable(false);
    }

    private void Update()
    {
        // Ready ��ư ���¸� ����
        UpdateReadyButton();
    }

    // ���ο� �ֹ��� ���� //
    private void CreateNewOrder()
    {
        // ���� �ֹ��� �ʱ�ȭ
        foreach (Transform child in _orderPanel.transform)
        {
            Destroy(child.gameObject);
        }
        _activeOrders.Clear();

        // ������ ������ ������ ������ �ֹ��� ����
        int orderCount = Random.Range(1, _currentRound.maxOrders + 1);  // 1 ~ maxOrders ������ ���� ��
        float itemHeight = _orderItemPrefab.GetComponent<RectTransform>().rect.height; // �ֹ� �׸� ����
        Vector3 startPosition = new Vector2(-660, 200); // ���� ��ġ

        for (int i = 0; i < orderCount; i++)
        {
            // ������ ���� �޴� ����
            FoodItem randomFood = _currentRound.availableMenu[Random.Range(0, _currentRound.availableMenu.Length)];

            // �ֹ� �׸� UI ����
            GameObject orderItemObject = Instantiate(_orderItemPrefab, _orderPanel.transform);

            // OrderItem UI ������Ʈ ����
            OrderItem orderItem = orderItemObject.GetComponent<OrderItem>();
            orderItem.SetupOrder(randomFood, Random.Range(1, randomFood.maxAmount));

            // ��ġ ����
            RectTransform rectTransform = orderItemObject.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(0, startPosition.y - (itemHeight * i)); // ��ġ ����

            _activeOrders.Add(orderItem);
        }

        // Ready ��ư�� ��Ȱ��ȭ ���·� �ʱ�ȭ
        SetReadyButtonInteractable(false);
    }

    // Ready ��ư ���� ����
    private void UpdateReadyButton()
    {
        // ��� �ֹ��� �Ϸ�Ǿ����� Ȯ��
        bool allOrdersCompleted = true;

        foreach (var order in _activeOrders)
        {
            if (!order.IsCompleted())
            {
                allOrdersCompleted = false;
                break;
            }
        }

        // ��� �ֹ��� �Ϸ�Ǹ� Ready ��ư�� Ȱ��ȭ
        SetReadyButtonInteractable(allOrdersCompleted);
    }

    // Ready ��ư ��ȣ�ۿ� ���� ���ο� ���� ����
    private void SetReadyButtonInteractable(bool interactable)
    {
        _readyButton.interactable = interactable;

        // ��ȣ�ۿ� ���� ���ο� ���� ��ư ���� ����
        if (interactable)
        {
            _readyButton.colors = _defaultButtonColors;   // �⺻ �������� ����
        }
        else
        {
            _readyButton.colors = _disabledButtonColors;  // ��ο� �������� ����
        }
    }

    // �ֹ� �Ϸ� �ݹ� //
    private void OnOrderCompleted(OrderItem orderItem)
    {
        // �ֹ� �׸� �Ϸ� ó��
        orderItem.CompleteOrder();

        // Ready ��ư ���� ������Ʈ
        UpdateReadyButton();
    }
}