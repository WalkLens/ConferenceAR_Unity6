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
        /// MRTK3 환경에 맞게 캔버스 설정을 확인합니다.
        /// World Space 캔버스에 camera가 할당되어 있으면 오류 메시지를 남기고 해제합니다.
        /// </summary>
        public void VerifyCanvasConfiguration()
        {
            Canvas canvas = GetComponent<Canvas>();

            // MRTK3에서는 World Space 캔버스에 카메라를 지정하지 않음
            if (canvas.worldCamera != null)
            {
                Debug.LogError("World Space Canvas에는 camera가 지정되지 않아야 합니다. MRTK3에서는 런타임에 자동으로 처리됩니다.");
                canvas.worldCamera = null;
            }

            if (EventSystem.current == null)
            {
                Debug.LogError("EventSystem이 감지되지 않았습니다. UI 이벤트가 Unity UI로 전파되지 않습니다.");
            }
        }
    }
}
