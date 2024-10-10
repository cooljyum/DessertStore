using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // UI ���ӽ����̽� �߰�

public class Cell : MonoBehaviour
{
    [SerializeField] private int _xIndex;
    [SerializeField] private int _yIndex;
    [SerializeField] private bool _isActive;
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

        // ���� ������ �������� ����
        if (_cellImg != null)
        {
            _cellImg.color = Color.yellow; // ��������� ����
        }
    }
}
