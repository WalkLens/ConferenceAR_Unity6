using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Microsoft.MixedReality.GraphicsTools;
using TMPro;
using MRTK.Tutorials.MultiUserCapabilities;
using Photon.Pun;

public class ARUIManager : MonoBehaviour
{
    public static ARUIManager main;
    //public Button targetButton; // 클릭할 버튼
    public RectTransform uiElement; // 변경할 UI 요소 (예: Panel)
    public TextMeshProUGUI header, text;
    public GameObject sprite, pinInputField, mainUI;
    //public Image uiImage; // UI에 적용할 이미지
    public ARUIData[] ARUIData;
    public int idx; // 0 : InputField, 1 : Incorrect, 2 : Correct
    public CustomKeyboard customKeyboard;
    private const float InputDelay = 0.5f; // 입력 후 대기 시간
    private float duration = 1.0f; // 애니메이션 지속 시간    

    void Awake()
    {
        main = this;
    }

    void Start()
    {
        // 키보드의 입력 완료 이벤트 구독
        customKeyboard.OnInputComplete += HandleInputComplete;
    }

    public void ChangeUI(int idx)
    {
        ApplyUIChanges(idx);

        // 이후 UI 상태로 변경
        DOVirtual.DelayedCall(ARUIData[idx].delayTime, () =>
        {
            switch (idx)
            {
                case 1:
                    RestoreUI(ARUIData[idx].delayTime);
                    break;
                case 2:
                    DisableInputField();
                    mainUI.SetActive(true);
                    HololenUIManager.Instance.LoadReceiveRequestPopupTextFromFolder();                  // SM ADD 0209

                    break;
            }
        });
    }

    public void ApplyUIChanges(int idx)
    {
        DOVirtual.DelayedCall(InputDelay, () =>
        {
            uiElement.DOScale(ARUIData[idx].targetScale, duration).SetEase(Ease.OutBack);
            Material changedMaterial = new Material(ARUIData[idx].material);
            uiElement.GetComponent<CanvasElementRoundedRect>().material = changedMaterial;
            header.transform.gameObject.SetActive(ARUIData[idx].isHeaderActivated);
            text.transform.gameObject.SetActive(ARUIData[idx].isTextActivated);
            sprite.SetActive(ARUIData[idx].isSpriteActivated);
            pinInputField.SetActive(!ARUIData[idx].isTextActivated);
            sprite.GetComponent<Image>().sprite = ARUIData[idx].spriteImage;
            text.text = ARUIData[idx].text;
        });
    }

    public void RestoreUI(float delay)
    {
        idx = 0;
        ApplyUIChanges(idx);
    }


    private void HandleInputComplete(string input)
    {
        Debug.Log("입력 완료됨: " + input);

        // 0.5초 후 입력 필드 초기화
        DOVirtual.DelayedCall(InputDelay, () =>
        {
            // DB 체크
            if (DatabaseManager.Instance.getUserData(input) != null)
            {
                ChangeUI(2);

                Debug.Log("PhotonNetwork.IsConnected: " + PhotonNetwork.IsConnected);

                if (!PhotonNetwork.IsConnected)
                {
                    Debug.LogWarning("PhotonNetwork에 연결되어 있지 않음. 다시 연결을 시도합니다.");
                    PhotonNetwork.ConnectUsingSettings();
                }
                else
                {
                    //// 현재 내 오브젝트 찾기
                    //PhotonUserConferenceAR[] allUsers = FindObjectsOfType<PhotonUserConferenceAR>();
                    //foreach (var user in allUsers)
                    //{
                    //    if (user.GetComponent<PhotonView>().IsMine)  // 내 로컬 오브젝트만 선택
                    //    {
                    //        user.UpdateNickName();
                    //        break;  // 내 것만 찾으면 루프 종료
                    //    }
                    //}
                    UserMatchingManager.Instance.myPin = input;                                      // SM ADD for find myself
                    PhotonLobbyConferenceAR.Lobby.JoinOrCreateRoom(input);
                    EyegazeUIManager.main.myPinNum = input;
                }

            }
            else
            {
                ChangeUI(1);
            }

            customKeyboard.ResetInput();
        });
    }

    // uiElement의 최상위 부모를 찾아 비활성화하는 함수
    public void DisableInputField()
    {
        Transform parent = uiElement.transform;

        // 부모를 두 단계 올라가서 InputField를 찾음
        parent = parent.parent.parent;

        // 찾은 최상위 부모를 비활성화
        parent.gameObject.SetActive(false);
    }

}
