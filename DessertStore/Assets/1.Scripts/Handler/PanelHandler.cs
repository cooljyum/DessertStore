using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelHandler : MonoBehaviour
{
    private void Start()
    {
        // transform 의 scale 값을 모두 0.1f로 변경합니다.
        transform.localScale = Vector3.one * 0.1f;
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
        StartCoroutine(ScaleOverTime(transform, Vector3.one * 1.1f, Vector3.one, 0.3f));
    }

    public void Hide()
    {
        StartCoroutine(ScaleOverTime(transform, Vector3.one * 1.1f, Vector3.one * 0.2f, 0.3f, () =>
        {
            gameObject.SetActive(false);
        }));
    }

    private IEnumerator ScaleOverTime(Transform target, Vector3 startScale, Vector3 endScale, float duration, System.Action onComplete = null)
    {
        float elapsedTime = 0f;
        Vector3 initialScale = target.localScale;

        // 첫 번째 단계로 startScale로 확대
        while (elapsedTime < duration / 2)
        {
            target.localScale = Vector3.Lerp(initialScale, startScale, (elapsedTime / (duration / 2)));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 두 번째 단계로 endScale로 축소
        elapsedTime = 0f;
        initialScale = target.localScale;
        while (elapsedTime < duration / 2)
        {
            target.localScale = Vector3.Lerp(initialScale, endScale, (elapsedTime / (duration / 2)));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 완료 시 콜백 실행
        onComplete?.Invoke();
    }
}
