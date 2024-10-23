using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class UIManager : MonoBehaviour
{
    static public UIManager Instance;

    [Header("UI Panel")]
    [SerializeField] private MenuManager _menuPanel;
    [SerializeField] private POSManager _posPanel;

    private List<Button> _menuButtons;  // Menu-음식 버튼들

    private List<OrderItem> _currentOrders;
    private Dictionary<string, int> _selectedFoodCount = new Dictionary<string, int>();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _menuButtons = new List<Button>(_menuPanel.GetComponentsInChildren<Button>());
    }

    // 메뉴 버튼 설정 및 초기화 //
    public void SetupMenuButtons(ItemData[] availableMenu, List<OrderItem> orders)
    {
        _currentOrders = orders;
        _selectedFoodCount.Clear();

        // 메뉴 버튼을 음식 이름에 맞춰 초기화
        for (int i = 0; i < _menuButtons.Count; i++)
        {
            if (i < availableMenu.Length)
            {
                ItemData foodItem = availableMenu[i];
                _menuButtons[i].gameObject.SetActive(true);
                _menuButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = foodItem.itemName;
                // 버튼 클릭 이벤트를 MenuManager의 OnMenuButtonClick 메서드로 설정
                _menuButtons[i].onClick.RemoveAllListeners();
                _menuButtons[i].onClick.AddListener(() => _menuPanel.OnMenuButtonClick(foodItem.itemName));
            }
            else
            {
                _menuButtons[i].gameObject.SetActive(false); // 사용되지 않는 버튼 비활성화
            }
        }

        UpdateReadyButton();
    }

    // 주문 상태에 따라 Ready 버튼 활성화 여부 결정 //
    private void UpdateReadyButton()
    {
        bool allOrdersReady = true;

        // 모든 주문의 조건이 만족되었는지 확인
        foreach (var order in _currentOrders)
        {
            string orderItemName = order.ItemName.text;
            int orderItemCount = int.Parse(order.ItemCount.text.Substring(2));  // "X " 부분을 제외하고 숫자 추출

            // 만약 선택된 음식 수량이 부족하면 false로 설정
            if (!_selectedFoodCount.ContainsKey(orderItemName) || _selectedFoodCount[orderItemName] < orderItemCount)
            {
                allOrdersReady = false;
                break; // 조건이 만족되지 않을 경우 루프 종료
            }
        }

        // Ready 버튼 상태 갱신
        if (_posPanel.ReadyButton != null) // 버튼이 null인지 확인
        {
            _posPanel.ReadyButton.interactable = allOrdersReady; // 한 번만 업데이트
        }
        else
        {
            Debug.LogWarning("ReadyButton이 유효하지 않습니다.");
        }
    }

    // 메뉴에서 선택한 음식 카운트와 현재 주문서를 비교 //
    public void CheckOrderStatus(Dictionary<string, int> selectedFoodCount)
    {
        bool allOrdersReady = false;

        foreach (var order in _posPanel.GetActiveOrders())
        {
            string orderItemName = order.ItemName.text;
            int orderItemCount = int.Parse(order.ItemCount.text.Substring(2));

            // 선택한 음식 수량이 정확한 경우 true로 변경
            if (selectedFoodCount.ContainsKey(orderItemName) && selectedFoodCount[orderItemName] >= orderItemCount)
            {
                allOrdersReady = true;
            }
            else
            {
                allOrdersReady = false; // 하나라도 조건이 맞지 않으면 false
                break; // 조건이 만족되지 않으면 더 확인할 필요가 없으니 루프 종료
            }
        }

        // Ready 버튼 상태 갱신
        _posPanel.ReadyButton.interactable = allOrdersReady;
    }
}