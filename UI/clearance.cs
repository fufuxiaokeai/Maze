using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class clearance : MonoBehaviour
{
    public Text Title;
    public Text text;
    public Image End;
    public Button again;
    public Button quit;

    public Button SetUp;
    public Button Chat;
    public Image MainSetImg;

    [Header("联机模式下的处理")]
    public GameObject WinplayerCamera;
    public Transform MainPlayer;
    public Image RankImg;
    public Text txt;

    [Header("通关与失败的音乐")]
    public AudioSource Audio;
    public AudioClip[] clips;
    private float durationWin = 3.6f;
    private float durationLose = 3.8f;
    private float time_Win = 0f;
    private float time_Lose = 0f;

    [System.Serializable]
    public class recordDate
    {
        public string name;
        public string UID, size, time, diff, algorithm;
        public int Is_challenge, Is_challenge_pass, contact;
        public int TIME, IsNight;
    }

    private void Awake()
    {
        ServicePointManager.ServerCertificateValidationCallback = MyRemoteCertificateValidationCallback;
        End.gameObject.SetActive(false);
        quit.onClick.AddListener(OK);
        again.onClick.AddListener(Again);
        PublicVar.IsPass = 0;
    }

    private static bool MyRemoteCertificateValidationCallback(System.Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
    {
        // 允许所有证书（包括无效/自签名证书）
        return true;
    }

    private void Again()
    {
        PublicVar.IsEnd = false;
        PublicVar.IsShow = false;
        PublicVar.Isconnect = false;
        PublicVar.IsEnter = false;
        PublicVar.Isdetails = false;
        SharedVariables.strength = SharedVariables.Maxstrength;
        PublicVar.reset = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OK()
    {
        StartCoroutine(transmit_data());
        MainSetImg.gameObject.SetActive(true);
        RoomUIManager.Instance.ResetUIState();
        PublicVar.IsEnd = false;
        PublicVar.IsShow = false;
        PublicVar.Isconnect = false;
        PublicVar.IsEnter = false;
        PublicVar.Isdetails = false;
        SharedVariables.strength = SharedVariables.Maxstrength;
        SceneManager.LoadScene("MainSet");
    }

    private IEnumerator transmit_data()
    {
        var name = PublicVar.playerName;
        var UID = PublicVar.FormattedUID;
        var size = PublicVar.Wmaze * 3 + "x" + PublicVar.Hmaze * 3;
        var time = PublicVar.Time;
        var diff = PublicVar.difficulty;
        var algorithm = PublicVar.algorithm;
        var Is_challenge = PublicVar.Is_challenge;
        var Is_challenge_pass = PublicVar.Is_challenge_pass;
        var contact = PublicVar.coll;
        var timeInt = PublicVar.TIME; // 获取时间整数值
        var IsNight = PublicVar.IsNight; // 获取夜晚模式状态

        recordDate recordDate = new recordDate
        {
            name = name,
            UID = UID,
            size = size,
            time = time,
            diff = diff,
            algorithm = algorithm,
            Is_challenge = Is_challenge,
            Is_challenge_pass = Is_challenge_pass,
            contact = contact,
            TIME = timeInt, // 将时间整数值赋给TIME字段
            IsNight = IsNight // 将夜晚模式状态赋给IsNight字段
        };

        string json = JsonUtility.ToJson(recordDate);
        byte[] data = System.Text.Encoding.UTF8.GetBytes(json);

        using(UnityWebRequest request = new UnityWebRequest($"{PublicVar.ServerUrl}/data","POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(data);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json; charset=UTF-8");
            yield return request.SendWebRequest();
            if(request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("数据发送成功！");
            }
            else
            {
                if(request.responseCode == 400)
                {
                    Debug.LogError("数据格式错误！");
                }
            }
        }

    }

    private void Update()
    {
        if (PublicVar.time && PublicVar.remainTime <= 0)
        {
            PublicVar.Is_challenge_pass = 0;
            SetUp.interactable = false;
            Chat.interactable = false;
            End.gameObject.SetActive(true);
            Title.text = "很遗憾你没能在规定时间内通关！";
            text.text = "你好，玩家" + PublicVar.playerName + "！\n" +
                "很遗憾你没能在规定时间内通关" + "\n" +
                "你所选的难度为：" + PublicVar.difficulty + "\n" +
                "迷宫大小：" + PublicVar.Wmaze * 3 + "*" + PublicVar.Hmaze * 3 + "\n" +
                "加油玩家" + PublicVar.playerName + "！希望你下一次可以更快的通过迷宫";
            PublicVar.IsEnd = true;
            PublicVar.IsShow = true;
            while (true)
            {
                time_Lose += Time.deltaTime;
                if (time_Lose >= durationLose) break;
                Audio.clip = clips[1]; // 设置失败音乐
                Audio.Play();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(PublicVar.IsEnd) return; // 如果已经结束，直接返回

        if (other.CompareTag("Player"))
        {
            PublicVar.Is_challenge_pass = 1;
            PublicVar.IsPass = 1;
            SetUp.interactable = false;
            Chat.interactable = false;
            Debug.Log(other.name + " 进入了触发器");
            PublicVar.PlayerOrientation = MainPlayer.position;
            if (!PublicVar.IsOnline)
            {
                End.gameObject.SetActive(true);
                Title.text = PublicVar.playerName + "恭喜你通过本次迷宫挑战！";
                text.text = PublicVar.playerName + "恭喜通关！\n" +
                                    "本次挑战的难度为：" + PublicVar.difficulty + "\n" +
                                    "迷宫大小的为：" + PublicVar.Wmaze * 3 + "x" + PublicVar.Hmaze * 3 + "\n" +
                                    "挑战所时间为：" + PublicVar.Time + "\n" +
                                    "时刻欢迎你再次挑战！";
                PublicVar.IsEnd = true;
                PublicVar.IsShow = true;
            }
            else
            {
                OnlineProces();
            }
            while (true)
            {
                time_Win += Time.deltaTime;
                if (time_Win >= durationWin) break;
                Audio.clip = clips[0]; // 设置通关音乐
                Audio.Play();
            }
        }
    }

    void OnlineProces()
    {
        NotOtherPlayer();
        RankImg.gameObject.SetActive(true);
        PublicVar.IsRanked = true;
        txt.text = $"恭喜玩家{PublicVar.playerName}成功完成挑战\n" +
            "这是当前玩家排行榜\n" +
            "按下叉键可以隐藏当前排行榜，进入观战模式\n" +
            "按下H键可以回到排行榜";
    }

    void NotOtherPlayer()
    {
        WinPlayerInfo winPlayerInfo = new WinPlayerInfo
        {
            type = "PlayerClea",
            roomId = RoomManager.Instance.currentRoomId,
            token = PublicVar.token,
            Time = PublicVar.Time,
        };
        NetworkManager.Instance.Send(JsonConvert.SerializeObject(winPlayerInfo));
    }

}
