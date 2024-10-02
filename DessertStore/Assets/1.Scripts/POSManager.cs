using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class POSManager : MonoBehaviour
{
    public RoundData currentRound;        // ���� ���� ������
    public GameObject orderPanel;         // �ֹ��� UI�� ǥ�õ� �г�
    public GameObject orderItemPrefab;    // �ֹ� �׸� UI ������

    private List<OrderItem> activeOrders = new List<OrderItem>();  // ���� Ȱ��ȭ�� �ֹ���
    private bool allOrdersComplete = false;

    void Start()
    {
        // ���� ���� �� ù �ֹ��� ����
        CreateNewOrder();
    }

    void Update()
    {
        // ��� �ֹ��� �Ϸ�Ǿ��� �� ���ο� �ֹ��� ����
        if (allOrdersComplete)
        {
            CreateNewOrder();
            allOrdersComplete = false;
        }
    }

    // ���ο� �ֹ��� ���� //
    void CreateNewOrder()
    {
        // ���� �ֹ��� �ʱ�ȭ
        foreach (Transform child in orderPanel.transform)
        {
            Destroy(child.gameObject);
        }
        activeOrders.Clear();

        // ������ ������ ������ ������ �ֹ��� ����
        int orderCount = Random.Range(1, currentRound.maxOrders + 1);  // 1 ~ maxOrders ������ ���� ��
        float itemHeight = orderItemPrefab.GetComponent<RectTransform>().rect.height; // �ֹ� �׸� ����
        Vector3 startPosition = new Vector2(-660, 200); // ���� ��ġ

        for (int i = 0; i < orderCount; i++)
        {
            // ������ ���� �޴� ����
            FoodItem randomFood = currentRound.availableMenu[Random.Range(0, currentRound.availableMenu.Length)];

            // �ֹ� �׸� UI ����
            GameObject orderItemObject = Instantiate(orderItemPrefab, orderPanel.transform);

            // OrderItem UI ������Ʈ ����
            OrderItem orderItem = orderItemObject.GetComponent<OrderItem>();
            orderItem.SetupOrder(randomFood, Random.Range(1, 5), OnOrderCompleted);

            // ��ġ ����
            RectTransform rectTransform = orderItemObject.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(0, startPosition.y - (itemHeight * i)); // ��ġ ����

            activeOrders.Add(orderItem);
        }
    }

    // �ֹ� �Ϸ� �ݹ� //
    void OnOrderCompleted(OrderItem orderItem)
    {
        // �ֹ� �׸��� �Ϸ�Ǹ� ����Ʈ���� ����
        activeOrders.Remove(orderItem);

        // ��� �ֹ��� �Ϸ�Ǿ����� Ȯ��
        if (activeOrders.Count == 0)
        {
            allOrdersComplete = true;
        }
    }
}