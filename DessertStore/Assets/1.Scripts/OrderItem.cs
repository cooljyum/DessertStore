using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OrderItem : MonoBehaviour
{
    [SerializeField] private List<Image> _itemImage;                 // 음식 이미지
    [SerializeField] private TextMeshProUGUI _itemNameText;    // 음식 이름 텍스트
    public TextMeshProUGUI ItemName => _itemNameText;
    [SerializeField] private TextMeshProUGUI _itemCountText;   // 음식 개수 텍스트
    public TextMeshProUGUI ItemCount => _itemCountText;

    private int _remainingCount;                               // 남은 주문 개수

    // 주문 항목 초기화 //
    public void SetupOrder(RecipeData foodItem, int itemCount)
    {
        // 이미지 설정 및 활성화
        for (int i = 0; i < _itemImage.Count; i++)
        {
            if (i < foodItem.packagingComponents.Count)
            {
                _itemImage[i].sprite = foodItem.packagingComponents[i].itemImage; // 순서대로 이미지 할당
                _itemImage[i].gameObject.SetActive(true);                        // 이미지 활성화

                _itemNameText.text = foodItem.recipeName; // 초기 음식 이름 설정

                if (foodItem.packagingComponents[i].itemType > 0)
                {
                    _itemNameText.text = foodItem.packagingComponents[i].itemName; // 음식 이름 설정
                }
            }
            else
            {
                _itemImage[i].gameObject.SetActive(false);                       // 나머지 이미지 비활성화
            }
        }

        _itemCountText.text = "X " + itemCount;      // 음식 개수 설정
        _remainingCount += itemCount;
    }

    // 주문 완료 처리 (POSManager에서 호출) //
    public void CompleteOrder()
    {
        _remainingCount = 0;
    }

    public bool IsCompleted()
    {
        return _remainingCount == 0;
    }
}
