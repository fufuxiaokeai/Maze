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

    [Header("����ģʽ�µĴ���")]
    public GameObject WinplayerCamera;
    public Transform MainPlayer;
    public Image RankImg;
    public Text txt;

    [Header("ͨ����ʧ�ܵ�����")]
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
        // ��������֤�飨������Ч/��ǩ��֤�飩
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
        var timeInt = PublicVar.TIME; // ��ȡʱ������ֵ
        var IsNight = PublicVar.IsNight; // ��ȡҹ��ģʽ״̬

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
            TIME = timeInt, // ��ʱ������ֵ����TIME�ֶ�
            IsNight = IsNight // ��ҹ��ģʽ״̬����IsNight�ֶ�
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
                Debug.Log("���ݷ��ͳɹ���");
            }
            else
            {
                if(request.responseCode == 400)
                {
                    Debug.LogError("���ݸ�ʽ����");
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
            Title.text = "���ź���û���ڹ涨ʱ����ͨ�أ�";
            text.text = "��ã����" + PublicVar.playerName + "��\n" +
                "���ź���û���ڹ涨ʱ����ͨ��" + "\n" +
                "����ѡ���Ѷ�Ϊ��" + PublicVar.difficulty + "\n" +
                "�Թ���С��" + PublicVar.Wmaze * 3 + "*" + PublicVar.Hmaze * 3 + "\n" +
                "�������" + PublicVar.playerName + "��ϣ������һ�ο��Ը����ͨ���Թ�";
            PublicVar.IsEnd = true;
            PublicVar.IsShow = true;
            while (true)
            {
                time_Lose += Time.deltaTime;
                if (time_Lose >= durationLose) break;
                Audio.clip = clips[1]; // ����ʧ������
                Audio.Play();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(PublicVar.IsEnd) return; // ����Ѿ�������ֱ�ӷ���

        if (other.CompareTag("Player"))
        {
            PublicVar.Is_challenge_pass = 1;
            PublicVar.IsPass = 1;
            SetUp.interactable = false;
            Chat.interactable = false;
            Debug.Log(other.name + " �����˴�����");
            PublicVar.PlayerOrientation = MainPlayer.position;
            if (!PublicVar.IsOnline)
            {
                End.gameObject.SetActive(true);
                Title.text = PublicVar.playerName + "��ϲ��ͨ�������Թ���ս��";
                text.text = PublicVar.playerName + "��ϲͨ�أ�\n" +
                                    "������ս���Ѷ�Ϊ��" + PublicVar.difficulty + "\n" +
                                    "�Թ���С��Ϊ��" + PublicVar.Wmaze * 3 + "x" + PublicVar.Hmaze * 3 + "\n" +
                                    "��ս��ʱ��Ϊ��" + PublicVar.Time + "\n" +
                                    "ʱ�̻�ӭ���ٴ���ս��";
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
                Audio.clip = clips[0]; // ����ͨ������
                Audio.Play();
            }
        }
    }

    void OnlineProces()
    {
        NotOtherPlayer();
        RankImg.gameObject.SetActive(true);
        PublicVar.IsRanked = true;
        txt.text = $"��ϲ���{PublicVar.playerName}�ɹ������ս\n" +
            "���ǵ�ǰ������а�\n" +
            "���²���������ص�ǰ���а񣬽����սģʽ\n" +
            "����H�����Իص����а�";
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
