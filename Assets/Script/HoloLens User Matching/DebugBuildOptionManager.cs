using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomLogger;

public class DebugBuildOptionManager : MonoBehaviour
{
    public static DebugBuildOptionManager Instance;
    public BuildOptions buildOptions;
    
    private void Awake()
    {
        if(Instance == null) Instance = this;
        
        DontDestroyOnLoad(this);
        
        // 플랫폼별 실행 코드
#if UNITY_IOS || UNITY_ANDROID
        string deviceModel = SystemInfo.deviceModel.ToLower();

        if (deviceModel.Contains("quest"))
        {
            buildOptions = BuildOptions.Hololens;
            FileLogger.Log("Quest에서 실행 중", this);
        }
        else
        {
            buildOptions = BuildOptions.Hololens;
            FileLogger.Log("모바일 Android에서 실행 중", this);
        }
            FileLogger.Log("📱 Running on Mobile (iOS or Android)", this);
            

            
#elif UNITY_WSA || UNITY_WINRT
            //FileLogger.Log("💻 Running on UWP (Windows Store App)", this);
            buildOptions = BuildOptions.Hololens;
#elif UNITY_EDITOR
        Screen.SetResolution(1080, 1920, FullScreenMode.Windowed);
#endif
        
    }


    public enum BuildOptions
    {
        Mobile,
        Hololens,
        Quest
    }
}
