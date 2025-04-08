using System;
using System.IO;
using UnityEngine;
using Photon.Pun;

namespace CustomLogger{
    public static class FileLogger
    {
        private static LoggingSetting _setting;
        private static string _tempLogPath;
        private static bool _isInitialized = false;
        private static string _logPath;
        private static string _userName;  // 사용자 이름 저장용 정적 변수
        // 사용자 이름 설정 메서드
        public static void SetUserName(string userName)
        {
            if (!_isInitialized)
            {
                _userName = userName;
                _logPath = $"{Application.persistentDataPath}/Conference_debug_log_{_userName}.txt";
                
                // 기존 임시 로그 파일이 있다면 새 파일로 복사
                if (!string.IsNullOrEmpty(_tempLogPath) && File.Exists(_tempLogPath))
                {
                    try
                    {
                        // File.Copy(_tempLogPath, _logPath, true);
                        string[] files = Directory.GetFiles(Application.persistentDataPath, "Conference_debug_log*");
                        foreach (string file in files)
                        {
                            File.Delete(file);
                        }
                        _isInitialized = true;
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Failed to transfer log file: {e.Message}");
                    }
                }
                else
                {
                    _isInitialized = true;
                }
            }
        }
        
        private static string LogPath
        {
            get
            {
                // 사용자 이름이 설정된 경우
                if (!string.IsNullOrEmpty(_userName))
                {
                    return _logPath;
                }
                
                // 임시 파일 사용
                if (string.IsNullOrEmpty(_tempLogPath))
                {
                    _tempLogPath = $"{Application.persistentDataPath}/Conference_debug_log_temp.txt";
                }
                return _tempLogPath;
            }
        }

        static FileLogger(){
            _setting = Resources.Load<LoggingSetting>("LoggingSetting");
            if(_setting == null){
                Debug.LogError("LoggingSetting is not found in Resources folder.");
            }
        }

        public static void Log(string message, object caller = null)
        {
            if (_setting == null || _setting.logLevel == LoggingSetting.LogLevel.None)
                return;

            #if UNITY_EDITOR
                // Unity Editor에서는 Debug.Log 사용
                if (_setting.logLevel == LoggingSetting.LogLevel.Detailed && caller != null)
                {
                    string className = caller.GetType().Name.PadRight(20);
                    string role = GetRoleString().PadRight(6);
                    string userName = PhotonNetwork.NickName.PadRight(15);
                    
                    Debug.Log($"{className} | {role} | {userName} | {message}");
                }
                else
                {
                    Debug.Log(message);
                }
            #else
                // 유저 ID가 설정되기 전에는 로깅하지 않음
                if (string.IsNullOrEmpty(PhotonNetwork.NickName))
                    return;

                string finalMessage;
                if (_setting.logLevel == LoggingSetting.LogLevel.Detailed && caller != null)
                {
                    string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff").PadRight(23);
                    string className = caller.GetType().Name.PadRight(20);
                    string role = GetRoleString().PadRight(6);
                    string userName = PhotonNetwork.NickName.PadRight(15);
                    
                    finalMessage = $"[{timestamp}] " +
                                  $"| {className} " +
                                  $"| {role} " +
                                  $"| {userName} " +
                                  $"| {message}";
                }
                else
                {
                    finalMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] {message}";
                }

                try
                {
                    File.AppendAllText(LogPath, finalMessage + Environment.NewLine);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Failed to write to log file: {e.Message}");
                }
            #endif
        }

        public static void ClearLog()
        {
            #if !UNITY_EDITOR
                // 유저 ID가 설정되기 전에는 클리어하지 않음
                if (string.IsNullOrEmpty(PhotonNetwork.NickName))
                    return;

                try
                {
                    string logPath = LogPath;
                    if (File.Exists(logPath))
                        File.Delete(logPath);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Failed to clear log file: {e.Message}");
                }
            #endif
        }

        public static string GetRoleString(){
            if(string.IsNullOrEmpty(PhotonNetwork.CurrentRoom?.Name))
                return "Client";
            
            var hostManager = HostBehaviourManager.Instance;
            if(hostManager != null && hostManager.IsCentralHost)
                return "CentralHost";
            else if(PhotonNetwork.IsMasterClient)
                return "RoomMaster";
            else
                return "Client";
        }
    }
}