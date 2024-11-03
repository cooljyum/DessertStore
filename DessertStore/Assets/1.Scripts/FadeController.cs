using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeController : MonoBehaviour
{
    [SerializeField] private Image fadeImage;
    [SerializeField] private float startFadeDuration = 1f; // 시작 시 페이드아웃 지속 시간 설정

    private void Start()
    {
        // 시작 시 페이드아웃
        FadeIn(startFadeDuration);
    }

    // 페이드 인 함수
    public void FadeIn(float duration)
    {
        if (fadeImage == null) InstantiateFadeOverlay();

        StartCoroutine(Fade(1, 0, duration));
    }

    // 페이드 아웃 함수
    public void FadeOut(float duration)
    {
        if (fadeImage == null) InstantiateFadeOverlay();

        StartCoroutine(Fade(0, 1, duration));
    }

    private void InstantiateFadeOverlay()
    {
        // fadeImage가 null일 경우 오버레이 이미지 생성 및 Canvas에 추가
        GameObject overlayInstance = new GameObject("FadeOverlay");
        fadeImage = overlayInstance.AddComponent<Image>();
        overlayInstance.transform.SetParent(GameObject.Find("Canvas").transform, false);

        RectTransform rt = fadeImage.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.sizeDelta = Vector2.zero;
    }

    private IEnumerator Fade(float startAlpha, float endAlpha, float duration)
    {
        Color color = fadeImage.color;
        color.a = startAlpha;
        fadeImage.color = color;

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            color.a = Mathf.Lerp(startAlpha, endAlpha, t / duration);
            fadeImage.color = color;
            yield return null;
        }

        color.a = endAlpha;
        fadeImage.color = color;

        // 페이드 인 후 이미지 제거
        if (endAlpha == 0)
        {
            Destroy(fadeImage.gameObject);
            fadeImage = null;
        }
    }
}
