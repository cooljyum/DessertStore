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
    private List<ItemData> _allItems = new List<ItemData>(); //��� ������ ������
    private Dictionary<string, Button> _menuBtns = new Dictionary<string, Button>();
    private Dictionary<string, int> _selectedFoodCount = new Dictionary<string, int>();
    private UIManager _uiManager;

    private void Start()
    {
        _uiManager = UIManager.Instance;

        // Resources�� ��� ItemData �ε�
        LoadAllItems();

        int index = 0;
        foreach (Button categoryButton in _categoryContent.GetComponentsInChildren<Button>())
        {
            int categoryIndex = index;
            categoryButton.onClick.AddListener(() => OnCategoryButtonClick(categoryIndex));
            index++;
        }

        // ó������ 0�� ī�װ� �����۸� �ε�
        LoadItemsByCategory(0);
    }

    // Resources �������� ��� ItemData ScriptableObject �ε�
    private void LoadAllItems()
    {
        // Resources �������� ItemData Ÿ���� ��� ������Ʈ�� �ε�
        ItemData[] items = Resources.LoadAll<ItemData>("ScriptableObject/Item");

        if (items != null && items.Length > 0)
        {
            _allItems.AddRange(items); // ����Ʈ�� ������ �߰�
            Debug.Log($"�ε�� ������ ��: {items.Length}");
        }
    }

    // ī�װ� ��ư Ŭ�� �� ȣ�� //
    public void OnCategoryButtonClick(int categoryIndex)
    {
        LoadItemsByCategory(categoryIndex);
    }

    // Ư�� ī�װ��� �����۵鸸 �ε� //
    public void LoadItemsByCategory(int category)
    {
        // ���� �޴� ��ư�� ����
        foreach (Transform child in _menuContent)
        {
            Destroy(child.gameObject);
        }
        _menuBtns.Clear();

        // ���õ� ī�װ��� �����۸� ��ư���� ����
        foreach (ItemData item in _allItems)
        {
            if (item.itemType == category) // ������ Ÿ���� ������ ī�װ��� ��ġ�ϴ� ���
            {
                GameObject newButton = Instantiate(_menuBtnPrefab, _menuContent);

                // ��ư�� �̸��� �̹��� ����
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
            Debug.Log($"{foodName} ��ư�� Ŭ���Ǿ����ϴ�.");

            // ������ ���� ���� �Ǵ�
            if (_selectedFoodCount.ContainsKey(foodName))
            {
                _selectedFoodCount[foodName]++;
            }
            else
            {
                _selectedFoodCount[foodName] = 1;
            }

            // �ֹ����� ���Ͽ� Ready ��ư ���� ����
            _uiManager.CheckOrderStatus(_selectedFoodCount);
        }
    }

    public void ResetSelection()
    {
        _selectedFoodCount.Clear();
    }
}