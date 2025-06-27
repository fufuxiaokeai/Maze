using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ADD_FriendCtrl : MonoBehaviour
{
    public Button AddFriendButton;
    public Button applyButton;

    public Image AddFriendImage;
    public Image applyImage;

    public float showInterval = 2f;

    [Header("好友申请")]
    public GameObject AddFrendBlock;
    public Transform AddfFrther;
    public GameObject AddFriendsList;
    public Transform AddFriendsFrther;
    public Button AddButton;
    public TMP_InputField Sentence;
    public TMP_Text ErrorText;

    private Image Index;
    private string Selected = "";

    void Start()
    {
        StartCoroutine(ShowAddFriendBlock());
        AddFriendImage.gameObject.SetActive(true);
        applyImage.gameObject.SetActive(false);
        AddFriendButton.onClick.AddListener(OnAddFriendButtonClick);
        applyButton.onClick.AddListener(OnApplyButtonClick);
        AddButton.onClick.AddListener(addfriend);
        ErrorText.text = "";
        StartCoroutine(RepeatShowAddFriendBlock());
    }

    IEnumerator RepeatShowAddFriendBlock()
    {
        while (true)
        {
            yield return new WaitForSeconds(showInterval);
            StartCoroutine(ShowAddFriendBlock());
            StartCoroutine(ShowNewFriend());
        }
    }

    void addfriend()
    {
        if (string.IsNullOrEmpty(Selected))
        {
            ErrorText.text = "你还没选你要添加哪个人呢";
            return;
        }
        AddFriendItem addfri = new AddFriendItem
        {
            type = "Add_AddFriend",
            token = PublicVar.token,
            ToName = Selected,
            Text = Sentence.text,
        };
        string json = JsonUtility.ToJson(addfri);
        NetworkManager.Instance.Send(json);
        Sentence.text = "";
    }

    void OnAddFriendButtonClick()
    {
        StartCoroutine(ShowAddFriendBlock());
        AddFriendImage.gameObject.SetActive(true);
        applyImage.gameObject.SetActive(false);
    }

    void OnApplyButtonClick()
    {
        StartCoroutine(ShowNewFriend());
        AddFriendImage.gameObject.SetActive(false);
        applyImage.gameObject.SetActive(true);
    }

    public IEnumerator ShowNewFriend()
    {
        foreach (Transform at in AddFriendsFrther)
        {
            Destroy(at.gameObject);
        }
        NewFriend nf = new NewFriend { token = PublicVar.token };
        string jsonData = JsonUtility.ToJson(nf);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        using (UnityWebRequest req = new UnityWebRequest($"{PublicVar.ServerUrl}/data/NewFriend", "POST"))
        {
            req.uploadHandler = new UploadHandlerRaw(bodyRaw);
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json; charset=UTF-8");
            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
            {
                NewFriends newFriends = JsonUtility.FromJson<NewFriends>(req.downloadHandler.text);
                foreach(var newf in newFriends.NewFriend)
                {
                    GameObject newfri = Instantiate(AddFriendsList, AddFriendsFrther);
                    newfri.transform.Find("Index").GetComponent<Image>().gameObject.SetActive(false);
                    newfri.transform.Find("Name").GetComponent<TMP_Text>().text = "名字：" + newf.name;
                    newfri.transform.Find("UID").GetComponent<TMP_Text>().text = "UID：" + newf.FormattedUID;

                    GameObject newfriend = newfri;
                    Button newfriendButton = newfri.GetComponent<Button>();
                    if(!newfriendButton) newfriendButton = newfri.AddComponent<Button>();
                    newfriendButton.onClick.AddListener(() => ShowIndex(newfriend));
                }
            }
        }
    }

    void ShowIndex(GameObject newfri)
    {
        ErrorText.text = "";
        string originalText = newfri.transform.Find("Name").GetComponent<TMP_Text>().text;
        string[] parts = originalText.Split(new string[] { "名字：" }, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length > 0)
        {
            Selected = parts[0];
            Debug.Log("提取出的名字：" + Selected);
        }

        if (Index!=null&& Index.gameObject!=null)
            Index.gameObject.SetActive(false);
        Image ImageShow = newfri.transform.Find("Index").GetComponent<Image>();
        Index = ImageShow;
        Index.gameObject.SetActive(true);
    }

    public IEnumerator ShowAddFriendBlock()
    {
        foreach(Transform t in AddfFrther)
        {
            Destroy(t.gameObject);
        }
        AddFriendItem AF = new AddFriendItem { token = PublicVar.token };
        string jsonData = JsonUtility.ToJson(AF);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        using (UnityWebRequest req = new UnityWebRequest($"{PublicVar.ServerUrl}/data/AddFriend", "POST"))
        {
            req.uploadHandler = new UploadHandlerRaw(bodyRaw);
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json; charset=UTF-8");
            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
            {
                AddFriendResponse af = JsonUtility.FromJson<AddFriendResponse>(req.downloadHandler.text);
                foreach(var a in af.Add_Friend)
                {
                    GameObject block = Instantiate(AddFrendBlock, AddfFrther);
                    block.transform.Find("Name").GetComponent<TMP_Text>().text = a.FromName;
                    block.transform.Find("UID").GetComponent<TMP_Text>().text = a.FromId;
                    block.transform.Find("txt").GetComponent<TMP_Text>().text = a.Text;
                    Button NoButton = block.transform.Find("ON").GetComponent<Button>();
                    if (!NoButton) NoButton = block.transform.Find("ON").AddComponent<Button>();

                    Button YesButton = block.transform.Find("Yes").GetComponent<Button>();
                    if (!YesButton) YesButton = block.transform.Find("Yes").AddComponent<Button>();

                    GameObject tempBlock = block;
                    NoButton.onClick.AddListener(() => refuse(tempBlock));
                    YesButton.onClick.AddListener(() => receive(tempBlock));
                }
            }
        }
    }

    void refuse(GameObject block)
    {
        if(string.IsNullOrEmpty(block.transform.Find("Name").GetComponent<TMP_Text>().text))
        {
            Debug.LogError("发生了错误");
            return;
        }
        AddFriendItem addFriend = new AddFriendItem
        {
            type = "Delect_AddFriend",
            FromName = block.transform.Find("Name").GetComponent<TMP_Text>().text
        };
        string json = JsonUtility.ToJson(addFriend);
        NetworkManager.Instance.Send(json);
        StartCoroutine(ShowAddFriendBlock());
    }

    void receive(GameObject block)
    {
        if (string.IsNullOrEmpty(block.transform.Find("Name").GetComponent<TMP_Text>().text))
        {
            Debug.LogError("发生了错误");
            return;
        }
        AddFriendItem addFriend = new AddFriendItem
        {
            type = "AddFriend",
            token = PublicVar.token,
            FromName = block.transform.Find("Name").GetComponent<TMP_Text>().text,
            FromId = block.transform.Find("UID").GetComponent<TMP_Text>().text,
        };
        string json = JsonUtility.ToJson(addFriend);
        NetworkManager.Instance.Send(json);
        StartCoroutine(ShowAddFriendBlock());
    }

}
