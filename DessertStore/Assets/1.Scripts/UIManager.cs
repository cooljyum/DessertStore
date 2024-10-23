using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class UIManager : MonoBehaviour
{
    static public UIManager Instance;

    [Header("UI Panel")]
    [SerializeField] private POSManager _posPanel;

    private void Awake()
    {
        Instance = this;
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
            if (!selectedFoodCount.ContainsKey(orderItemName) || selectedFoodCount[orderItemName] < orderItemCount)
            {
                allOrdersReady = false;
                break;
            }
        }

        // Ready ��ư ���� ����
        _posPanel.ReadyButton.interactable = allOrdersReady;
    }
}