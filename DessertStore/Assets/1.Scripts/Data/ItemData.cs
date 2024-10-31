using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Order System/Item Data")]
public class ItemData : ScriptableObject
{
    public string itemName;  // 아이템 이름
    public Sprite itemImage; // 아이템 이미지
    public int itemType;     // 아이템 타입  ->  0:포장 1:음료 2:마카롱 3:푸딩 4:케이크
    public Vector2Int itemSize;   // 아이템 크기 : 11 => 1X1
    public int maxAmount;    // 주문 최대 개수
}
