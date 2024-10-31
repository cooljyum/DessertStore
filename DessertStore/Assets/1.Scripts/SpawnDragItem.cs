using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class SpawnDragItem : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private GameObject _dropItem;
    [SerializeField] private ItemData _itemData; // ������ �����͸� �巡�� �����ۿ� ����

    // ���콺�� ������ �� ȣ��Ǵ� �Լ�
    public void OnPointerDown(PointerEventData eventData)
    {
        SpawnDropItem(_itemData);
    }

    // �巡�� ������ �������� �����ϴ� �Լ�
    public void SpawnDropItem(ItemData itemData)
    {
        GameObject cloneDropItem = Instantiate(_dropItem, transform.position + new Vector3(0, 2, 0), Quaternion.identity);
        cloneDropItem.GetComponent<DragBlock>().Setup(transform.position, itemData);
    }
}
