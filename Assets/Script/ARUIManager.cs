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
    //public Button targetButton; // Ŭ���� ��ư
    public RectTransform uiElement; // ������ UI ��� (��: Panel)
    public TextMeshProUGUI header, text;
    public GameObject sprite, pinInputField, mainUI;
    //public Image uiImage; // UI�� ������ �̹���
    public ARUIData[] ARUIData;
    public int idx; // 0 : InputField, 1 : Incorrect, 2 : Correct
    public CustomKeyboard customKeyboard;
    private const float InputDelay = 0.5f; // �Է� �� ��� �ð�
    private float duration = 1.0f; // �ִϸ��̼� ���� �ð�    

    void Awake()
    {
        main = this;
    }

    void Start()
    {
        // Ű������ �Է� �Ϸ� �̺�Ʈ ����
        customKeyboard.OnInputComplete += HandleInputComplete;
    }

    public void ChangeUI(int idx)
    {
        ApplyUIChanges(idx);

        // ���� UI ���·� ����
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
        Debug.Log("�Է� �Ϸ��: " + input);

        // 0.5�� �� �Է� �ʵ� �ʱ�ȭ
        DOVirtual.DelayedCall(InputDelay, () =>
        {
            // DB üũ
            if (DatabaseManager.Instance.getUserData(input) != null)
            {
                ChangeUI(2);

                Debug.Log("PhotonNetwork.IsConnected: " + PhotonNetwork.IsConnected);

                if (!PhotonNetwork.IsConnected)
                {
                    Debug.LogWarning("PhotonNetwork�� ����Ǿ� ���� ����. �ٽ� ������ �õ��մϴ�.");
                    PhotonNetwork.ConnectUsingSettings();
                }
                else
                {
                    //// ���� �� ������Ʈ ã��
                    //PhotonUserConferenceAR[] allUsers = FindObjectsOfType<PhotonUserConferenceAR>();
                    //foreach (var user in allUsers)
                    //{
                    //    if (user.GetComponent<PhotonView>().IsMine)  // �� ���� ������Ʈ�� ����
                    //    {
                    //        user.UpdateNickName();
                    //        break;  // �� �͸� ã���� ���� ����
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

    // uiElement�� �ֻ��� �θ� ã�� ��Ȱ��ȭ�ϴ� �Լ�
    public void DisableInputField()
    {
        Transform parent = uiElement.transform;

        // �θ� �� �ܰ� �ö󰡼� InputField�� ã��
        parent = parent.parent.parent;

        // ã�� �ֻ��� �θ� ��Ȱ��ȭ
        parent.gameObject.SetActive(false);
    }

}
