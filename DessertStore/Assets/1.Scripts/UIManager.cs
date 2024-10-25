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

    // 메뉴에서 선택한 음식 카운트와 현재 주문서를 비교 //
    public void CheckOrderStatus(Dictionary<string, int> selectedFoodCount)
    {
        bool allOrdersReady = true;

        foreach (var order in _posPanel.GetActiveOrders())
        {
            string orderItemName = order.ItemName.text;
            int orderItemCount = int.Parse(order.ItemCount.text.Substring(2));

            // 선택한 음식 수량이 정확한지 확인
            if (!selectedFoodCount.TryGetValue(orderItemName, out int selectedCount))
            {
                Debug.Log($"선택되지 않은 항목: {orderItemName}");
                allOrdersReady = false;
            }
            else if (selectedCount != orderItemCount)
            {
                Debug.Log($"수량 불일치: {orderItemName} (선택된 수량: {selectedCount}, 주문된 수량: {orderItemCount})");
                allOrdersReady = false;
            }
            else
            {
                Debug.Log($"일치: {orderItemName} (수량: {selectedCount})");
            }
        }

        // Ready 버튼 상태 갱신
        _posPanel.ReadyButton.interactable = allOrdersReady;
    }
}