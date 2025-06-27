// RoomSelect.cs
using UnityEngine;
using UnityEngine.UI;

public class RoomSelect : MonoBehaviour
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

    private void Start()
    {
        // 初始化UI管理器
        RoomUIManager uiManager = gameObject.AddComponent<RoomUIManager>();
        uiManager.SelectRoom = SelectRoom;
        uiManager.RoomName = RoomName;
        uiManager.RoomPassWord = RoomPassWord;
        uiManager.RoomDiff = RoomDiff;
        uiManager.WmazeType = WmazeType;
        uiManager.HmazeType = HmazeType;
        uiManager.nightmodel = nightmodel;
        uiManager.TimerMode = TimerMode;
        uiManager.CreateRoomButton = CreateRoomButton;
        uiManager.JoinRoomPassword = JoinRoomPassword;
        uiManager.JoinRoomButton = JoinRoomButton;
        uiManager.RoomInfoText = RoomInfoText;
        uiManager.Message = Message;
        uiManager.RoomPanel = RoomPanel;
        uiManager.Vpassword = Vpassword;
        uiManager.father = father;
        uiManager.PlayInfo = PlayInfo;
        uiManager.Playerfather = Playerfather;
        uiManager.roomName = roomName;
        uiManager.LevelRoom = LevelRoom;
        uiManager.StartRoom = StartRoom;
        uiManager.Exit = Exit;
        uiManager.cancel = cancel;
        uiManager.confirm = confirm;
        uiManager.Room = Room;
        uiManager.OnlineHall = OnlineHall;
        uiManager.MainSetImg = MainSetImg;

        if (NetworkManager.Instance == null)
        {
            GameObject networkManagerObj = new GameObject("NetworkManager");
            networkManagerObj.AddComponent<NetworkManager>();
        }

        if (RoomManager.Instance == null)
        {
            GameObject roomManagerObj = new GameObject("RoomManager");
            roomManagerObj.AddComponent<RoomManager>();
        }
    }
}