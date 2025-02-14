using System;
using Photon.Pun;
using UnityEngine;
using CustomLogger;

[Serializable]
public class HostOnlyBehaviour : MonoBehaviourPunCallbacks
{
    public bool isActiveAsHost = false;
    private bool isRegistered = false;

    protected virtual void Start()
    {
        if (!isRegistered)
        {
            FileLogger.Log($"[{GetType().Name}] Start", this);
            HostBehaviourManager.Instance.RegisterHostBehaviour(this);
            isRegistered = true;
        }
    }

    #region PhotonEventOVERRIDE
    public override void OnJoinedRoom()
    {
        FileLogger.Log("OnJoinedRoom", this);
        base.OnJoinedRoom();

        HostBehaviourManager.Instance.UpdateCentralHostStatus();
        // CheckAndUpdateHostStatus();

        HostBehaviourManager.Instance.HandleOnJoinedRoom();
    }

    #endregion

    #region HostBehaviourOVERRIDE

    public virtual void HandleOnJoinedRoom()
    {
        // 자식 클래스에서 이 메서드를 오버라이드하여 사용

        FileLogger.Log("Handle On Joined Room", this);
    }

    // 호스트가 되었을 때 실행할 가상 메서드
    public virtual void OnBecameHost()
    {
        // TODO: 파생 클래스에서 필요한 초기화 구현

        FileLogger.Log("호스트 등록: 호스트 매니저 초기화 완료", this);
    }

    // 호스트 권한을 잃었을 때 실행할 가상 메서드
    public virtual void OnStoppedBeingHost()
    {
        // TODO: 파생 클래스에서 필요한 잉여 데이터 정리 작업 구현

        FileLogger.Log("호스트 해제: 기존 호스트의 잉여 데이터 정리 완료", this);
    }
    
    #endregion
}