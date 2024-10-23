using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private Transform _menuContent;
    private Dictionary<string, Button> _menuButtons = new Dictionary<string, Button>(); // 이름으로 버튼 매핑
    private Dictionary<string, int> _selectedFoodCount = new Dictionary<string, int>(); // 선택한 음식 카운트
    private UIManager _uiManager;

    private void Start()
    {
        _uiManager = UIManager.Instance;

        // 메뉴의 버튼을 모두 가져와서 이름으로 딕셔너리 구성
        foreach (Button button in _menuContent.GetComponentsInChildren<Button>())
        {
            string foodName = button.GetComponentInChildren<TextMeshProUGUI>().text;
            _menuButtons[foodName] = button;

            // 모든 버튼에 클릭 이벤트 등록
            button.onClick.AddListener(() => OnMenuButtonClick(foodName));
        }
    }

    public void OnMenuButtonClick(string foodName)
    {
        if (_menuButtons.ContainsKey(foodName) && _menuButtons[foodName] != null)
        {
            Debug.Log($"{foodName} 버튼이 클릭되었습니다.");

            // 선택한 음식 수량 판단
            if (_selectedFoodCount.ContainsKey(foodName))
            {
                _selectedFoodCount[foodName]++;
            }
            else
            {
                _selectedFoodCount[foodName] = 1;
            }

            // 주문서와 비교하여 Ready 버튼 상태 갱신
            _uiManager.CheckOrderStatus(_selectedFoodCount);
        }
        else
        {
            Debug.LogError($"'{foodName}' 버튼이 존재하지 않거나 이미 파괴되었습니다.");
        }
    }

    public void ResetSelection()
    {
        _selectedFoodCount.Clear();
    }
}
