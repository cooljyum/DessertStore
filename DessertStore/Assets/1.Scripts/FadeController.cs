using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FadeController : MonoBehaviour
{
    private Image fadeImage;

    // ���̵� �� �Լ�
    public void FadeIn(float duration)
    {
        if (fadeImage == null) InstantiateFadeOverlay();

        StartCoroutine(Fade(1, 0, duration));
    }

    // ���̵� �ƿ� �Լ�
    public void FadeOut(float duration)
    {
        if (fadeImage == null) InstantiateFadeOverlay();

        StartCoroutine(Fade(0, 1, duration));
    }

    private void InstantiateFadeOverlay()
    {
        GameObject overlayInstance = Instantiate(fadeOverlayPrefab);
        fadeImage = overlayInstance.GetComponent<Image>();
        overlayInstance.transform.SetParent(GameObject.Find("Canvas").transform, false); // Canvas�� ���̱�
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

        // ���̵� �� �� �̹��� ����
        if (endAlpha == 0)
        {
            Destroy(fadeImage.gameObject);
            fadeImage = null;
        }
    }
}