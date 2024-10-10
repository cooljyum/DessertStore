using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Food Item", menuName = "Order System/Food Item")]
public class FoodItem : ScriptableObject
{
    public string itemName;  // ���� �̸�
    public Sprite itemImage;  // ���� �̹���
    public int maxAmount;  // �ֹ� �ִ� ����
}
