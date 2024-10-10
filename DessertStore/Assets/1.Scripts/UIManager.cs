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

    private List<Button> _menuButtons;  // Menu-���� ��ư��
    private Button _readyButton;        // POS-�غ� �Ϸ� ��ư

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

    // �޴� ��ư ���� �� �ʱ�ȭ //
    public void SetupMenuButtons(FoodItem[] availableMenu, List<OrderItem> orders)
    {
        _currentOrders = orders;
        _selectedFoodCount.Clear();

        // �޴� ��ư�� ���� �̸��� ���� �ʱ�ȭ
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
                _menuButtons[i].gameObject.SetActive(false); // ������ �ʴ� ��ư ��Ȱ��ȭ
            }
        }

        UpdateReadyButton();
    }

    private void OnMenuButtonClick(FoodItem foodItem) // �޴� ��ư Ŭ�� �� ����
    {
        // �ش� ������ ó�� Ŭ���� ���
        if (!_selectedFoodCount.ContainsKey(foodItem.itemName))
        {
            _selectedFoodCount[foodItem.itemName] = 1;
        }
        else
        {
            // �̹� ���õ� �����̸� ���� ����
            _selectedFoodCount[foodItem.itemName]++;
        }

        // ��ư Ŭ�� �� ���� ������Ʈ
        UpdateReadyButton();
    }

    // �ֹ� ���¿� ���� Ready ��ư Ȱ��ȭ ���� ���� //
    private void UpdateReadyButton()
    {
        bool allOrdersReady = true;

        // ��� �ֹ��� ������ �����Ǿ����� Ȯ��
        foreach (var order in _currentOrders)
        {
            string orderItemName = order.ItemName.text;
            int orderItemCount = int.Parse(order.ItemCount.text.Substring(2));  // "X " �κ��� �����ϰ� ���� ����

            // ���� ���õ� ���� ������ �����ϸ� false�� ����
            if (!_selectedFoodCount.ContainsKey(orderItemName) || _selectedFoodCount[orderItemName] < orderItemCount)
            {
                allOrdersReady = false;
                break;
            }
        }

        // POSManager�� �ֹ� �ϼ� ��ư�� ���� ���� ������Ʈ //
        foreach (var orderItem in _currentOrders)
        {
            Button completeButton = orderItem.GetComponentInChildren<Button>();
            completeButton.interactable = allOrdersReady;
        }
    }
}
