using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using static achievement;

public class MainSetAche : MonoBehaviour
{
    [Header("大厅处成就处理")]
    public Image MainSetImg;
    public Button AcheButton;
    public Image AcheImg;
    public Button AchExitButton;
    public Transform[] imgs; // 成就图片数组

    private Dictionary<string, Transform> achievementUIMap = new Dictionary<string, Transform>();

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
        MainSetImg.gameObject.SetActive(true);
        AcheImg.gameObject.SetActive(false);
        CacheAchievementUI();
        AcheButton.onClick.AddListener(OpenAche);
        AchExitButton.onClick.AddListener(CloseAche);
    }

    void OpenAche()
    {
        MainSetImg.gameObject.SetActive(false);
        AcheImg.gameObject.SetActive(true);
    }

    void CloseAche()
    {
        MainSetImg.gameObject.SetActive(true);
        AcheImg.gameObject.SetActive(false);
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
                if (req.responseCode == 200)
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
