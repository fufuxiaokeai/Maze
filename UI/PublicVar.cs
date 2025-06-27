using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PublicVar
{
    public const string ServerUrl = "http://127.0.0.1:8000";  //http://192.168.5.15:3000，http://192.168.152.131:3000，http://172.29.211.55:3000

    public static string playerName = "芙芙宝宝";
    public static int UID;
    public static string token;
    public static string Chattype = "public";
    public static string toUserId;
    public static string toUserName;
    public static bool Isconnect = false;
    public static bool IsEnter = false;
    public static bool Isdetails = false;
    public static bool IsShow = false;
    public static bool IsEnd = false;
    public static bool IsAchievement = false;
    public static bool reset = false;
    public static bool night = false;
    public static bool time = false;
    public static bool IsChart = false;
    public static bool IsDelect_Prim = false;
    public static bool IsDelect_Kru = false;
    public static bool IsOnline = false;
    public static bool IsRanked = false;
    public static bool IsOnlineEnd = false;
    public static bool IsMapState = false;

    public static int mazeSeed = 0;
    public static int Wmaze = 10;
    public static int Hmaze = 10;
    public static int coll = 0;
    public static int Is_challenge = 0;
    public static int Is_challenge_pass = 0;
    public static int IsPass = 0;
    public static int image_index = 0;
    public static float remainTime = 0;
    public static int TIME = 0;
    public static int IsNight = 0;
    public static float WR = 1f;
    public static float WG = 0f;
    public static float WB = 0f;
    public static float GR = 0f;
    public static float GG = 0f;
    public static float GB = 0f;

    public static string difficulty = "简单";
    public static string algorithm;
    public static string Time = "00:00:00";
    public static string roomName = "";

    public static string direct;//直接动作：走路、冲刺、奔跑（与IsMove共存）
    public static string Junmp; //跳跃（Jump与Jump-Move）
    public static bool IsLand = true;

    public static Vector3 PlayerOrientation = Vector3.zero;

    public static string FormattedUID
    {
        get { return UID.ToString("D8"); }
    }
}
