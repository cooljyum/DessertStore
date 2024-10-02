using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OrderItem : MonoBehaviour
{
    [SerializeField] private Image itemImage;        // ���� �̹���
    [SerializeField] private TextMeshProUGUI itemNameText;      // ���� �̸� �ؽ�Ʈ
    [SerializeField] private TextMeshProUGUI itemCountText;     // ���� ���� �ؽ�Ʈ
    [SerializeField] private Button completeButton;  // �ֹ� �Ϸ� ��ư

    private System.Action<OrderItem> onCompleteCallback;

    //�ֹ� �׸� �ʱ�ȭ//
    public void SetupOrder(FoodItem foodItem, int itemCount, System.Action<OrderItem> onComplete)
    {
        itemImage.sprite = foodItem.itemImage;           // ���� �̹��� ����
        itemNameText.text = foodItem.itemName;           // ���� �̸� ����
        itemCountText.text = "X " + itemCount.ToString();  // ���� ���� ����
        onCompleteCallback = onComplete;

        // ��ư Ŭ�� �� �ֹ� �Ϸ� ó��
        completeButton.onClick.AddListener(OnOrderComplete);
    }

    //�ֹ� �Ϸ� ó��//
    private void OnOrderComplete()
    {
        // �ֹ� �Ϸ� �� �ݹ� ȣ��
        if (onCompleteCallback != null)
        {
            onCompleteCallback(this);
        }

        // UI ��Ȱ��ȭ �Ǵ� ����
        Destroy(gameObject);
    }
}
