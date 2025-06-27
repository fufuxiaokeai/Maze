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
        // ��ʼ������UI״̬
        UISetBackground.gameObject.SetActive(false);
        UIGameExitBackGround.gameObject.SetActive(false);
        UIUID.gameObject.SetActive(true);
        TimeText.gameObject.SetActive(true);

        // ��ʼ����ť״̬
        SetButton.interactable = true;
        UIchat.interactable = true;
        UIRLButton.interactable = true;
        UIAchievementButton.interactable = true;

        // ����UID�ı�
        UIUID.text = "UID��" + PublicVar.FormattedUID;
        username.text = PublicVar.playerName; // �����û���
        UID.text = PublicVar.FormattedUID; // ����UID
    }

    void SetupEventListeners()
    {
        // ���ð�ť����
        UISetUp = transform.Find("SetUp");
        UISetUp.GetComponent<Button>().onClick.AddListener(OpenSettings);

        // �˳���Ϸ��ؼ���
        UIGameExit.GetComponent<Button>().onClick.AddListener(ShowExitConfirmation);
        cancelButton.onClick.AddListener(CloseExitConfirmation);
        sureButton.onClick.AddListener(SureExitGame);
        returnButton.onClick.AddListener(ReturnGame);
        AgainButton.onClick.AddListener(AgainGame);

        // ���ȫ��ESC������
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

    // Э�̼���ESC��
    IEnumerator EscapeKeyMonitor()
    {
        while (true)
        {
            if(PublicVar.IsEnd||PublicVar.IsOnlineEnd) yield break; // ����Ѿ�������ֱ�ӷ���
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ToggleSettings();
            }
            yield return null;
        }
    }

    void OpenSettings()
    {
        // �����ý���
        UISetBackground.gameObject.SetActive(true);
        UpdateSettingsUIState(false);
        PublicVar.IsShow = true;
        PublicVar.Isconnect = true;
    }

    void CloseSettings()
    {
        // �ر����ý���
        UISetBackground.gameObject.SetActive(false);
        UpdateSettingsUIState(true);
        PublicVar.IsShow = false;
        PublicVar.Isconnect = false;
    }

    void ToggleSettings()
    {
        if (UISetBackground.gameObject.activeSelf)
        {
            print("�ر����ý���");
            CloseSettings();
        }
        else
        {
            print("�����ý���");
            OpenSettings();
        }
    }

    void UpdateSettingsUIState(bool state)
    {
        // �����������UI״̬
        UIUID.gameObject.SetActive(state);
        TimeText.gameObject.SetActive(state);
        UIchat.interactable = state;
        PublicVar.Isconnect = !state;
    }

    void ShowExitConfirmation()
    {
        // ��ʾ�˳�ȷ�����
        UIGameExitBackGround.gameObject.SetActive(true);
        SetButton.interactable = false;
        UIchatButton.interactable = false;
        UIRLButton.interactable = false;
        UIAchievementButton.interactable = false;
    }

    void CloseExitConfirmation()
    {
        // �ر��˳�ȷ�����
        UIGameExitBackGround.gameObject.SetActive(false);
        SetButton.interactable = true;
        UIchatButton.interactable = true;
        UIRLButton.interactable = true;
        UIAchievementButton.interactable = true;
    }

    void SureExitGame()
    {
        // ʵ���˳��߼�
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Debug.Log("�˳���Ϸ");
    }
}
