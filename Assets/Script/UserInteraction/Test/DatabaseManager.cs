using System;
using System.Net;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Threading.Tasks;

// 내부 구현은 (코루틴 등으로) 바뀔 수 있으나, 큰 함수 구조는 바뀌지 않을 듯 합니다.
//
// Usage:
// bool isPINDuplicate(string PIN) -> 중복된 PIN이 있는지 확인
// bool registerProfile(UserData userData) -> UserData 등록 시도 (중복 시 web에서 error 뱉음)
// bool editProfile(string PIN, UserData userData) -> PIN으로 findUser에서 ID 찾은 후, userData 수정 시도
// int findUser(string PIN) -> PIN으로 DB의 userID 반환 (0: 없음, -1: Error)
// UserData getUserData(string PIN) -> PIN으로 UserData 받아옴

public class DatabaseManager : MonoBehaviour
{
    // 러시아 - "ru-RU"
    // 스페인어 - "es-ES"
    // 독일어 - "de-DE"
    // 중국어 - "zh-HK"
    // 한국어 - "ko-KR"
    // 영어 - "en-US"
    // 일본어 - "ja-JP"
    
    [SerializeField] private UserData userDataTest;
    public UserData playerUserData;
    
    public static DatabaseManager Instance { get; private set;}
    [SerializeField] private string address; // REST API IP Address
    [SerializeField] private string port; // REST API Port
    // private Dictionary<string, string> userData;
    
    // Start is called before the first frame update
    void Start()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("DatabaseManager already exists. This instance will be destroyed.");
            Destroy(this);
        }
    }

    void Update()
    {
        if(Input.GetKey(KeyCode.T))
        {
            //Debug.Log("isPINDuplicate: " + isPINDuplicate("12345").ToString());
            //Debug.Log("registerProfile: " + registerProfile(userDataTest).ToString());
            //Debug.Log("editProfile: " + editProfile(userDataTest.pin, userDataTest));
            //Debug.Log("findUser: " + findUser("12345").ToString());
            Debug.Log("getUserData: " + getUserData("12345").ToString());
        }
    }

    public bool isPINDuplicate(string PIN) // PIN이 있는지 확인
    {
        string apiUrl = $"http://{address}:{port}/users/search?PIN={PIN}";
        int response;
        
        try
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiUrl);
            request.Method = "GET";

            using (HttpWebResponse httpResponse = (HttpWebResponse)request.GetResponse())
            {
                using (StreamReader reader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    string result = reader.ReadToEnd();
                    if (int.TryParse(result, out int parsedResponse))
                    {
                        response = parsedResponse;
                    }
                    else
                    {
                        Debug.LogError("Invalid response format.");
                        response = -1;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error: {ex.Message}");
            response = -1;
        }

        if(response == -1) // 0: 없음, -1: 오류, 나머지: userid
        {
            throw new Exception("PIN Error!");
        }
        
        if(response == 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public bool registerProfile(UserData userData)
    {
        // 요청 URL
        string apiUrl = $"http://{address}:{port}/users/";

        // UserData 객체 생성 및 데이터 초기화
        // UserData userData = new UserData
        // {
        //     pin = user_pin,
        //     name = name_str,
        //     job = job,
        //     language = language,
        //     introduction_1 = introduction_1,
        //     introduction_2 = introduction_2,
        //     introduction_3 = introduction_3,
        //     introduction_4 = introduction_4,
        //     introduction_5 = introduction_5,
        //     interest_1 = interest_1,
        //     interest_2 = interest_2,
        //     interest_3 = interest_3,
        //     interest_4 = interest_4,
        //     interest_5 = interest_5,
        //     introduction_text = introduction_text,
        //     url = url,
        //     photo_url = photo_url,
        //     autoaccept = autoaccept
        // };

        // JSON 데이터 생성
        string jsonData = JsonUtility.ToJson(userData);
        Debug.Log("Serialized JSON Data: " + jsonData); // 디버깅용 로그 출력

        // UnityWebRequest 객체 생성
        using (UnityWebRequest webRequest = new UnityWebRequest(apiUrl, "POST"))
        {
            // 헤더 설정
            webRequest.SetRequestHeader("Content-Type", "application/json");

            // JSON 데이터를 전송할 수 있도록 설정
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonData);
            webRequest.uploadHandler = new UploadHandlerRaw(jsonToSend);
            webRequest.downloadHandler = new DownloadHandlerBuffer();

            // 요청 전송 및 완료 대기
            webRequest.SendWebRequest();

            while (!webRequest.isDone)
            {
                // 요청이 완료될 때까지 대기
            }

            // 요청 성공 여부 확인
            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("User registered successfully: " + webRequest.downloadHandler.text);
                return true;
            }
            else
            {
                Debug.LogError("Failed to register user: " + webRequest.error);
                return false;
            }
        }
    }

    public bool editProfile(string PIN, UserData userdata)
    {
        int userID = findUser(PIN);
        string apiUrl = $"http://{address}:{port}/users/{userID}";

        try
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiUrl);
            request.Method = "PUT";
            request.ContentType = "application/json";

            // Serialize userdata to JSON
            string jsonData = JsonUtility.ToJson(userdata);
            byte[] dataBytes = System.Text.Encoding.UTF8.GetBytes(jsonData);

            // Write data to request stream
            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(dataBytes, 0, dataBytes.Length);
            }

            using (HttpWebResponse httpResponse = (HttpWebResponse)request.GetResponse())
            {
                using (StreamReader reader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    string result = reader.ReadToEnd();
                    Debug.Log($"Profile updated successfully: {result}");
                    return true;
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error updating profile: {ex.Message}");
            return false;
        }
    }

    public int findUser(string PIN) // return userID
    {
        string apiUrl = $"http://{address}:{port}/users/search?PIN={PIN}";
        int userID;
        
        try
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiUrl);
            request.Method = "GET";

            using (HttpWebResponse httpResponse = (HttpWebResponse)request.GetResponse())
            {
                using (StreamReader reader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    string result = reader.ReadToEnd();
                    if (int.TryParse(result, out int parsedResponse))
                    {
                        userID = parsedResponse;
                    }
                    else
                    {
                        Debug.LogError("Invalid response format.");
                        userID = -1;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error: {ex.Message}");
            userID = -1;
        }
        
        return userID;
    }

    public UserData getUserData(string PIN)
    {
        int userID = findUser(PIN);
        string apiUrl = $"http://{address}:{port}/users/{userID}";
        
        UserData receivedUserData = null;

        if (userID > 0) // 0, -1이 아님
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiUrl);
                request.Method = "GET";
                request.ContentType = "application/json";

                using (HttpWebResponse httpResponse = (HttpWebResponse)request.GetResponse())
                {
                    using (StreamReader reader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        string jsonResponse = reader.ReadToEnd();
                        receivedUserData = JsonUtility.FromJson<UserData>(jsonResponse);
                        //Debug.Log($"User data retrieved successfully: {jsonResponse}");
                        //Debug.Log(receivedUserData.job);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error retrieving user data: {ex.Message}");
            }
        }
        
        return receivedUserData;
    }
}
