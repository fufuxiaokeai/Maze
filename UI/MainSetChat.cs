using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class MainSetChat : MonoBehaviour
{
    [Header("聊天系统")]
    public Button ChatButton;
    public Button ChatExitButton;
    public Image ChatImage;
    public Image img;
    public Image AddFriendImg;
    public Sprite[] images;
    public Transform father;

    [Header("添加好友")]
    public Button AddFriendButton;
    public Button AddFriendExitButton;
    public GameObject frined;
    public Transform FriendFather;
    private Image ImageShowed;
    public Button MainChatWindow;
    public Button JoinPrivateChat;

    public float showInterval = 5f;

    private void Start()
    {
        ChatImage.gameObject.SetActive(false);
        AddFriendImg.gameObject.SetActive(false);
        StartCoroutine(FriendList());
        ChatButton.onClick.AddListener(OpenChat);
        ChatExitButton.onClick.AddListener(ExitChat);
        AddFriendButton.onClick.AddListener(() => AddFriendImg.gameObject.SetActive(true));
        AddFriendExitButton.onClick.AddListener(() => AddFriendImg.gameObject.SetActive(false));
        MainChatWindow.onClick.AddListener(MainChat);
        JoinPrivateChat.onClick.AddListener(PrivateChat);
        StartCoroutine(RepeatShowAddFriendBlock());
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
        foreach (Transform child in father)
        {
            Destroy(child.gameObject);
        }
    }

    IEnumerator RepeatShowAddFriendBlock()
    {
        while (true)
        {
            yield return new WaitForSeconds(showInterval);
            StartCoroutine(FriendList());
        }
    }

    void ExitChat()
    {
        ChatImage.gameObject.SetActive(false);
    }

    void OpenChat()
    {
        ChatImage.gameObject.SetActive(true);
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

            if (req.result == UnityWebRequest.Result.Success)
            {
                FriendsInfo friendsInfo = JsonUtility.FromJson<FriendsInfo>(req.downloadHandler.text);
                foreach (var friends in friendsInfo.friends)
                {
                    GameObject friend = Instantiate(frined, FriendFather);
                    friend.transform.Find("Selected").gameObject.SetActive(false);
                    friend.transform.Find("Name").GetComponent<Text>().text = friends.FriendName;
                    friend.transform.Find("UID").GetComponent<Text>().text = friends.FriendId;

                    GameObject currentFriend = friend;
                    Button friendButton = friend.GetComponent<Button>();
                    if (friendButton == null)
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

    private void Update()
    {
        img.sprite = images[PublicVar.image_index];
    }

}
