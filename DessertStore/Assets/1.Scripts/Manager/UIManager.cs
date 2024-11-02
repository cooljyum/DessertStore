using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    [SerializeField] private StageManager _stagePanel;
    public StageManager StagePanel => _stagePanel;

    [SerializeField] private Slider _staminaBar;             // 체력바 UI
    private float _currentStamina;                           // 현재 남은 체력
    private float _maxStamina = 100f;                        // 최대 체력
    private float _staminaCostPerCell = 5f;                  // 셀 하나 이동 시 소모되는 체력

    [Header("Start Panel")]
    private int _currentDayNumber = 1;
    public int CurrentDayNumber => _currentDayNumber;

    [Header("End Panel")]
    private int _successCount;
    public int SuccessCount => _successCount;
    private int _failureCount;
    public int FailureCount => _failureCount;

    private float _dayTime = 60f;  //*****제한시간 임의로 둔거*****//
    private float _remainingTime;           // 남은 시간
    private bool _isDayActive = false;      // 게임 진행중인지
    private bool _allOrdersReady = true;    // 주문서 내 아이템 모두 준비됐는지
    public bool AllOrdersReady => _allOrdersReady;

    [SerializeField]public ScrollRect _menuScrollRect; //메뉴의 스크롤랙트 


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
        //*****합치면서 수정해야될듯*****//
        if (_isDayActive)
        {
            _remainingTime -= Time.deltaTime;

            if (_remainingTime <= 0)
            {
                EndDay();
                Debug.Log($"Day {_currentDayNumber} 장사 끝!");
            }
        }

        UpdateStaminaGauge();
    }

    // 하루 시작 //
    private void StartNewDay()
    {
        _popUpPanel.ShowDayStartPanel(_currentDayNumber);
    }

    // 실제 게임 시작 // (버튼 클릭 호출)
    public void StartDayTime()
    {
        //*****타이머 시작 지점*****//
        _isDayActive = true;
        _remainingTime = _dayTime;
        _posPanel.CreateNewOrder();
    }


    // 메뉴에서 선택한 음식 카운트와 현재 주문서를 비교 //
    public void CheckOrderStatus(Dictionary<string, int> selectedFoodCount)
    {
        _allOrdersReady = true;

        foreach (var order in _posPanel.GetActiveOrders())
        {
            string orderItemName = order.ItemName.text;
            int orderItemCount = int.Parse(order.ItemCount.text.Substring(2));

            // 선택한 음식 수량이 정확한지 확인
            if (!selectedFoodCount.TryGetValue(orderItemName, out int selectedCount))
            {
                Debug.Log($"선택되지 않은 항목: {orderItemName}");
                _allOrdersReady = false;
            }
            else if (selectedCount != orderItemCount)
            {
                Debug.Log($"불일치: {orderItemName} (선택된 수량: {selectedCount}, 주문된 수량: {orderItemCount})");
                _allOrdersReady = false;
            }
            else
            {
                Debug.Log($"일치: {orderItemName} (수량: {selectedCount})");
                _allOrdersReady = true;
                break;
            }
        }

        // Ready 버튼 상태 갱신
        _posPanel.SetReadyButtonInteractable(_allOrdersReady);
    }

    // 주문 처리 상태 세팅 //
    public void SetOrderStatus(bool isSuccess)
    {
        _ = isSuccess ? _successCount++ : _failureCount++;

        _popUpPanel.ShowOrderStatusPanel(isSuccess);
    }

    // 하루 끝 //
    private void EndDay()
    {
        _isDayActive = false;
        _popUpPanel.ShowDayEndPanel();
    }

    // 다음 날 준비 및 시작 // (버튼 클릭 호출)
    public void PrepareForNextDay()
    {
        _currentDayNumber++;
        _successCount = _failureCount = 0;
        StartNewDay();
    }

    private void UpdateStaminaGauge()
    {
        //음.. 이거 합쳐야할수도
        _staminaBar.value = ScoreManager.Instance.GetCurStaminaGauge();
    }

    public void SetScrollEnabled(bool isEnabled)
    {
        if (_menuScrollRect != null)
        {
            _menuScrollRect.enabled = isEnabled;
        }
    }

}