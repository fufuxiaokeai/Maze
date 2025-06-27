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
    public int night = 0; // 默认为白天
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
    public Transform father; // 用于存放排行榜数据的父物体

    private bool IsShow = false; // 是否显示排行榜界面
    private float lastHeartbeat = 0f;
    private int activeRequestId = 0;

    private void Awake()
    {
        ServicePointManager.ServerCertificateValidationCallback = MyRemoteCertificateValidationCallback;
        chartsImg.gameObject.SetActive(false);
        PublicVar.IsChart = false; // 初始化排行榜状态
        difficulty.options.Clear();
        difficulty.options.Add(new Dropdown.OptionData("简单"));//DFS
        difficulty.options.Add(new Dropdown.OptionData("普通"));//Prim
        difficulty.options.Add(new Dropdown.OptionData("困难"));//Kruskal
        difficulty.options.Add(new Dropdown.OptionData("地狱"));//在Kruskal的基础上增加了迷雾
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
        // 允许所有证书（包括无效/自签名证书）
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
            case "简单":
                widthOptions = new List<string> { "10", "20", "30" };
                heightOptions = new List<string> { "10", "20", "30" };
                break;
            case "普通":
                widthOptions = new List<string> { "40", "50", "60" };
                heightOptions = new List<string> { "40", "50", "60" };
                break;
            case "困难":
                widthOptions = new List<string> { "70", "80", "90", "100" };
                heightOptions = new List<string> { "70", "80", "90", "100" };
                break;
            case "地狱":
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
            night = nightToggle.isOn ? 1 : 0, // 根据夜晚模式设置值
            difficultyLevel = difficultyLevel,
            online = lineToggle.isOn // 根据在线模式设置值
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
                        Debug.Log($"服务器返回错误: {response.message}");
                    }
                    else
                    {
                        if (response.exists)
                        {
                            Debug.Log("排行榜数据已存在，更新成功！");
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
                    Debug.Log("排行榜数据不存在，可能是第一次查询或数据未生成。请先生成数据。");
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
            Open(); // 如果排行榜界面需要显示，调用显示方法
        }
        // 每2秒发送一次心跳
        if (Time.time - lastHeartbeat > 20f)
        {
            StartCoroutine(SendHeartbeat());
            lastHeartbeat = Time.time;
        }
    }

    private void Exit()
    {
        chartsImg.gameObject.SetActive(false);
        IsShow = false; // 关闭排行榜界面时重置状态
        PublicVar.IsChart = false;
    }

    private void Open()
    {
        chartsImg.gameObject.SetActive(true);
        PublicVar.IsChart = true;
    }


}


