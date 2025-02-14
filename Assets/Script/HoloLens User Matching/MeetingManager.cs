using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CustomLogger;
using MixedReality.Toolkit.UX;
using UnityEngine;
using UnityEngine.Serialization;

public class MeetingManager : MonoBehaviour
{
    public static MeetingManager Instance;

    private float meetingTimeLeftValue = 0f; // 현재 남은 시간
    private bool isNotificationScheduled = false; // 알림 예약 여부

    [Header("Meeting Info(Input)")] public float meetingTimeLeftScrollSelected;
    public TimePicker timePicker;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    #region 미팅 예약, 시간에 따른 메서드 호출

    private Dictionary<string, Coroutine> notificationCoroutines = new Dictionary<string, Coroutine>();

    // Meeting Info에 따라 개별적으로 알림 예약 관리
    public void AddAlarmFromMeetingInfo(MeetingInfo meetingInfo)
    {
        string matchKey = meetingInfo.MatchKey;
        int meetingTimeLeft = MatchingUtils.GetRemainingMinutes(meetingInfo.MeetingDateTime);

        // 기존 예약이 있으면 해당 예약만 취소
        if (notificationCoroutines.ContainsKey(matchKey))
        {
            StopCoroutine(notificationCoroutines[matchKey]);
            notificationCoroutines.Remove(matchKey);
            FileLogger.Log($"기존 알림({matchKey}) 취소됨.", this);
        }

        if (meetingTimeLeft > 0)
        {
            // 새로운 meetingInfo 예약
            Coroutine newCoroutine = StartCoroutine(ScheduleNotification(meetingTimeLeft, matchKey));
            notificationCoroutines[matchKey] = newCoroutine;

            FileLogger.Log($"{meetingTimeLeft}분 후 알림 예약됨. (MatchKey: {matchKey})", this);
        }
        else
        {
            FileLogger.Log($"알림 예약 실패: 시간이 이미 지남 (MatchKey: {matchKey})", this);
        }
    }

    private IEnumerator ScheduleNotification(float timeLeft, string matchKey)
    {
        yield return new WaitForSeconds(timeLeft * 60); // timeLeft 분 후 알림 실행
        OnMatchTimerEnded(matchKey);
    }

    private void OnMatchTimerEnded(string matchKey)
    {
        FileLogger.Log("Meeting 시간 도달! 알림 실행!", this);
        isNotificationScheduled = false;

        // 매칭 성공 : myGameObject, partnerGameObject 할당
        // 전송 받은 UserName에 부합한 홀로렌즈 혹은 퀘스트 오브젝트 찾기
        if (!UserMatchingManager.Instance.myGameObject)
            UserMatchingManager.Instance.myGameObject = FindObjectsOfType<GameObject>()
                .FirstOrDefault(obj => obj.name.Contains(UserMatchingManager.Instance.myUserInfo.PhotonUserName));

        // MeetingInfo에서 targetUserName 추출하기
        string targetUserName =
            MatchingUtils.ExtractOtherUser(matchKey, UserMatchingManager.Instance.myGameObject.name);
        UserMatchingManager.Instance.FindPartnerHololensGameObject(targetUserName);

        if (UserMatchingManager.Instance.partnerGameObject != null)
            FileLogger.Log(
                $"파트너 게임 오브젝트 설정[Accept 보냄] : Target GameObject Name: {UserMatchingManager.Instance.partnerGameObject.name}",
                this);
        else
            FileLogger.Log($"파트너 게임 오브젝트 찾지 못함[Accept 보냄]: Target GameObject Name: {targetUserName}", this);

        // UI 활성화: 드디어 OOO님과 만날 시간이예요! UI(길 안내 받기, 거절하기)
        HololenUIManager.Instance.OpenMatchingStartPopupUI();

        // 코루틴 종료 시, 자기 자신을 Dictionary에서 삭제
        if (notificationCoroutines.ContainsKey(matchKey))
        {
            notificationCoroutines.Remove(matchKey);
            FileLogger.Log($"✅ {matchKey} 알림 완료 및 제거됨.", this);
        }
    }

    /// <summary>
    /// 설정된 time에 맞춰 나와 상대방에게 알람을 동기화합니다.
    /// Request 받은 이후에 호출됩니다.
    /// </summary>
    /// <param name="timeLeft">현재 시간 기준 timeLeft후에 미팅을 예약합니다.</param>
    public void SetAndSendMeetingInfo(float timeLeft = 0)
    {
        if (meetingTimeLeftScrollSelected > 0)
        {
            meetingTimeLeftValue = meetingTimeLeftScrollSelected;
            HololenUIManager.Instance.SetTime((int)meetingTimeLeftValue * 60);
        }
        else
        {
            meetingTimeLeftValue = timeLeft / 60;
        }

        // 동기화돼야 할 User A, B 이름 추출
        string myName = UserMatchingManager.Instance.myUserInfo.PhotonUserName;
        string targetUserPin = UserMatchingManager.Instance.GetPartnerUserPinNumber();
        string targetUserName = targetUserPin[..5] + "_hololens";
        MeetingInfo newMeetingInfo = MatchingUtils.GetMeetingInfo(myName, targetUserName, meetingTimeLeftValue);

        Debug.Log($"MatchKey: {newMeetingInfo.MatchKey}, MeetingDateTime: {newMeetingInfo.MeetingDateTime}");
        Debug.Log("만나는 시간: " + MatchingUtils.ConvertStringToDateTime(newMeetingInfo.MeetingDateTime));
        Debug.Log("만나기 까지 남은 시간: " + MatchingUtils.GetRemainingMinutes(newMeetingInfo.MeetingDateTime));

        // 0으로 초기화
        meetingTimeLeftScrollSelected = 0;

        // 전송 (pin + hololens)
        UserMatchingManager.Instance.SendMeetingInfo(newMeetingInfo,
            PhotonUserUtility.GetPlayerActorNumber(targetUserName));
    }

    #endregion
}