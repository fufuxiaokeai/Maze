using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using WebSocketSharp;

[Serializable]
public class Message
{
    public string Name;
    public string message;
}

[Serializable]
public class PublicMessage
{
    public Message[] PublicHistory;
}

[Serializable]
public class info
{
    public string FormName;
    public string ToName;
    public string message;
}

[Serializable]
public class PrivateMessage
{
    public info[] PrivateHistory;
}

public class ChatClient : MonoBehaviour
{
    public InputField input;
    public Button sendButton;

    public GameObject My;
    public GameObject Other;

    public Transform father;

    private string type;
    public static ChatClient instance;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(instance);
        NetworkManager.Instance.OnMessageReceived += HandleWebSocketMessage;
    }

    private void Start()
    {
        StartCoroutine(PubliMess());
        sendButton.onClick.AddListener(SendMessage);
    }

    public IEnumerator PubliMess()
    {
        Token token = new Token { token = PublicVar.token };
        string jsonData = JsonUtility.ToJson(token);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);

        using (UnityWebRequest req = new UnityWebRequest($"{PublicVar.ServerUrl}/data/PublicMessage", "POST"))
        {
            req.uploadHandler = new UploadHandlerRaw(bodyRaw);
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json; charset=UTF-8");
            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
            {
                PublicMessage message = JsonUtility.FromJson<PublicMessage>(req.downloadHandler.text);
                foreach(var mess in message.PublicHistory)
                {
                    if(mess.Name == PublicVar.playerName)
                    {
                        GameObject my = Instantiate(My, father);
                        my.transform.Find("name").GetComponent<Text>().text = mess.Name;
                        my.transform.transform.Find("Text").GetComponent<Text>().text = mess.message;
                    }
                    else if(mess.Name != PublicVar.playerName)
                    {
                        GameObject other = Instantiate(Other, father);
                        other.transform.Find("nameP2").GetComponent<Text>().text = mess.Name;
                        other.transform.Find("Text").GetComponent<Text>().text = mess.message;
                    }
                }
            }

        }
    }

    private void HandleWebSocketMessage(string message)
    {
        var data = JsonUtility.FromJson<MessageData>(message);
        if (data.type == "public")
        {
            Debug.Log("收到消息: " + data.message + data.P_name);
            GameObject messageObject = Instantiate(Other, father);
            Text text = messageObject.transform.Find("Text").GetComponent<Text>();
            Text name = messageObject.transform.Find("nameP2").GetComponent<Text>();
            text.text = data.message;
            name.text = data.P_name;
        }
        if(data.type == "private")
        {
            Debug.Log("收到消息: " + data.message + data.P_name);
            GameObject messageObject = Instantiate(Other, father);
            Text text = messageObject.transform.Find("Text").GetComponent<Text>();
            Text name = messageObject.transform.Find("nameP2").GetComponent<Text>();
            text.text = data.message;
            name.text = data.P_name;
        }
    }

    private void Update()
    {
        if (!string.IsNullOrEmpty(PublicVar.toUserId))
            type = "private";
        else type = "";
        if (Input.GetKeyDown(KeyCode.Return) && !string.IsNullOrEmpty(input.text))
        {
            SendMessage();
        }
    }

    public IEnumerator PrivMess()
    {
        MessageData data = new MessageData
        {
            token = PublicVar.token,
            toUserName = PublicVar.toUserName,
        };
        string jsonData = JsonUtility.ToJson(data);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        using (UnityWebRequest req = new UnityWebRequest($"{PublicVar.ServerUrl}/data/PrivateMessage", "POST"))
        {
            req.uploadHandler = new UploadHandlerRaw(bodyRaw);
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json; charset=UTF-8");
            yield return req.SendWebRequest();
            if (req.result == UnityWebRequest.Result.Success)
            {
                PrivateMessage info = JsonUtility.FromJson<PrivateMessage>(req.downloadHandler.text);
                foreach (var PD in info.PrivateHistory)
                {
                    if (PD.FormName == PublicVar.playerName)
                    {
                        GameObject my = Instantiate(My, father);
                        my.transform.Find("name").GetComponent<Text>().text = PD.FormName;
                        my.transform.transform.Find("Text").GetComponent<Text>().text = PD.message;
                    }
                    else
                    {
                        GameObject other = Instantiate(Other, father);
                        other.transform.Find("nameP2").GetComponent<Text>().text = PD.FormName;
                        other.transform.Find("Text").GetComponent<Text>().text = PD.message;
                    }
                }
            }
        }
    }

    private void SendMessage()
    {
        string message = input.text;
        if (!string.IsNullOrEmpty(message))
        {
            MessageData mess =new MessageData
            {
                type = type,
                message = message,
                P_name = PublicVar.playerName,
                toUserId = PublicVar.toUserId,
                toUserName = PublicVar.toUserName,
            };
            string json = JsonUtility.ToJson(mess);
            NetworkManager.Instance.Send(json);
            input.text = "";
            input.ActivateInputField();
            GameObject messageObject = Instantiate(My, father);
            Text text = messageObject.transform.Find("Text").GetComponent<Text>();
            Text name = messageObject.transform.Find("name").GetComponent<Text>();
            text.text = message;
            name.text = PublicVar.playerName;
        }
    }

    private void OnDestroy()
    {
        // 移除按钮事件监听
        if (sendButton != null)
        {
            sendButton.onClick.RemoveListener(SendMessage);
        }
    }
}