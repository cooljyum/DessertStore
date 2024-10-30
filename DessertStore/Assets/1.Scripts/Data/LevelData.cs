using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Level Data", menuName = "Order System/Level Data")]
public class LevelData : ScriptableObject
{
    public int levelNumber;          // ���� ����
    public ItemData[] availableMenu; // �ش� ���忡�� �ֹ� ������ ���� ���
    public int maxOrders;            // �ش� ���忡�� �ִ� �ֹ� ����
}