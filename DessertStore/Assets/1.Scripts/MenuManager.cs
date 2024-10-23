using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private Transform _menuContent;
    [SerializeField] private GameObject _menuBtnPrefab;
    [SerializeField] private Transform _categoryContent;
    private List<ItemData> _allItems = new List<ItemData>(); //모든 아이템 데이터
    private Dictionary<string, Button> _menuBtns = new Dictionary<string, Button>();
    private Dictionary<string, int> _selectedFoodCount = new Dictionary<string, int>();
    private UIManager _uiManager;

    private void Start()
    {
        _uiManager = UIManager.Instance;

        // Resources의 모든 ItemData 로드
        LoadAllItems();

        int index = 0;
        foreach (Button categoryButton in _categoryContent.GetComponentsInChildren<Button>())
        {
            int categoryIndex = index;
            categoryButton.onClick.AddListener(() => OnCategoryButtonClick(categoryIndex));
            index++;
        }

        // 처음에는 0번 카테고리 아이템만 로드
        LoadItemsByCategory(0);
    }

    // Resources 폴더에서 모든 ItemData ScriptableObject 로드
    private void LoadAllItems()
    {
        // Resources 폴더에서 ItemData 타입의 모든 오브젝트를 로드
        ItemData[] items = Resources.LoadAll<ItemData>("ScriptableObject/Item");

        if (items != null && items.Length > 0)
        {
            _allItems.AddRange(items); // 리스트에 아이템 추가
            Debug.Log($"로드된 아이템 수: {items.Length}");
        }
    }

    // 카테고리 버튼 클릭 시 호출 //
    public void OnCategoryButtonClick(int categoryIndex)
    {
        LoadItemsByCategory(categoryIndex);
    }

    // 특정 카테고리의 아이템들만 로드 //
    public void LoadItemsByCategory(int category)
    {
        // 기존 메뉴 버튼들 삭제
        foreach (Transform child in _menuContent)
        {
            Destroy(child.gameObject);
        }
        _menuBtns.Clear();

        // 선택된 카테고리의 아이템만 버튼으로 생성
        foreach (ItemData item in _allItems)
        {
            if (item.itemType == category) // 아이템 타입이 선택한 카테고리와 일치하는 경우
            {
                GameObject newButton = Instantiate(_menuBtnPrefab, _menuContent);

                // 버튼에 이름과 이미지 설정
                newButton.GetComponentInChildren<TextMeshProUGUI>().text = item.itemName;
                newButton.GetComponentInChildren<Image>().sprite = item.itemImage;

                Button buttonComponent = newButton.GetComponent<Button>();
                _menuBtns[item.itemName] = buttonComponent;
                buttonComponent.onClick.AddListener(() => OnMenuButtonClick(item.itemName));
            }
        }
    }

    public void OnMenuButtonClick(string foodName)
    {
        if (_menuBtns.ContainsKey(foodName) && _menuBtns[foodName] != null)
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
    }

    public void ResetSelection()
    {
        _selectedFoodCount.Clear();
    }
}