using UnityEngine;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Input.Utilities
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Canvas))]
    [AddComponentMenu("Scripts/MRTK/Services/CanvasUtility")]
    public class CanvasUtility : MonoBehaviour
    {
        private void Start()
        {
            VerifyCanvasConfiguration();
        }

        /// <summary>
        /// MRTK3 ȯ�濡 �°� ĵ���� ������ Ȯ���մϴ�.
        /// World Space ĵ������ camera�� �Ҵ�Ǿ� ������ ���� �޽����� ����� �����մϴ�.
        /// </summary>
        public void VerifyCanvasConfiguration()
        {
            Canvas canvas = GetComponent<Canvas>();

            // MRTK3������ World Space ĵ������ ī�޶� �������� ����
            if (canvas.worldCamera != null)
            {
                Debug.LogError("World Space Canvas���� camera�� �������� �ʾƾ� �մϴ�. MRTK3������ ��Ÿ�ӿ� �ڵ����� ó���˴ϴ�.");
                canvas.worldCamera = null;
            }

            if (EventSystem.current == null)
            {
                Debug.LogError("EventSystem�� �������� �ʾҽ��ϴ�. UI �̺�Ʈ�� Unity UI�� ���ĵ��� �ʽ��ϴ�.");
            }
        }
    }
}
