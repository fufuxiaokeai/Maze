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

    private float totalTime; // 累计时间

    private bool IsShow = false; // 是否显示
    int hours, hours2;
    int minutes, minutes2;
    float seconds, seconds2;

    void Start()
    {
        detailsBK.gameObject.SetActive(false);
        switch (PublicVar.difficulty)
        {
            case "简单":
                startTime = 300f; // 5分钟
                break;
            case "普通":
                startTime = 1200f; // 20分钟
                break;
            case "困难":
                startTime = 3000f; // 50分钟
                break;
            case "地狱":
                startTime = 5400f; // 90分钟
                break;
        }
        PublicVar.remainTime = startTime;
        PublicVar.IsEnd = false; // 初始化游戏未结束状态
    }

    // Update is called once per frame
    void Update()
    {
        if (!PublicVar.time)
        {
            //print("IsShow：" + PublicVar.IsShow);
            //print("IsEnd：" + PublicVar.IsEnd);
            //print("IsAchievement：" + PublicVar.IsAchievement);
            //print("IsChart：" + PublicVar.IsChart);
            //print(!PublicVar.IsShow && !PublicVar.IsAchievement && !PublicVar.IsChart);
            //!PublicVar.IsShow || !PublicVar.IsEnd && !PublicVar.IsAchievement || !PublicVar.IsChart
            if ((!PublicVar.IsShow && !PublicVar.IsAchievement && !PublicVar.IsChart&&!PublicVar.IsOnlineEnd) && !PublicVar.IsEnd)
            {
                totalTime += Time.deltaTime; // 累计时间
            }
            if (PublicVar.reset)
            {
                totalTime = 0; // 重置时间
                PublicVar.reset = false;
            }
            hours2 = Mathf.FloorToInt(totalTime / 3600);
            minutes2 = Mathf.FloorToInt((totalTime % 3600) / 60);
            seconds2 = totalTime % 60;
            TimeText.text = "耗时：" + string.Format("{0:D2}:{1:D2}:{2:F1}", hours2, minutes2, seconds2);
            PublicVar.Time = string.Format("{0:D2}:{1:D2}:{2:F1}", hours2, minutes2, seconds2);
            PublicVar.TIME = Mathf.FloorToInt(totalTime); // 更新时间
        }
        else
        {
            PublicVar.remainTime = startTime; // 更新剩余时间
            if ((!PublicVar.IsShow && !PublicVar.IsAchievement && !PublicVar.IsChart && !PublicVar.IsOnlineEnd) && !PublicVar.IsEnd)
            {
                startTime -= Time.deltaTime;
                totalTime += Time.deltaTime; // 累计时间
            }
            //print("IsEnd：" + !PublicVar.IsEnd);
            if (!PublicVar.IsEnd)
            {
                if (startTime <= 0)
                {
                    startTime = 0; // 确保时间不会变成负数
                    PublicVar.IsEnd = true; // 游戏结束
                    IsShow = false; // 隐藏详情
                }
                else
                {
                    PublicVar.IsEnd = false; // 游戏未结束
                }
            }
            //print(startTime);
            hours = Mathf.FloorToInt(startTime / 3600);
            minutes = Mathf.FloorToInt((startTime % 3600) / 60);
            seconds = startTime % 60;
            hours2 = Mathf.FloorToInt(totalTime / 3600);
            minutes2 = Mathf.FloorToInt((totalTime % 3600) / 60);
            seconds2 = totalTime % 60;
            TimeText.text = "剩余时间：" + string.Format("{0:D2}:{1:D2}:{2:F1}", hours, minutes, seconds);
            PublicVar.Time = string.Format("{0:D2}:{1:D2}:{2:F1}", hours2, minutes2, seconds2);
            PublicVar.TIME = Mathf.FloorToInt(totalTime); // 更新时间
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
                detailsText.text = "你好，玩家" + PublicVar.playerName + "！\n" +
                    "你所选的难度为：" + PublicVar.difficulty + "\n" +
                    "目前所用的时间：" + string.Format("{0:D2}:{1:D2}:{2:F1}", hours2, minutes2, seconds2) + "\n" +
                    "迷宫大小：" + PublicVar.Wmaze * 3 + "*" + PublicVar.Hmaze * 3 + "\n" +
                    "加油玩家" + PublicVar.playerName + "！希望你可以更快的通过迷宫";
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
