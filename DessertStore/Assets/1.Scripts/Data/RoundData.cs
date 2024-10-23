using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Round Data", menuName = "Order System/Round Data")]
public class RoundData : ScriptableObject
{
    public int roundNumber;          // ���� ��ȣ
    public ItemData[] availableMenu; // �ش� ���忡�� �ֹ� ������ ���� ���
    public int maxOrders;            // �ش� ���忡�� �ִ� �ֹ� ����
}