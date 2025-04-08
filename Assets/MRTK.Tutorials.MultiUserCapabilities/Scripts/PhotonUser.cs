using Photon.Pun;
using UnityEngine;
using UnityEngine.UIElements;

namespace MRTK.Tutorials.MultiUserCapabilities
{
    public class PhotonUser : MonoBehaviour
    {
        private PhotonView pv;
        private string username;
        private string pinNum;

        private void Start()
        {
            pv = GetComponent<PhotonView>();

            if (!pv.IsMine) return;

            ////------------------ SM ADD ---------------------//
            //if (PhotonNetwork.NickName.StartsWith("CentralHost"))
            //{
            //    username = "User1";
            //}
            //else if (PhotonNetwork.NickName.StartsWith("Player"))
            //{
            //    username = "User" + PhotonNetwork.NickName.Substring(7);
            //}
            ////------------------ SM ADD ---------------------//

            pinNum = PhotonLobby.Lobby.input_PIN;
            EyegazeUIManager.main.myPinNum = pinNum;

            pv.RPC("PunRPC_SetNickName", RpcTarget.AllBuffered, pinNum);

            //username = "User" + PhotonNetwork.NickName;
            //pv.RPC("PunRPC_SetNickName", RpcTarget.AllBuffered, username);
        }

        public string GetPIN()
        {
            return gameObject.name;
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
    }
}
