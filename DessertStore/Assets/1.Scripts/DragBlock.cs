using System.Collections;
using UnityEngine;

public class DragBlock : MonoBehaviour
{
    [SerializeField] private AnimationCurve _curveMovement; // �̵� ���� �׷���
    [SerializeField] private AnimationCurve _curveScale;    // ũ�� ���� �׷���

    private float _appearTime = 0.5f;  // ��� ���� �ҿ� �ð�
    private float _returnTime = 0.1f;  // ��� ���� ��ġ ���ư� �� �ҿ� �ð�

    [SerializeField] private ItemData _itemData; //������ ������

    private Vector2 _parentPosition;

    [field: SerializeField] public Vector2Int BlockCount { private set; get; } // ��ī��Ʈ
    private BoxCollider2D _collider;

    private void Awake()
    {
        _collider = GetComponent<BoxCollider2D>();
    }

    public void Setup(Vector3 parentPosition, ItemData itemData)
    {
        _parentPosition = parentPosition;
        _itemData = itemData;
    }

    private void OnMouseDown()
    {
        // �ڷ�ƾ ȣ�� �� ���ڿ��� �ƴ� �޼��带 ���� ȣ��
        StopCoroutine(OnScaleTo(Vector3.one));
        StartCoroutine(OnScaleTo(Vector3.one * 1.3f));
    }

    private void OnMouseDrag()
    {
        // ���콺�� z�� ���� ����
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.WorldToScreenPoint(transform.position).z;
        transform.position = Camera.main.ScreenToWorldPoint(mousePos);

        GridManager.Instance.CheckCellOverlap(_collider, BlockCount.x, BlockCount.y);
    }

    private void OnMouseUp()
    {
        float x = Mathf.RoundToInt(transform.position.x-BlockCount.x%2*0.5f)+BlockCount.x%2*0.5f;
        float y = Mathf.RoundToInt(transform.position.y-BlockCount.y%2*0.5f)+BlockCount.y%2*0.5f;
        transform.position = new Vector3(x, y, 0);
        // �ڷ�ƾ ȣ�� �� ���ڿ��� �ƴ� �޼��带 ���� ȣ��
        StopCoroutine(OnScaleTo(Vector3.one * 1.3f));
        StartCoroutine(OnScaleTo(Vector3.one));

        //StartCoroutine(OnMoveTo(_parentPosition, _returnTime));
    }

    /// <summary>
    /// ���� ��ġ���� end ��ġ���� time �ð����� �̵�
    /// </summary>
    private IEnumerator OnMoveTo(Vector3 end, float time)
    {
        Vector3 start = transform.position;
        float current = 0f;
        float percent = 0f;

        while (percent < 1f)
        {
            current += Time.deltaTime;
            percent = current / time;

            transform.position = Vector3.Lerp(start, end, _curveMovement.Evaluate(percent));

            yield return null;
        }
    }

    private IEnumerator OnScaleTo(Vector3 end)
    {
        Vector3 start = transform.localScale;
        float current = 0f;
        float percent = 0f;

        while (percent < 1f)
        {
            current += Time.deltaTime;
            percent = current / _returnTime;

            transform.localScale = Vector3.Lerp(start, end, _curveScale.Evaluate(percent));
            yield return null;
        }
    }
}
