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

    // 새로운 주문서 생성 // (버튼 클릭 호출)
    public void CreateNewOrder()
    {
        // 기존 주문서 초기화
        foreach (Transform child in _orderPanel.transform)
        {
            Destroy(child.gameObject);
        }
        _activeOrders.Clear();
        _usedFoodItems.Clear();

        // 무작위 PuzzleData 선택
        PuzzleData randomPuzzleData;
        do
        {
            randomPuzzleData = _currentLevel.puzzleDatas[Random.Range(0, _currentLevel.puzzleDatas.Length)];
        } while (_usedFoodItems.Contains(randomPuzzleData));

        _usedFoodItems.Add(randomPuzzleData);

        // Dictionary를 사용하여 중복 레시피 및 수량 추적
        Dictionary<RecipeData, int> recipeCountDict = new Dictionary<RecipeData, int>();

        // puzzleData에서 GridRecipeSizeData를 순회하며 레시피 생성
        foreach (var gridRecipeSizeData in randomPuzzleData.gridRecipeDatas)
        {
            int countToSelect = gridRecipeSizeData.gridRecipeCount;

            for (int i = 0; i < countToSelect; i++)
            {
                // 레시피 목록에서 무작위로 선택
                RecipeData selectedRecipe = gridRecipeSizeData.gridRecipeData.recipeDatas[Random.Range(0, gridRecipeSizeData.gridRecipeData.recipeDatas.Length)];

                // 같은 RecipeData가 있으면 count를 증가시키고 없으면 추가
                if (recipeCountDict.ContainsKey(selectedRecipe))
                {
                    recipeCountDict[selectedRecipe] += 1;
                }
                else
                {
                    recipeCountDict[selectedRecipe] = 1;
                }

                _orderRecipeData.Add(selectedRecipe); //비교 레시피 데이터 추가
            }
        }

        // Dictionary에 저장된 각 레시피로 주문 항목 생성
        foreach (var kvp in recipeCountDict)
        {
            RecipeData recipe = kvp.Key;
            int itemCount = kvp.Value;

            GameObject orderItemObject = Instantiate(_orderItemPrefab, _orderPanel.transform);
            OrderItem orderItem = orderItemObject.GetComponent<OrderItem>();
            orderItem.SetupOrder(recipe, itemCount); // 중복된 경우 itemCount가 증가된 값으로 전달
            _activeOrders.Add(orderItem);
        }

        SetReadyButtonInteractable(false);
    }

    // 레디 버튼 클릭 가능/불가능 설정 //
    public void SetReadyButtonInteractable(bool interactable)
    {
        _readyButton.interactable = interactable;
    }

    public void OnReadyButtonClicked()
    {
        bool allOrdersMatched = true;

        foreach (var order in _activeOrders)
        {
            bool isOrderMatched = CheckOrder(order); // 주문 항목 체크
            if (isOrderMatched)
            {
             //   _score += 1; // 주문이 일치하면 점수 추가
                Debug.Log("점수 +1, 현재 점수: " );
            }
            else
            {
                allOrdersMatched = false; // 하나라도 불일치하면 false로 설정
            }
        }

        if (allOrdersMatched)
        {
            Debug.Log("모든 주문이 일치합니다!");
        }

        UIManager.Instance.MenuPanel.ResetSelection();
        UIManager.Instance.SetOrderStatus(UIManager.Instance.AllOrdersReady);
    }

    private bool CheckOrder(OrderItem order)
    {
        // 각 주문 항목에 대해 필요한 아이템의 레시피 데이터를 확인합니다.
        // order.RecipeData는 주문 항목의 레시피 데이터입니다.

        // 현재 셀의 아이템 목록 가져오기

      
        List<DragBlock> occupyingItems = order.GetMatchingCell().GetOccupyingItems();

        // 주문에 필요한 레시피 데이터
        RecipeData requiredRecipe = order.GetRecipeData();

        // 필요한 레시피 데이터가 존재하는지 확인
        if (occupyingItems.Count == 0)
        {
            Debug.Log("셀에 아이템이 없습니다.");
            return false;
        }

        // 현재 셀의 아이템과 주문의 레시피가 일치하는지 확인
        foreach (var item in occupyingItems)
        {
            if (item.ItemData.recipeData == requiredRecipe)
            {
                return true; // 주문과 일치하는 아이템이 존재
            }
        }

        Debug.Log("주문과 일치하는 아이템이 없습니다.");
        return false; // 일치하는 아이템이 없음
    }

    public List<OrderItem> GetActiveOrders()
    {
        return _activeOrders;
    }

    private void OnReturnButtonClicked()
    {
        UIManager.Instance.MenuPanel.ResetSelection();
        Debug.Log("모든 선택된 음식 수량 초기화 완료.");
    }
}