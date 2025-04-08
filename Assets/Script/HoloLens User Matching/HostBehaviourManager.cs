using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using System.Collections.Generic;
using CustomLogger;

public class HostBehaviourManager : MonoBehaviourPunCallbacks
{
    private static HostBehaviourManager instance;
    public List<HostOnlyBehaviour> hostBehaviours = new List<HostOnlyBehaviour>();
    public static HostBehaviourManager Instance => instance;

    public bool IsCentralHost
    {
        get
        {
            if (PhotonNetwork.IsMasterClient &&
                PhotonNetwork.InRoom &&
                PhotonNetwork.CurrentRoom.Name == "DefaultRoom" &&
                !PhotonNetwork.NickName.Contains("CentralHost")
               )
            {
                PhotonNetwork.NickName += "CentralHost";

                // TODO: 코드가 불필요하게 복잡한 느낌.. 추후 수정 필요
                foreach (var hostBehaviour in hostBehaviours)
                {
                    // 최초 1회 Central Host로 변경 시에만 호출(반복 호출 방지)
                    if (hostBehaviour.TryGetComponent(out UserMatchingManager hb))
                    {
                        FileLogger.Log($"hostBehaviour: {hb}");
                        FileLogger.Log($"Photon Nickname을 {PhotonNetwork.NickName}으로 변경", this);
                        hb.UpdateNickNameAfterJoin(PhotonNetwork.NickName);
                        hb.myUserInfo.PhotonUserName = PhotonNetwork.NickName;
                        hb.myUserInfo.PhotonRole = "CentralHost";

                        // myUserInfo와 동일한 photonUserName이 없고, photonRole이 존재하는 경우 추가
                        if (!hb.userInfos.Exists(user => user.PhotonUserName == hb.myUserInfo.PhotonUserName)
                            && (hb.myUserInfo.PhotonUserName.Contains("Mobile") 
                                || hb.myUserInfo.PhotonUserName.Contains("hololens")
                                || hb.myUserInfo.PhotonUserName.Contains("Quest")))
                        {
                            hb.userInfos.Add(hb.myUserInfo);
                            FileLogger.Log($"UserInfo added: {hb.myUserInfo.PhotonUserName}");
                        }
                        else
                        {
                            FileLogger.Log($"Can't add UserInfo: {hb.myUserInfo.PhotonUserName}, Role: {hb.myUserInfo.PhotonRole}");
                        }
                    }
                }

                return true;
            }
            else if (PhotonNetwork.NickName.Contains("CentralHost"))
                return true;
            else
                return false;
        }
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void RegisterHostBehaviour(HostOnlyBehaviour behaviour)
    {
        if (!hostBehaviours.Contains(behaviour))
        {
            FileLogger.Log($"Registered HostBehaviour: {behaviour}", this);
            hostBehaviours.Add(behaviour);
        }
    }

    #region PhotonEventOVERRIDE

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        FileLogger.Log("OnMasterClientSwitched", this);
        base.OnMasterClientSwitched(newMasterClient);

        if (PhotonNetwork.CurrentRoom.Name != "DefaultRoom")
            return;

        UpdateCentralHostStatus(newMasterClient);
    }

    #endregion

    #region HostStatus

    // 중앙 호스트 상태 업데이트
    public void UpdateCentralHostStatus(Player newMasterClient)
    {
        bool wasCentralHost = PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.Name == "DefaultRoom";
        bool willBeCentralHost = PhotonNetwork.LocalPlayer.Equals(newMasterClient);

        foreach (var behaviour in hostBehaviours)
        {
            if (wasCentralHost && behaviour.isActiveAsHost)
            {
                behaviour.OnStoppedBeingHost();
                behaviour.isActiveAsHost = false;
                FileLogger.Log($"[{behaviour.GetType().Name}] 호스트 비활성화", this);
            }

            if (willBeCentralHost)
            {
                behaviour.isActiveAsHost = true;
                behaviour.OnBecameHost();
                PhotonNetwork.NickName = "CentralHost";
                FileLogger.Log($"[{behaviour.GetType().Name}] 호스트 활성화", this);
            }
        }
    }

    public void UpdateCentralHostStatus()
    {
        foreach (var behaviour in hostBehaviours)
        {
            if (!IsCentralHost)
            {
                if (behaviour.isActiveAsHost)
                {
                    behaviour.isActiveAsHost = false;
                    behaviour.OnStoppedBeingHost();
                    FileLogger.Log($"[{behaviour.GetType().Name}] 호스트 비활성화", this);
                }
            }
            else
            {
                if (!behaviour.isActiveAsHost)
                {
                    behaviour.isActiveAsHost = true;
                    behaviour.OnBecameHost();
                    FileLogger.Log($"[{behaviour.GetType().Name}] 호스트 활성화", this);
                }
            }
        }
    }

    public void HandleOnJoinedRoom()
    {
        foreach (var behaviour in hostBehaviours)
        {
            behaviour.HandleOnJoinedRoom();
        }
    }

    #endregion
}