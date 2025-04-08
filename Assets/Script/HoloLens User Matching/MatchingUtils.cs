using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CustomLogger;
using Unity.VisualScripting;
using UnityEngine;

public class MatchingUtils
{ 
    
    /// <summary>
    /// Meeting Info를 생성하고 반환합니다.
    /// </summary>
    /// <param name="name1">메세지 주고 받는 유저 A(순서 상관 없음)</param>
    /// <param name="name2">메세지 주고 받는 유저 B(순서 상관 없음)</param>
    /// <param name="meetingTimeLeft">만나기까지 남은 시간(현재 시간 기준)</param>
    /// <returns></returns>
    public static MeetingInfo GetMeetingInfo(string name1, string name2, float meetingTimeLeft)
    {
        MeetingInfo meetingInfo = new MeetingInfo();
        
        meetingInfo.MatchKey = GenerateMatchKey(name1, name2);
        meetingInfo.MeetingDateTime = GetFutureTimeAsString(meetingTimeLeft);
        
        return meetingInfo;
    }
    
    
    /// <summary>
    /// 두 사용자 이름을 정렬하고 현재 시간을 포함한 고유한 매칭 키를 생성합니다.
    /// </summary>
    /// <param name="user1">첫 번째 사용자 이름</param>
    /// <param name="user2">두 번째 사용자 이름</param>
    /// <returns>형식: "yyMMdd-HH:mm-ssfff_정렬된유저1-유저2"의 문자열</returns>
    public static string GenerateMatchKey(string user1, string user2)
    {
        // 날짜 및 시간 포맷 (yyMMdd-HH:mm-ssfff)
        string timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");

        // 사용자 이름 정렬 (사전순 정렬)
        string[] sortedUsers = new string[] { user1, user2 }.OrderBy(name => name).ToArray();

        // 매칭 키 조합
        string matchKey = $"{timestamp}_{sortedUsers[0]}-{sortedUsers[1]}";

        return matchKey;
    }
    /// <summary>
    /// 매칭 키에서 내 이름을 제외한 상대방의 이름을 추출합니다.
    /// </summary>
    /// <param name="matchKey">매칭 키 (형식: "yyyy-MM-dd HH:mm:ss_유저1-유저2")</param>
    /// <param name="myName">내 이름</param>
    /// <returns>상대방 이름 (없으면 빈 문자열)</returns>
    public static string ExtractOtherUser(string matchKey, string myName)
    {
        // 매칭 키 구조 확인
        string[] parts = matchKey.Split('_');
        if (parts.Length < 2) return string.Empty; // 잘못된 형식이면 반환 X

        // 유저 정보 추출
        string[] users = parts[1].Split('-'); // "유저1-유저2" 부분 분리

        // 내 이름이 포함된 경우 상대방 반환
        if (users.Length == 2)
        {
            if (users[0] == myName) return users[1];
            if (users[1] == myName) return users[0];
        }

        return string.Empty; // 매칭되지 않으면 빈 값 반환
    }

    /// <summary>
    /// 현재 시간에서 특정 분(minute) 후의 시간을 문자열로 반환 (형식: "yyyy-MM-dd HH:mm:ss")
    /// </summary>
    public static string GetFutureTimeAsString(float meetingTimeLeft)
    {
        DateTime futureTime = DateTime.Now.AddMinutes(meetingTimeLeft);
        return futureTime.ToString("yyyy-MM-dd HH:mm:ss");
    }
    
    /// <summary>
    /// 문자열을 DateTime 형식으로 변환
    /// </summary>
    public static DateTime ConvertStringToDateTime(string timeString)
    {
        DateTime parsedTime;
        if (DateTime.TryParseExact(timeString, "yyyy-MM-dd HH:mm:ss", 
                System.Globalization.CultureInfo.InvariantCulture, 
                System.Globalization.DateTimeStyles.None, 
                out parsedTime))
        {
            return parsedTime;
        }
        else
        {
            throw new FormatException($"Invalid time format: {timeString}");
        }
    }
    
    /// <summary>
    /// 주어진 timeString과 현재 시간의 차이를 분 단위로 반환
    /// </summary>
    /// <param name="timeString">yyMMdd-HH:mm-ssfff 형식의 시간 문자열</param>
    /// <returns>현재 시간과 비교한 남은 시간 (분 단위, 음수면 이미 지난 시간)</returns>
    public static int GetRemainingMinutes(string timeString)
    {
        try
        {
            // 문자열을 DateTime으로 변환
            DateTime targetTime = DateTime.ParseExact(timeString, "yyyy-MM-dd HH:mm:ss", null);

            // 현재 시간 가져오기
            DateTime now = DateTime.Now;

            // 시간 차이를 계산 (분 단위)
            TimeSpan timeDifference = targetTime - now;
            return (int)timeDifference.TotalMinutes; // 소수점 버림
        }
        catch (Exception ex)
        {
            FileLogger.Log("[GetRemainingMinutes] 시간 변환 실패");
            return int.MinValue; // 변환 실패 시 오류 값 반환
        }
    }
    
    /// <summary>
    /// 두 사람의 흥미 목록에서 공통된 흥미를 찾고, 존재 여부를 반환하는 메서드.
    /// </summary>
    /// <param name="personA">첫 번째 사람의 흥미 목록</param>
    /// <param name="personB">두 번째 사람의 흥미 목록</param>
    /// <param name="commonInterests">공통된 흥미 목록 (out 매개변수)</param>
    /// <returns>공통된 흥미가 존재하면 true, 없으면 false</returns>
    public static bool CheckCommonInterests(List<string> personA, List<string> personB, out List<string> commonInterests)
    {
        // 공통된 흥미 찾기
        commonInterests = personA.Intersect(personB).ToList();

        if (commonInterests.Count > 0)
        {
            FileLogger.Log($"공통된 흥미 발견! : {string.Join(", ", commonInterests)}");
            return true;
        }
        else
        {
            FileLogger.Log("공통된 흥미가 없습니다.");
            return false;
        }
    }
}