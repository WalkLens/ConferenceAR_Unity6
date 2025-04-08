using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System;

public class CustomKeyboard : MonoBehaviour
{
    public TextMeshProUGUI[] inputFieldArray;
    private int idx = 0;
    private string _inputFieldString;

    // 입력 완료 시 호출할 이벤트 (ARUIManager 에서 구독)
    public event Action<string> OnInputComplete;

    public string InputFieldString
    {
        get => _inputFieldString;
        private set
        {
            _inputFieldString = value;
            Debug.Log("InputFieldString 변경됨: " + _inputFieldString);
        }
    }

    public void OnKeyPress(string character)
    {
        if (idx < inputFieldArray.Length)
        {
            inputFieldArray[idx].text = character;
            InputFieldString += character;
            idx++;

            // 입력이 완료되었으면 이벤트 호출
            if (idx == inputFieldArray.Length)
            {
                OnInputComplete?.Invoke(InputFieldString);
            }
        }
        else
        {
            Debug.Log("PIN 입력칸이 꽉 찼습니다");
        }

        Debug.Log("inputFieldString : " + InputFieldString);
    }

    public void OnBackspace()
    {
        if (idx > 0)
        {
            idx--;
            inputFieldArray[idx].text = "";
            InputFieldString = InputFieldString.Substring(0, InputFieldString.Length - 1);
        }
        else
        {
            Debug.Log("지울 PIN이 없습니다");
        }

        Debug.Log("inputFieldString : " + InputFieldString);
    }

    public void ResetInput()
    {
        idx = 0;
        InputFieldString = string.Empty;

        foreach (var i in inputFieldArray)
        {
            i.text = "";
        }
    }
}
