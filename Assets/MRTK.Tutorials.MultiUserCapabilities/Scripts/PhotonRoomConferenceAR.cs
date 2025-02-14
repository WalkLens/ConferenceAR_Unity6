using CustomLogger;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace MRTK.Tutorials.MultiUserCapabilities
{
    public class PhotonRoomConferenceAR : MonoBehaviourPunCallbacks, IInRoomCallbacks
    {
        public static PhotonRoomConferenceAR Room;

        [SerializeField] private GameObject photonUserPrefab = default;
        //[SerializeField] private GameObject roverExplorerPrefab = default;
        //[SerializeField] private Transform roverExplorerLocation = default;

        // private PhotonView pv;
        private Player[] photonPlayers;
        private int playersInRoom;
        private int myNumberInRoom;

        // private GameObject module;
        // private Vector3 moduleLocation = Vector3.zero;

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            base.OnPlayerEnteredRoom(newPlayer);
            photonPlayers = PhotonNetwork.PlayerList;
            playersInRoom++;
        }

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
            // pv = GetComponent<PhotonView>();

            // Allow prefabs not in a Resources folder
            if (PhotonNetwork.PrefabPool is DefaultPool pool)
            {
                if (photonUserPrefab != null) pool.ResourceCache.Add(photonUserPrefab.name, photonUserPrefab);

                //if (roverExplorerPrefab != null) pool.ResourceCache.Add(roverExplorerPrefab.name, roverExplorerPrefab);
            }
        }

        public override void OnJoinedRoom()
        {
            base.OnJoinedRoom();

            photonPlayers = PhotonNetwork.PlayerList;
            playersInRoom = photonPlayers.Length;
            myNumberInRoom = playersInRoom;
            //PhotonNetwork.NickName = myNumberInRoom.ToString();

            #region Cddd

            // For Debugging
            FileLogger.Log("PhotonLobbyConferenceAR.OnJoinedRoom()", this);
            FileLogger.Log("Current room name: " + PhotonNetwork.CurrentRoom.Name, this);
            FileLogger.Log("Other players in room: " + PhotonNetwork.CountOfPlayersInRooms, this);
            FileLogger.Log("Total players in room: " + (PhotonNetwork.CountOfPlayersInRooms + 1), this);

            //string newNickName = "Player_" + PhotonNetwork.LocalPlayer.ActorNumber;
            string newNickName = PhotonLobbyConferenceAR.Lobby.input_PIN + "_";


            // �÷����� ���� �ڵ�
#if UNITY_IOS || UNITY_ANDROID
            FileLogger.Log("?? Running on (Quest)Android)", this);
            string deviceModel = SystemInfo.deviceModel.ToLower();
            newNickName += "hololens";
            //newNickName += deviceModel;
            FileLogger.Log("Quest에서 실행 중", this);
#elif UNITY_WSA || UNITY_WINRT
            FileLogger.Log("?? Running on UWP (Windows Store App)", this);
            newNickName += "hololens";
#elif UNITY_EDITOR
            FileLogger.Log(
                $"?? Running in Unity Editor, ������ ���� �ɼ�(0: mobile, 1: hololens)=> {DebugBuildOptionManager.Instance.buildOptions}�� �̿��� �̸��� �����մϴ�.",
                this);
            if (DebugBuildOptionManager.Instance.buildOptions == DebugBuildOptionManager.BuildOptions.Mobile)
            {
                newNickName += "Mobile";
            }
            else if (DebugBuildOptionManager.Instance.buildOptions == DebugBuildOptionManager.BuildOptions.Hololens)
            {
                newNickName += "hololens";
            }
            else if (DebugBuildOptionManager.Instance.buildOptions == DebugBuildOptionManager.BuildOptions.Quest)
            {
                newNickName += "hololens";
            } 
#else
            if (DebugBuildOptionManager.Instance.buildOptions == DebugBuildOptionManager.BuildOptions.Mobile)
            {
                newNickName += "Mobile";
            }else if (DebugBuildOptionManager.Instance.buildOptions == DebugBuildOptionManager.BuildOptions.Hololens)
            {
                newNickName += "hololens";
            }
            else if (DebugBuildOptionManager.Instance.buildOptions == DebugBuildOptionManager.BuildOptions.Quest)
            {
                newNickName += "hololens";
            }
            Debug.Log("?? Running on an Window platform OR Something wrong");
#endif

            UserMatchingManager.Instance.UpdateNickNameAfterJoin(newNickName);
            UserMatchingManager.Instance.TrySendingUserInfo();

            FileLogger.Log($"���� �뿡 ���� �Ϸ�: |�г��� ����: [{newNickName}] |���� ���� ����ȭ �õ� ", this);

            #endregion


            //UserMatchingManager.Instance.UpdateNickNameAfterJoin(newNickName);

            // ��: PhotonUserConferenceAR.cs�� Start() �Ǵ� ������ ��ġ���� ����

            if (newNickName.Contains("hololens") || newNickName.Contains("Quest"))
            {
                // ���� ��� "12345_HoloLens"��� PIN�� "12345"�Դϴ�.
                string pin = newNickName.Split('_')[0];
                // ����� Ŭ���̾�Ʈ �г����� "12345_Mobile"�̶�� �����մϴ�.
                string mobileUserNick = $"{pin}_Mobile";

                // ��ƿ��Ƽ �Լ��� ����� �ش� �г����� ActorNumber�� �����ɴϴ�.
                int targetActorNumber = PhotonUserUtility.GetPlayerActorNumber(mobileUserNick);
                if (targetActorNumber != -1)
                {
                    // UserMatchingManager�� ���� �޼��带 ȣ���� "Successssssssss"�� �����ϴ�.
                    UserMatchingManager.Instance.SendCustomStringToTarget(targetActorNumber, "Successssssssss");
                    Debug.Log("SuccessssssssssSuccessssssssssSuccessssssssssSuccessssssssss");
                }
                else
                {
                    Debug.LogError($"����� ����� {mobileUserNick}�� ActorNumber�� ã�� �� �����ϴ�.");
                }
            }


            UserMatchingManager.Instance.TrySendingUserInfo();
            FileLogger.Log($"���� �뿡 ���� �Ϸ�: |�г��� ����: [{newNickName}] |���� ���� ����ȭ �õ� ", this);

            StartGame();


        }

        private void StartGame()
        {
            CreatPlayer();

            if (!PhotonNetwork.IsMasterClient) return;

            //if (TableAnchor.Instance != null) CreateInteractableObjects();
        }

        private void CreatPlayer()
        {
            var player = PhotonNetwork.Instantiate(photonUserPrefab.name, Vector3.zero, Quaternion.identity);
        }

        /*private void CreateInteractableObjects()
        {
            var position = roverExplorerLocation.position;
            var positionOnTopOfSurface = new Vector3(position.x, position.y + roverExplorerLocation.localScale.y / 2,
                position.z);

            var go = PhotonNetwork.Instantiate(roverExplorerPrefab.name, positionOnTopOfSurface,
                roverExplorerLocation.rotation);
        }*/

        // private void CreateMainLunarModule()
        // {
        //     module = PhotonNetwork.Instantiate(roverExplorerPrefab.name, Vector3.zero, Quaternion.identity);
        //     pv.RPC("Rpc_SetModuleParent", RpcTarget.AllBuffered);
        // }
        //
        // [PunRPC]
        // private void Rpc_SetModuleParent()
        // {
        //     Debug.Log("Rpc_SetModuleParent- RPC Called");
        //     module.transform.parent = TableAnchor.Instance.transform;
        //     module.transform.localPosition = moduleLocation;
        // }
    }
}