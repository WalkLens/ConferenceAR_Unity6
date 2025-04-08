using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;



public class NetworkRoom : MonoBehaviourPunCallbacks, IInRoomCallbacks
{
    public static NetworkRoom Room;
    [SerializeField] private GameObject photonUserPrefab = default;

    private Player[] photonPlayers;
    private int playersInRoom;
    private int myNumberInRoom;

    private void Awake()
    {
        if (Room == null)
        {
            Room = this;
        }
        else
        {
            if (Room != this)
            {
                Destroy(Room.gameObject);
                Room = this;
            }
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        photonPlayers = PhotonNetwork.PlayerList;
        playersInRoom++;
    }

    public override void OnEnable()
    {
        base.OnEnable();
        PhotonNetwork.AddCallbackTarget(this);
    }

    public override void OnDisable()
    {
        base.OnDisable();
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    private void Start()
    {
        if (PhotonNetwork.PrefabPool is DefaultPool pool)
        {
            if (photonUserPrefab != null) pool.ResourceCache.Add(photonUserPrefab.name, photonUserPrefab);
        }
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        photonPlayers = PhotonNetwork.PlayerList;
        playersInRoom = photonPlayers.Length;
        myNumberInRoom = playersInRoom;
        PhotonNetwork.NickName = myNumberInRoom.ToString();

        StartGame();
    }

    private void StartGame()
    {
        CreatePlayer();

        if (!PhotonNetwork.IsMasterClient) return;
    }

    private void CreatePlayer()
    {
        var player = PhotonNetwork.Instantiate(photonUserPrefab.name, Vector3.zero, Quaternion.identity);
    }

}
