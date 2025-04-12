using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;
using MixedReality.Toolkit;
using UnityEngine.Diagnostics;
using System;
using System.Linq;
using CustomLogger;
using Unity.VisualScripting;
using MixedReality.Toolkit.UX;

public class HololenUIManager : MonoBehaviour
{
    public static HololenUIManager Instance;

    public HmdUIEvent hmdUIEvent;

    [Header("MainBar")] public TextMeshProUGUI currentTime;
    public Image myImage;

    [Space] [Header("Calendar_L")] public TextMeshProUGUI[] SenderDataText;
    public List<GameObject> requestCanvas;

    [Space] [Header("Calendar_R")] public TextMeshProUGUI[] ReservatedDataText;
    public Image ReservatedDataCircleTimer;
    public Image ReservatedDataProfileImage;
    public TextMeshProUGUI[] RecentMetDataText;

    [Space] [Header("Base UI Canvas")] public GameObject MatchingRequestDataPrefab;
    public Transform MatchingRequestDataParent;
    public List<GameObject> MatchingRequestData; // ��Ī ��û�� ���� ������ ����Ʈ�� �߰��ž� ��
    public GameObject ReservedDataPrefab;
    public Transform ReservedDataParent;
    public List<GameObject> ReservedData; // ��Ī ��û�� ó���� ������ ����Ʈ�� �߰��ž� ��

    //public Dictionary<string, float> timers = new Dictionary<string, float>();
    public bool isTimerUpdated;

    /// <summary>
    /// time은 전역 변수로 관리되어 스크롤(선택)된 값에 따라 변화합니다.
    /// </summary>
    private int time = 0;
    private int distanceToPartner = 0;

    [Space] [Header("MatchingRequest")] public Image ReceiveRequestDetailProfileImage;
    public TextMeshProUGUI[] ReceiveRequestDetailText;
    public Image ViewProfileImage;
    public TextMeshProUGUI[] ViewProfileText;

    [Space] [Header("Popups")] public TextMeshProUGUI[] SendAcceptPopupText;
    public TextMeshProUGUI[] SendDeclinePopupText;
    public Image ReceiveRequestPopupProfileImage;
    public TextMeshProUGUI[] ReceiveRequestPopupText;
    public Image ReceiveAcceptPopupProfileImage;
    public TextMeshProUGUI[] ReceiveAcceptPopupText;
    public Image ReceiveDeclinePopupProfileImage;
    public TextMeshProUGUI[] ReceiveDeclinePopupText;
    public Image matchingStartPopupUIProfileImage;
    public TextMeshProUGUI[] matchingStartPopupUIText;
    [SerializeField] GameObject matchingStartPopupUI;
    [Header("Common Interests Popup")]
    public GameObject commonInterestsPopupUI;
    public TextMeshProUGUI[] commonInterestsPopupText;

    // Start is called before the first frame update
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }


    // Update is called once per frame
    void Update()
    {
        // �ð� ����
        currentTime.text = DateTime.Now.ToString(("hh:mm tt"));

        ReservatedDataUpdate();

        // DB ����
        if (isTimerUpdated)
        {
            MatchedUserData temp;
            temp.pin = UserMatchingManager.Instance.GetPartnerUserPinNumber();
            temp.time = (float)GetTime();
            UserMatchingManager.Instance.matchedUserData.Add(temp);

            //timers[UserMatchingManager.Instance.GetPartnerUserPinNumber()] = (float)GetTime();
            LoadReservatedDataFromDB();
            LoadRecentMetDataFromDB();

            isTimerUpdated = false;
            SetTime(0);
        }

        //if (Input.GetKey(KeyCode.M))                            // ��Ī ��û�� �� ���� �����ϴ� �Է�
        //{
        //    AddMatchingRequestData();
        //    LoadMatchingSenderDataFromDB();
        //}
    }

    //=================== Mainbar ====================//
    public void LoadReceiveRequestPopupTextFromFolder()
    {
        Sprite newSprite = Resources.Load<Sprite>(UserMatchingManager.Instance.myPin);
        if (newSprite != null)
        {
            myImage.sprite = newSprite;
        }
    }


    //=================== Calendar_L =================//
    // 이 부분은 동적으로 요청 받을 때 로드되도록 바뀜
    public void LoadMatchingSenderDataFromDB()
    {
        SenderDataText[0].text = DatabaseManager.Instance
            .getUserData(UserMatchingManager.Instance.GetPartnerUserPinNumber()).name;
        SenderDataText[1].text = DatabaseManager.Instance
            .getUserData(UserMatchingManager.Instance.GetPartnerUserPinNumber()).job;

        // TODO : �̹���, ���� ���µ� �ε� �ʿ�
    }

    //=================== Calendar_R =================//
    public void LoadReservatedDataFromDB()
    {
        ReservatedDataText[2].text = DatabaseManager.Instance
            .getUserData(UserMatchingManager.Instance.GetPartnerUserPinNumber()).name;
        ReservatedDataText[3].text = DatabaseManager.Instance
            .getUserData(UserMatchingManager.Instance.GetPartnerUserPinNumber()).job;
        // TODO : �̹���, ���� ���µ� �ε� �ʿ�
    }

    public void ReservatedDataUpdate()
    {
        //List<string> keys = new List<string>(timers.Keys); // ����� �����鿡 ���ؼ� Ÿ�̸� �۵�

        for(int i=0; i < UserMatchingManager.Instance.matchedUserData.Count; i++ )
        {
            var data = UserMatchingManager.Instance.matchedUserData[i];

            if (data.time >= 3600)
            {
                data.time -= Time.deltaTime;
                ReservatedDataText[0].text = (data.time / 3600).ToString("F0") + "시간 " +
                                             ((data.time % 3600) / 60).ToString("F0") + "분 이내";
                ReservatedDataText[1].text =
                    DateTime.Now.AddSeconds(data.time)
                        .ToString("hh:mm tt"); // !!! ��� ������ �ʿ� �����Ƿ� ���Ŀ� ������ ������
                ReservatedDataCircleTimer.fillAmount = data.time / (4 * 60 * 60);

                UserMatchingManager.Instance.matchedUserData[i] = data;
            }
            else if (data.time >= 60)
            {
                data.time -= Time.deltaTime;
                ReservatedDataText[0].text = (data.time / 60).ToString("F0") + "분 이내";
                ReservatedDataText[1].text =
                    DateTime.Now.AddSeconds(data.time)
                        .ToString("hh:mm tt"); // !!! ��� ������ �ʿ� �����Ƿ� ���Ŀ� ������ ������
                ReservatedDataCircleTimer.fillAmount = data.time / (4 * 60 * 60);

                UserMatchingManager.Instance.matchedUserData[i] = data;
            }
            else if (data.time >= 0)
            {
                data.time -= Time.deltaTime;
                ReservatedDataText[0].text = data.time.ToString("F0") + "초 이내";
                ReservatedDataText[1].text =
                    DateTime.Now.AddSeconds(data.time)
                        .ToString("hh:mm tt"); // !!! ��� ������ �ʿ� �����Ƿ� ���Ŀ� ������ ������
                ReservatedDataCircleTimer.fillAmount = data.time / (4 * 60 * 60);

                UserMatchingManager.Instance.matchedUserData[i] = data;
            }
            else
            {
                if (data.time != -1)
                {
                    data.time = -1; // 한번만 작동하도록
                    hmdUIEvent.OpenMatchingStartPopupUI();
                    //RemoveReservedData();
                    RemoveReservedData2(data.pin);
                }

                ReservatedDataText[1].text =
                    DateTime.Now.AddSeconds(data.time)
                        .ToString("hh:mm tt"); // !!! ��� ������ �ʿ� �����Ƿ� ���Ŀ� ������ ������
                ReservatedDataCircleTimer.fillAmount = data.time / (4 * 60 * 60);

                UserMatchingManager.Instance.matchedUserData[i] = data;
            }
        }
    }

    public void LoadRecentMetDataFromDB()
    {
        if (RecentMetDataText[0] != null)
            RecentMetDataText[0].text = DatabaseManager.Instance
                .getUserData(UserMatchingManager.Instance.GetPartnerUserPinNumber()).name;

        // TODO : �̹���, ���� ���µ� �ε� �ʿ�
    }


    /// <summary>
    /// 팝업을 누르지 않으면 자동으로 캘린더 내부에 요청 메세지를 추가한다.
    /// 내부 요청 메세지에 필요한 기능을 추가합니다.
    /// </summary>
    public void AddMatchingRequestData() // !!! ����� ���� ����� �κ�
    {
        string partnerPin = UserMatchingManager.Instance.GetPartnerUserPinNumber();

        GameObject newObject = Instantiate(MatchingRequestDataPrefab);

        // �θ�� MatchingRequestData�� ����
        newObject.transform.SetParent(MatchingRequestDataParent);

        int index = MatchingRequestDataParent.childCount;
        //newObject.name = "MatchingRequestData_" + index;
        newObject.name = "MatchingRequestData_" + partnerPin;       //+++ 0405
        newObject.transform.localPosition = Vector3.zero;
        newObject.transform.localScale = Vector3.one;
        newObject.transform.localRotation = Quaternion.identity;

        // MatchingRequestData ����Ʈ�� �߰�
        MatchingRequestData.Add(newObject);

        // �ؽ�Ʈ�� ����
        Transform senderProfileImageObject = newObject.transform.Find("Data0/ProfileImageMask/ProfileImage");
        Image profileImage = senderProfileImageObject.GetComponent<Image>();
        Sprite newSprite = Resources.Load<Sprite>(partnerPin);
        profileImage.sprite = newSprite;

        Transform senderNameObject = newObject.transform.Find("Data0/ProfileBaseData - V/Name");
        Transform senderPositionObject = newObject.transform.Find("Data0/ProfileBaseData - V/Position|Team");
        SenderDataText[0] = senderNameObject.GetComponent<TextMeshProUGUI>();
        SenderDataText[1] = senderPositionObject.GetComponent<TextMeshProUGUI>();

        // ��ư�鿡 ��� ����
        Transform sendMatchingRequestObject = newObject.transform.Find("Buttons - H/Send Matching Request");
        Transform declineObject = newObject.transform.Find("Buttons - H/Decline");
        Transform timePlusObject = newObject.transform.Find("Buttons - H/TimePlus");
        //Transform expandObject = newObject.transform.Find("Data0/Expand");

        PressableButton[] MatchingRequestButton = new PressableButton[4];
        MatchingRequestButton[0] = sendMatchingRequestObject.GetComponent<PressableButton>();
        MatchingRequestButton[1] = declineObject.GetComponent<PressableButton>();
        MatchingRequestButton[2] = timePlusObject.GetComponent<PressableButton>();
        //MatchingRequestButton[3] = expandObject.GetComponent<PressableButton>();

        MatchingRequestButton[0].OnClicked.AddListener(() =>
        {
            RemoveMatchingRequestData(newObject);                                               // 수정필요
            //hmdUIEvent.SendAcceptMessage();
            //string partnerPin = newObject.name.Substring(newObject.name.Length - 5);
            hmdUIEvent.SendAcceptMessage2(partnerPin);    //+++ 0405 핀번호 넘기기
            AddReservedData(); // ���濡�� ���� ��û ���� ��, UI�� ǥ��

            MeetingManager.Instance.meetingTimeLeftScrollSelected = 0;
            MeetTimeUpdate();

            MatchingRequestButton[0].OnClicked.RemoveAllListeners();
            MatchingRequestButton[2].OnClicked.RemoveAllListeners();
        });

        MatchingRequestButton[1].OnClicked.AddListener(() =>
        {
            RemoveMatchingRequestData(newObject);
            //hmdUIEvent.SendDeclineMessage();
            string partnerPin = newObject.name.Substring(newObject.name.Length - 5);
            hmdUIEvent.SendDeclineMessage2(partnerPin);    //+++ 0405 핀번호 넘기기
            SetTime(0);

            MatchingRequestButton[1].OnClicked.RemoveAllListeners();
            MatchingRequestButton[2].OnClicked.RemoveAllListeners();
        });

        MatchingRequestButton[2].OnClicked.AddListener(() =>
        {
            //+++ 0413 TimePicker 기능 수행에서 올바른 사용자에게 송신할 수 있도록 추가 
            UserMatchingManager.Instance.SetPartnerUserPinNumber(partnerPin);                               //+++ 0413 임시로 partner 정보를 바꾸도록 수정함 -> 더 올바른 방법으로 변경 필요

            // MeetTimePlus();
            MeetingManager.Instance.timePicker.parentGameObject.SetActive(true);
            RemoveMatchingRequestData(newObject);

            MatchingRequestButton[2].OnClicked.RemoveAllListeners();
        });

        /*MatchingRequestButton[3].OnClicked.AddListener(() =>
        {
            notificationManager.OpenProfileUI();
            LoadMatchingSenderDetailsFromDB();

            MatchingRequestButton[3].OnClicked.RemoveAllListeners();
        });*/

        requestCanvas.Add(newObject);
    }

    public void RemoveMatchingRequestData(GameObject objectToRemove)
    {
        if (MatchingRequestData.Contains(objectToRemove)) // �ش� ������Ʈ�� ����Ʈ�� ���ԵǾ� �ִ��� Ȯ��
        {
            MatchingRequestData.Remove(objectToRemove); // ����Ʈ���� �ش� ������Ʈ ����
            Destroy(objectToRemove); // ������ �ش� ������Ʈ ����
        }
    }

    public void AddReservedData()
    {
        Debug.Log("Add Reserved Data");
        GameObject newObject = Instantiate(ReservedDataPrefab);

        //// �θ�� MatchingRequestData�� ����
        newObject.transform.SetParent(ReservedDataParent);

        int index = ReservedDataParent.childCount;
        newObject.name = "ReservedData_" + index;
        newObject.transform.localPosition = Vector3.zero;
        newObject.transform.localScale = Vector3.one;
        newObject.transform.localRotation = Quaternion.identity;

        // ReservedData.Add(newObject);
        ReservedData.Insert(0, newObject);

        // �ؽ�Ʈ�� ����
        Transform leftTimeObject = newObject.transform.Find("DataContainer/TimeData/ProfileBaseData - V/LeftTime");
        Transform futureTimeObject = newObject.transform.Find("DataContainer/TimeData/ProfileBaseData - V/FutureTime");
        Transform timePlusObject = newObject.transform.Find("DataContainer/UserData/ProfileBaseData - V/Name");
        Transform positionObject = newObject.transform.Find("DataContainer/UserData/ProfileBaseData - V/Position|Team");
        Transform timerCircleObject = newObject.transform.Find("DataContainer/TimeData/TimerBackground/Timer");

        Transform metProfileImageObject =
            newObject.transform.Find("DataContainer/UserData/ProfileImageMask/ProfileImage");
        Image profileImage = metProfileImageObject.GetComponent<Image>();
        Sprite newSprite = Resources.Load<Sprite>(UserMatchingManager.Instance.GetPartnerUserPinNumber());
        profileImage.sprite = newSprite;

        ReservatedDataText = new TextMeshProUGUI[4];
        ReservatedDataText[0] = leftTimeObject.GetComponent<TextMeshProUGUI>();
        ReservatedDataText[1] = futureTimeObject.GetComponent<TextMeshProUGUI>();
        ReservatedDataText[2] = timePlusObject.GetComponent<TextMeshProUGUI>();
        ReservatedDataText[3] = positionObject.GetComponent<TextMeshProUGUI>();
        ReservatedDataCircleTimer = timerCircleObject.GetComponent<Image>();
    }

    public void RemoveReservedData()
    {
        if (ReservedData.Count > 0)
        {
            GameObject lastItem = ReservedData[ReservedData.Count - 1]; // 마지막 오브젝트 가져오기
            ReservedData.RemoveAt(ReservedData.Count - 1); // 리스트에서 제거
            Destroy(lastItem); // 오브젝트 삭제
        }
        else
        {
            Debug.LogWarning("ReservedData 리스트가 비어 있습니다.");
        }
        /*if (ReservedData.Count > 0)
        {
            ReservedData.RemoveAt(ReservedData.Count-1);        // ù ��° ��Ҹ� ����Ʈ���� ����
            Destroy(ReservedData[ReservedData.Count-1]);        // ������ ù ��° ����� ���� ������Ʈ�� ����
        }*/
    }

    public void RemoveReservedData2(string pinNum)
    {
        UserMatchingManager.Instance.matchedUserData.RemoveAll(data => data.pin == pinNum);
    }


    //=================== MatchingRequest =================//
    public void LoadReceiveRequestDetailTextFromDB()
    {
        Sprite newSprite = Resources.Load<Sprite>(UserMatchingManager.Instance.GetPartnerUserPinNumber());
        if (newSprite != null)
        {
            ReceiveRequestDetailProfileImage.sprite = newSprite;
        }

        ReceiveRequestDetailText[0].text = DatabaseManager.Instance
            .getUserData(UserMatchingManager.Instance.GetPartnerUserPinNumber()).introduction_text;
        ReceiveRequestDetailText[1].text = DatabaseManager.Instance
            .getUserData(UserMatchingManager.Instance.GetPartnerUserPinNumber()).job;
        ReceiveRequestDetailText[2].text =
            DatabaseManager.Instance.getUserData(UserMatchingManager.Instance.GetPartnerUserPinNumber()).name +
            "님께서 만남을 요청하셨습니다!";
    }


    public void LoadMatchingSenderDetailsFromDB()
    {
        Sprite newSprite = Resources.Load<Sprite>(UserMatchingManager.Instance.GetPartnerUserPinNumber());
        if (newSprite != null)
        {
            ViewProfileImage.sprite = newSprite;
        }

        ViewProfileText[0].text = DatabaseManager.Instance
            .getUserData(UserMatchingManager.Instance.GetPartnerUserPinNumber()).name;
        ViewProfileText[1].text = DatabaseManager.Instance
            .getUserData(UserMatchingManager.Instance.GetPartnerUserPinNumber()).job;
        ViewProfileText[2].text = DatabaseManager.Instance
            .getUserData(UserMatchingManager.Instance.GetPartnerUserPinNumber()).introduction_text;
        ViewProfileText[3].text = DatabaseManager.Instance
            .getUserData(UserMatchingManager.Instance.GetPartnerUserPinNumber()).introduction_1;
        ViewProfileText[4].text = DatabaseManager.Instance
            .getUserData(UserMatchingManager.Instance.GetPartnerUserPinNumber()).introduction_2;
        ViewProfileText[5].text = DatabaseManager.Instance
            .getUserData(UserMatchingManager.Instance.GetPartnerUserPinNumber()).introduction_3;
        //ViewProfileText[6].text = DatabaseManager.Instance.getUserData(UserMatchingManager.Instance.GetPartnerUserPinNumber()).introduction_4;
        //ViewProfileText[7].text = DatabaseManager.Instance.getUserData(UserMatchingManager.Instance.GetPartnerUserPinNumber()).introduction_5;
        ViewProfileText[6].text = DatabaseManager.Instance
            .getUserData(UserMatchingManager.Instance.GetPartnerUserPinNumber()).interest_1;
        ViewProfileText[7].text = DatabaseManager.Instance
            .getUserData(UserMatchingManager.Instance.GetPartnerUserPinNumber()).interest_2;
        ViewProfileText[8].text = DatabaseManager.Instance
            .getUserData(UserMatchingManager.Instance.GetPartnerUserPinNumber()).interest_3;
        //ViewProfileText[11].text = DatabaseManager.Instance.getUserData(UserMatchingManager.Instance.GetPartnerUserPinNumber()).interest_4;
        //ViewProfileText[12].text = DatabaseManager.Instance.getUserData(UserMatchingManager.Instance.GetPartnerUserPinNumber()).interest_5;
        ViewProfileText[9].text = DatabaseManager.Instance
            .getUserData(UserMatchingManager.Instance.GetPartnerUserPinNumber()).url;

        // TODO : �̹���, ���� ���µ� �ε� �ʿ�
    }

    //=================== Popups =================//
    public void LoadReceiveRequestPopupTextFromDB()
    {
        Sprite newSprite = Resources.Load<Sprite>(UserMatchingManager.Instance.GetPartnerUserPinNumber());
        if (newSprite != null)
        {
            ReceiveRequestPopupProfileImage.sprite = newSprite;
        }

        ReceiveRequestPopupText[0].text =
            DatabaseManager.Instance.getUserData(UserMatchingManager.Instance.GetPartnerUserPinNumber()).name +
            "님께서 만남을 요청했습니다";
    }

    public void LoadReceiveAcceptPopupTextFromDB()
    {
        Sprite newSprite = Resources.Load<Sprite>(UserMatchingManager.Instance.GetPartnerUserPinNumber());
        if (newSprite != null)
        {
            ReceiveAcceptPopupProfileImage.sprite = newSprite;
        }

        ReceiveAcceptPopupText[0].text =
            DatabaseManager.Instance.getUserData(UserMatchingManager.Instance.GetPartnerUserPinNumber()).name +
            "님께서 만남을 수락했습니다";
    }

    public void LoadReceiveDeclinePopupTextFromDB()
    {
        Sprite newSprite = Resources.Load<Sprite>(UserMatchingManager.Instance.GetPartnerUserPinNumber());
        if (newSprite != null)
        {
            ReceiveDeclinePopupProfileImage.sprite = newSprite;
        }

        ReceiveDeclinePopupText[0].text =
            DatabaseManager.Instance.getUserData(UserMatchingManager.Instance.GetPartnerUserPinNumber()).name +
            "님께서 만남을 거절했습니다";
    }

    public void LoadMatchingStartPopupUITextFromDB()
    {
        Sprite newSprite = Resources.Load<Sprite>(UserMatchingManager.Instance.GetPartnerUserPinNumber());
        if (newSprite != null)
        {
            matchingStartPopupUIProfileImage.sprite = newSprite;
        }

        matchingStartPopupUIText[0].text = "드디어 " +
                                           DatabaseManager.Instance
                                               .getUserData(UserMatchingManager.Instance.GetPartnerUserPinNumber())
                                               .name + "님과 만날 시간이예요!";
        matchingStartPopupUIText[1].text = "드디어 " + DatabaseManager.Instance
            .getUserData(UserMatchingManager.Instance.GetPartnerUserPinNumber()).job;
        matchingStartPopupUIText[2].text = distanceToPartner.ToString() + "m";
    }

    public void SetAndShowCommonInterestsText(List<string> commonInterests)
    {
        commonInterestsPopupUI.SetActive(true);
        // TODO: 공통된 데이터에 따른 처리
        for (int i = 0; i < commonInterests.Count; i++)
        {
            commonInterestsPopupText[i].text = commonInterests[i];
            commonInterestsPopupText[i].gameObject.SetActive(true);
            
            FileLogger.Log($"나와 파트너의 공통 관심사 {i+1}번: {commonInterests[i]}");
        }
    } 
    
    /// <summary>
    /// 활성화된 공통 흥미 UI를 끄고 잔여 데이터를 지운다. 
    /// </summary>
    public void HideCommonInterests()
    {
        for (int i = 0; i < commonInterestsPopupText.Length; i++)
        {
            commonInterestsPopupText[i].text = "@@@";
            commonInterestsPopupText[i].gameObject.SetActive(false);
        }
        commonInterestsPopupUI.SetActive(false);
    }
    
    //=================== Time  =================//
    public void MeetTimePlus()
    {
        time += 600;
        Debug.Log(time);
    }

    public void MeetTimeMinus()
    {
        int temp = time;
        temp -= 600;
        if (temp > 0)
        {
            time = temp;
        }
        else
        {
            time = 0;
        }

        Debug.Log(time);
    }

    /// <summary>
    /// 설정된 time에 맞춰 나와 상대방에게 알람을 동기화합니다.
    /// </summary>
    public void MeetTimeUpdate()
    {
        isTimerUpdated = true;

        MeetingManager.Instance.SetAndSendMeetingInfo(time);
    }

    public int GetTime()
    {
        return time;
    }

    public void SetTime(int newTime)
    {
        time = newTime;
    }


    public void SetDistanceToPartner(int val)
    {
        distanceToPartner = val;
    }
}