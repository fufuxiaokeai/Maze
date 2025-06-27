using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[System.Serializable]
public class heartbeat
{
    public string name;
}

[System.Serializable]
public class playerchart
{
    public string name;
    public int UID;
    public string time;
    public string spendTime;
    public int contact;
}

[System.Serializable]
public class PChartResponse
{
    public bool exists;
    public string message;
    public playerchart[] player_chart;
}

[System.Serializable]
public class Data
{
    public string name;
    public int UID;
    public int Width;
    public int Height;
    public int night = 0; // Ĭ��Ϊ����
    public string difficultyLevel;
    public bool online;
}

public class charts : MonoBehaviour
{
    public Image chartsImg;

    public Toggle nightToggle;
    public Toggle lineToggle;

    public Button ChartsExitButton;
    public Button ChartsOpenButton;

    public Dropdown difficulty;
    public Dropdown WmazeType;
    public Dropdown HmazeType;

    public GameObject chartsPanel;
    public Transform father; // ���ڴ�����а����ݵĸ�����

    private bool IsShow = false; // �Ƿ���ʾ���а����
    private float lastHeartbeat = 0f;
    private int activeRequestId = 0;

    private void Awake()
    {
        ServicePointManager.ServerCertificateValidationCallback = MyRemoteCertificateValidationCallback;
        chartsImg.gameObject.SetActive(false);
        PublicVar.IsChart = false; // ��ʼ�����а�״̬
        difficulty.options.Clear();
        difficulty.options.Add(new Dropdown.OptionData("��"));//DFS
        difficulty.options.Add(new Dropdown.OptionData("��ͨ"));//Prim
        difficulty.options.Add(new Dropdown.OptionData("����"));//Kruskal
        difficulty.options.Add(new Dropdown.OptionData("����"));//��Kruskal�Ļ���������������
        difficulty.value = 0;
        difficulty.onValueChanged.AddListener(OnDifficultyChanged);
        OnDifficultyChanged(difficulty.value);
        difficulty.onValueChanged.AddListener(RefreshLeaderboard);
        lineToggle.onValueChanged.AddListener((isOn) => RefreshLeaderboard(0));
        nightToggle.onValueChanged.AddListener((isOn) => RefreshLeaderboard(0));
        WmazeType.onValueChanged.AddListener(RefreshLeaderboard);
        HmazeType.onValueChanged.AddListener(RefreshLeaderboard);
    }
    private static bool MyRemoteCertificateValidationCallback(System.Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
    {
        // ��������֤�飨������Ч/��ǩ��֤�飩
        return true;
    }

    private IEnumerator SendHeartbeat()
    {
        heartbeat data = new heartbeat
        {
            name = PublicVar.playerName
        };

        string json = JsonUtility.ToJson(data);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);

        using (UnityWebRequest req = new UnityWebRequest($"{PublicVar.ServerUrl}/heartbeat", "POST"))
        {
            req.uploadHandler = new UploadHandlerRaw(bodyRaw);
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");
            yield return req.SendWebRequest();
        }
    }

    private void RefreshLeaderboard(int _)
    {
        string selectedDifficulty = difficulty.options[difficulty.value].text;
        int width = int.Parse(WmazeType.options[WmazeType.value].text);
        int height = int.Parse(HmazeType.options[HmazeType.value].text);
        StartCoroutine(UpdataChats(selectedDifficulty, width, height));
    }

    private void OnDifficultyChanged(int index)
    {
        string selectedDifficulty = difficulty.options[index].text;
        UpdateMazeSizeOptions(selectedDifficulty);
    }

    private void UpdateMazeSizeOptions(string difficultyLevel)
    {
        List<string> widthOptions = new List<string>();
        List<string> heightOptions = new List<string>();
        switch (difficultyLevel)
        {
            case "��":
                widthOptions = new List<string> { "10", "20", "30" };
                heightOptions = new List<string> { "10", "20", "30" };
                break;
            case "��ͨ":
                widthOptions = new List<string> { "40", "50", "60" };
                heightOptions = new List<string> { "40", "50", "60" };
                break;
            case "����":
                widthOptions = new List<string> { "70", "80", "90", "100" };
                heightOptions = new List<string> { "70", "80", "90", "100" };
                break;
            case "����":
                widthOptions = new List<string> { "100", "500", "1000" };
                heightOptions = new List<string> { "100", "500", "1000" };
                break;
        }
        UpdateDropdownOptions(WmazeType, widthOptions);
        UpdateDropdownOptions(HmazeType, heightOptions);
        RefreshLeaderboard(0);
    }

    private IEnumerator UpdataChats(string difficultyLevel, int width, int height)
    {
        int requestId = ++activeRequestId;
        foreach (Transform child in father)
        {
            Destroy(child.gameObject);
        }

        var name = PublicVar.playerName;
        var UID = PublicVar.FormattedUID;

        Data data = new Data
        {
            name = name,
            UID = int.Parse(UID),
            Width = width * 3,
            Height = height * 3,
            night = nightToggle.isOn ? 1 : 0, // ����ҹ��ģʽ����ֵ
            difficultyLevel = difficultyLevel,
            online = lineToggle.isOn // ��������ģʽ����ֵ
        };

        string json = JsonUtility.ToJson(data);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);

        using (UnityWebRequest req = new UnityWebRequest($"{PublicVar.ServerUrl}/data/the_charts", "POST"))
        {
            req.uploadHandler = new UploadHandlerRaw(bodyRaw);
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");
            yield return req.SendWebRequest();

            if (requestId != activeRequestId) yield break;

            if (req.result == UnityWebRequest.Result.ConnectionError ||
                req.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Error: {req.error}");
            }
            else
            {
                if(req.responseCode == 200)
                {
                    PChartResponse response = JsonUtility.FromJson<PChartResponse>(req.downloadHandler.text);
                    if (!string.IsNullOrEmpty(response.message))
                    {
                        Debug.Log($"���������ش���: {response.message}");
                    }
                    else
                    {
                        if (response.exists)
                        {
                            Debug.Log("���а������Ѵ��ڣ����³ɹ���");
                            foreach (var player in response.player_chart)
                            {
                                GameObject obj = Instantiate(chartsPanel, father);
                                obj.transform.Find("Name").GetComponent<Text>().text = player.name;
                                obj.transform.Find("UID").GetComponent<Text>().text = player.UID.ToString();
                                obj.transform.Find("time").GetComponent<Text>().text = player.time;
                                obj.transform.Find("contact").GetComponent<Text>().text = player.contact.ToString();
                            }
                        }
                    }
                }
                else
                {
                    Debug.Log("���а����ݲ����ڣ������ǵ�һ�β�ѯ������δ���ɡ������������ݡ�");
                }
            }
        }
    }

    private void UpdateDropdownOptions(Dropdown dropdown, List<string> options)
    {
        dropdown.ClearOptions();
        dropdown.AddOptions(options);
        if (options.Count > 0)
        {
            dropdown.value = 0;
        }
    }

    private void Start()
    {
        ChartsExitButton.onClick.AddListener(Exit);
        ChartsOpenButton.onClick.AddListener(Open);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.C))
        {
            IsShow = true;
        }
        if(IsShow)
        {
            Open(); // ������а������Ҫ��ʾ��������ʾ����
        }
        // ÿ2�뷢��һ������
        if (Time.time - lastHeartbeat > 20f)
        {
            StartCoroutine(SendHeartbeat());
            lastHeartbeat = Time.time;
        }
    }

    private void Exit()
    {
        chartsImg.gameObject.SetActive(false);
        IsShow = false; // �ر����а����ʱ����״̬
        PublicVar.IsChart = false;
    }

    private void Open()
    {
        chartsImg.gameObject.SetActive(true);
        PublicVar.IsChart = true;
    }


}


