using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;
    public GameObject player;
    private Dictionary<string, Other_players> otherPlayers=
        new Dictionary<string, Other_players>();

    public Image RankImg;
    public GameObject Rank;
    public Button Exit;
    public Transform RankFather;

    [Header("联机模式下的处理")]
    public GameObject WinplayerCamera;
    public Transform MainPlayer;
    public Transform[] Player;
    public Vector3[] vector3s;

    [Header("游戏结束后退出房间的设置")]
    public Button OnlineExitButton;

    private bool IsRank = false;
    private bool IsMain = false;
    private GameObject Camera;
    private int i = 0;

    private void Awake()
    {
        if(instance == null) instance = this;
        else Destroy(instance);
        RankImg.gameObject.SetActive(false);
        Exit.onClick.AddListener(exit);
        i = 0;
    }

    private void Start()
    {
        foreach(Transform Rank in RankFather)
        {
            Destroy(Rank.gameObject);
        }
        OnlineExitButton.gameObject.SetActive(false);
        OnlineExitButton.onClick.AddListener(OnlineExit);
    }

    public void OnlineExit()
    {
        PublicVar.IsOnlineEnd = false;
        PublicVar.IsOnline = false;
        RoomUIManager.Instance.ResetUIState();
        RoomManager.Instance.Leaveroom();
        SceneManager.LoadScene("MainSet");

    }

    private void Update()
    {
        if (PublicVar.IsOnline && PublicVar.Is_challenge_pass == 1 && !PublicVar.IsOnlineEnd)
        {
            if (Input.GetKey(KeyCode.H))
            {
                IsRank = !IsRank;
                RankImg.gameObject.SetActive(IsRank);
                if (IsRank) PublicVar.IsRanked = true;
            }
        }
        if (IsMain && otherPlayers.Count == 0)
        {
            print("进入");
            PublicVar.IsOnlineEnd = true;
            Camera.transform.position = new Vector3(1399.9f, 1.85f, 2493.99f);
            Camera.transform.rotation = Quaternion.Euler(0, 180, 0);
            OnlineExitButton.gameObject.SetActive(true);
            IsMain = false;
        }
    }

    private void exit()
    {
        RankImg.gameObject.SetActive(false);
        Camera = Instantiate(WinplayerCamera);
        MainPlayer.gameObject.SetActive(false);
        PublicVar.IsRanked = false;
    }

    public void Ranking(string message)
    {
        var data = JsonUtility.FromJson<WinPlayerInfo>(message);
        GameObject Ranking = Instantiate(Rank, RankFather);
        Ranking.transform.Find("Name").GetComponent<Text>().text = $"用户：{data.name};ID:{data.userId}";
        Ranking.transform.Find("Time").GetComponent<Text>().text = $"用时：{data.Time}";
        Player[i].transform.position = vector3s[i];
        Player[i].Find("Canvas/Text").GetComponent<TMP_Text>().text = data.name;
        i++;
        if(i==3)
            Player[3].transform.rotation = Player[0].transform.rotation;
    }

    public void CreatePlayer(string userId, string userName)
    {
        if(!otherPlayers.ContainsKey(userId))
        {
            GameObject playerobj = Instantiate(player);
            playerobj.transform.Find("Canvas/Text").GetComponent<TMP_Text>().text = userName;
            otherPlayers.Add(userId,playerobj.GetComponent<Other_players>());
            playerobj.name = userId;
            Debug.Log($"玩家{userId}创立成功");
        }
    }

    public void DelectPlayer(string userId, string message)
    {
        var data = JsonUtility.FromJson<WinPlayerInfo>(message);
        if(data.name==data.authorName) IsMain = true;
        if (otherPlayers.TryGetValue(userId,out Other_players player))
        {
            Destroy(player.gameObject);
            otherPlayers.Remove(userId);
            Player[i].transform.position = vector3s[i];
            Debug.Log($"移除玩家: {userId}");
        }
    }

    public void UpdatePlayerPosition(string userId,string message, string userName)
    {
        if(otherPlayers.TryGetValue(userId, out Other_players player))
        {
            player.HandIePlayerMove(message);
        }
        else
        {
            CreatePlayer(userId, userName);
            if (otherPlayers.TryGetValue(userId, out player))
            {
                player.HandIePlayerMove(message);
            }
        }
    }


}
