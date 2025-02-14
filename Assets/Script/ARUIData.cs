using UnityEngine;

[CreateAssetMenu(fileName = "ARUI Data", menuName = "Scripatable Object/ARUI Data", order = int.MaxValue)]
public class ARUIData : ScriptableObject
{
    public Vector3 targetScale; // 목표 크기
    public Gradient targetGradient; // 목표 그라데이션 색상
    public float delayTime; // 딜레이 시간
    public Sprite spriteImage; // UI에 출력할 이미지값
    public bool isHeaderActivated, isSpriteActivated, isTextActivated;
    public string text;
    public Material material;
}
