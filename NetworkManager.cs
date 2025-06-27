using System.Collections.Concurrent;
using UnityEngine;
using WebSocketSharp;

[DefaultExecutionOrder(-10)] // ȷ�����ȳ�ʼ��
public class NetworkManager : MonoBehaviour
{
    private static NetworkManager _instance;
    public static NetworkManager Instance => _instance;

    public string serverAddress = "ws://127.0.0.1:8000";
    private WebSocket ws;
    private ConcurrentQueue<string> messageQueue = new ConcurrentQueue<string>();

    public event System.Action<string> OnMessageReceived;

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

    private void Start()
    {
        Connect();
    }

    private void Connect()
    {
        ws = new WebSocket(serverAddress);
        ws.OnOpen += (sender, e) => {
            Debug.Log("WebSocket���ӳɹ�");
            RoomManager.Instance.SendAuth();
            RoomManager.Instance.RequestRoomList(PublicVar.roomName);
        };
        ws.OnMessage += (sender, e) => {
            messageQueue.Enqueue(e.Data);
        };
        ws.OnClose += (sender, e) => {
            Debug.Log("WebSocket���ӹرգ�����: " + e.Code + "��ԭ��: " + e.Reason);
        };
        ws.OnError += (sender, e) => {
            Debug.LogError("WebSocket����: " + e.Message);
        };

        try
        {
            ws.Connect();
        }
        catch (System.Exception ex)
        {
            Debug.LogError("����ʧ��: " + ex.Message);
        }
    }

    public void Send(string message)
    {
        if (ws != null && ws.ReadyState == WebSocketState.Open)
        {
            ws.Send(message);
        }
    }

    private void Update()
    {
        while (messageQueue.TryDequeue(out string message))
        {
            OnMessageReceived?.Invoke(message);
        }
    }

    private void OnDestroy()
    {
        if (ws != null && ws.ReadyState == WebSocketState.Open)
        {
            ws.Close();
        }
    }
}