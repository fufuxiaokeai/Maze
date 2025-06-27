using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[System.Serializable]
public class mess
{
    public string text;
    public string message;
}

public class feedback : MonoBehaviour
{
    public TMP_InputField feedbackInput;
    public TMP_Text text;
    public Button button;

    private void Awake()
    {
        text.text = "";
        button.onClick.AddListener(() => StartCoroutine(SendFeedBack()));
    }

    private IEnumerator SendFeedBack()
    {
        button.interactable = false;
        text.text = "�����ύ����...";

        if (string.IsNullOrEmpty(feedbackInput.text))
        {
            text.text = "�������������Ϊ��";
            yield break;
        }

        mess m = new mess { text = feedbackInput.text };
        string json = JsonUtility.ToJson(m);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);

        using (UnityWebRequest req = new UnityWebRequest($"{PublicVar.ServerUrl}/Feedback", "POST"))
        {
            req.uploadHandler = new UploadHandlerRaw(bodyRaw);
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");
            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
            {
                mess mess = JsonUtility.FromJson<mess>(req.downloadHandler.text);
                text.text = mess.text;
                feedbackInput.text = "";
            }

        }
        button.interactable = true;
    }
}
