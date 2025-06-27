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

    public Transform[] imgs; // 成就图片数组

    private bool IsAchievement = false; // 是否显示成就界面

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
            Text textComponent = img.GetComponentInChildren<Text>(); // 更可靠的获取方式
            if (textComponent != null)
            {
                achievementUIMap[textComponent.text] = img.transform;
            }
            else
            {
                Debug.LogWarning($"UI元素 {img.name} 缺少Text组件");
            }
        }
    }

    private void Awake()
    {
        StartCoroutine(Achi_proc());
        AchImage.gameObject.SetActive(false);
        AchButton.onClick.AddListener(OnAchButtonClick);
        AchExitButton.onClick.AddListener(OnAchExitClick);
        CacheAchievementUI(); // 缓存成就UI元素
        ServicePointManager.ServerCertificateValidationCallback = MyRemoteCertificateValidationCallback;
    }

    private static bool MyRemoteCertificateValidationCallback(System.Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
    {
        // 允许所有证书（包括无效/自签名证书）
        return true;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.V))
        {
            IsAchievement = true; // 切换成就界面显示状态
        }
        if (IsAchievement)
        {
            OnAchButtonClick(); // 如果成就界面需要显示，调用显示方法
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
        IsAchievement = false; // 重置成就界面显示状态
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
                        Debug.Log($"服务器返回错误: {response.message}");
                    }
                    else
                    {
                        if (response.exists)
                        {
                            Debug.Log("成就业已存在！");
                            foreach (var ach in response.achievement)
                            {
                                // 使用字典快速查找
                                if (achievementUIMap.TryGetValue(ach.AchName, out Transform uiElement))
                                {
                                    // 预缓存引用（如果频繁调用可以存储到类成员）
                                    Transform no = uiElement.Find("No");
                                    Transform yes = uiElement.Find("Yes");

                                    if (no != null && yes != null)
                                    {
                                        no.gameObject.SetActive(false);
                                        yes.gameObject.SetActive(true);
                                    }
                                    else
                                    {
                                        Debug.LogWarning($"UI元素 {ach.AchName} 缺少状态图标");
                                    }
                                }
                                else
                                {
                                    Debug.LogWarning($"未找到成就 {ach.AchName} 对应的UI元素");
                                }
                            }
                        
                        }
                        else
                        {
                            Debug.Log("成就业不存在");
                        }
                    }
                }
                else
                {
                    Debug.LogError($"服务器错误: {req.responseCode}");
                }
            }
        }

    }

}
