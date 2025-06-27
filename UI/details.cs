using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class details : MonoBehaviour
{
    private float startTime;

    public Image detailsBK;
    public Text detailsText;
    public Text TimeText;

    private float totalTime; // �ۼ�ʱ��

    private bool IsShow = false; // �Ƿ���ʾ
    int hours, hours2;
    int minutes, minutes2;
    float seconds, seconds2;

    void Start()
    {
        detailsBK.gameObject.SetActive(false);
        switch (PublicVar.difficulty)
        {
            case "��":
                startTime = 300f; // 5����
                break;
            case "��ͨ":
                startTime = 1200f; // 20����
                break;
            case "����":
                startTime = 3000f; // 50����
                break;
            case "����":
                startTime = 5400f; // 90����
                break;
        }
        PublicVar.remainTime = startTime;
        PublicVar.IsEnd = false; // ��ʼ����Ϸδ����״̬
    }

    // Update is called once per frame
    void Update()
    {
        if (!PublicVar.time)
        {
            //print("IsShow��" + PublicVar.IsShow);
            //print("IsEnd��" + PublicVar.IsEnd);
            //print("IsAchievement��" + PublicVar.IsAchievement);
            //print("IsChart��" + PublicVar.IsChart);
            //print(!PublicVar.IsShow && !PublicVar.IsAchievement && !PublicVar.IsChart);
            //!PublicVar.IsShow || !PublicVar.IsEnd && !PublicVar.IsAchievement || !PublicVar.IsChart
            if ((!PublicVar.IsShow && !PublicVar.IsAchievement && !PublicVar.IsChart&&!PublicVar.IsOnlineEnd) && !PublicVar.IsEnd)
            {
                totalTime += Time.deltaTime; // �ۼ�ʱ��
            }
            if (PublicVar.reset)
            {
                totalTime = 0; // ����ʱ��
                PublicVar.reset = false;
            }
            hours2 = Mathf.FloorToInt(totalTime / 3600);
            minutes2 = Mathf.FloorToInt((totalTime % 3600) / 60);
            seconds2 = totalTime % 60;
            TimeText.text = "��ʱ��" + string.Format("{0:D2}:{1:D2}:{2:F1}", hours2, minutes2, seconds2);
            PublicVar.Time = string.Format("{0:D2}:{1:D2}:{2:F1}", hours2, minutes2, seconds2);
            PublicVar.TIME = Mathf.FloorToInt(totalTime); // ����ʱ��
        }
        else
        {
            PublicVar.remainTime = startTime; // ����ʣ��ʱ��
            if ((!PublicVar.IsShow && !PublicVar.IsAchievement && !PublicVar.IsChart && !PublicVar.IsOnlineEnd) && !PublicVar.IsEnd)
            {
                startTime -= Time.deltaTime;
                totalTime += Time.deltaTime; // �ۼ�ʱ��
            }
            //print("IsEnd��" + !PublicVar.IsEnd);
            if (!PublicVar.IsEnd)
            {
                if (startTime <= 0)
                {
                    startTime = 0; // ȷ��ʱ�䲻���ɸ���
                    PublicVar.IsEnd = true; // ��Ϸ����
                    IsShow = false; // ��������
                }
                else
                {
                    PublicVar.IsEnd = false; // ��Ϸδ����
                }
            }
            //print(startTime);
            hours = Mathf.FloorToInt(startTime / 3600);
            minutes = Mathf.FloorToInt((startTime % 3600) / 60);
            seconds = startTime % 60;
            hours2 = Mathf.FloorToInt(totalTime / 3600);
            minutes2 = Mathf.FloorToInt((totalTime % 3600) / 60);
            seconds2 = totalTime % 60;
            TimeText.text = "ʣ��ʱ�䣺" + string.Format("{0:D2}:{1:D2}:{2:F1}", hours, minutes, seconds);
            PublicVar.Time = string.Format("{0:D2}:{1:D2}:{2:F1}", hours2, minutes2, seconds2);
            PublicVar.TIME = Mathf.FloorToInt(totalTime); // ����ʱ��
        }

        if (!PublicVar.IsEnd || !PublicVar.IsAchievement)
        {
            if (Input.GetKeyDown(KeyCode.J) && !PublicVar.IsOnlineEnd)
            {
                PublicVar.IsShow = !PublicVar.IsShow;
                IsShow = !IsShow;
            }
        }
        //print(PublicVar.IsShow);
        if (IsShow)
        {
            detailsBK.gameObject.SetActive(true);
            if (!PublicVar.night)
            {
                detailsText.text = "��ã����" + PublicVar.playerName + "��\n" +
                    "����ѡ���Ѷ�Ϊ��" + PublicVar.difficulty + "\n" +
                    "Ŀǰ���õ�ʱ�䣺" + string.Format("{0:D2}:{1:D2}:{2:F1}", hours2, minutes2, seconds2) + "\n" +
                    "�Թ���С��" + PublicVar.Wmaze * 3 + "*" + PublicVar.Hmaze * 3 + "\n" +
                    "�������" + PublicVar.playerName + "��ϣ������Ը����ͨ���Թ�";
            }
            PublicVar.Isdetails = true;
        }
        else
        {
            detailsBK.gameObject.SetActive(false);
            PublicVar.Isdetails = false;
        }
    }
}
