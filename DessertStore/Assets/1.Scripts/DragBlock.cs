using System.Collections;
using UnityEngine;

public class DragBlock : MonoBehaviour
{
    [SerializeField] private AnimationCurve _curveMovement; // 이동 제어 그래프
    [SerializeField] private AnimationCurve _curveScale;    // 크기 제어 그래프

    private float _appearTime = 0.5f;  // 블록 등장 소요 시간
    private float _returnTime = 0.1f;  // 블록 원래 위치 돌아갈 때 소요 시간

    [SerializeField] private ItemData _itemData; //아이템 데이터

    private Vector2 _parentPosition;

    [field: SerializeField] public Vector2Int BlockCount { private set; get; } // 블럭카운트
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
        // 코루틴 호출 시 문자열이 아닌 메서드를 직접 호출
        StopCoroutine(OnScaleTo(Vector3.one));
        StartCoroutine(OnScaleTo(Vector3.one * 1.3f));
    }

    private void OnMouseDrag()
    {
        // 마우스의 z축 깊이 설정
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
        // 코루틴 호출 시 문자열이 아닌 메서드를 직접 호출
        StopCoroutine(OnScaleTo(Vector3.one * 1.3f));
        StartCoroutine(OnScaleTo(Vector3.one));

        //StartCoroutine(OnMoveTo(_parentPosition, _returnTime));
    }

    /// <summary>
    /// 현재 위치에서 end 위치까지 time 시간동안 이동
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
