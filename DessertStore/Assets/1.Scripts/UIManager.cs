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
    public void SetupMenuButtons(ItemData[] availableMenu, List<OrderItem> orders)
    {
        _currentOrders = orders;
        _selectedFoodCount.Clear();

        // �޴� ��ư�� ���� �̸��� ���� �ʱ�ȭ
        for (int i = 0; i < _menuButtons.Count; i++)
        {
            if (i < availableMenu.Length)
            {
                ItemData foodItem = availableMenu[i];
                _menuButtons[i].gameObject.SetActive(true);
                _menuButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = foodItem.itemName;
                // ��ư Ŭ�� �̺�Ʈ�� MenuManager�� OnMenuButtonClick �޼���� ����
                _menuButtons[i].onClick.RemoveAllListeners();
                _menuButtons[i].onClick.AddListener(() => _menuPanel.OnMenuButtonClick(foodItem.itemName));
            }
            else
            {
                _menuButtons[i].gameObject.SetActive(false); // ������ �ʴ� ��ư ��Ȱ��ȭ
            }
        }

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
                break; // ������ �������� ���� ��� ���� ����
            }
        }

        // Ready ��ư ���� ����
        if (_posPanel.ReadyButton != null) // ��ư�� null���� Ȯ��
        {
            _posPanel.ReadyButton.interactable = allOrdersReady; // �� ���� ������Ʈ
        }
        else
        {
            Debug.LogWarning("ReadyButton�� ��ȿ���� �ʽ��ϴ�.");
        }
    }

    // �޴����� ������ ���� ī��Ʈ�� ���� �ֹ����� �� //
    public void CheckOrderStatus(Dictionary<string, int> selectedFoodCount)
    {
        bool allOrdersReady = false;

        foreach (var order in _posPanel.GetActiveOrders())
        {
            string orderItemName = order.ItemName.text;
            int orderItemCount = int.Parse(order.ItemCount.text.Substring(2));

            // ������ ���� ������ ��Ȯ�� ��� true�� ����
            if (selectedFoodCount.ContainsKey(orderItemName) && selectedFoodCount[orderItemName] >= orderItemCount)
            {
                allOrdersReady = true;
            }
            else
            {
                allOrdersReady = false; // �ϳ��� ������ ���� ������ false
                break; // ������ �������� ������ �� Ȯ���� �ʿ䰡 ������ ���� ����
            }
        }

        // Ready ��ư ���� ����
        _posPanel.ReadyButton.interactable = allOrdersReady;
    }
}