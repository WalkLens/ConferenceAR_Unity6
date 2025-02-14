using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PhotonUserUtility : MonoBehaviourPunCallbacks
{
    public static int GetCentralHostActorNumber()
    {
        // 모든 플레이어를 순회하여 중앙 호스트의 ActorNumber를 반환
        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (player.IsMasterClient && player.NickName.Contains("CentralHost"))
            {
                return player.ActorNumber; // 중앙 호스트의 ActorNumber 반환
            }
        }

        return -1; // 중앙 호스트가 아닐 경우
    }

    // Player에게 메세지 보내기 위해 Actor Number 반환
    public static int GetPlayerActorNumber(string photonUserName)
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player.NickName.Contains(photonUserName))
                return player.ActorNumber;
        }

        return -1;
    }
}
