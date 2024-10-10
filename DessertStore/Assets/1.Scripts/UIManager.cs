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
    private Button _readyButton;        // POS-준비 완료 버튼

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
    public void SetupMenuButtons(FoodItem[] availableMenu, List<OrderItem> orders)
    {
        _currentOrders = orders;
        _selectedFoodCount.Clear();

        // 메뉴 버튼을 음식 이름에 맞춰 초기화
        for (int i = 0; i < _menuButtons.Count; i++)
        {
            if (i < availableMenu.Length)
            {
                FoodItem foodItem = availableMenu[i];
                _menuButtons[i].gameObject.SetActive(true);
                _menuButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = foodItem.itemName;
                _menuButtons[i].onClick.RemoveAllListeners();
                _menuButtons[i].onClick.AddListener(() => OnMenuButtonClick(foodItem));
            }
            else
            {
                _menuButtons[i].gameObject.SetActive(false); // 사용되지 않는 버튼 비활성화
            }
        }

        UpdateReadyButton();
    }

    private void OnMenuButtonClick(FoodItem foodItem) // 메뉴 버튼 클릭 시 실행
    {
        // 해당 음식이 처음 클릭된 경우
        if (!_selectedFoodCount.ContainsKey(foodItem.itemName))
        {
            _selectedFoodCount[foodItem.itemName] = 1;
        }
        else
        {
            // 이미 선택된 음식이면 수량 증가
            _selectedFoodCount[foodItem.itemName]++;
        }

        // 버튼 클릭 후 상태 업데이트
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
                break;
            }
        }

        // POSManager의 주문 완성 버튼을 통해 상태 업데이트 //
        foreach (var orderItem in _currentOrders)
        {
            Button completeButton = orderItem.GetComponentInChildren<Button>();
            completeButton.interactable = allOrdersReady;
        }
    }
}
