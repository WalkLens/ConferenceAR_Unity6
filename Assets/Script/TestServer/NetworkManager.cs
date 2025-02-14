using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public static NetworkManager Lobby;

    private void Awake()
    {
        if (Lobby == null)
        {
            Lobby = this;
        }
        else
        {
            if (Lobby != this)
            {
                Destroy(Lobby.gameObject);
                Lobby = this;
            }
        }

        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        PhotonNetwork.NickName = "Player_" + Random.Range(1000, 9999);  // �ӽ� �÷��̾� �̸� ����
        PhotonNetwork.ConnectUsingSettings();  // Photon ������ ����
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinOrCreateRoom("SharedRoom", new RoomOptions { MaxPlayers = 10 }, null);  // �濡 ����
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room: " + PhotonNetwork.CurrentRoom.Name);

        

        // ���� ���� �ο����� ���
        Debug.Log("Current Player Count: " + PhotonNetwork.CurrentRoom.PlayerCount);

        Debug.Log("\nPhotonLobby.OnJoinedRoom()");
        Debug.Log("Current room name: " + PhotonNetwork.CurrentRoom.Name);
        Debug.Log("Other players in room: " + PhotonNetwork.CountOfPlayersInRooms);
        Debug.Log("Total players in room: " + (PhotonNetwork.CountOfPlayersInRooms + 1));
    }

    

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        // ���ο� �÷��̾ �������� �� ���� �ο��� ����
        Debug.Log("Player entered: " + newPlayer.NickName);
        Debug.Log("Current Player Count: " + PhotonNetwork.CurrentRoom.PlayerCount);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        // �÷��̾ ������ �� ���� �ο��� ����
        Debug.Log("Player left: " + otherPlayer.NickName);
        Debug.Log("Current Player Count: " + PhotonNetwork.CurrentRoom.PlayerCount);
    }
}
