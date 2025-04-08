using MixedReality.Toolkit.UX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicUIButton : MonoBehaviour
{
    public RectTransform[] animatedContents;
    public GameObject[] rotateObjects;
    public PressableButton[] pressableButtons;
    private Quaternion[] originalRotations;
    public float maxZRotationValue = -20f;

    // Start is called before the first frame update
    void Start()
    {
        originalRotations = new Quaternion[rotateObjects.Length];
        for (int i = 0; i < rotateObjects.Length; i++)
        {
            originalRotations[i] = rotateObjects[i].transform.localRotation;
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int index = 0; index < animatedContents.Length; index++)
        {
            if (pressableButtons[index].IsToggled)
            {
                Quaternion additionalRotation = Quaternion.Euler(0f, 0f, maxZRotationValue);
                rotateObjects[index].transform.localRotation = originalRotations[index] * additionalRotation;
            }

            else
            {
                var animatedContent = animatedContents[index];
                if (animatedContent.localPosition.z < 4)
                {
                    // ������ ���� -4 ~ 0������ ������ ȸ���ϴ� ���� 0 ~ -15���� ����
                    float t = Mathf.InverseLerp(4f, 0f, animatedContent.localPosition.z);
                    float rotationZ = Mathf.Lerp(0f, maxZRotationValue, t);

                    Quaternion additionalRotation = Quaternion.Euler(0f, 0f, rotationZ);
                    rotateObjects[index].transform.localRotation = originalRotations[index] * additionalRotation;
                }
                else
                {
                    // z�� 4 �̻��� ��� ���� ȸ�������� ����
                    rotateObjects[index].transform.localRotation = originalRotations[index];
                }
            }
        }
    }
}
