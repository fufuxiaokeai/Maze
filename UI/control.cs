using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class control : MonoBehaviour
{
    private Transform UISetUp;
    public Transform UISetBackground;

    public Button SetButton;

    public Transform UIGameExit;
    public Transform UIGameExitBackGround;
    public Button cancelButton;
    public Button sureButton;
    public Button returnButton;
    public Button AgainButton;

    public Button UIchat;
    public Button UIchatButton;

    public Button UIRLButton;

    public Button UIAchievementButton;

    public Text username;
    public Text UID;

    public Text UIUID;

    public Text TimeText;

    private void Awake()
    {
        PublicVar.IsShow = false;
        PublicVar.Isconnect = false;
        if (PublicVar.IsOnline)
        {
            returnButton.interactable = false;
            AgainButton.interactable = false;
        }
        InitializeUI();
        SetupEventListeners();
    }

    void InitializeUI()
    {
        // 初始化所有UI状态
        UISetBackground.gameObject.SetActive(false);
        UIGameExitBackGround.gameObject.SetActive(false);
        UIUID.gameObject.SetActive(true);
        TimeText.gameObject.SetActive(true);

        // 初始化按钮状态
        SetButton.interactable = true;
        UIchat.interactable = true;
        UIRLButton.interactable = true;
        UIAchievementButton.interactable = true;

        // 设置UID文本
        UIUID.text = "UID：" + PublicVar.FormattedUID;
        username.text = PublicVar.playerName; // 设置用户名
        UID.text = PublicVar.FormattedUID; // 设置UID
    }

    void SetupEventListeners()
    {
        // 设置按钮监听
        UISetUp = transform.Find("SetUp");
        UISetUp.GetComponent<Button>().onClick.AddListener(OpenSettings);

        // 退出游戏相关监听
        UIGameExit.GetComponent<Button>().onClick.AddListener(ShowExitConfirmation);
        cancelButton.onClick.AddListener(CloseExitConfirmation);
        sureButton.onClick.AddListener(SureExitGame);
        returnButton.onClick.AddListener(ReturnGame);
        AgainButton.onClick.AddListener(AgainGame);

        // 添加全局ESC键监听
        StartCoroutine(EscapeKeyMonitor());
    }

    private void ReturnGame()
    {
        RoomUIManager.Instance.ResetUIState();
        SceneManager.LoadScene("MainSet");
    }

    void AgainGame()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }

    // 协程监听ESC键
    IEnumerator EscapeKeyMonitor()
    {
        while (true)
        {
            if(PublicVar.IsEnd||PublicVar.IsOnlineEnd) yield break; // 如果已经结束，直接返回
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ToggleSettings();
            }
            yield return null;
        }
    }

    void OpenSettings()
    {
        // 打开设置界面
        UISetBackground.gameObject.SetActive(true);
        UpdateSettingsUIState(false);
        PublicVar.IsShow = true;
        PublicVar.Isconnect = true;
    }

    void CloseSettings()
    {
        // 关闭设置界面
        UISetBackground.gameObject.SetActive(false);
        UpdateSettingsUIState(true);
        PublicVar.IsShow = false;
        PublicVar.Isconnect = false;
    }

    void ToggleSettings()
    {
        if (UISetBackground.gameObject.activeSelf)
        {
            print("关闭设置界面");
            CloseSettings();
        }
        else
        {
            print("打开设置界面");
            OpenSettings();
        }
    }

    void UpdateSettingsUIState(bool state)
    {
        // 更新所有相关UI状态
        UIUID.gameObject.SetActive(state);
        TimeText.gameObject.SetActive(state);
        UIchat.interactable = state;
        PublicVar.Isconnect = !state;
    }

    void ShowExitConfirmation()
    {
        // 显示退出确认面板
        UIGameExitBackGround.gameObject.SetActive(true);
        SetButton.interactable = false;
        UIchatButton.interactable = false;
        UIRLButton.interactable = false;
        UIAchievementButton.interactable = false;
    }

    void CloseExitConfirmation()
    {
        // 关闭退出确认面板
        UIGameExitBackGround.gameObject.SetActive(false);
        SetButton.interactable = true;
        UIchatButton.interactable = true;
        UIRLButton.interactable = true;
        UIAchievementButton.interactable = true;
    }

    void SureExitGame()
    {
        // 实际退出逻辑
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Debug.Log("退出游戏");
    }
}
