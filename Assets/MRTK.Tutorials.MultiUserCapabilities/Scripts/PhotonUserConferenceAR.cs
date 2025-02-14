using System;
using Photon.Pun;
using UnityEngine;

namespace MRTK.Tutorials.MultiUserCapabilities
{
    public class PhotonUserConferenceAR : MonoBehaviour
    {
        public PhotonView pv;
        public string username;

        private void Start()
        {
            pv = GetComponent<PhotonView>();

            if (!pv.IsMine) return;

            username = PhotonNetwork.NickName;
            Debug.Log("Username " + username);
            gameObject.name = username;
            /*username = PhotonLobbyConferenceAR.Lobby.input_PIN;
                        EyegazeUIManager.main.myPinNum = pinNum;*/

            pv.RPC("PunRPC_SetNickName", RpcTarget.AllBuffered, username);
        }


        public string GetPIN()
        {
            return gameObject.name;
        }

        public void ReSetPunRPC_SetNickName()
        {
            pv = GetComponent<PhotonView>();

            pv.RPC("PunRPC_SetNickName", RpcTarget.AllBuffered, username);
        }
        [PunRPC]
        private void PunRPC_SetNickName(string nName)
        {
            gameObject.name = nName;
        }

        [PunRPC]
        private void PunRPC_ShareAzureAnchorId(string anchorId)
        {
            GenericNetworkManager.Instance.azureAnchorId = anchorId;

            Debug.Log("\nPhotonUser.PunRPC_ShareAzureAnchorId()");
            Debug.Log("GenericNetworkManager.instance.azureAnchorId: " + GenericNetworkManager.Instance.azureAnchorId);
            Debug.Log("Azure Anchor ID shared by user: " + pv.Controller.UserId);
        }

        public void ShareAzureAnchorId()
        {
            if (pv != null)
                pv.RPC("PunRPC_ShareAzureAnchorId", RpcTarget.AllBuffered,
                    GenericNetworkManager.Instance.azureAnchorId);
            else
                Debug.LogError("PV is null");
        }

        //public void UpdateNickName()
        //{
        //    if (pv.IsMine) // 로컬 플레이어인지 확인
        //    {
        //        // PhotonLobbyConferenceAR.Lobby.input_PIN 값을 닉네임으로 설정
        //        username = PhotonLobbyConferenceAR.Lobby.input_PIN;
        //        Debug.Log("닉네임 업데이트됨: " + username);

        //        // 모든 클라이언트에서 닉네임 동기화
        //        pv.RPC("PunRPC_SetNickName", RpcTarget.AllBuffered, username);
        //    }
        //}
    }
}