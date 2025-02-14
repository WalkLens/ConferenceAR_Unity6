using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using System.Linq;

public class MeetingTest : MonoBehaviour
{
    public GameObject buttonsParents;
    public GameObject userSelection;
    public List<Button> buttons = new List<Button>();
    public List<Button> userSelectionButtons = new List<Button>();
    public float meetingTimeLeft;
    public MeetingInfo meetingInfo;

    string user1 = "00000";
    string user2 = "11111";
    public string user3;
    public List<string> usersString = new List<string>();
    
    void Start()
    {
        SetButtonInfo();
        
        Debug.Log(MatchingUtils.GenerateMatchKey(user1, user2));
    }

    void SetButtonInfo()
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            float timeLeft = i * 5;
            Debug.Log(buttons[i].gameObject);

            TextMeshProUGUI textComponent = buttons[i].GetComponentInChildren<TextMeshProUGUI>();
    
            if (textComponent == null)
            {
                Debug.LogError($"버튼 {buttons[i].gameObject.name}에 TextMeshProUGUI가 없습니다!");
            }
            else
            {
                textComponent.text = $"만나는 시간: {timeLeft}분 뒤로 설정";
            }

            // ❗Lambda 표현식으로 실행 시점 제어
            buttons[i].onClick.AddListener(() => SetMeetingTimeLeftTest(timeLeft));
        }
        
        for (int j = 0; j < userSelectionButtons.Count; j++)
        {
            userSelectionButtons[j].GetComponentInChildren<TextMeshProUGUI>().text = $"idx: {j} 플레이어로 지정";

            // ✅ 지역 변수를 캡처하여 버튼이 각기 다른 `j` 값을 사용하도록 보장
            int capturedIndex = j;
            userSelectionButtons[j].onClick.AddListener(() => SetUsers(capturedIndex.ToString()));
        }             
    }
    
    void SetMeetingTimeLeftTest(float timeLeft)
    {
        meetingTimeLeft = timeLeft;
        
        string userA = usersString.LastOrDefault() ?? "No Last User"; // 마지막 값이 없으면 기본 문자열 반환
        string userB = usersString.Count >= 2 
            ? usersString.ElementAt(usersString.Count - 2) 
            : "No Second Last User"; // 최소 2개 이상의 요소가 있을 때만 마지막-1 요소 가져오기
        
        
        meetingInfo = MatchingUtils.GetMeetingInfo(userA, userB, meetingTimeLeft); 
        
        Debug.Log($"MatchKey: {meetingInfo.MatchKey}, MeetingDateTime: {meetingInfo.MeetingDateTime}");
        Debug.Log("만나는 시간: "+ MatchingUtils.ConvertStringToDateTime(meetingInfo.MeetingDateTime));
        Debug.Log("만나기 까지 남은 시간: "+MatchingUtils.GetRemainingMinutes(meetingInfo.MeetingDateTime));
        
        UserMatchingManager.Instance.SendMeetingInfo(meetingInfo, int.Parse(user3));
    }

    void SetUsers(string index)
    {
        Debug.Log(index);
        usersString.Add(index);
    }
}