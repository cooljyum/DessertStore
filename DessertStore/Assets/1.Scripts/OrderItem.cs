using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OrderItem : MonoBehaviour
{
    [SerializeField] private Image _itemImage;                 // 음식 이미지
    [SerializeField] private TextMeshProUGUI _itemNameText;    // 음식 이름 텍스트
    public TextMeshProUGUI ItemName => _itemNameText;
    [SerializeField] private TextMeshProUGUI _itemCountText;   // 음식 개수 텍스트
    public TextMeshProUGUI ItemCount => _itemCountText;

    private int _remainingCount;                               // 남은 주문 개수

    // 주문 항목 초기화 //
    public void SetupOrder(ItemData foodItem, int itemCount)
    {
        _itemImage.sprite = foodItem.itemImage;                 // 음식 이미지 설정
        _itemNameText.text = foodItem.itemName;                 // 음식 이름 설정
        _itemCountText.text = "X " + itemCount.ToString();      // 음식 개수 설정
        _remainingCount = itemCount;                            // 주문 초기 남은 개수 설정
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
