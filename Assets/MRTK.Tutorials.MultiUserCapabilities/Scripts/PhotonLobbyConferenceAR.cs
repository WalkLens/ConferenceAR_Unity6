using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using CustomLogger;

namespace MRTK.Tutorials.MultiUserCapabilities
{
    public class PhotonLobbyConferenceAR : MonoBehaviourPunCallbacks
    {
        public static PhotonLobbyConferenceAR Lobby;
        private int roomNumber = 1;
        private int userIdCount;
        public string input_PIN;

        private void Awake()
        {
            Debug.Log(Application.persistentDataPath);
#if !UNITY_EDITOR
                FileLogger.ClearLog();  
#endif

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

            GenericNetworkManager.OnReadyToStartNetwork += StartNetwork;
        }

        public override void OnConnectedToMaster()
        {
            var randomUserId = Random.Range(0, 999999);
            PhotonNetwork.AutomaticallySyncScene = false;
            PhotonNetwork.AuthValues = new AuthenticationValues();
            PhotonNetwork.AuthValues.UserId = randomUserId.ToString();
            userIdCount++;
            PhotonNetwork.NickName = PhotonNetwork.AuthValues.UserId;

            // 먼저 로비에 입장
            TypedLobby conferenceLobby = new TypedLobby("Conference", LobbyType.Default);
            PhotonNetwork.JoinLobby(conferenceLobby);


            FileLogger.Log($"마스터 서버 {conferenceLobby.Name} 연결 완료, 로비 입장 시도", this);
        }

        public override void OnJoinedLobby()
        {
            FileLogger.Log("로비 입장 완료", this);

            

            // 사용자 이름 설정
            FileLogger.SetUserName(PhotonNetwork.NickName);

            //// 마스터 클라이언트만 방을 생성하도록 함
            //if (PhotonNetwork.IsMasterClient)
            //{
            //    var roomOptions = new RoomOptions { IsVisible = true, IsOpen = true, MaxPlayers = 10 };
            //    PhotonNetwork.CreateRoom("DefaultRoom", roomOptions);
            //}
            //else
            //{
            //    // 마스터가 아닌 클라이언트는 방 입장만 시도
            //    PhotonNetwork.JoinRoom("DefaultRoom");
            //}
        }
        public override void OnLeftLobby()
        {
            Debug.Log("로비 퇴장");
        }

        public override void OnJoinedRoom()
        {
            base.OnJoinedRoom();

            Debug.Log("Joined Room: " + PhotonNetwork.CurrentRoom.Name);



            // 현재 방의 인원수를 출력
            Debug.Log("Current Player Count: " + PhotonNetwork.CurrentRoom.PlayerCount);

            Debug.Log("\nPhotonLobby.OnJoinedRoom()");
            Debug.Log("Current room name: " + PhotonNetwork.CurrentRoom.Name);
            Debug.Log("Other players in room: " + PhotonNetwork.CountOfPlayersInRooms);
            Debug.Log("Total players in room: " + (PhotonNetwork.CountOfPlayersInRooms + 1));

            /*// For Debugging
                        FileLogger.Log("PhotonLobbyConferenceAR.OnJoinedRoom()", this);
                        FileLogger.Log("Current room name: " + PhotonNetwork.CurrentRoom.Name, this);
                        FileLogger.Log("Other players in room: " + PhotonNetwork.CountOfPlayersInRooms, this);
                        FileLogger.Log("Total players in room: " + (PhotonNetwork.CountOfPlayersInRooms + 1), this);

                        //string newNickName = "Player_" + PhotonNetwork.LocalPlayer.ActorNumber;
                        string newNickName = input_PIN + "_";

                        // 플랫폼별 실행 코드
            #if UNITY_IOS || UNITY_ANDROID
                        FileLogger.Log("📱 Running on Mobile (iOS or Android)", this);
                        newNickName += "Mobile";
            #elif UNITY_WSA || UNITY_WINRT
                        FileLogger.Log("💻 Running on UWP (Windows Store App)", this);
                        newNickName += "hololens";
            #elif UNITY_EDITOR
                        FileLogger.Log($"🖥 Running in Unity Editor, 지정된 빌드 옵션(0: mobile, 1: hololens)=> {DebugBuildOptionManager.Instance.buildOptions}를 이용해 이름을 수정합니다.", this);
                        if (DebugBuildOptionManager.Instance.buildOptions == DebugBuildOptionManager.BuildOptions.Mobile)
                        {
                            newNickName += "Mobile";
                        }else if (DebugBuildOptionManager.Instance.buildOptions == DebugBuildOptionManager.BuildOptions.hololens)
                        {
                            newNickName += "hololens";
                        }
            #else
                        FileLogger.Log($"🖥 Running in Unity Editor, 지정된 빌드 옵션(0: mobile, 1: hololens)=> {DebugBuildOptionManager.Instance.buildOptions}를 이용해 이름을 수정합니다.", this);
                        if (DebugBuildOptionManager.Instance.buildOptions == DebugBuildOptionManager.BuildOptions.Mobile)
                        {
                            newNickName += "Mobile";
                        }else if (DebugBuildOptionManager.Instance.buildOptions == DebugBuildOptionManager.BuildOptions.hololens)
                        {
                            newNickName += "hololens";
                        }
                        Debug.Log("🧐 Running on an Window platform");
            #endif

                        UserMatchingManager.Instance.UpdateNickNameAfterJoin(newNickName);
                        UserMatchingManager.Instance.TrySendingUserInfo();

                        FileLogger.Log($"포톤 룸에 입장 완료: |닉네입 설정: [{newNickName}] |유저 정보 동기화 시도 ",this);*/
        }
        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            FileLogger.Log("방 참가 실패 - 방이 존재하지 않아 새로 생성합니다.", this);
            CreateRoom();
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            FileLogger.Log("방 생성 실패: " + message, this);
            CreateRoom();
        }

        public override void OnCreatedRoom()
        {
            base.OnCreatedRoom();
            roomNumber++;
            FileLogger.Log("방 생성 완료: " + PhotonNetwork.CurrentRoom.Name, this);
            FileLogger.Log("방 번호: " + roomNumber, this);
        }

        public void JoinOrCreateRoom(string input)
        {
            input_PIN = input;
            // 마스터 클라이언트만 방을 생성하도록 함
            if (PhotonNetwork.IsMasterClient)
            {
                var roomOptions = new RoomOptions { IsVisible = true, IsOpen = true, MaxPlayers = 10 };
                PhotonNetwork.CreateRoom("DefaultRoom", roomOptions);
            }
            else
            {
                // 마스터가 아닌 클라이언트는 방 입장만 시도
                PhotonNetwork.JoinRoom("DefaultRoom");
            }
        }

        private void StartNetwork()
        {
            PhotonNetwork.ConnectUsingSettings();
            Lobby = this;
        }

        private void CreateRoom()
        {
            var roomOptions = new RoomOptions { IsVisible = true, IsOpen = true, MaxPlayers = 10 };

            // For Debugging
            // TODO: User Matching Manager에서 관리하는 이름으로 방 생성
            PhotonNetwork.CreateRoom("DefaultRoom", roomOptions);
        }

        public void DisconnectFromPhoton()
        {
            if (PhotonNetwork.IsConnected)
            {
                PhotonNetwork.Disconnect();
                Debug.Log("Disconnected from Photon.");
            }
        }
    }
}