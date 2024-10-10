using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Food Item", menuName = "Order System/Food Item")]
public class FoodItem : ScriptableObject
{
    public string itemName;  // 음식 이름
    public Sprite itemImage;  // 음식 이미지
    public int maxAmount;  // 주문 최대 개수
}
