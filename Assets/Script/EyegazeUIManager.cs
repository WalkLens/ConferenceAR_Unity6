using DG.Tweening;
using Microsoft.CognitiveServices.Speech.Transcription;
using MRTK.Tutorials.MultiUserCapabilities;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class EyegazeUIManager : MonoBehaviour
{
    public static EyegazeUIManager main;
    public GameObject eyegazeUI;
    public float yOffset = 0.0f;
    //public DataManagerCtrl dataManagerCtrl;
    public string myPinNum;
    public GameObject flexibleButton;
    public GameObject eyegazeUIClone;
    public UserData eyegazedUserData;
    private TextMeshProUGUI[] profileInfos;
    private HorizontalLayoutGroup[] layout;
    private const float FULLSIZE = 280.0f;
    private const float InputDelay = 0.5f; // �Է� �� ��� �ð�
    private string[] keywordArray;
    private string[] interestsArray;

    private void Awake()
    {
        main = this;
    }

    private void Start()
    {
        keywordArray = new string[5];
        interestsArray = new string[5];
    }

    public void ActivateEyegazeUI(RaycastHit photonUser)
    {
        PhotonUserConferenceAR photonUserInfo = photonUser.collider.GetComponent<PhotonUserConferenceAR>();
        string pinNum = photonUserInfo.GetPIN().Substring(0, 5);

        Vector3 newPosition = photonUserInfo.gameObject.transform.position + Vector3.up * yOffset;

        if (myPinNum != pinNum)
        {
            Debug.Log("myPinNum : " + myPinNum + ", hitpinNum : " + pinNum);
            eyegazeUIClone = Instantiate(eyegazeUI, newPosition, Quaternion.identity);

            Transform cameraTransform = Camera.main.transform;
            eyegazeUIClone.transform.LookAt(cameraTransform);
            eyegazeUIClone.transform.Rotate(0, 180, 0);

            //Transform eyegazeLeft = eyegazeUIClone.transform.Find("Eyegaze_Left");
            //Transform eyegazeRight = eyegazeUIClone.transform.Find("Eyegaze_Right");

            //TextMeshProUGUI[] leftInfo = eyegazeLeft.gameObject.GetComponentsInChildren<TextMeshProUGUI>();

            profileInfos = eyegazeUIClone.gameObject.GetComponentsInChildren<TextMeshProUGUI>();
            eyegazedUserData = DatabaseManager.Instance.getUserData(pinNum.Substring(0, 5));
            Debug.Log("eyegazedUserData : " + pinNum.Substring(0, 5));

            // 0 : Name, 1 : Job, 2 : Team(Language), 4 : Intro, 8 : URL
            // �ϵ��ڵ��� �κ��̶� ���� �����ϴ°� ��ǥ�� ��
            profileInfos[0].text = eyegazedUserData.name;
            profileInfos[1].text = eyegazedUserData.job;
            profileInfos[2].text = eyegazedUserData.language;
            profileInfos[4].text = eyegazedUserData.introduction_text;
            profileInfos[8].text = eyegazedUserData.url;

            keywordArray[0] = eyegazedUserData.introduction_1;
            keywordArray[1] = eyegazedUserData.introduction_2;
            keywordArray[2] = eyegazedUserData.introduction_3;
            keywordArray[3] = eyegazedUserData.introduction_4;
            keywordArray[4] = eyegazedUserData.introduction_5;

            interestsArray[0] = eyegazedUserData.interest_1;
            interestsArray[1] = eyegazedUserData.interest_2;
            interestsArray[2] = eyegazedUserData.interest_3;
            interestsArray[3] = eyegazedUserData.interest_4;
            interestsArray[4] = eyegazedUserData.interest_5;


            layout = eyegazeUIClone.gameObject.GetComponentsInChildren<HorizontalLayoutGroup>();
            foreach (HorizontalLayoutGroup group in layout)
            {
                Debug.Log("Horizontal : " + group.name);
            }
            int layoutIndex = 1;
            for (int i = 0; i < keywordArray.Length; i++)
            {
                GameObject buttonClone = Instantiate(flexibleButton);
                TextMeshProUGUI text = buttonClone.GetComponentInChildren<TextMeshProUGUI>();
                text.text = keywordArray[i];
                buttonClone.transform.SetParent(layout[layoutIndex].transform, false);
                // UI ���� ���� (��� ����)
                Canvas.ForceUpdateCanvases();
                LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)layout[layoutIndex].transform);

                if ((i + 1) % 3 == 0)
                {
                    layoutIndex++;
                    if (layoutIndex >= layout.Length)
                    {
                        layoutIndex = layout.Length - 1;
                    }
                }
            }

            layoutIndex = 3;
            for (int i = 0; i < interestsArray.Length; i++)
            {
                GameObject buttonClone = Instantiate(flexibleButton);
                TextMeshProUGUI text = buttonClone.GetComponentInChildren<TextMeshProUGUI>();
                text.text = interestsArray[i];
                buttonClone.transform.SetParent(layout[layoutIndex].transform, false);
                // UI ���� ���� (��� ����)
                Canvas.ForceUpdateCanvases();
                LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)layout[layoutIndex].transform);

                if ((i + 1) % 3 == 0)
                {
                    layoutIndex++;
                    if (layoutIndex >= layout.Length)
                    {
                        layoutIndex = layout.Length - 1;
                    }
                }
            }

            for (int i = 3; i < profileInfos.Length; i++)
            {
                profileInfos[i].transform.gameObject.SetActive(false);
            }

            for (int i = 1; i < layout.Length; i++)
            {
                layout[i].transform.gameObject.SetActive(false);
            }

            // Canvas.ForceUpdateCanvases();
        }
        else
        {
            Debug.Log("You can't see me!!!");
        }
    }

    public void ActivateAllUIInfos()
    {
        Debug.Log("UI Ǯȭ�� Ȱ��ȭ");
        Transform canvas = eyegazeUIClone.transform.GetChild(0);
        RectTransform plate = canvas.GetChild(0).gameObject.GetComponent<RectTransform>();


        //Vector2 newSize = plate.sizeDelta;
        //newSize.y = FULLSIZE;
        //plate.sizeDelta = newSize;

        plate.DOSizeDelta(new Vector2(plate.sizeDelta.x, FULLSIZE), InputDelay).SetEase(Ease.OutQuad);

        DOVirtual.DelayedCall(InputDelay, () =>
        {
            for (int i = 3; i < profileInfos.Length; i++)
            {
                profileInfos[i].transform.gameObject.SetActive(true);
            }

            for (int i = 1; i < layout.Length; i++)
            {
                layout[i].transform.gameObject.SetActive(true);
            }
        });

    }

    public void DeactivateEyegazeUI()
    {
        GameObject[] eyegazeUIs = GameObject.FindGameObjectsWithTag("Eyegaze");
        foreach (GameObject eyegazeUI in eyegazeUIs)
        {
            Destroy(eyegazeUI);
        }
    }
}


