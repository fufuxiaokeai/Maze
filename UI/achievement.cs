using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Ubiety.Dns.Core;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class achievement : MonoBehaviour
{
    public Image SetImage;
    public Image ChatImage;

    public Image AchImage;
    public Button AchButton;
    public Button AchExitButton;

    public Button SetButton;
    public Button ChatButton;

    public Text Time;

    public Transform[] imgs; // �ɾ�ͼƬ����

    private bool IsAchievement = false; // �Ƿ���ʾ�ɾͽ���

    private Dictionary<string, Transform> achievementUIMap = new Dictionary<string, Transform>();

    [System.Serializable]
    public class Play_Data
    {
        public string name, UID;
    }

    [System.Serializable]
    public class AchievementData
    {
        public string AchName;
    }

    [System.Serializable]
    public class AchievementResponse
    {
        public bool exists;
        public string message;
        public AchievementData[] achievement;
    }

    private void CacheAchievementUI()
    {
        foreach (var img in imgs)
        {
            Text textComponent = img.GetComponentInChildren<Text>(); // ���ɿ��Ļ�ȡ��ʽ
            if (textComponent != null)
            {
                achievementUIMap[textComponent.text] = img.transform;
            }
            else
            {
                Debug.LogWarning($"UIԪ�� {img.name} ȱ��Text���");
            }
        }
    }

    private void Awake()
    {
        StartCoroutine(Achi_proc());
        AchImage.gameObject.SetActive(false);
        AchButton.onClick.AddListener(OnAchButtonClick);
        AchExitButton.onClick.AddListener(OnAchExitClick);
        CacheAchievementUI(); // ����ɾ�UIԪ��
        ServicePointManager.ServerCertificateValidationCallback = MyRemoteCertificateValidationCallback;
    }

    private static bool MyRemoteCertificateValidationCallback(System.Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
    {
        // ��������֤�飨������Ч/��ǩ��֤�飩
        return true;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.V))
        {
            IsAchievement = true; // �л��ɾͽ�����ʾ״̬
        }
        if (IsAchievement)
        {
            OnAchButtonClick(); // ����ɾͽ�����Ҫ��ʾ��������ʾ����
        }
    }

    private void OnAchButtonClick()
    {
        PublicVar.IsShow = true;
        SetButton.interactable = false;
        ChatButton.interactable = false;
        AchImage.gameObject.SetActive(true);
        PublicVar.IsAchievement = true;
        SetImage.gameObject.SetActive(false);
        ChatImage.gameObject.SetActive(false);
    }

    private void OnAchExitClick()
    {
        SetButton.interactable = true;
        ChatButton.interactable = true;
        AchImage.gameObject.SetActive(false);
        PublicVar.IsAchievement = false;
        PublicVar.IsShow = false;
        PublicVar.Isconnect = false;
        PublicVar.IsEnter = false;
        Time.gameObject.SetActive(true);
        IsAchievement = false; // ���óɾͽ�����ʾ״̬
    }

    private IEnumerator Achi_proc()
    {
        var name = PublicVar.playerName;
        var UID = PublicVar.FormattedUID;

        Play_Data play_Data = new Play_Data
        {
            name = name,
            UID = UID
        };

        string json = JsonUtility.ToJson(play_Data);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);

        using (UnityWebRequest req = new UnityWebRequest($"{PublicVar.ServerUrl}/data/achievement", "POST"))
        {
            req.uploadHandler = new UploadHandlerRaw(bodyRaw);
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");
            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.ConnectionError ||
                req.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Error: {req.error}");
            }
            else
            {
                if(req.responseCode == 200)
                {
                    AchievementResponse response = JsonUtility.FromJson<AchievementResponse>(req.downloadHandler.text);
                    if (!string.IsNullOrEmpty(response.message))
                    {
                        Debug.Log($"���������ش���: {response.message}");
                    }
                    else
                    {
                        if (response.exists)
                        {
                            Debug.Log("�ɾ�ҵ�Ѵ��ڣ�");
                            foreach (var ach in response.achievement)
                            {
                                // ʹ���ֵ���ٲ���
                                if (achievementUIMap.TryGetValue(ach.AchName, out Transform uiElement))
                                {
                                    // Ԥ�������ã����Ƶ�����ÿ��Դ洢�����Ա��
                                    Transform no = uiElement.Find("No");
                                    Transform yes = uiElement.Find("Yes");

                                    if (no != null && yes != null)
                                    {
                                        no.gameObject.SetActive(false);
                                        yes.gameObject.SetActive(true);
                                    }
                                    else
                                    {
                                        Debug.LogWarning($"UIԪ�� {ach.AchName} ȱ��״̬ͼ��");
                                    }
                                }
                                else
                                {
                                    Debug.LogWarning($"δ�ҵ��ɾ� {ach.AchName} ��Ӧ��UIԪ��");
                                }
                            }
                        
                        }
                        else
                        {
                            Debug.Log("�ɾ�ҵ������");
                        }
                    }
                }
                else
                {
                    Debug.LogError($"����������: {req.responseCode}");
                }
            }
        }

    }

}
