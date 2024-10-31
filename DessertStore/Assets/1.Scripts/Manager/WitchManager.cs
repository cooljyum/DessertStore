using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WitchManager : MonoBehaviour
{
    [SerializeField] private Image _witchFaceImage;
    public Image WitchFaceImage => _witchFaceImage;

    // 마녀 표정 이미지 세팅 //
    public void ChangeWitchFace(string expression)
    {
        Sprite newFace = Resources.Load<Sprite>($"Sprite/WitchFace/{expression}");

        if (expression == "neutral")
        {
            _witchFaceImage.sprite = null;
        }
        else
        {
            _witchFaceImage.sprite = Resources.Load<Sprite>(expression);
        }
    }
}
