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
        PhotonNetwork.NickName = "Player_" + Random.Range(1000, 9999);  // 임시 플레이어 이름 설정
        PhotonNetwork.ConnectUsingSettings();  // Photon 서버에 연결
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinOrCreateRoom("SharedRoom", new RoomOptions { MaxPlayers = 10 }, null);  // 방에 접속
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room: " + PhotonNetwork.CurrentRoom.Name);

        

        // 현재 방의 인원수를 출력
        Debug.Log("Current Player Count: " + PhotonNetwork.CurrentRoom.PlayerCount);

        Debug.Log("\nPhotonLobby.OnJoinedRoom()");
        Debug.Log("Current room name: " + PhotonNetwork.CurrentRoom.Name);
        Debug.Log("Other players in room: " + PhotonNetwork.CountOfPlayersInRooms);
        Debug.Log("Total players in room: " + (PhotonNetwork.CountOfPlayersInRooms + 1));
    }

    

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        // 새로운 플레이어가 입장했을 때 방의 인원수 갱신
        Debug.Log("Player entered: " + newPlayer.NickName);
        Debug.Log("Current Player Count: " + PhotonNetwork.CurrentRoom.PlayerCount);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        // 플레이어가 나갔을 때 방의 인원수 갱신
        Debug.Log("Player left: " + otherPlayer.NickName);
        Debug.Log("Current Player Count: " + PhotonNetwork.CurrentRoom.PlayerCount);
    }
}
