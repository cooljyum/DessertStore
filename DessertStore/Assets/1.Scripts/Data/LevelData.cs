using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Level Data", menuName = "Order System/Level Data")]
public class LevelData : ScriptableObject
{
    public int levelNumber;          // 가게 레벨
    public ItemData[] availableMenu; // 해당 라운드에서 주문 가능한 음식 목록
    public int maxOrders;            // 해당 라운드에서 최대 주문 개수
}