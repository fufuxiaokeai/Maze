using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnlineUISet : MonoBehaviour
{
    public Image SetOnlineImg;
    public Button ExitButton;
    public Button OnlineButton;

    private void Awake()
    {
        SetOnlineImg.gameObject.SetActive(false);
        ExitButton.onClick.AddListener(OnExitButtonClick);
        OnlineButton.onClick.AddListener(OnOnlineButtonClick);
    }

    private void OnExitButtonClick()
    {
        SetOnlineImg.gameObject.SetActive(false);
    }

    private void OnOnlineButtonClick()
    {
        SetOnlineImg.gameObject.SetActive(true);
    }

}
