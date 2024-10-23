using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private Transform _menuContent;
    private Dictionary<string, Button> _menuButtons = new Dictionary<string, Button>(); // �̸����� ��ư ����
    private Dictionary<string, int> _selectedFoodCount = new Dictionary<string, int>(); // ������ ���� ī��Ʈ
    private UIManager _uiManager;

    private void Start()
    {
        _uiManager = UIManager.Instance;

        // �޴��� ��ư�� ��� �����ͼ� �̸����� ��ųʸ� ����
        foreach (Button button in _menuContent.GetComponentsInChildren<Button>())
        {
            string foodName = button.GetComponentInChildren<TextMeshProUGUI>().text;
            _menuButtons[foodName] = button;

            // ��� ��ư�� Ŭ�� �̺�Ʈ ���
            button.onClick.AddListener(() => OnMenuButtonClick(foodName));
        }
    }

    public void OnMenuButtonClick(string foodName)
    {
        if (_menuButtons.ContainsKey(foodName) && _menuButtons[foodName] != null)
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
        else
        {
            Debug.LogError($"'{foodName}' ��ư�� �������� �ʰų� �̹� �ı��Ǿ����ϴ�.");
        }
    }

    public void ResetSelection()
    {
        _selectedFoodCount.Clear();
    }
}
