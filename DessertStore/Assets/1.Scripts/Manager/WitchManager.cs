using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WitchManager : MonoBehaviour
{
    [SerializeField] private Image _witchFaceImage;
    public Image WitchFaceImage => _witchFaceImage;

    // ���� ǥ�� �̹��� ���� //
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
