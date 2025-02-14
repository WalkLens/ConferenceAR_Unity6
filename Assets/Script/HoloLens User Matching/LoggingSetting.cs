using UnityEngine;

[CreateAssetMenu(fileName = "LoggingSetting", menuName = "Debug/Logging Setting")]
public class LoggingSetting : ScriptableObject{
    public enum LogLevel{
        None,           // 로그 출력 안함
        Basic,          // 기본 로그 출력
        Detailed        // 상세 로그 출력(클래스명, 마스터 클라이언트 여부, 유저 명 포함)
    }

    public LogLevel logLevel = LogLevel.Basic;
}