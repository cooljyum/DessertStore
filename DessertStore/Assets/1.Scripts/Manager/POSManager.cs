using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class POSManager : MonoBehaviour
{
    [SerializeField] private LevelData _currentLevel;
    [SerializeField] private GameObject _orderPanel;
    [SerializeField] private GameObject _orderItemPrefab;
    [SerializeField] private Button _readyButton;
    public Button ReadyButton => _readyButton;
    [SerializeField] private Button _returnButton;

    private List<OrderItem> _activeOrders = new List<OrderItem>();
    private HashSet<PuzzleData> _usedFoodItems = new HashSet<PuzzleData>();
    private List<RecipeData> _orderRecipeData= new List<RecipeData>();

    private void Start()
    {
        CreateNewOrder();
        _readyButton.onClick.AddListener(OnReadyButtonClicked);
        //_returnButton.onClick.AddListener(OnReturnButtonClicked);
    }

    // ���ο� �ֹ��� ���� // (��ư Ŭ�� ȣ��)
    public void CreateNewOrder()
    {
        // ���� �ֹ��� �ʱ�ȭ
        foreach (Transform child in _orderPanel.transform)
        {
            Destroy(child.gameObject);
        }
        _activeOrders.Clear();
        _usedFoodItems.Clear();

        // ������ PuzzleData ����
        PuzzleData randomPuzzleData;
        do
        {
            randomPuzzleData = _currentLevel.puzzleDatas[Random.Range(0, _currentLevel.puzzleDatas.Length)];
        } while (_usedFoodItems.Contains(randomPuzzleData));

        _usedFoodItems.Add(randomPuzzleData);

        // Dictionary�� ����Ͽ� �ߺ� ������ �� ���� ����
        Dictionary<RecipeData, int> recipeCountDict = new Dictionary<RecipeData, int>();

        // puzzleData���� GridRecipeSizeData�� ��ȸ�ϸ� ������ ����
        foreach (var gridRecipeSizeData in randomPuzzleData.gridRecipeDatas)
        {
            int countToSelect = gridRecipeSizeData.gridRecipeCount;

            for (int i = 0; i < countToSelect; i++)
            {
                // ������ ��Ͽ��� �������� ����
                RecipeData selectedRecipe = gridRecipeSizeData.gridRecipeData.recipeDatas[Random.Range(0, gridRecipeSizeData.gridRecipeData.recipeDatas.Length)];

                // ���� RecipeData�� ������ count�� ������Ű�� ������ �߰�
                if (recipeCountDict.ContainsKey(selectedRecipe))
                {
                    recipeCountDict[selectedRecipe] += 1;
                }
                else
                {
                    recipeCountDict[selectedRecipe] = 1;
                }

                _orderRecipeData.Add(selectedRecipe); //�� ������ ������ �߰�
            }
        }

        // Dictionary�� ����� �� �����Ƿ� �ֹ� �׸� ����
        foreach (var kvp in recipeCountDict)
        {
            RecipeData recipe = kvp.Key;
            int itemCount = kvp.Value;

            GameObject orderItemObject = Instantiate(_orderItemPrefab, _orderPanel.transform);
            OrderItem orderItem = orderItemObject.GetComponent<OrderItem>();
            orderItem.SetupOrder(recipe, itemCount); // �ߺ��� ��� itemCount�� ������ ������ ����
            _activeOrders.Add(orderItem);
        }

        SetReadyButtonInteractable(false);
    }

    // ���� ��ư Ŭ�� ����/�Ұ��� ���� //
    public void SetReadyButtonInteractable(bool interactable)
    {
        _readyButton.interactable = interactable;
    }

    public void OnReadyButtonClicked()
    {
        bool allOrdersMatched = true;

        foreach (var order in _activeOrders)
        {
            bool isOrderMatched = CheckOrder(order); // �ֹ� �׸� üũ
            if (isOrderMatched)
            {
             //   _score += 1; // �ֹ��� ��ġ�ϸ� ���� �߰�
                Debug.Log("���� +1, ���� ����: " );
            }
            else
            {
                allOrdersMatched = false; // �ϳ��� ����ġ�ϸ� false�� ����
            }
        }

        if (allOrdersMatched)
        {
            Debug.Log("��� �ֹ��� ��ġ�մϴ�!");
        }

        UIManager.Instance.MenuPanel.ResetSelection();
        UIManager.Instance.SetOrderStatus(UIManager.Instance.AllOrdersReady);
    }

    private bool CheckOrder(OrderItem order)
    {
        // �� �ֹ� �׸� ���� �ʿ��� �������� ������ �����͸� Ȯ���մϴ�.
        // order.RecipeData�� �ֹ� �׸��� ������ �������Դϴ�.

        // ���� ���� ������ ��� ��������

      
        List<DragBlock> occupyingItems = order.GetMatchingCell().GetOccupyingItems();

        // �ֹ��� �ʿ��� ������ ������
        RecipeData requiredRecipe = order.GetRecipeData();

        // �ʿ��� ������ �����Ͱ� �����ϴ��� Ȯ��
        if (occupyingItems.Count == 0)
        {
            Debug.Log("���� �������� �����ϴ�.");
            return false;
        }

        // ���� ���� �����۰� �ֹ��� �����ǰ� ��ġ�ϴ��� Ȯ��
        foreach (var item in occupyingItems)
        {
            if (item.ItemData.recipeData == requiredRecipe)
            {
                return true; // �ֹ��� ��ġ�ϴ� �������� ����
            }
        }

        Debug.Log("�ֹ��� ��ġ�ϴ� �������� �����ϴ�.");
        return false; // ��ġ�ϴ� �������� ����
    }

    public List<OrderItem> GetActiveOrders()
    {
        return _activeOrders;
    }

    private void OnReturnButtonClicked()
    {
        UIManager.Instance.MenuPanel.ResetSelection();
        Debug.Log("��� ���õ� ���� ���� �ʱ�ȭ �Ϸ�.");
    }
}