using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OrderItem : MonoBehaviour
{
    [SerializeField] private Image itemImage;        // 음식 이미지
    [SerializeField] private TextMeshProUGUI itemNameText;      // 음식 이름 텍스트
    [SerializeField] private TextMeshProUGUI itemCountText;     // 음식 개수 텍스트
    [SerializeField] private Button completeButton;  // 주문 완료 버튼

    private System.Action<OrderItem> onCompleteCallback;

    //주문 항목 초기화//
    public void SetupOrder(FoodItem foodItem, int itemCount, System.Action<OrderItem> onComplete)
    {
        itemImage.sprite = foodItem.itemImage;           // 음식 이미지 설정
        itemNameText.text = foodItem.itemName;           // 음식 이름 설정
        itemCountText.text = "X " + itemCount.ToString();  // 음식 개수 설정
        onCompleteCallback = onComplete;

        // 버튼 클릭 시 주문 완료 처리
        completeButton.onClick.AddListener(OnOrderComplete);
    }

    //주문 완료 처리//
    private void OnOrderComplete()
    {
        // 주문 완료 시 콜백 호출
        if (onCompleteCallback != null)
        {
            onCompleteCallback(this);
        }

        // UI 비활성화 또는 제거
        Destroy(gameObject);
    }
}
