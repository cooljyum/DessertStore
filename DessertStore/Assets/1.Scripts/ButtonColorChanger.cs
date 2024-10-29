using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonColorChanger : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private Image buttonImage;
    public Color pressedColor; // 클릭할 때 색상
    [SerializeField] private Color originalColor;

    void Start()
    {
        buttonImage = GetComponent<Image>();
        originalColor = buttonImage.color; // 원래 색상 저장
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        buttonImage.color = pressedColor; // 클릭 시 색상 변경
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        buttonImage.color = originalColor; // 원래 색상으로 복원
    }
}