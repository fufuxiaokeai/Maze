using MySql.Data.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[Serializable]
public class HistReco
{
    public string DATE;
    public string time;
    public string contact;
    public string spendTime;
}

[Serializable]
public class HistRecos
{
    public HistReco[] HistoricalRecords;
}

public class MainSetHR : MonoBehaviour
{
    [Header("个人历史记录")]
    public Button HistoricalRecordsButton;
    public Button HistoricalRecordsExitButton;
    public Image HRImg;
    public Toggle nightToggle;
    public Dropdown difficulty;
    public Dropdown WmazeType;
    public Dropdown HmazeType;

    public GameObject HRPanel;
    public Transform father;
    public Text title;

    private int activeRequestId = 0;
    public float showInterval = 60f;

    private void Awake()
    {
        HRImg.gameObject.SetActive(false);
        HistoricalRecordsButton.onClick.AddListener(() => HRImg.gameObject.SetActive(true));
        HistoricalRecordsExitButton.onClick.AddListener(() => HRImg.gameObject.SetActive(false));
        title.text = "独属于" + PublicVar.playerName + "的历史记录";
        difficulty.options.Clear();
        difficulty.options.Add(new Dropdown.OptionData("简单"));//DFS
        difficulty.options.Add(new Dropdown.OptionData("普通"));//Prim
        difficulty.options.Add(new Dropdown.OptionData("困难"));//Kruskal
        difficulty.options.Add(new Dropdown.OptionData("地狱"));//在Kruskal的基础上增加了迷雾
        difficulty.value = 0;
        difficulty.onValueChanged.AddListener(OnDifficultyChanged);
        OnDifficultyChanged(difficulty.value);
        difficulty.onValueChanged.AddListener(RefreshLeaderboard);
        nightToggle.onValueChanged.AddListener((isOn) => RefreshLeaderboard(0));
        WmazeType.onValueChanged.AddListener(RefreshLeaderboard);
        HmazeType.onValueChanged.AddListener(RefreshLeaderboard);
        StartCoroutine(RepeatShowAddFriendBlock());
    }

    IEnumerator RepeatShowAddFriendBlock()
    {
        while (true)
        {
            yield return new WaitForSeconds(showInterval);
            string selectedDifficulty = difficulty.options[difficulty.value].text;
            int width = int.Parse(WmazeType.options[WmazeType.value].text);
            int height = int.Parse(HmazeType.options[HmazeType.value].text);
            StartCoroutine(UpdataChats(selectedDifficulty, width, height));
        }
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

    private void UpdateDropdownOptions(Dropdown dropdown, List<string> options)
    {
        dropdown.ClearOptions();
        dropdown.AddOptions(options);
        if (options.Count > 0)
        {
            dropdown.value = 0;
        }
    }

    private void RefreshLeaderboard(int _)
    {
        string selectedDifficulty = difficulty.options[difficulty.value].text;
        int width = int.Parse(WmazeType.options[WmazeType.value].text);
        int height = int.Parse(HmazeType.options[HmazeType.value].text);
        StartCoroutine(UpdataChats(selectedDifficulty, width, height));
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
            night = nightToggle.isOn ? 1 : 0,
            difficultyLevel = difficultyLevel,
        };

        string json = JsonUtility.ToJson(data);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);

        using (UnityWebRequest req = new UnityWebRequest($"{PublicVar.ServerUrl}/data/HistoricalRecords", "POST"))
        {
            req.uploadHandler = new UploadHandlerRaw(bodyRaw);
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");
            yield return req.SendWebRequest();

            if (requestId != activeRequestId) yield break;

            if (req.result == UnityWebRequest.Result.Success)
            {
                HistRecos HR = JsonUtility.FromJson<HistRecos>(req.downloadHandler.text);
                foreach (var Hist in HR.HistoricalRecords)
                {
                    GameObject RecordCard = Instantiate(HRPanel, father);
                    DateTime dateTime = DateTime.Parse(Hist.DATE);
                    RecordCard.transform.Find("DATE").GetComponent<TMP_Text>().text = dateTime.ToString("yyyy-MM-dd HH:mm:ss");
                    Debug.Log(dateTime.ToString("yyyy-MM-dd HH:mm:ss"));
                    RecordCard.transform.Find("time").GetComponent<TMP_Text>().text = "用时:" + Hist.time;
                    RecordCard.transform.Find("contact").GetComponent<TMP_Text>().text = "碰壁:" + Hist.contact + "次";
                    RecordCard.transform.Find("spendTime").GetComponent<TMP_Text>().text = "花费:" + Hist.spendTime + "秒";
                }
            }

        }

    }


}
