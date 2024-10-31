using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // UI ���ӽ����̽� �߰�

public class Cell : MonoBehaviour
{
    [SerializeField] private int _xIndex;
    public int xIndex
    { get { return _xIndex; } }

    [SerializeField] private int _yIndex;
    public int yIndex
    { get { return _yIndex; } }

    [SerializeField] private bool _isActive;
    public bool isActive
    { get { return _isActive; } }

    [SerializeField] private DragBlock _occupyingItem; // �ش� ���� �����ϴ� ������ (������ null)

    private SpriteRenderer _cellImg; // Image ������Ʈ �߰�

    // ���� �������� �����ߴ��� ����
    [SerializeField] private bool _isOccupiedByPackaging;

    private void Start()
    {
        // ���� Image ������Ʈ�� �����ɴϴ�.
        _cellImg = GetComponent<SpriteRenderer>();
    }

    public void Initialize(GridManager manager, int x, int y)
    {
        _xIndex = x;
        _yIndex = y;
    }

    // ���� �������� �����ߴ��� Ȯ��
    public bool IsOccupied()
    {
        return _occupyingItem != null;
    }

    public bool CanPlacePackaging()
    {
        // ���� �������� �������� �ʴ� ��쿡�� True ��ȯ
        return !_isOccupiedByPackaging;
    }

    public void SetOccupyingItem(DragBlock item)
    {
       // _occupyingItem = item;
        _isActive = item != null;

        if (item != null && item.ItemData.itemType == 0) // ���� �������� ���
        {
            _isOccupiedByPackaging = true; // ���� �������� ������
            _cellImg.color = Color.green; // �������� ��ġ�Ǹ� �ʷϻ� ����
        }
        else
        {
            _isOccupiedByPackaging = false; // ������ �������� �������� ����
            _cellImg.color = Color.yellow; // �������� ��ġ�Ǹ� ��������� ����
        }

        
    }

    // ���� ������ ��ġ
    public void SetItem(DragBlock item)
    {
        SetOccupyingItem(item);
    }

    // ������ ������ ����
    public void ClearItem()
    {
        if (_occupyingItem != null && _occupyingItem.ItemData.itemType == 0) // ���� �������� ���ŵ� ���
        {
            _isOccupiedByPackaging = false; // ���� ������ ���� ����
        }

        _occupyingItem = null;
        _isActive = false;

        // �� ������ ������� ���� (��� �ִ� ����)
        if (_cellImg != null)
        {
            _cellImg.color = Color.white;
        }
    }

    public DragBlock GetOccupyingItem()
    {
        return _occupyingItem;
    }

    // ���� Ŭ���� �� ȣ��
    public void ClickCell()
    {
        PrintIndex(); // �ε��� ���
        ChangeColor(); // ���� ����
    }

    private void PrintIndex()
    {
        Debug.Log($"xIndex: {_xIndex}, yIndex: {_yIndex}");
    }

    private void ChangeColor()
    {
        _isActive = true;

        // ���� ������ ����
        if (_cellImg != null)
        {
            _cellImg.color = Color.yellow; // ��������� ����
        }
    }

    public void ChangeColor(Color color)
    {
        _cellImg.color = color;
    }

    public bool HasPackagingItem()
    {
        return _isOccupiedByPackaging;
    }
}
