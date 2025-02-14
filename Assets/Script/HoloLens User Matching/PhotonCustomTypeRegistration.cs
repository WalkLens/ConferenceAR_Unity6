using System;
using System.Collections.Generic;
using System.IO;
using ExitGames.Client.Photon;
using Photon.Pun;

#region PhotonCustomDataType

[Serializable]
public class UserInfo
{
    public string CurrentRoomNumber { get; set; }
    public string PhotonRole { get; set; }
    public string PhotonUserName { get; set; }
    public string CurrentState { get; set; }
}

[Serializable]
public class MatchInfo
{
    public string UserWhoSend { get; set; }
    public string UserWhoReceive { get; set; }
    public string MatchRequest { get; set; }
}

[Serializable]
public class MeetingInfo
{
    public string MatchKey { get; set; }
    public string MeetingDateTime { get; set; }
}

#endregion

// Photon 메세지로 주고받는 데이터 타입 정의하는 스크립트
public class PhotonCustomTypeRegistration : MonoBehaviourPunCallbacks
{
    #region Initialize PhotonCustomTypeRegistration

    private void Awake()
    {
        // UserInfo 직렬화 등록
        PhotonPeer.RegisterType(
            typeof(UserInfo), // 타입 지정
            (byte)'U', // 타입의 식별자 (고유해야 함)
            SerializeUserInfo, // 직렬화 메서드
            DeserializeUserInfo // 역직렬화 메서드
        );

        PhotonPeer.RegisterType(
            typeof(List<UserInfo>),
            (byte)'L', // 고유 식별자
            SerializeUserInfoList,
            DeserializeUserInfoList
        );

        PhotonPeer.RegisterType(
            typeof(MatchInfo),
            (byte)'M',
            SerializeMatchInfo,
            DeserializeMatchInfo
        );
        PhotonPeer.RegisterType(
            typeof(MeetingInfo),
            (byte)'T',
            SerializeMeetingInfo,
            DeserializeMeetingInfo
        );
    }

    #endregion

    #region DataSerializeMethod

    // MeetingInfo 직렬화, 역직렬화하는 함수
    private static byte[] SerializeMeetingInfo(object data)
    {
        MeetingInfo meetingInfo = (MeetingInfo)data;
        using (MemoryStream stream = new MemoryStream())
        {
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                writer.Write(meetingInfo.MatchKey ?? "");
                writer.Write(meetingInfo.MeetingDateTime ?? "");
            }

            return stream.ToArray();
        }
    }

    private static object DeserializeMeetingInfo(byte[] data)
    {
        MeetingInfo meetingInfo = new MeetingInfo();
        using (MemoryStream stream = new MemoryStream(data))
        {
            using (BinaryReader reader = new BinaryReader(stream))
            {
                meetingInfo.MatchKey = reader.ReadString();
                meetingInfo.MeetingDateTime = reader.ReadString();
            }
        }

        return meetingInfo;
    }

    // MatchInfo 직렬화, 역직렬화하는 함수
    private static byte[] SerializeMatchInfo(object data)
    {
        MatchInfo matchInfo = (MatchInfo)data;
        using (MemoryStream stream = new MemoryStream())
        {
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                writer.Write(matchInfo.UserWhoSend ?? "");
                writer.Write(matchInfo.UserWhoReceive ?? "");
                writer.Write(matchInfo.MatchRequest ?? "");
            }

            return stream.ToArray();
        }
    }

    private static object DeserializeMatchInfo(byte[] data)
    {
        MatchInfo matchInfo = new MatchInfo();
        using (MemoryStream stream = new MemoryStream(data))
        {
            using (BinaryReader reader = new BinaryReader(stream))
            {
                matchInfo.UserWhoSend = reader.ReadString();
                matchInfo.UserWhoReceive = reader.ReadString();
                matchInfo.MatchRequest = reader.ReadString();
            }
        }

        return matchInfo;
    }

    // UserInfo 직렬화, 역직렬화하는 함수
    private static byte[] SerializeUserInfo(object customObject)
    {
        UserInfo userInfo = (UserInfo)customObject;
        using (MemoryStream stream = new MemoryStream())
        {
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                writer.Write(userInfo.CurrentRoomNumber ?? "");
                writer.Write(userInfo.PhotonRole ?? "");
                writer.Write(userInfo.PhotonUserName ?? "");
                writer.Write(userInfo.CurrentState ?? "");
            }

            return stream.ToArray();
        }
    }

    private static object DeserializeUserInfo(byte[] data)
    {
        UserInfo userInfo = new UserInfo();
        using (MemoryStream stream = new MemoryStream(data))
        {
            using (BinaryReader reader = new BinaryReader(stream))
            {
                userInfo.CurrentRoomNumber = reader.ReadString();
                userInfo.PhotonRole = reader.ReadString();
                userInfo.PhotonUserName = reader.ReadString();
                userInfo.CurrentState = reader.ReadString();
            }
        }

        return userInfo;
    }

    // UserInfo 직렬화, 역직렬화하는 함수
    private static byte[] SerializeUserInfoList(object customObject)
    {
        List<UserInfo> userInfoList = (List<UserInfo>)customObject;
        using (MemoryStream stream = new MemoryStream())
        {
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                writer.Write(userInfoList.Count); // 리스트 크기 기록
                foreach (var userInfo in userInfoList)
                {
                    writer.Write(userInfo.CurrentRoomNumber ?? "");
                    writer.Write(userInfo.PhotonRole ?? "");
                    writer.Write(userInfo.PhotonUserName ?? "");
                    writer.Write(userInfo.CurrentState ?? "");
                }
            }

            return stream.ToArray();
        }
    }

    private static object DeserializeUserInfoList(byte[] data)
    {
        List<UserInfo> userInfoList = new List<UserInfo>();
        using (MemoryStream stream = new MemoryStream(data))
        {
            using (BinaryReader reader = new BinaryReader(stream))
            {
                int count = reader.ReadInt32(); // 리스트 크기 읽기
                for (int i = 0; i < count; i++)
                {
                    UserInfo userInfo = new UserInfo
                    {
                        CurrentRoomNumber = reader.ReadString(),
                        PhotonRole = reader.ReadString(),
                        PhotonUserName = reader.ReadString(),
                        CurrentState = reader.ReadString()
                    };
                    userInfoList.Add(userInfo);
                }
            }
        }

        return userInfoList;
    }

    #endregion
}