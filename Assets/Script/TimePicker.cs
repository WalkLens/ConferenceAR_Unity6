using MixedReality.Toolkit.UX;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimePicker : MonoBehaviour
{
    public GameObject parentGameObject;
    public PressableButton acceptButton;
    public PressableButton declineButton;
    public NotificationManager notificationManager;
    [Space] public ScrollRect scrollRect; // ��ũ�� rect
    public RectTransform content; // ��ũ���� content
    public TextMeshProUGUI[] timeTexts; // timeTexts �迭
    public RectTransform centerMarker; // �߾� ������
    public TextMeshProUGUI selectedTimeText;

    public int selectedIndex = 0; // ���õ� �ε���
    public bool isSnapping = false; // ���� ������ ����
    private string _partnerObjectName = ""; 
    void Start()
    {
        // timeTexts �ʱ�ȭ
        timeTexts = new TextMeshProUGUI[content.childCount];
        for (int i = 0; i < content.childCount; i++)
        {
            timeTexts[i] = content.GetChild(i).GetComponentInChildren<TextMeshProUGUI>();
        }

        // ó�� ���õ� �ؽ�Ʈ�� ù ��° �׸�
        selectedIndex = 0;
        SetBoldText(selectedIndex); // ù ��° �׸��� bold�� ����
        selectedTimeText = timeTexts[selectedIndex];

        // ScrollRect�� onValueChanged �̺�Ʈ�� ������ �߰�
        scrollRect.onValueChanged.AddListener(OnScroll);

        InitializeButtons();
    }

    public void CachePartnerName(string partnerName)
    {
        _partnerObjectName = partnerName;
    }
    public void InitializeButtons()
    {
        // _partnerName을 새로 할당했으니 다시 버튼 초기화
        acceptButton.OnClicked.RemoveAllListeners();
        declineButton.OnClicked.RemoveAllListeners();
        
        acceptButton.OnClicked.AddListener((() =>
        {
            parentGameObject.SetActive(false);
            notificationManager.OpenSendAcceptPopupUI();
            
            notificationManager.SendAcceptMessage(_partnerObjectName);
            CachePartnerName("");   // 다시 데이터 비우기
            
            HololenUIManager.Instance.AddReservedData(); // ���濡�� ���� ��û ���� ��, UI�� ǥ��
            HololenUIManager.Instance.MeetTimeUpdate();

            UserMatchingManager.Instance.MatchingStateUpdateAsTrue();
        }));
        declineButton.OnClicked.AddListener((() =>
        {
            parentGameObject.SetActive(false);
            notificationManager.OpenSendDeclinePopupUI();
            notificationManager.SendDeclineMessage(_partnerObjectName);

            HololenUIManager.Instance.SetTime(0);

            UserMatchingManager.Instance.MatchingStateUpdateAsTrue();
        }));
    }

    // ��ũ���� ��ȭ�� ������ ȣ��Ǵ� �Լ�
    void OnScroll(Vector2 position)
    {
        Debug.Log("Scroll...");
        // ��ũ���� ���� ���������� ó��
        if (!isSnapping)
        {
            FindClosestTextToCenter(); // ���� ����� �ؽ�Ʈ�� ã�Ƽ� ����
        }
    }

    void FindClosestTextToCenter()
    {
        // ���� ����� �ؽ�Ʈ�� ã�� ���� distance ���
        float closestDistance = float.MaxValue;
        int closestIndex = selectedIndex;

        for (int i = 0; i < timeTexts.Length; i++)
        {
            float distance = Mathf.Abs(GetWorldY(timeTexts[i].rectTransform) - GetWorldY(centerMarker));
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestIndex = i;
            }
        }

        // ���õ� �ε����� �ٲ������ �ؽ�Ʈ ��Ÿ���� �����ϰ�, ��ũ���� ����
        if (selectedIndex != closestIndex)
        {
            SetBoldText(closestIndex); // ���ο� �ؽ�Ʈ�� Bold�� ����
            selectedIndex = closestIndex; // ���õ� �ε��� ����
            selectedTimeText = timeTexts[selectedIndex];

            if (float.TryParse(selectedTimeText.text.Substring(0, selectedTimeText.text.Length - 1), out float result))
            {
                MeetingManager.Instance.meetingTimeLeftScrollSelected = result;
            }
            else
            {
                Debug.Log("변환 실패");
            }
        }
    }

    // ���� ����� �ؽ�Ʈ�� bold�� �����ϰ� �������� normal�� ����
    void SetBoldText(int index)
    {
        // ��� �ؽ�Ʈ�� Normal�� ����
        foreach (var text in timeTexts)
        {
            text.fontStyle = FontStyles.Normal;
        }

        // ���õ� �ؽ�Ʈ�� Bold�� ����
        timeTexts[index].fontStyle = FontStyles.Bold;
    }

    // RectTransform�� World Y ��ǥ ��ȯ
    private float GetWorldY(RectTransform rect)
    {
        return rect.position.y;
    }
}