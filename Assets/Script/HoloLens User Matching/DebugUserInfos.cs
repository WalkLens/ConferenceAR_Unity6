using System;
using System.Collections;
using System.Collections.Generic;
using CustomLogger;
using ExitGames.Client.Photon;
using MixedReality.Toolkit.UX;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class DebugUserInfos : MonoBehaviour
{
    public TextMeshProUGUI myUserInfoText;
    public TextMeshProUGUI userInfosText;

    public Transform[] buttonTransforms;
    public TextMeshProUGUI[] userButtonTexts;
    public Button[] userButtons;
    public static DebugUserInfos Instance;

    public MatchInfo matchInfo; // 보낼 Match Info
    public MatchInfo receivedMatchInfo; // 받을 Match Info
    public GameObject matchButtonGameObject;
    public GameObject newMatchGameObject;
    public PressableButton[] matchButtons;          //============ SM MODI ============//
    public TextMeshProUGUI receivedMatchInfoText;
    public TextMeshProUGUI matchInfoText;

    public const byte SendMatchInfoEvent = 4; // 매칭 요청 이벤트 코드

    //============ SM ADD ============//
    public NotificationManager notificationManager;
    public int selectedUserIdx;
    //============ SM ADD ============//

    private void Awake()
    {
        if (Instance == null) Instance = this;
        CacheDebugUserButtons();
        // Debug.Log(Application.persistentDataPath);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            DebugAllUsersInfo();
        }
    }

    #region ButtonSetup

    // 각 버튼에 필요한 기능을 할당한다.
    private void SetButtonTextsFromAllUsersInfo()
    {
        foreach (var userButton in userButtons)
        {
            if (userButton != null)
            {
                userButton.onClick.RemoveAllListeners();
            }
        }

        for (int i = 0; i < userButtonTexts.Length; i++)
        {
            userButtonTexts[i].text = "player_@";
        }

        for (int i = 0; i < UserMatchingManager.Instance.userInfos.Count; i++)
        {
            int index = i; // 안전한 캡처를 위해 별도의 변수 사용
            userButtonTexts[i].text = UserMatchingManager.Instance.userInfos[index].PhotonUserName;
            userButtons[i].onClick
                .AddListener(() =>
                {
                    selectedUserIdx = index;
                    FileLogger.Log($"선택한 유저의 이름: {UserMatchingManager.Instance.userInfos[selectedUserIdx].PhotonUserName}", this);
                    SendMatchRequestToAUser(UserMatchingManager.Instance.userInfos[selectedUserIdx].PhotonUserName,
                        UserMatchingManager.Instance.myUserInfo, 0);
                });
        }
    }

    /// <summary>
    /// 다른 사람에게 매칭 요청을 보낼 시 작동
    /// </summary>
    /// <param name="targetUserName">이 메세지를 받을 유저의 이름(selectedIdx에 해당 or receivedMatchInfo.userWhoSend)</param>
    /// <param name="myUserInfo">나의 유저 정보</param>
    /// <param name="matchRequestId">0: 매칭 요청, 1: 매칭 승인, 2: 매칭 거절, default: 지정X</param>
    public void SendMatchRequestToAUser(string targetUserName, UserInfo myUserInfo, int matchRequestId = -1)
    {
        // player 이름에 해당하는 photon Actor Number 획득
        int targetActorNumber = PhotonUserUtility.GetPlayerActorNumber(targetUserName);

        FileLogger.Log($"Send Message to {targetUserName}({targetActorNumber})", this);

        // TODO: 매칭 요청 보내는 작업 구현


        // 유효성 검사: Actor Number 확인
        if (PhotonNetwork.CurrentRoom.GetPlayer(targetActorNumber) == null)
        {
            FileLogger.Log($"Invalid targetActorNumber: {targetActorNumber}", this);
            return;
        }

        matchInfo.UserWhoSend = myUserInfo.PhotonUserName;
        matchInfo.UserWhoReceive = targetUserName;
        switch (matchRequestId)
        {
            case 0:
                matchInfo.MatchRequest = "Request...";
                FileLogger.Log("Set matchRequest: {Request...}", this);
                break;
            case 1:
                matchInfo.MatchRequest = "Accept";
                FileLogger.Log("Set matchRequest: {Accept}", this);
                break;
            case 2:
                matchInfo.MatchRequest = "Decline";
                FileLogger.Log("Set matchRequest: {Decline}", this);
                break;
            default:
                FileLogger.Log("Match Request ID is not Available", this);
                break;
        }
        DebugMatchText();

        FileLogger.Log($"MatchInfo to send : User who send: {matchInfo.UserWhoSend}, " +
                       $"User who receive: {matchInfo.UserWhoReceive}, matchRequest: {matchInfo.MatchRequest}");

        // MatchInfo 포함한 데이터 생성
        object[] data = new object[] { matchInfo };

        // RaiseEventOptions 설정
        RaiseEventOptions options = new RaiseEventOptions
        {
            TargetActors = new int[] { targetActorNumber }
        };

        try
        {
            // 실제 Photon에서 데이터를 보내는 부분
            PhotonNetwork.RaiseEvent(SendMatchInfoEvent, data, options, SendOptions.SendReliable);
            FileLogger.Log($"Send Message to {targetUserName}({targetActorNumber})", this);

            // 전송 후에 match 정보 지움.
            //matchInfo = null;
        }
        catch (Exception ex)
        {
            FileLogger.Log($"Failed to send UserInfo: {ex.Message}", this);
        }
    }

    #endregion

    #region DEBUG

    private void CacheDebugUserButtons()
    {
        // 버튼, 텍스트 할당
        userButtonTexts = new TextMeshProUGUI[buttonTransforms.Length];
        userButtons = new Button[buttonTransforms.Length];
        for (int i = 0; i < userButtonTexts.Length; i++)
        {
            TextMeshProUGUI buttonText = buttonTransforms[i].GetComponentInChildren<TextMeshProUGUI>();
            userButtonTexts[i] = buttonText;

            Button button = buttonTransforms[i].GetComponentInChildren<Button>();
            userButtons[i] = button;
        }
    }

    public void DebugMatchText()
    {
        //Debug.Log("XYZ1");
        receivedMatchInfoText.text = $"• userWhoSend: {receivedMatchInfo.UserWhoSend} \n" +
                                     $"• userWhoReceived: {receivedMatchInfo.UserWhoReceive} \n" +
                                     $"• matchRequest: {receivedMatchInfo.MatchRequest} ";
        //Debug.Log("XYZ2");
        matchInfoText.text = $"• userWhoSend: {matchInfo.UserWhoSend} \n" +
                             $"• userWhoReceived: {matchInfo.UserWhoReceive} \n" +
                             $"• matchRequest: {matchInfo.MatchRequest} ";
        //Debug.Log("XYZ3");
    }

    public void DebugMyUserInfo(UserInfo userInfo)
    {
        myUserInfoText.text = $"• Current Room Number: {userInfo.CurrentRoomNumber} \n" +
                              $"• Photon Role: {userInfo.PhotonRole} \n" +
                              $"• Photon UserName: {userInfo.PhotonUserName} \n" +
                              $"• Photon State: {userInfo.CurrentState} \n";
    }

    public void DebugAllUsersInfo()
    {
        Debug.Log($"UserInfos {UserMatchingManager.Instance.userInfos.Count}개 존재");
        userInfosText.text = ""; // 텍스트 초기화
        foreach (var userInfo in UserMatchingManager.Instance.userInfos)
        {
            userInfosText.text +=
                $"Room: {userInfo.CurrentRoomNumber}, Role: {userInfo.PhotonRole}, UserName: {userInfo.PhotonUserName}, State: {userInfo.CurrentState}, \n";
            Debug.Log($"유저 {userInfo.PhotonUserName} 감지됨");
        }
        
        for (int i = 0; i < userButtonTexts.Length; i++)
        {
            userButtonTexts[i].text = "player_@";
        }

        SetButtonTextsFromAllUsersInfo();
        //SetUITextsFromAllUsersInfo();       // 클릭한 사람의 pin 번호를 입력받아서 시각화
    }

    /*public void DebugMyMatchInfo()
    {
        if (receivedMatchInfo.UserWhoSend != null)
            Debug.Log($"• receivedMatchInfo.userWhoSend: {receivedMatchInfo.UserWhoSend}");
        if (receivedMatchInfo.UserWhoReceive != null)
            Debug.Log($"• receivedMatchInfo.userWhoReceive: {receivedMatchInfo.UserWhoReceive}");
        if (receivedMatchInfo.MatchRequest != null)
            Debug.Log($"• receivedMatchInfo.matchRequest: {receivedMatchInfo.MatchRequest}");

        // 받는 쪽 기준, matchInfo 부터는 안 뜸
        if (matchInfo == null)
            matchInfo = new MatchInfo();
        if (matchInfo.UserWhoSend != null)
            Debug.Log($"• matchInfo.userWhoSend: {matchInfo.UserWhoSend}");
        if (matchInfo.UserWhoReceive != null)
            Debug.Log($"• matchInfo.userWhoReceive: {matchInfo.UserWhoReceive}");
        if (matchInfo.MatchRequest != null)
            Debug.Log($"• matchInfo.matchRequest: {matchInfo.MatchRequest} ");
    }*/

    public void LogAllUsersInfo(ref List<UserInfo> allUsersInfo)
    {
        foreach (UserInfo userInfo in allUsersInfo)
        {
            FileLogger.Log(
                $"{userInfo.CurrentRoomNumber} || {userInfo.PhotonRole} || {userInfo.PhotonUserName} || {userInfo.CurrentState}");
        }
    }

    #endregion

    public void ShowMatchRequestUI()
    {
        notificationManager.OpenReceiveRequestPopupUI();
    }
    public void ShowNewMatchRequestUI()
    {
        notificationManager.OpenReceiveRequestPopupUI();
    }
    public void ShowReceiveAcceptUI()
    {
        notificationManager.OpenReceiveAcceptPopupUI();
    }
    public void ShowReceiveDeclineUI()
    {
        notificationManager.OpenReceiveDeclinePopupUI();
    }

    void OnDestroy()
    {
        foreach (var userButton in userButtons)
        {
            if (userButton != null) userButton.onClick.RemoveAllListeners();
        }
    }
}