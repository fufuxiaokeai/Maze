using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using static achievement;

public class MainSetAche : MonoBehaviour
{
    [Header("�������ɾʹ���")]
    public Image MainSetImg;
    public Button AcheButton;
    public Image AcheImg;
    public Button AchExitButton;
    public Transform[] imgs; // �ɾ�ͼƬ����

    private Dictionary<string, Transform> achievementUIMap = new Dictionary<string, Transform>();

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
