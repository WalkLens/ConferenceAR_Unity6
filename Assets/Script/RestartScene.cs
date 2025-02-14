using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartScene : MonoBehaviourPunCallbacks
{
    public void RestartCurrentScene()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom(); // 먼저 방을 떠남
        }
        else
        {
            DisconnectAndReload();
        }
    }

    public override void OnLeftRoom()
    {
        DisconnectAndReload(); // 방을 떠난 후 씬을 재시작
    }

    private void DisconnectAndReload()
    {
        // 씬을 다시 로드하기 전에 Prefab Pool 초기화
        if (PhotonNetwork.PrefabPool is DefaultPool pool)
        {
            pool.ResourceCache.Clear();
        }

        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect(); // Photon 서버에서 완전히 연결 해제
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // 서버에서 연결 해제 후 씬을 다시 로드
    }

    public void TryReconnectAndCreateRoom()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings(); // Photon 서버에 다시 연결 시도
        }
    }

    //public override void OnConnectedToMaster()
    //{
    //    Debug.Log("Connected to Master Server. Now creating room...");
    //    PhotonNetwork.CreateRoom("MyRoom", new RoomOptions { MaxPlayers = 4 });
    //}
}
