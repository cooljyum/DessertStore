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

    // 메뉴에서 선택한 음식 카운트와 현재 주문서를 비교 //
    public void CheckOrderStatus(Dictionary<string, int> selectedFoodCount)
    {
        bool allOrdersReady = true;

        foreach (var order in _posPanel.GetActiveOrders())
        {
            string orderItemName = order.ItemName.text;
            int orderItemCount = int.Parse(order.ItemCount.text.Substring(2));

            // 선택한 음식 수량이 정확한지 확인
            if (!selectedFoodCount.ContainsKey(orderItemName) || selectedFoodCount[orderItemName] < orderItemCount)
            {
                allOrdersReady = false;
                break;
            }
        }

        // Ready 버튼 상태 갱신
        _posPanel.ReadyButton.interactable = allOrdersReady;
    }
}