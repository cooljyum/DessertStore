using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Order System/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;  // ������ �̸�
    public Sprite itemImage; // ������ �̹���
    public int itemType;     // ������ Ÿ��  ->  0:���� 1:���� 2:��ī�� 3:Ǫ�� 4:����ũ
    public int itemSize;   // ������ ũ�� : 11 => 1X1
    public int maxAmount;    // �ֹ� �ִ� ����
}