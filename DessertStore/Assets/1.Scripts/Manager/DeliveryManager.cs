using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeliveryManager : MonoBehaviour
{
    [SerializeField] private Transform _gridTransform;      // �� �׸���
    [SerializeField] private Slider _staminaBar;            // ü�¹�
    public MapGrid MapGrid;
    private GameObject _homePoint;                          // ��� �� ���� ����
    private List<Vector3> _pathPoints = new List<Vector3>();
    private LineRenderer _lineRenderer;
    private bool _isDragging;
    private float _currentStamina;
    private float _maxStamina = 100f;
    private float _staminaCostPerCell = 5f;

    void Start()
    {
        // MapGrid�� Ȩ ����Ʈ�� ������ ����
        if (MapGrid != null)
        {
            _homePoint = MapGrid.GetHomePoint();
        }
        else
        {
            Debug.LogError("MapGrid�� �������� �ʾҽ��ϴ�!");
            return;
        }

        _lineRenderer = gameObject.AddComponent<LineRenderer>();
        _lineRenderer.positionCount = 0;
        _lineRenderer.startWidth = 0.1f;
        _lineRenderer.endWidth = 0.1f;
        _currentStamina = _maxStamina;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // ���콺 Ŭ�� ��ġ�� Ȩ ����Ʈ�� ��ġ�ϸ� �巡�� ����
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;

            if (_homePoint != null && Vector3.Distance(mousePosition, _homePoint.transform.position) < 0.5f)
            {
                StartDrag();
            }
        }

        if (_isDragging)
        {
            ContinueDrag();
        }

        if (Input.GetMouseButtonUp(0))
        {
            EndDrag();
        }
    }

    void StartDrag()
    {
        _pathPoints.Clear();
        _lineRenderer.positionCount = 0;

        if (_homePoint != null)
        {
            _pathPoints.Add(_homePoint.transform.position);
        }

        _isDragging = true;
    }

    void ContinueDrag()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;

        Collider2D hitCollider = Physics2D.OverlapPoint(mousePosition);
        if (hitCollider != null && hitCollider.CompareTag("PathCell")) // "PathCell" �±� Ȯ��
        {
            if (_pathPoints.Count == 1 || Vector3.Distance(mousePosition, _pathPoints[_pathPoints.Count - 1]) >= 1f)
            {
                _pathPoints.Add(mousePosition);
                _lineRenderer.positionCount = _pathPoints.Count;
                _lineRenderer.SetPositions(_pathPoints.ToArray());

                var spriteRenderer = hitCollider.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null)
                {
                    spriteRenderer.color = Color.yellow; // ��� ���� �� ����
                }

                _currentStamina -= _staminaCostPerCell;
                if (_currentStamina <= 0)
                {
                    ResetPath();
                }
            }
        }
    }

    void EndDrag()
    {
        _isDragging = false;

        // �巡�׸� ���� �� ������ ���ƿԴ��� Ȯ��
        if (_homePoint != null && Vector3.Distance(_homePoint.transform.position, _pathPoints[_pathPoints.Count - 1]) < 1f)
        {
            Debug.Log("���������� ���ƿԽ��ϴ�!");
        }
        else
        {
            Debug.Log("���� ���ư��� ���߽��ϴ�.");
            ResetPath();
        }
    }

    void ResetPath()
    {
        _pathPoints.Clear();
        _lineRenderer.positionCount = 0;
        _currentStamina = _maxStamina;

        // ��� PathCell�� ���� �ʱ�ȭ
        foreach (var point in _pathPoints)
        {
            Collider2D collider = Physics2D.OverlapPoint(point);
            if (collider != null && collider.CompareTag("PathCell"))
            {
                var spriteRenderer = collider.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null)
                {
                    spriteRenderer.color = Color.white;
                }
            }
        }
    }
}