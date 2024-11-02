using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeliveryManager : MonoBehaviour
{
    [SerializeField] private Transform _gridTransform;       // �� �׸��� ������Ʈ
    [SerializeField] private Slider _staminaBar;             // ü�¹� UI
    [SerializeField] private Button _packageButton;          // ���尡�� ��ư
    [SerializeField] private TextMeshProUGUI _catMessage;    // Ű�� �� TMP
    [SerializeField] private List<MessageData> deliveryTextDataList; // ��Ȳ�� �޽���

    public MapGrid MapGrid;                                  // MapGrid Ŭ���� ����
    private GameObject _homePoint;                           // ��� �� ���� ����
    private List<Vector3> _pathPoints = new List<Vector3>(); // ���� ��θ� �����ϴ� ����Ʈ
    private LineRenderer _lineRenderer;                      // ��θ� �׸��� ���� ������
    private bool _isDragging;                                // �巡�� ���¸� Ȯ���ϴ� �÷���
    private float _currentStamina;                           // ���� ���� ü��
    private float _maxStamina = 100f;                        // �ִ� ü��
    private float _staminaCostPerCell = 5f;                  // �� �ϳ� �̵� �� �Ҹ�Ǵ� ü��
    private Dictionary<GameObject, int> _cellVisitCount = new Dictionary<GameObject, int>(); // �� �湮 Ƚ���� �����
    private Dictionary<MessageType, string> deliveryTextDictionary;                  // ��Ȳ�� �޽���

    private void Start()
    {
        _packageButton.onClick.AddListener(OnPackageButtonClicked);

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
        SetCatMessage(MessageType.Default);

        InvokeRepeating("LogStatus", 1f, 1f); // 1�ʸ��� ü�°� �� �湮 Ƚ�� ���
    }

    private void Update()
    {
        UpdateDrag();
        UpdateStaminaGauge();
    }

    private void StartDrag()
    {
        if (_homePoint != null)
        {
            _pathPoints.Add(_homePoint.transform.position);
        }

        _isDragging = true;
    }

    private void ContinueDrag()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;

        Collider2D hitCollider = Physics2D.OverlapPoint(mousePosition);
        if (hitCollider != null)
        {
            if (hitCollider.CompareTag("BlockCell"))
            {
                // BlockCell�� �浹�ϸ� �巡�� �ߴ�
                _isDragging = false;
                return;
            }
            else if (hitCollider.CompareTag("PathCell"))
            {
                Vector3 lastPoint = _pathPoints[_pathPoints.Count - 1];

                // ���� ���� ���� ���� �̿����� Ȯ�� (�Ѻױ׸���)
                if (Vector3.Distance(mousePosition, lastPoint) > 1.08f)
                {
                    return; // �������� ������ ����
                }

                // �̹� ��ο� �߰��� ������ ��ġ�� ����� ������ �־�� �߰�
                if (_pathPoints.Count == 1 || Vector3.Distance(mousePosition, _pathPoints[_pathPoints.Count - 1]) >= 1f)
                {
                    _pathPoints.Add(mousePosition);
                    _lineRenderer.positionCount = _pathPoints.Count;
                    _lineRenderer.SetPositions(_pathPoints.ToArray());

                    // �湮 Ƚ�� ����
                    if (_cellVisitCount.ContainsKey(hitCollider.gameObject))
                    {
                        _cellVisitCount[hitCollider.gameObject]++;
                    }
                    else
                    {
                        _cellVisitCount[hitCollider.gameObject] = 1;
                    }

                    // �湮 Ƚ���� ���� ���� ����
                    int visitCount = _cellVisitCount[hitCollider.gameObject];
                    var spriteRenderer = hitCollider.GetComponent<SpriteRenderer>();
                    if (spriteRenderer != null)
                    {
                        float colorValue = Mathf.Clamp01(1f - (visitCount * 0.2f)); // �湮 Ƚ���� ���� ���� ��ο���
                        spriteRenderer.color = new Color(colorValue, colorValue, 0f); // ��������� �����Ͽ� ���� ��ο���
                    }

                    _currentStamina -= _staminaCostPerCell;
                    if (_currentStamina <= 0)
                    {
                        EndDrag(); // ü�� ���� �� �巡�� ����
                    }
                }
            }
        }
    }

    private void EndDrag()
    {
        _isDragging = false;

        bool hasReturnedHome = false;

        if (_homePoint != null)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;
            float distanceToHome = Vector3.Distance(mousePosition, _homePoint.transform.position);
            hasReturnedHome = distanceToHome <= 0.5f;
        }

        if (hasReturnedHome)
        {
            SetCatMessage(MessageType.Success);
            Debug.Log("����!");
        }
        else
        {
            SetCatMessage(MessageType.Failure);
            Debug.Log("����!");
        }

        ResetPath();
    }

    private void SetCatMessage(MessageType messageType)
    {
        MessageData messageData = deliveryTextDataList.Find(data => data.messageType == messageType);

        if (messageData != null)
        {
            _catMessage.text = messageData.text; // �ؽ�Ʈ ���
        }
        else
        {
            _catMessage.text = "�޽����� ã�� �� �����ϴ�."; // �޽��� �����Ͱ� ���� ��
        }
    }

    private void ResetPath()
    {
        _pathPoints.Clear();
        _lineRenderer.positionCount = 0;
        _currentStamina = _maxStamina;
        _cellVisitCount.Clear(); // �湮 Ƚ�� �ʱ�ȭ

        // ��� PathCell�� ���� �ʱ�ȭ
        foreach (var cell in MapGrid.MapCells)
        {
            if (cell.tag == "PathCell" && cell != _homePoint)
            {
                var spriteRenderer = cell.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null)
                    spriteRenderer.color = Color.gray;
            }
        }
    }

    private void UpdateDrag()
    {
        if (Input.GetMouseButtonDown(0))
        {
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

    private void UpdateStaminaGauge()
    {
        _staminaBar.value = _currentStamina / _maxStamina;
    }

    private void LogStatus()
    {
        Debug.Log($"���� ü��: {_currentStamina}");
    }

    public void OnPackageButtonClicked()
    {
        SceneManager.LoadScene("MainScene");
    }
}