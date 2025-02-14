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
            PhotonNetwork.LeaveRoom(); // ���� ���� ����
        }
        else
        {
            DisconnectAndReload();
        }
    }

    public override void OnLeftRoom()
    {
        DisconnectAndReload(); // ���� ���� �� ���� �����
    }

    private void DisconnectAndReload()
    {
        // ���� �ٽ� �ε��ϱ� ���� Prefab Pool �ʱ�ȭ
        if (PhotonNetwork.PrefabPool is DefaultPool pool)
        {
            pool.ResourceCache.Clear();
        }

        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect(); // Photon �������� ������ ���� ����
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // �������� ���� ���� �� ���� �ٽ� �ε�
    }

    public void TryReconnectAndCreateRoom()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings(); // Photon ������ �ٽ� ���� �õ�
        }
    }

    //public override void OnConnectedToMaster()
    //{
    //    Debug.Log("Connected to Master Server. Now creating room...");
    //    PhotonNetwork.CreateRoom("MyRoom", new RoomOptions { MaxPlayers = 4 });
    //}
}
