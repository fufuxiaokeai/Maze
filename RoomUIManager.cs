using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RoomUIManager : MonoBehaviour
{
    // UI引用
    public InputField SelectRoom;
    public InputField RoomName;
    public InputField RoomPassWord;
    public Dropdown RoomDiff;
    public Dropdown WmazeType;
    public Dropdown HmazeType;
    public Toggle nightmodel;
    public Toggle TimerMode;
    public Button CreateRoomButton;
    public InputField JoinRoomPassword;
    public Button JoinRoomButton;
    public Text RoomInfoText;
    public Text Message;
    public GameObject RoomPanel;
    public Transform Vpassword;
    public Transform father;

    public GameObject PlayInfo;
    public Transform Playerfather;
    public Text roomName;
    public Button LevelRoom;
    public Button StartRoom;

    public Image Exit;
    public Button cancel;
    public Button confirm;

    public Image Room;
    public Image OnlineHall;
    public Image MainSetImg;

    private string[] identity = { "房主", "玩家2", "玩家3", "玩家4" };
    private int IsnightModel = 0; // 0表示白天，1表示夜晚
    private int IsTimerMode = 0; // 0表示不计时，1表示计时
    private static RoomUIManager _instance;
    public static RoomUIManager Instance => _instance;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ResetUIState()
    {
        if (MainSetImg != null)
        {
            MainSetImg.gameObject.SetActive(true);
        }
    }

    private void Start()
    {
        RoomInfoText.text = "";
        Message.text = "";
        roomName.text = "";
        StartRoom.interactable = false;
        Vpassword.gameObject.SetActive(false);
        Exit.gameObject.SetActive(false);
        MainSetImg.gameObject.SetActive(true);

        // 初始化难度下拉框
        RoomDiff.options.Clear();
        RoomDiff.options.Add(new Dropdown.OptionData("简单"));
        RoomDiff.options.Add(new Dropdown.OptionData("普通"));
        RoomDiff.options.Add(new Dropdown.OptionData("困难"));
        RoomDiff.options.Add(new Dropdown.OptionData("地狱"));
        RoomDiff.value = 0;

        // 添加监听器
        RoomDiff.onValueChanged.AddListener(OnDifficultyChanged);
        nightmodel.onValueChanged.AddListener(OnNightModelChanged);
        TimerMode.onValueChanged.AddListener(OnTimerModeChanged);
        CreateRoomButton.onClick.AddListener(CreateRoom);
        JoinRoomButton.onClick.AddListener(JoinRoom);
        LevelRoom.onClick.AddListener(LeaveroomUI);
        SelectRoom.onValueChanged.AddListener(OnInputValueChanged);
        StartRoom.onClick.AddListener(StartGame);

        // 初始化难度选项
        OnDifficultyChanged(RoomDiff.value);
    }

    public void CreateRoom()
    {
        if (string.IsNullOrEmpty(RoomName.text))
        {
            Debug.Log("房间名称不能为空");
            return;
        }

        string roomName = RoomName.text;
        string roomPassword = RoomPassWord.text;
        string roomDiff = RoomDiff.options[RoomDiff.value].text;
        int Wmaze = int.Parse(WmazeType.options[WmazeType.value].text);
        int Hmaze = int.Parse(HmazeType.options[HmazeType.value].text);

        RoomManager.Instance.CreateRoom(roomName, roomPassword, roomDiff,
                                      Wmaze, Hmaze, IsnightModel, IsTimerMode);

        // 清空输入框
        RoomName.text = "";
        RoomPassWord.text = "";
    }

    public void JoinRoom()
    {
        if (string.IsNullOrEmpty(RoomManager.Instance.currentRoomId))
        {
            Debug.Log("请先选择一个房间");
            return;
        }

        string roomPassword = JoinRoomPassword.text;
        RoomManager.Instance.JoinRoom(RoomManager.Instance.currentRoomId, roomPassword);
    }

    public void UpdateRoomListUI(Dictionary<string, RoomInfo> roomList)
    {
        foreach (Transform child in father)
        {
            Destroy(child.gameObject);
        }

        foreach (var room in roomList.Values)
        {
            GameObject roomItem = Instantiate(RoomPanel, father);
            roomItem.transform.Find("RoomName").GetComponent<Text>().text = "房间名：" + room.roomName;
            roomItem.transform.Find("diff").GetComponent<Text>().text = "难度：" + room.roomDiff;
            string Size = room.Wmaze + "x" + room.Hmaze;
            roomItem.transform.Find("Size").GetComponent<Text>().text = "大小：" + Size;
            roomItem.transform.Find("NOP").GetComponent<Text>().text = $"({room.CurrentPlayers}/{room.MaxPlayers})人";
            roomItem.transform.Find("author").GetComponent<Text>().text = "创建者：" + room.authorName;
            string nightmodel = room.IsnightModel == 1 ? "夜晚模式" : "白天模式";
            string timermodel = room.IsTimerModel == 1 ? "计时模式" : "非计时模式";
            roomItem.transform.Find("model").GetComponent<Text>().text = $"{nightmodel} | {timermodel}";

            Button roomButton = roomItem.GetComponent<Button>();
            if (roomButton == null)
            {
                roomButton = roomItem.AddComponent<Button>();
            }

            roomButton.onClick.AddListener(() => ShowRoomDetails(room));
        }
    }

    public void ShowRoomDetails(RoomInfo room)
    {
        RoomInfoText.text = $"房间名: {room.roomName}\t" +
                            $"难度: {room.roomDiff}\n" +
                            $"大小: {room.Wmaze}x{room.Hmaze}\t" +
                            $"当前人数: {room.CurrentPlayers}/{room.MaxPlayers}\n" +
                            $"创建者: {room.authorName}\n" +
                            $"夜晚模式: {(room.IsnightModel == 1 ? "是" : "否")}\n" +
                            $"计时模式: {(room.IsTimerModel == 1 ? "是" : "否")}";

        RoomManager.Instance.currentRoomId = room.roomId;
        roomName.text = room.roomName;
        Vpassword.gameObject.SetActive(!string.IsNullOrEmpty(room.roomPassword));
    }

    public void UpdatePlayerListUI(PlayerList[] playerLists)
    {
        int i = 0;
        foreach (Transform cli in Playerfather)
        {
            Destroy(cli.gameObject);
        }

        if (playerLists == null || playerLists.Length == 0)
        {
            Debug.LogWarning("收到空玩家列表");
            return;
        }

        foreach (var players in playerLists)
        {
            GameObject player = Instantiate(PlayInfo, Playerfather);
            player.transform.Find("Name").GetComponent<Text>().text = "姓名：" + players.userName;
            player.transform.Find("identity").GetComponent<Text>().text = identity[i++];

            if (players.userName == PublicVar.playerName)
            {
                player.GetComponent<Image>().color = new Color(0.8f, 1f, 0.8f);
            }
        }
    }

    public void HandleRoomCreated(RoomInfo roomInfo)
    {
        roomName.text = roomInfo.roomName;
        Debug.Log($"房间创建成功: {roomInfo.roomName}");
        StartRoom.interactable = true;
        OnlineHall.gameObject.SetActive(false);
        Room.gameObject.SetActive(true);
        RoomManager.Instance.RequestPlayerList();
    }

    public void HandleJoinRoomResult(MessageData data)
    {
        if (data.success)
        {
            RoomInfoText.text = "";
            JoinRoomPassword.text = "";
            OnlineHall.gameObject.SetActive(false);
            Room.gameObject.SetActive(true);
            RoomManager.Instance.RequestPlayerList();
        }
        else
        {
            Message.text = data.message;
        }
    }

    public void HandIeStartGame(MessageData data)
    {
        PublicVar.mazeSeed = data.mazeSeed;
        PublicVar.IsOnline = true;
        var room = data.room;
        PublicVar.difficulty = room.roomDiff;
        PublicVar.Wmaze = room.Wmaze;
        PublicVar.Hmaze = room.Hmaze;
        PublicVar.time = (room.IsTimerModel == 1);
        PublicVar.night = (room.IsnightModel == 1);
        Room.gameObject.SetActive(false);
        MainSetImg.gameObject.SetActive(false);
        if (PublicVar.night)
        {
            PublicVar.IsNight = 1;
            SceneManager.LoadScene("Night mode");
        }
        else
        {
            PublicVar.IsNight = 0;
            SceneManager.LoadScene("SampleScene");
        }
    }

    public void HandlePlayersLeave(MessageData data)
    {
        RoomManager.Instance.RequestPlayerList();
        if (data.userName == PublicVar.playerName)
        {
            Room.gameObject.SetActive(false);
            RoomManager.Instance.currentRoomId = "";
        }
    }

    public void HandleLeftHomeowner()
    {
        Room.gameObject.SetActive(false);
        RoomManager.Instance.currentRoomId = "";
    }

    // 其他UI相关方法...

    private void OnInputValueChanged(string text)
    {
        PublicVar.roomName = text;
        RoomManager.Instance.RequestRoomList(PublicVar.roomName);
    }

    private void OnNightModelChanged(bool isOn)
    {
        IsnightModel = isOn ? 1 : 0;
    }

    private void OnTimerModeChanged(bool isOn)
    {
        IsTimerMode = isOn ? 1 : 0;
    }

    private void OnDifficultyChanged(int index)
    {
        string selectedDifficulty = RoomDiff.options[index].text;
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

    // 离开房间相关UI方法
    public void LeaveroomUI()
    {
        Exit.gameObject.SetActive(true);
        LevelRoom.interactable = false;
        cancel.onClick.AddListener(cancelroom);
        confirm.onClick.AddListener(Leaveroom);
    }

    public void cancelroom()
    {
        Exit.gameObject.SetActive(false);
        LevelRoom.interactable = true;
    }

    public void Leaveroom()
    {
        Exit.gameObject.SetActive(false);
        LevelRoom.interactable = true;
        RoomManager.Instance.Leaveroom();
    }

    public void StartGame()
    {
        if (string.IsNullOrEmpty(RoomManager.Instance.currentRoomId))
        {
            Debug.Log("发生了错误，房间ID没了");
            return;
        }
        RoomManager.Instance.Start_Game(RoomManager.Instance.currentRoomId);
    }

}