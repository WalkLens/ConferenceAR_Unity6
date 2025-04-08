using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Microsoft.MixedReality.WebView;
using UnityEngine.XR.Interaction.Toolkit;
using MixedReality.Toolkit;
using UnityEngine.EventSystems;

public class WebViewBrowser : MRTKBaseInteractable, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    public Button BackButton;
    public Button GoButton;
    public TMP_InputField URLField;
    public MeshCollider Collider;
    private IWebView _webView;

    private void Start()
    {
        var webViewComponent = GetComponent<WebView>();
        webViewComponent.GetWebViewWhenReady(InitializeWebView);
    }

    private void InitializeWebView(IWebView webView)
    {
        _webView = webView;

        if (webView is IWithBrowserHistory history)
        {
            BackButton.onClick.AddListener(() => history.GoBack());
            history.CanGoBackUpdated += CanGoBack;
        }

        GoButton.onClick.AddListener(() => webView.Load(new Uri(URLField.text)));
        webView.Navigated += OnNavigated;

        if (webView.Page != null)
        {
            URLField.text = webView.Page.AbsoluteUri;
        }
    }

    private void OnNavigated(string path)
    {
        URLField.text = path;
    }

    private void CanGoBack(bool value)
    {
        BackButton.interactable = value;
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        Debug.Log("OnSelectEntered");
        base.OnSelectEntered(args);
        Vector3 hitPoint = GetRayHitPoint(args.interactorObject as UnityEngine.XR.Interaction.Toolkit.Interactors.XRRayInteractor);
        if (hitPoint != Vector3.zero)
        {
            TranslateToWebViewMouseEvent(hitPoint, WebViewMouseEventData.EventType.MouseDown);
        }
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        Debug.Log("OnSelectExited");
        base.OnSelectExited(args);
        Vector3 hitPoint = GetRayHitPoint(args.interactorObject as UnityEngine.XR.Interaction.Toolkit.Interactors.XRRayInteractor);
        if (hitPoint != Vector3.zero)
        {
            TranslateToWebViewMouseEvent(hitPoint, WebViewMouseEventData.EventType.MouseUp);
        }
    }

    private Vector3 GetRayHitPoint(UnityEngine.XR.Interaction.Toolkit.Interactors.XRRayInteractor rayInteractor)
    {
        if (rayInteractor != null && rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hitInfo))
        {
            return hitInfo.point;
        }
        return Vector3.zero;
    }

    /// <summary>
    /// ���콺 �Է� ó�� (IPointerHandler �������̽� ����)
    /// </summary>
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Mouse pointer down");

        TranslateToWebViewMouseEvent(eventData.pointerCurrentRaycast.worldPosition, WebViewMouseEventData.EventType.MouseDown);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("Mouse pointer up");

        TranslateToWebViewMouseEvent(eventData.pointerCurrentRaycast.worldPosition, WebViewMouseEventData.EventType.MouseUp);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Mouse pointer click");

        // �ʿ� �� Ŭ�� �̺�Ʈ�� �߰������� ó���� �� ����
    }

    private void TranslateToWebViewMouseEvent(Vector3 inputPoint, WebViewMouseEventData.EventType eventType, bool isScreenSpace = false)
    {
        if (_webView == null || Collider == null) return;

        Vector2 hitCoord;

        if (isScreenSpace)
        {
            // ���콺 �Է�: ȭ�� ��ǥ�� ���� ��ǥ�� ��ȯ
            Vector3 worldPoint = Camera.main.ScreenToWorldPoint(new Vector3(inputPoint.x, inputPoint.y, Camera.main.nearClipPlane));
            hitCoord = NormalizeWorldPoint(worldPoint);
        }
        else
        {
            // Ray �Է�: ���� ��ǥ �״�� ���
            hitCoord = NormalizeWorldPoint(inputPoint);
        }

        // WebView �ؽ�ó ũ�⿡ �°� ����
        hitCoord.x *= _webView.Width;
        hitCoord.y *= _webView.Height;

        if (_webView is IWithMouseEvents mouseEventsWebView)
        {
            WebViewMouseEventData mouseEvent = new WebViewMouseEventData
            {
                X = (int)hitCoord.x,
                Y = (int)hitCoord.y,
                Device = WebViewMouseEventData.DeviceType.Pointer,
                Type = eventType,
                Button = WebViewMouseEventData.MouseButton.ButtonLeft,
                TertiaryAxisDeviceType = WebViewMouseEventData.TertiaryAxisDevice.PointingDevice
            };

            mouseEventsWebView.MouseEvent(mouseEvent);
            Debug.Log($"Sent Mouse Event: {eventType} at ({mouseEvent.X}, {mouseEvent.Y})");
        }
    }


    private Vector2 NormalizeWorldPoint(Vector3 worldPoint)
    {
        Vector3 localPoint = transform.InverseTransformPoint(worldPoint);

        var bounds = Collider.sharedMesh.bounds;
        Vector3 boundsSize = bounds.size;
        Vector3 boundsExtents = bounds.max;

        Vector2 uvTouchPoint = new Vector2((localPoint.x + boundsExtents.x), -1.0f * (localPoint.y - boundsExtents.y));

        return new Vector2(uvTouchPoint.x / boundsSize.x, uvTouchPoint.y / boundsSize.y);
    }
}
