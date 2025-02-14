using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PinInputManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField[] pinFields; // TMP InputFields 배열
    [SerializeField] private Button submitButton;       // PIN 제출 버튼

    private string pinCode = "";

    void Start()
    {
        // 초기화 및 각 필드의 입력 이벤트 추가
        for (int i = 0; i < pinFields.Length; i++)
        {
            int index = i; // Closure 문제 방지
            pinFields[i].onValueChanged.AddListener((value) => OnPinValueChanged(index, value));
        }

        submitButton.onClick.AddListener(OnSubmit);
    }

    private void OnPinValueChanged(int index, string value)
    {
        if (value.Length > 0)
        {
            pinCode = UpdatePinCode(); // PIN 값 갱신
            if (index < pinFields.Length - 1)
            {
                pinFields[index + 1].Select(); // 다음 필드로 포커스 이동
            }
        }
        else if (value.Length == 0 && index > 0)
        {
            pinFields[index - 1].Select(); // 이전 필드로 포커스 이동
        }
    }

    private string UpdatePinCode()
    {
        string code = "";
        foreach (var field in pinFields)
        {
            code += field.text;
        }
        return code;
    }

    private void OnSubmit()
    {
        Debug.Log("PIN Code Entered: " + pinCode);
        if (pinCode.Length == pinFields.Length)
        {
            Debug.Log("PIN 입력 완료!");
            // 여기서 PIN 검증 로직 추가
            UIManager.Instance.pin = pinCode;
        }
        else
        {
            Debug.Log("PIN 입력이 부족합니다!");
        }
    }
}
