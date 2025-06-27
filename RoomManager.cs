using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    private static RoomManager _instance;
    public static RoomManager Instance => _instance;

    public string currentRoomId = "";
    public Dictionary<string, RoomInfo> roomList = new Dictionary<string, RoomInfo>();

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

        // ע��������Ϣ����
        NetworkManager.Instance.OnMessageReceived += HandleWebSocketMessage;
    }

    public void RequestRoomList(string roomName)
    {
        MessageData request = new MessageData { type = "getRoomList",roomName = roomName };
        NetworkManager.Instance.Send(JsonUtility.ToJson(request));
    }

    public void RequestPlayerList()
    {
        if (string.IsNullOrEmpty(currentRoomId)) return;

        RoomInfo req = new RoomInfo { type = "getPlayerList", roomId = currentRoomId };
        NetworkManager.Instance.Send(JsonUtility.ToJson(req));
    }

    public void SendAuth()
    {
        if (!string.IsNullOrEmpty(PublicVar.token))
        {
            MessageData authData = new MessageData
            {
                type = "auth",
                token = PublicVar.token
            };
            NetworkManager.Instance.Send(JsonUtility.ToJson(authData));
        }
    }

    public void CreateRoom(string roomName, string roomPassword, string roomDiff,
                          int Wmaze, int Hmaze, int IsnightModel, int IsTimerModel)
    {
        RoomInfo roomInfo = new RoomInfo
        {
            roomName = roomName,
            roomId = PublicVar.FormattedUID, // ʹ����ҵ�UID��Ϊ����ID
            roomPassword = string.IsNullOrEmpty(roomPassword) ? null : roomPassword,
            roomDiff = roomDiff,
            Wmaze = Wmaze,
            Hmaze = Hmaze,
            IsnightModel = IsnightModel,
            IsTimerModel = IsTimerModel,
            authorName = PublicVar.playerName, // ��������
            authorId = PublicVar.UID, //����ID
            MaxPlayers = 4,
            CurrentPlayers = 1
        };

        MessageData createRoomData = new MessageData
        {
            type = "createRoom",
            room = roomInfo // ��������Ϣ����room�ֶ�
        };

        NetworkManager.Instance.Send(JsonUtility.ToJson(createRoomData));
    }

    public void JoinRoom(string roomId, string roomPassword)
    {
        if (!roomList.TryGetValue(roomId, out RoomInfo roomInfo))
        {
            Debug.LogError("�޷��ҵ�����: " + roomId);
            return;
        }

        if (!string.IsNullOrEmpty(roomInfo.roomPassword) && roomInfo.roomPassword != roomPassword)
        {
            Debug.Log("�����������");
            return;
        }

        MessageData joinRoomData = new MessageData
        {
            type = "joinRoom",
            room = roomInfo,
            token = PublicVar.token,
            Password = roomPassword
        };

        NetworkManager.Instance.Send(JsonUtility.ToJson(joinRoomData));
    }

    public void Leaveroom()
    {
        RoomInfo roomIdInfo = new RoomInfo { roomId = currentRoomId };
        MessageData data = new MessageData
        {
            type = "LeaveRoom",
            token = PublicVar.token,
            room = roomIdInfo
        };
        NetworkManager.Instance.Send(JsonUtility.ToJson(data));
    }

    public void Start_Game(string gameId)
    {
        RoomInfo room = new RoomInfo
        {
            type = "StartGame",
            roomId = gameId,
            mazeSeed = Random.Range(0, int.MaxValue)
        };
        NetworkManager.Instance.Send(JsonUtility.ToJson(room));
    }

    private void HandleWebSocketMessage(string message)
    {
        var data = JsonUtility.FromJson<MessageData>(message);
        switch (data.type)
        {
            case "roomCreated":
                HandleRoomCreated(data);
                break;
            case "roomList":
                HandleRoomList(data);
                break;
            case "joinRoomResult":
                RoomUIManager.Instance.HandleJoinRoomResult(data);
                break;
            case "StartRoomGame":
                RoomUIManager.Instance.HandIeStartGame(data);
                break;
            case "roomPlayerUpdate":
                RoomUIManager.Instance.UpdatePlayerListUI(data.playerLists);
                break;
            case "LeftHomeowner":
                RoomUIManager.Instance.HandleLeftHomeowner();
                break;
            case "PlayersLeave":
                RoomUIManager.Instance.HandlePlayersLeave(data);
                break;
            case "Player_Move":
                PlayerManager.instance.UpdatePlayerPosition(data.userId, message, data.userName);
                break;
            case "WinPlayerInfo":
                PlayerManager.instance.DelectPlayer(data.userId, message);
                PlayerManager.instance.Ranking(message);
                break;
        }
    }

    private void HandleRoomCreated(MessageData data)
    {
        if (string.IsNullOrEmpty(data.room.roomId))
        {
            Debug.LogError("���䴴��ʧ��: roomIdΪ��");
            return;
        }

        var roomInfo = data.room;
        currentRoomId = roomInfo.roomId;
        roomList[roomInfo.roomId] = roomInfo;

        RoomUIManager.Instance.HandleRoomCreated(roomInfo);
    }

    private void HandleRoomList(MessageData data)
    {
        if (data.rooms == null)
        {
            Debug.LogError("HandleRoomList: �����б�Ϊ��");
            return;
        }

        roomList.Clear();
        foreach (var room in data.rooms)
        {
            if (!string.IsNullOrEmpty(room.roomId))
            {
                roomList[room.roomId] = room;
            }
            else
            {
                Debug.LogWarning("������Ч����: roomIdΪ��");
            }
        }

        RoomUIManager.Instance.UpdateRoomListUI(roomList);
    }
}