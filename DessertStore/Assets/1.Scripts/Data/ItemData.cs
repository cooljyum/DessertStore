using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Packaging = 0,  // 포장
    Beverage = 1,   // 음료
    Macaron = 2,    // 마카롱
    Pudding = 3,    // 푸딩
    Cake = 4        // 케이크
}

[CreateAssetMenu(fileName = "New Item", menuName = "Order System/Item Data")]
public class ItemData : ScriptableObject
{
    [Header("Basic Item Infor")]
    public string itemName;         // 아이템의 이름
    public Sprite itemImage;        // 아이템의 이미지

    [Header("Item Type")]
    public ItemType itemType;       // 아이템의 주 타입 (예: 포장, 음료 등)
    public int subType;             // 아이템의 서브 타입 (타입 구분을 위한 추가 정보)

    [Header("Order Infor")]
    public int orderIndex;          // 아이템의 놓을 수 있는 순서 (레시피에서의 위치)
    public int maxAmount;           // 주문 시 최대 개수

    [Header("Item Size")]
    public Vector2Int itemSize;     // 아이템의 크기 (width, height)
}
