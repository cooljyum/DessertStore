using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // UI ���ӽ����̽� �߰�

public class Cell : MonoBehaviour
{
    [SerializeField] private int _xIndex;
    [SerializeField] private int _yIndex;
    [SerializeField] private bool _isActive;

    [SerializeField] private DragItem _occupyingItem; // �ش� ���� �����ϴ� ������ (������ null)
    private GridManager _gridManager;
    private Image _cellImg; // Image ������Ʈ �߰�

    private void Start()
    {
        // ���� Image ������Ʈ�� �����ɴϴ�.
        _cellImg = GetComponent<Image>();
    }

    public void Initialize(GridManager manager, int x, int y)
    {
        _gridManager = manager;
        _xIndex = x;
        _yIndex = y;
    }

    // ���� �������� �����ߴ��� Ȯ��
    public bool IsOccupied()
    {
        return _occupyingItem != null;
    }

    // ���� ������ ��ġ
    public void SetItem(DragItem item)
    {
        _occupyingItem = item;
        _isActive = true;

        // ���� ������ �����̹Ƿ� ���� ����
        if (_cellImg != null)
        {
            _cellImg.color = Color.yellow; // �������� ��ġ�Ǹ� ��������� ����
        }
    }

    // ������ ������ ����
    public void ClearItem()
    {
        _occupyingItem = null;
        _isActive = false;

        // �� ������ ������� ���� (��� �ִ� ����)
        if (_cellImg != null)
        {
            _cellImg.color = Color.white;
        }
    }

    public DragItem GetOccupyingItem()
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
        Debug.Log($"xIndex: {_xIndex}, zIndex: {_yIndex}");
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
}
