using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("UI Panel")]
    [SerializeField] private POSManager _posPanel;
    [SerializeField] private MenuManager _menuPanel;
    public MenuManager MenuPanel { get { return _menuPanel; } }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // �޴����� ������ ���� ī��Ʈ�� ���� �ֹ����� �� //
    public void CheckOrderStatus(Dictionary<string, int> selectedFoodCount)
    {
        bool allOrdersReady = true;

        foreach (var order in _posPanel.GetActiveOrders())
        {
            string orderItemName = order.ItemName.text;
            int orderItemCount = int.Parse(order.ItemCount.text.Substring(2));

            // ������ ���� ������ ��Ȯ���� Ȯ��
            if (!selectedFoodCount.TryGetValue(orderItemName, out int selectedCount))
            {
                Debug.Log($"���õ��� ���� �׸�: {orderItemName}");
                allOrdersReady = false;
            }
            else if (selectedCount != orderItemCount)
            {
                Debug.Log($"���� ����ġ: {orderItemName} (���õ� ����: {selectedCount}, �ֹ��� ����: {orderItemCount})");
                allOrdersReady = false;
            }
            else
            {
                Debug.Log($"��ġ: {orderItemName} (����: {selectedCount})");
            }
        }

        // Ready ��ư ���� ����
        _posPanel.ReadyButton.interactable = allOrdersReady;
    }
}