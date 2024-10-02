using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Round Data", menuName = "Order System/Round Data")]
public class RoundData : ScriptableObject
{
    public int roundNumber;          // 라운드 번호
    public FoodItem[] availableMenu; // 해당 라운드에서 주문 가능한 음식 목록
    public int maxOrders;            // 해당 라운드에서 최대 주문 개수
}