using UnityEngine;

[CreateAssetMenu(fileName = "ARUI Data", menuName = "Scripatable Object/ARUI Data", order = int.MaxValue)]
public class ARUIData : ScriptableObject
{
    public Vector3 targetScale; // ��ǥ ũ��
    public Gradient targetGradient; // ��ǥ �׶��̼� ����
    public float delayTime; // ������ �ð�
    public Sprite spriteImage; // UI�� ����� �̹�����
    public bool isHeaderActivated, isSpriteActivated, isTextActivated;
    public string text;
    public Material material;
}
