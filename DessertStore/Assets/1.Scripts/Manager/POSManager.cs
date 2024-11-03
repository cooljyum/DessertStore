using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class POSManager : MonoBehaviour
{
    [SerializeField] private LevelData _currentLevel;
    public LevelData currentLevel { get { return _currentLevel; } }

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
        _orderRecipeData.Clear();

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
    }

    // 레디 버튼 클릭 가능/불가능 설정 //
    public void SetReadyButtonInteractable(bool interactable)
    {
        _readyButton.interactable = interactable;
    }

    public void OnReadyButtonClicked()
    {
        CheckOrder(); // 주문 항목 체크
        bool isOrderMatched = CheckOrderItemList(); // 주문 항목 체크
        if (isOrderMatched)
        {
            Debug.Log("모든 주문이 일치합니다!");
            Debug.Log("점수 +1, 현재 점수: " );
            GameManager.Instance.OnPackagingSuccess();
        }
        else
        {
            Debug.Log("아이템 불일치");
            GameManager.Instance.PlayWitchAction(CharacterState.Sad);
        }

        CreateNewOrder();

        GridManager.Instance.ClearAllItems();
    }

    private void CheckOrder()
    {
        // 현재 셀의 아이템 목록 가져오기
        List<ItemData>[] cellItemData = GridManager.Instance.GetCellItemData();
        List<RecipeData> _orderRecipeDataRemove = new List<RecipeData>();

        // 주문의 레시피 데이터 확인
        foreach (var recipe in _orderRecipeData)
        {
            // 레시피의 포장 구성 요소를 가져옴
            List<ItemData> packagingComponents = recipe.packagingComponents;

            // 모든 아이템이 발견되었는지를 체크하는 변수
            bool allItemsFound = true;

            // 제거할 아이템을 저장할 리스트
            List<ItemData> itemsToRemove = new List<ItemData>();

            // 포장 구성 요소를 확인하여 GridManager의 아이템과 비교
            foreach (ItemData requiredItem in packagingComponents)
            {
                bool found = false; // 아이템 발견 여부 체크

                // 이중 배열을 순회하면서 아이템 찾기
                for (int x = 0; x < cellItemData.GetLength(0); x++)
                {
                    if (cellItemData[x].Contains(requiredItem))
                    {
                        found = true; // 아이템 발견

                        // 해당 아이템 제거
                        cellItemData[x].Remove(requiredItem);
                        Debug.Log($"아이템 {requiredItem}이(가) 그리드에서 제거되었습니다.");

                        // 제거할 아이템 리스트에 추가
                        itemsToRemove.Add(requiredItem);
                        break; // 아이템을 찾았으므로 더 이상 체크할 필요 없음
                    }
                    
                    if (found) break; // 이미 아이템을 찾았다면 더 이상 확인할 필요 없음
                }

                // 아이템이 발견되지 않았다면
                if (!found)
                {
                    allItemsFound = false; // 모든 아이템이 발견되지 않음
                    Debug.Log($"아이템 {requiredItem}이(가) 그리드에서 발견되지 않았습니다.");
                    break; // 더 이상 확인할 필요 없음
                }
            }

            // 모든 아이템이 발견되었다면 레시피에서 제거
            if (allItemsFound)
            {
                _orderRecipeDataRemove.Add(recipe);
            }
        }

        // 발견된 모든 레시피 데이터 제거
        foreach (var recipe in _orderRecipeDataRemove)
        {
            _orderRecipeData.Remove(recipe);
        }

        // 모든 레시피가 제거되었는지 확인
        if (_orderRecipeDataRemove.Count == _orderRecipeData.Count)
        {
            _orderRecipeData.Clear();
        }
    }


    private bool CheckOrderItemList() 
    {
        return _orderRecipeData.Count <= 0; // 리스트에 아이템이 있으면 occupied
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