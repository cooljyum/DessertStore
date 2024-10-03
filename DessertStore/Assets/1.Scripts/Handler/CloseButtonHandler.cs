using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseButtonHandler : MonoBehaviour
{
    public PanelHandler popupWindow;

    public void OnButtonClick()
    {
        StartCoroutine(ScaleButtonAndClose());
    }

    private IEnumerator ScaleButtonAndClose()
    {
        // 0.95f로 줄임
        yield return StartCoroutine(ScaleOverTime(transform, Vector3.one, Vector3.one * 0.95f, 0.1f));
        // 1.05f로 늘림
        yield return StartCoroutine(ScaleOverTime(transform, Vector3.one * 0.95f, Vector3.one * 1.05f, 0.1f));
        // 1f로 되돌림
        yield return StartCoroutine(ScaleOverTime(transform, Vector3.one * 1.05f, Vector3.one, 0.1f));

        // 버튼 애니메이션이 끝난 후 창을 닫음
        popupWindow.Hide();
    }

    private IEnumerator ScaleOverTime(Transform target, Vector3 startScale, Vector3 endScale, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            target.localScale = Vector3.Lerp(startScale, endScale, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        target.localScale = endScale; // 정확한 끝값으로 설정
    }
}
