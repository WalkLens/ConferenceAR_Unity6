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

    // �Է� �Ϸ� �� ȣ���� �̺�Ʈ (ARUIManager ���� ����)
    public event Action<string> OnInputComplete;

    public string InputFieldString
    {
        get => _inputFieldString;
        private set
        {
            _inputFieldString = value;
            Debug.Log("InputFieldString �����: " + _inputFieldString);
        }
    }

    public void OnKeyPress(string character)
    {
        if (idx < inputFieldArray.Length)
        {
            inputFieldArray[idx].text = character;
            InputFieldString += character;
            idx++;

            // �Է��� �Ϸ�Ǿ����� �̺�Ʈ ȣ��
            if (idx == inputFieldArray.Length)
            {
                OnInputComplete?.Invoke(InputFieldString);
            }
        }
        else
        {
            Debug.Log("PIN �Է�ĭ�� �� á���ϴ�");
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
            Debug.Log("���� PIN�� �����ϴ�");
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
