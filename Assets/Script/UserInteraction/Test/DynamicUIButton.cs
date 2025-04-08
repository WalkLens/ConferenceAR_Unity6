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
                    // 누르는 깊이 -4 ~ 0까지의 범위를 회전하는 각도 0 ~ -15도로 매핑
                    float t = Mathf.InverseLerp(4f, 0f, animatedContent.localPosition.z);
                    float rotationZ = Mathf.Lerp(0f, maxZRotationValue, t);

                    Quaternion additionalRotation = Quaternion.Euler(0f, 0f, rotationZ);
                    rotateObjects[index].transform.localRotation = originalRotations[index] * additionalRotation;
                }
                else
                {
                    // z가 4 이상일 경우 원래 회전값으로 리셋
                    rotateObjects[index].transform.localRotation = originalRotations[index];
                }
            }
        }
    }
}
