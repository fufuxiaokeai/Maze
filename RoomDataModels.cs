using Org.BouncyCastle.Asn1.X509;
using System;
using System.Globalization;
using System.Numerics;

[Serializable]
public class MessageData
{
    public string type; // ��Ϣ����
    public string message; // ��Ϣ����
    public string token; // �û�����
    public string Password; // ����ķ�������
    public int mazeSeed; // ����
    public bool success; // �Ƿ�ɹ�
    public string userId;
    public string userName;
    public string roomName;
    public string P_name;
    public string toUserId;
    public string toUserName;

    public RoomInfo room;
    public RoomInfo[] rooms;
    public PlayerList[] playerLists;
    public PlayerState playerState;
    public Vector3 position;
    public float rotation;
}

[Serializable]
public class RoomInfo
{
    public string type;
    public string roomId;
    public string roomName;
    public string roomPassword = null; // �������룬����Ϊ��
    public string roomDiff;
    public int mazeSeed;
    public int Wmaze;
    public int Hmaze;
    public int MaxPlayers;
    public int CurrentPlayers;
    public int IsnightModel;
    public int IsTimerModel;
    public string authorName;
    public int authorId;
}

[Serializable]
public class PlayerList
{
    public string userId;
    public string userName;
}

[Serializable]
public class PlayerState
{
    public string direct;//ֱ�Ӷ�������·����̡����ܣ���IsMove���棩
    public string Junmp; //��Ծ��Jump��Jump-Move��
    public bool IsLand;
}

[Serializable]
public class PlayerPosition
{
    public float x;
    public float y;
    public float z;

    public Vector3 ToVector3()
    {
        return new Vector3(x, y, z);
    }
}

[Serializable]
public class WinPlayerInfo
{
    public string type;
    public string roomId;
    public string token;
    public string userId;
    public string name;
    public string authorName;
    public string Time = "00:00:00";
}


[Serializable]
public class Token
{
    public string token;
}

[Serializable]
public class AddFriendItem
{
    public string type;
    public string token;
    public string ToName;
    public string FromName;
    public string FromId;
    public string Text;
}

[Serializable]
public class AddFriendResponse
{
    public AddFriendItem[] Add_Friend;
}

[Serializable]
public class NewFriend
{
    public string token;
    public string name;
    public int id;
    public string FormattedUID
    {
        get { return id.ToString("D8"); }
    }
}

[Serializable]
public class NewFriends
{
    public NewFriend[] NewFriend;
}

