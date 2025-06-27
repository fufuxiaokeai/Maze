using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.UI;

[System.Serializable]
public class FriendInfo
{
    public string FriendName;
    public string FriendId;
}

[System.Serializable]
public class FriendsInfo
{
    public FriendInfo[] friends;
}

public class Chatctrl : MonoBehaviour
{
    public Button ChatButton;
    public Button MainChatButton;
    public Button SetButton;

    public Image SetBK;
    public Text UIDText;

    public Image img;
    public Sprite[] images;
    public Image ChatBK;
    public InputField input;
    public Button sendButton;

    public Button ChatExitButton;

    public Button Add_Friend;
    public Image Add_FriendBK;
    public Button Add_FriendExitButton;

    public Text TimeText;

    public Transform father;
    public float showInterval = 2f;
    [Header("好友列表的处理")]
    public GameObject frined;
    public Transform FriendFather;
    private Image ImageShowed;
    public Button MainChatWindow;
    public Button JoinPrivateChat;

    private bool IsChat = false; // 是否显示
    public static Chatctrl instance;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(instance);
        StartCoroutine(FriendList());
        ChatBK.gameObject.SetActive(false);
        Add_FriendBK.gameObject.SetActive(false);
        ChatButton.onClick.AddListener(OnChatButtonClick);
        MainChatButton.onClick.AddListener(OnChatButtonClick);
        ChatExitButton.onClick.AddListener(OnChatExitClick);
        Add_Friend.onClick.AddListener(OnAddFriendClick);
        Add_FriendExitButton.onClick.AddListener(OnAddFriendExitClick);
        MainChatWindow.onClick.AddListener(MainChat);
        JoinPrivateChat.onClick.AddListener(PrivateChat);
        StartCoroutine(RepeatShowAddFriendBlock());
    }

    IEnumerator RepeatShowAddFriendBlock()
    {
        while (true)
        {
            yield return new WaitForSeconds(showInterval);
            StartCoroutine(FriendList());
        }
    }

    public void MainChat()
    {
        PublicVar.toUserId = "";
        PublicVar.toUserName = "";
        DelectHistory();
        if (ImageShowed != null && ImageShowed.gameObject != null)
            ImageShowed.gameObject.SetActive(false);
        StartCoroutine(ChatClient.instance.PubliMess());
    }

    public void PrivateChat()
    {
        if (string.IsNullOrEmpty(PublicVar.toUserName)) return;
        DelectHistory();
        StartCoroutine(ChatClient.instance.PrivMess());
    }

    void DelectHistory()
    {
        foreach(Transform child in father)
        {
            Destroy(child.gameObject);
        }
    }

    public IEnumerator FriendList()
    {
        foreach (Transform child in FriendFather)
        {
            Destroy(child.gameObject);
        }
        Token token = new Token { token = PublicVar.token };
        string jsonData = JsonUtility.ToJson(token);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);

        using (UnityWebRequest req = new UnityWebRequest($"{PublicVar.ServerUrl}/friend", "POST"))
        {
            req.uploadHandler = new UploadHandlerRaw(bodyRaw);
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json; charset=UTF-8");
            yield return req.SendWebRequest();

            if(req.result == UnityWebRequest.Result.Success)
            {
                FriendsInfo friendsInfo = JsonUtility.FromJson<FriendsInfo>(req.downloadHandler.text);
                foreach(var friends in friendsInfo.friends)
                {
                    GameObject friend = Instantiate(frined, FriendFather);
                    friend.transform.Find("Selected").gameObject.SetActive(false);
                    friend.transform.Find("Name").GetComponent<Text>().text = friends.FriendName;
                    friend.transform.Find("UID").GetComponent<Text>().text = friends.FriendId;
                    
                    GameObject currentFriend = friend;
                    Button friendButton = friend.GetComponent<Button>();
                    if(friendButton == null)
                    {
                        friendButton = friend.AddComponent<Button>();
                    }
                    friendButton.onClick.AddListener(() => ShowCheck(currentFriend));

                }
            }
        }
    }

    void ShowCheck(GameObject friend)
    {
        if (ImageShowed != null && ImageShowed.gameObject != null)
            ImageShowed.gameObject.SetActive(false);
        Image ImageShow = friend.transform.Find("Selected").GetComponent<Image>();

        PublicVar.toUserId = friend.transform.Find("UID").GetComponent<Text>().text;
        PublicVar.toUserName = friend.transform.Find("Name").GetComponent<Text>().text;

        ImageShowed = ImageShow;
        ImageShowed.gameObject.SetActive(true);
        friend.transform.Find("Selected").gameObject.SetActive(true);
    }

    private void OnAddFriendExitClick()
    {
        ChatBK.gameObject.SetActive(true);
        Add_FriendBK.gameObject.SetActive(false);
    }

    private void OnAddFriendClick()
    {
        ChatBK.gameObject.SetActive(false);
        Add_FriendBK.gameObject.SetActive(true);
    }

    void OnChatButtonClick()
    {
        // 完全保持原始打开逻辑
        ChatBK.gameObject.SetActive(true);
        SetButton.interactable = false;
        SetBK.gameObject.SetActive(false);
        UIDText.gameObject.SetActive(false);
        TimeText.gameObject.SetActive(false);
        PublicVar.IsShow = true;
        PublicVar.IsEnter = true;
    }

    void OnChatExitClick()
    {
        // 完全保持原始关闭逻辑
        ChatBK.gameObject.SetActive(false);
        SetButton.interactable = true;
        UIDText.gameObject.SetActive(true);
        TimeText.gameObject.SetActive(true);
        PublicVar.IsShow = false;
        PublicVar.IsEnter = false;
        IsChat = false;
    }

    private void Update() // 保留必要的最小化Update
    {
        img.sprite = images[PublicVar.image_index]; // 更新图片
        if (PublicVar.IsEnd) return; // 如果已经结束，直接返回
        if (Input.GetKeyDown(KeyCode.Return) && !PublicVar.IsOnlineEnd)
        {
            ToggleChatWindow();
            IsChat = true;
        }
    }

    void ToggleChatWindow()
    {
        if (ChatBK.gameObject.activeSelf && !IsChat)
        {
            OnChatExitClick();
        }
        else
        {
            OnChatButtonClick();
        }
    }
}
