using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class UIMazeSetExit : MonoBehaviour
{
    public Button ExitButton;
    public Image ExitImage;
    public Button ReturnButton;
    public Button LogOnButton;
    public Button MainExitButton;
    public TMP_InputField inputField;

    public Button recordButton;
    public Button chartsButton;
    public Button achievementButton;
    public Button ChatButton;
    public Button StartButton;
    public Button RoomButton;
    public Button SetButton;

    private void Awake()
    {
        ExitImage.gameObject.SetActive(false);
    }

    private void Start()
    {
        ExitButton.onClick.AddListener(Exit);
    }

    void Exit()
    {
        inputField.gameObject.SetActive(false);
        ExitImage.gameObject.SetActive(true);
        recordButton.interactable = false;
        chartsButton.interactable = false;
        achievementButton.interactable = false;
        ChatButton.interactable = false;
        StartButton.interactable = false;
        RoomButton.interactable = false;
        SetButton.interactable = false;
        ReturnButton.onClick.AddListener(Return);
        LogOnButton.onClick.AddListener(Log_On);
        MainExitButton.onClick.AddListener(MainExit);
    }

    void Return()
    {
        inputField.gameObject.SetActive(true);
        ExitImage.gameObject.SetActive(false);
        recordButton.interactable = true;
        chartsButton.interactable = true;
        ChatButton.interactable = true;
        achievementButton.interactable = true;
        StartButton.interactable = true;
        RoomButton.interactable = true;
        SetButton.interactable = true;
    }

    void Log_On()
    {
        SceneManager.LoadScene("log on");
    }

    void MainExit()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Debug.Log("ÍË³öÓÎÏ·");
    }

}
