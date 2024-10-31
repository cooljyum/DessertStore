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
    public POSManager PosPanel => _posPanel;
    [SerializeField] private MenuManager _menuPanel;
    public MenuManager MenuPanel => _menuPanel;
    [SerializeField] private PopUpManager _popUpPanel;
    public PopUpManager PopUpPanel => _popUpPanel;
    [SerializeField] private GameObject _storyPanel;
    public GameObject StoryPanel => _storyPanel;
    [SerializeField] private GameObject _stagePanel;
    public GameObject StagePanel => _stagePanel;

    [Header("Start Panel")]
    private int _currentDayNumber = 1;
    public int CurrentDayNumber => _currentDayNumber;

    [Header("End Panel")]
    private int _successCount;
    public int SuccessCount => _successCount;
    private int _failureCount;
    public int FailureCount => _failureCount;
    private int _starpoint;
    public int Starpoint => _starpoint;

    private float _dayTime = 60f;  //*****�ð� ���Ƿ� �а�*****//
    private float _remainingTime;
    private bool _isDayActive = false;
    private bool _allOrdersReady = true;
    public bool AllOrdersReady => _allOrdersReady;


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

    private void Start()
    {
        StartNewDay();
    }

    private void Update()
    {
        //*****��ġ�鼭 �����ؾߵɵ�*****//
        if (_isDayActive)
        {
            _remainingTime -= Time.deltaTime;

            if (_remainingTime <= 0)
            {
                EndDay();
                Debug.Log($"Day {_currentDayNumber} ��� ��!");
            }
        }
    }

    // �Ϸ� ���� //
    private void StartNewDay()
    {
        _popUpPanel.ShowDayStartPanel(_currentDayNumber);
    }

    // ���� ���� ���� // (��ư Ŭ�� ȣ��)
    public void StartDayTime()
    {
        //*****Ÿ�̸� ���� ����*****//
        _isDayActive = true;
        _remainingTime = _dayTime;
        _posPanel.CreateNewOrder();
    }


    // �޴����� ������ ���� ī��Ʈ�� ���� �ֹ����� �� //
    public void CheckOrderStatus(Dictionary<string, int> selectedFoodCount)
    {
        _allOrdersReady = true;

        foreach (var order in _posPanel.GetActiveOrders())
        {
            string orderItemName = order.ItemName.text;
            int orderItemCount = int.Parse(order.ItemCount.text.Substring(2));

            // ������ ���� ������ ��Ȯ���� Ȯ��
            if (!selectedFoodCount.TryGetValue(orderItemName, out int selectedCount))
            {
                Debug.Log($"���õ��� ���� �׸�: {orderItemName}");
                _allOrdersReady = false;
            }
            else if (selectedCount != orderItemCount)
            {
                Debug.Log($"����ġ: {orderItemName} (���õ� ����: {selectedCount}, �ֹ��� ����: {orderItemCount})");
                _allOrdersReady = false;
            }
            else
            {
                Debug.Log($"��ġ: {orderItemName} (����: {selectedCount})");
                _allOrdersReady = true;
                break;
            }
        }

        // Ready ��ư ���� ����
        _posPanel.SetReadyButtonInteractable(_allOrdersReady);
    }

    // �ֹ� ó�� ���� ���� //
    public void SetOrderStatus(bool isSuccess)
    {
        _ = isSuccess ? _successCount++ : _failureCount++;

        _popUpPanel.ShowOrderStatusPanel(isSuccess);
    }

    // �Ϸ� �� //
    private void EndDay()
    {
        _isDayActive = false;
        _popUpPanel.ShowDayEndPanel();
    }

    // ���� �� �غ� �� ���� // (��ư Ŭ�� ȣ��)
    public void PrepareForNextDay()
    {
        _currentDayNumber++;
        _successCount = _failureCount = 0;
        StartNewDay();
    }
}