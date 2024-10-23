using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OrderItem : MonoBehaviour
{
    [SerializeField] private Image _itemImage;                 // ���� �̹���
    [SerializeField] private TextMeshProUGUI _itemNameText;    // ���� �̸� �ؽ�Ʈ
    public TextMeshProUGUI ItemName => _itemNameText;
    [SerializeField] private TextMeshProUGUI _itemCountText;   // ���� ���� �ؽ�Ʈ
    public TextMeshProUGUI ItemCount => _itemCountText;

    private int _remainingCount;                               // ���� �ֹ� ����

    // �ֹ� �׸� �ʱ�ȭ //
    public void SetupOrder(ItemData foodItem, int itemCount)
    {
        _itemImage.sprite = foodItem.itemImage;                 // ���� �̹��� ����
        _itemNameText.text = foodItem.itemName;                 // ���� �̸� ����
        _itemCountText.text = "X " + itemCount.ToString();      // ���� ���� ����
        _remainingCount = itemCount;                            // �ֹ� �ʱ� ���� ���� ����
    }

    // �ֹ� �Ϸ� ó�� (POSManager���� ȣ��) //
    public void CompleteOrder()
    {
        _remainingCount = 0;
    }

    public bool IsCompleted()
    {
        return _remainingCount == 0;
    }
}
