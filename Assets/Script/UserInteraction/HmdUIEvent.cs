using ExitGames.Client.Photon;
using MixedReality.Toolkit.UX;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using CustomLogger;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HmdUIEvent : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] DebugUserInfos userInfos;

    [Header("Base UI")]
    [SerializeField] GameObject calendarLUI;
    [SerializeField] GameObject calendarRUI;
    [SerializeField] GameObject settingUI;
    [SerializeField] GameObject settingCloseUI;
    [SerializeField] GameObject logoutUI;

    [Header("Matching UI")]
    [SerializeField] Transform[] receiveRequestDetailUIElements;
    [SerializeField] GameObject receiveRequestDetailUI;
    [SerializeField] GameObject profileUI;
    [SerializeField] GameObject routeVisualizationUI;
    [SerializeField] TextMeshProUGUI uiText;
    [SerializeField] Transform arrowRot;

    [Header("Pop UP UI")]
    [SerializeField] GameObject receiveRequestPopupUI;
    [SerializeField] GameObject receiveAcceptPopupUI;
    [SerializeField] GameObject receiveDeclinePopupUI;
    [SerializeField] GameObject sendAcceptPopupUI;
    [SerializeField] GameObject sendDeclinePopupUI;
    [SerializeField] GameObject matchingStartPopupUI;

    private string processingUserId = "";

    //   Ī   û  ޼     ̺ Ʈ  Լ 
    public void SendRequestMessage()
    {
        userInfos.SendMatchRequestToAUser(UserMatchingManager.Instance.userInfos[userInfos.selectedUserIdx].PhotonUserName,
                        UserMatchingManager.Instance.myUserInfo);
    }
    public void SendRequestMessage(int selectedUserId)
    {
        userInfos.SendMatchRequestToAUser(UserMatchingManager.Instance.userInfos[selectedUserId].PhotonUserName,
            UserMatchingManager.Instance.myUserInfo);
    }

    public void SendAcceptMessage()
    {
        FileLogger.Log($"{userInfos.receivedMatchInfo.UserWhoSend} try to match! received Request: ({userInfos.receivedMatchInfo.MatchRequest})", this);
        
        string targetUserName = userInfos.receivedMatchInfo.UserWhoSend;
        
        // 매칭 승인 메세지 보내기
        userInfos.SendMatchRequestToAUser(targetUserName, UserMatchingManager.Instance.myUserInfo, 1);
        
        // 매칭 성공 : myGameObject, partnerGameObject 할당
        // 전송 받은 UserName에 부합한 홀로렌즈 혹은 퀘스트 오브젝트 찾기
        if(!UserMatchingManager.Instance.myGameObject) UserMatchingManager.Instance.myGameObject = FindObjectsOfType<GameObject>()
            .FirstOrDefault(obj => obj.name.Contains(UserMatchingManager.Instance.myUserInfo.PhotonUserName));
        UserMatchingManager.Instance.partnerGameObject = FindObjectsOfType<GameObject>()
            .FirstOrDefault(obj => obj.name.Contains(targetUserName));
                
        if(UserMatchingManager.Instance.partnerGameObject != null)
            FileLogger.Log($"파트너 게임 오브젝트 설정[Accept 보냄] : Target GameObject Name: {UserMatchingManager.Instance.partnerGameObject.name}", this);
        else
            FileLogger.Log($"파트너 게임 오브젝트 찾지 못함[Accept 보냄]: Target GameObject Name: {targetUserName}", this);
        
        // 매칭 성공 로그
        FileLogger.Log($"Accept matching request(from {userInfos.receivedMatchInfo.UserWhoSend})");
    }

    public void SendDeclineMessage()
    {
        FileLogger.Log($"{userInfos.receivedMatchInfo.UserWhoSend} try to match! received Request: ({userInfos.receivedMatchInfo.MatchRequest})", this);

        // 매칭 거절 메세지 보내기
        userInfos.SendMatchRequestToAUser(userInfos.receivedMatchInfo.UserWhoSend,
            UserMatchingManager.Instance.myUserInfo, 2);
        OpenSendDeclinePopupUI();

        // 매칭 성공 처리
        FileLogger.Log($"Decline matching request(from {userInfos.receivedMatchInfo.UserWhoSend})");
    }

    //============ Base ============//
    public void OpenCalendarUI()
    {
        calendarLUI.SetActive(true);
        calendarRUI.SetActive(true);
    }
    public void CloseCalendarUI()
    {
        calendarLUI.SetActive(false);
        calendarRUI.SetActive(false);
    }

    public void OpenSettingUI()
    {
        settingUI.SetActive(true);
        settingCloseUI.SetActive(true);
    }
    public void CloseSettingUI()
    {
        settingUI.SetActive(false);
        settingCloseUI.SetActive(false);
    }

    public void OpenLogoutUI()
    {
        logoutUI.SetActive(true);
    }
    public void CloseLogoutUI()
    {
        logoutUI.SetActive(false);
    }


    //============ Matching ============//
    // 팝업을 클릭한 뒤 보여지는 UI 기능을 넣고 활성화합니다.
    public void OpenReceiveRequestDetailUI()
    {
        receiveRequestDetailUI.SetActive(true);

        // ��ư�鿡 ��� ����
        Transform sendMatchingRequestObject = receiveRequestDetailUIElements[0];
        Transform declineObject = receiveRequestDetailUIElements[1];
        Transform timePlusObject = receiveRequestDetailUIElements[2];
        Transform expandObject = receiveRequestDetailUIElements[3];

        PressableButton[] MatchingRequestButton = new PressableButton[4];
        MatchingRequestButton[0] = sendMatchingRequestObject.GetComponent<PressableButton>();
        MatchingRequestButton[1] = declineObject.GetComponent<PressableButton>();
        MatchingRequestButton[2] = timePlusObject.GetComponent<PressableButton>();
        MatchingRequestButton[3] = expandObject.GetComponent<PressableButton>();

        MatchingRequestButton[0].OnClicked.AddListener(() =>
        {
            SendAcceptMessage();
            HololenUIManager.Instance.RemoveMatchingRequestData(HololenUIManager.Instance.requestCanvas[HololenUIManager.Instance.requestCanvas.Count - 1]);
            HololenUIManager.Instance.AddReservedData();                              // ���濡�� ���� ��û ���� ��, UI�� ǥ��

            MeetingManager.Instance.meetingTimeLeftScrollSelected = 0;
            HololenUIManager.Instance.MeetTimeUpdate();
            
            CloseReceiveRequestDetailUI();
            UserMatchingManager.Instance.MatchingStateUpdateAsTrue();
            Debug.Log("수라라라라라락");
            
            MatchingRequestButton[0].OnClicked.RemoveAllListeners();
            MatchingRequestButton[2].OnClicked.RemoveAllListeners();
        });

        MatchingRequestButton[1].OnClicked.AddListener(() =>
        {
            SendDeclineMessage();
            HololenUIManager.Instance.RemoveMatchingRequestData(HololenUIManager.Instance.requestCanvas[HololenUIManager.Instance.requestCanvas.Count - 1]);
            CloseReceiveRequestDetailUI();
            HololenUIManager.Instance.SetTime(0);
            Debug.Log("거저저저저절");
            
            MatchingRequestButton[1].OnClicked.RemoveAllListeners();
            MatchingRequestButton[2].OnClicked.RemoveAllListeners();
        });

        MatchingRequestButton[2].OnClicked.AddListener(() =>
        {
            // HololenUIManager.Instance.MeetTimePlus();
            MeetingManager.Instance.timePicker.parentGameObject.SetActive(true);
            HololenUIManager.Instance.RemoveMatchingRequestData(HololenUIManager.Instance.requestCanvas[HololenUIManager.Instance.requestCanvas.Count - 1]);
            
            MatchingRequestButton[2].OnClicked.RemoveAllListeners();
            
        });

        MatchingRequestButton[3].OnClicked.AddListener(() =>
        {
            /*OpenProfileUI();
            HololenUIManager.Instance.LoadMatchingSenderDetailsFromDB();*/
            
            MatchingRequestButton[3].OnClicked.RemoveAllListeners();
        });
    }
    public void CloseReceiveRequestDetailUI()
    {
        receiveRequestDetailUI.SetActive(false);
    }

    public void OpenProfileUI(string pinNum)
    {
        profileUI.SetActive(true);
        ViewProfileUIDataUpload setprofile = profileUI.GetComponent<ViewProfileUIDataUpload>();
        setprofile.SetdetailProfile(pinNum);
    }
    public void CloseProfileUI()
    {
        profileUI.SetActive(false);
    }


    public void OpenRouteVisualizationUI()
    {
        routeVisualizationUI.SetActive(true);
        //uiText.text = direction.magnitude.ToString() + "m left..";
    }

    public void CloseRouteVisualizationUI()
    {
        routeVisualizationUI.SetActive(false);
    }
    public void UpdateRouteVisualizationUI(Vector3 direction, float myRotY)
    {
        uiText.text = direction.magnitude.ToString() + "m 남았습니다";

        float rotY = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + 180 - myRotY;
        arrowRot.localEulerAngles = new Vector3(60, rotY, 0);
    }

    public void ShowMeetingUI()
    {
        Debug.Log("Show Meeting UI");
        Vector3 temp = UserMatchingManager.Instance.partnerGameObject.transform.position -
                       UserMatchingManager.Instance.myGameObject.transform.position;
        HololenUIManager.Instance.SetDistanceToPartner((int)temp.magnitude);
    }


    //============ PopUps ============//
    public void OpenReceiveRequestPopupUI()
    {
        receiveRequestPopupUI.SetActive(true);
        StartCoroutine(ActivateReceiveRequestPopupUI());
    }
    private IEnumerator ActivateReceiveRequestPopupUI()
    {
        receiveRequestPopupUI.SetActive(true);
        yield return new WaitForSeconds(3f);
        receiveRequestPopupUI.SetActive(false);
    }
    public void CloseReceiveRequestPopupUI()
    {
        receiveRequestPopupUI.SetActive(false);
    }

    public void OpenReceiveAcceptPopupUI()
    {
        receiveAcceptPopupUI.SetActive(true);
        StartCoroutine(ActivateReceiveAcceptPopupUI());
    }
    private IEnumerator ActivateReceiveAcceptPopupUI()
    {
        receiveAcceptPopupUI.SetActive(true);
        yield return new WaitForSeconds(3f);
        receiveAcceptPopupUI.SetActive(false);
    }

    public void OpenReceiveDeclinePopupUI()
    {
        receiveDeclinePopupUI.SetActive(true);
        StartCoroutine(ActivateReceiveDeclinePopupUI());
    }
    private IEnumerator ActivateReceiveDeclinePopupUI()
    {
        receiveDeclinePopupUI.SetActive(true);
        yield return new WaitForSeconds(3f);
        receiveDeclinePopupUI.SetActive(false);
    }

    public void OpenSendAcceptPopupUI()
    {
        sendAcceptPopupUI.SetActive(true);
        StartCoroutine(ActivateSendAcceptPopupUI());
    }
    private IEnumerator ActivateSendAcceptPopupUI()
    {
        sendAcceptPopupUI.SetActive(true);
        yield return new WaitForSeconds(3f);
        sendAcceptPopupUI.SetActive(false);
    }

    public void OpenSendDeclinePopupUI()
    {
        sendDeclinePopupUI.SetActive(true);
        StartCoroutine(ActivateSendDeclinePopupUI());
    }
    private IEnumerator ActivateSendDeclinePopupUI()
    {
        sendDeclinePopupUI.SetActive(true);
        yield return new WaitForSeconds(3f);
        sendDeclinePopupUI.SetActive(false);
    }

    public void OpenMatchingStartPopupUI()
    {
        matchingStartPopupUI.SetActive(true);
    }

    public void CloseMatchingStartPopupUI()
    {
        matchingStartPopupUI.SetActive(false);
    }
}
