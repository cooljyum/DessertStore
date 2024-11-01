using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopUpManager : MonoBehaviour
{
    [SerializeField] private GameObject _startPanel;
    [SerializeField] private GameObject _statusPanel;
    [SerializeField] private GameObject _endPanel;

    private void Start()
    {
        HideAllPanels();
        AssignOkButtonEvents();
    }

    // daystart 패널 활성화 //
    public void ShowDayStartPanel(int currentDay)
    {
        _startPanel.SetActive(true);

        TextMeshProUGUI dayText = _startPanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        dayText.text = $"Day{UIManager.Instance.CurrentDayNumber}";
    }

    // orderstatus 패널 활성화 //
    public void ShowOrderStatusPanel(bool orderStatus)
    {
        _statusPanel.SetActive(true);

        TextMeshProUGUI statusText = _statusPanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        statusText.text = orderStatus ? "주문 처리 성공" : "주문 처리 실패";
    }

    // dayend 패널 활성화 //
    public void ShowDayEndPanel()
    {
        _endPanel.SetActive(true);

        TextMeshProUGUI resultText = _endPanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        resultText.text = $"성공한 주문 {UIManager.Instance.SuccessCount}\n실패한 주문 {UIManager.Instance.FailureCount}\n오늘의 별점 {UIManager.Instance.StagePanel.StarPointByStage[UIManager.Instance.CurrentDayNumber]}";
    }

    // 패널 모두 비활성화 //
    public void HideAllPanels()
    {
        _startPanel.SetActive(false);
        _statusPanel.SetActive(false);
        _endPanel.SetActive(false);
    }

    // 확인 버튼 클릭 시 부모 패널 비활성화 //
    public void HidePopUpPanel(Button okButton)
    {
        GameObject parentPanel = okButton.transform.parent.gameObject;
        if (parentPanel != null)
        {
            parentPanel.SetActive(false);
        }
    }

    // 확인 버튼 이벤트 할당 //
    private void AssignOkButtonEvents()
    {
        Button startOkButton = _startPanel.GetComponentInChildren<Button>();
        if (startOkButton != null)
        {
            startOkButton.onClick.AddListener(() =>
            {
                HidePopUpPanel(startOkButton);            // start 패널 비활성화
                UIManager.Instance.StartDayTime();        // StartDayTime 호출
            });
        }

        Button statusOkButton = _statusPanel.GetComponentInChildren<Button>();
        if (statusOkButton != null)
        {
            statusOkButton.onClick.AddListener(() =>
            {
                HidePopUpPanel(statusOkButton);                 // status 패널 비활성화
                UIManager.Instance.PosPanel.CreateNewOrder();   // CreateNewOrder 호출
            });
        }

        Button endOkButton = _endPanel.GetComponentInChildren<Button>();
        if (endOkButton != null)
        {
            endOkButton.onClick.AddListener(() =>
            {
                HidePopUpPanel(endOkButton);               // end 패널 비활성화
                UIManager.Instance.PrepareForNextDay();    // PrepareForNextDay 호출
            });
        }
    }

    public bool IsDayStartPanelActive()
    {
        return _startPanel.activeSelf;
    }

    public bool IsOrderStatusPanelActive()
    {
        return _statusPanel.activeSelf;
    }

    public bool IsDayEndPanelActive()
    {
        return _endPanel.activeSelf;
    }
}