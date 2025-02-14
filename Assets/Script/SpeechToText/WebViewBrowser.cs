using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Microsoft.MixedReality.WebView;
using UnityEngine.XR.Interaction.Toolkit;
using MixedReality.Toolkit;
using UnityEngine.EventSystems;
using Photon.Pun;

public class WebViewBrowser : MRTKBaseInteractable, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    public Button BackButton;
    public Button GoButton;
    public TMP_InputField URLField;
    public MeshCollider Collider;
    private IWebView _webView;

    // PhotonRPCWrapper�� �θ𿡼� ã���ϴ�.
    private PhotonRPCWrapper rpcWrapper;

    // hand ray �Է����� ���θ� �Ǵ��ϱ� ���� �÷���
    private bool isHandRayInteraction = false;

    /*private void Awake()
    {
        rpcWrapper = GetComponentInParent<PhotonRPCWrapper>();
        if (rpcWrapper == null)
        {
            Debug.LogError("�θ� ������Ʈ���� PhotonRPCWrapper�� ã�� �� �����ϴ�.");
        }
    }*/

    private void Start()
    {
        rpcWrapper = GetComponentInParent<PhotonRPCWrapper>();
        if (rpcWrapper == null)
        {
            Debug.LogError("�θ� ������Ʈ���� PhotonRPCWrapper�� ã�� �� �����ϴ�.");
        }
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

        // Go ��ư Ŭ�� �� URL �ε� ��, RPC�� ���� �ٸ� Ŭ���̾�Ʈ���� �����մϴ�.
        GoButton.onClick.AddListener(() =>
        {
            string url = URLField.text;
            _webView.Load(new Uri(url));
            if (rpcWrapper != null)
            {
                rpcWrapper.photonView.RPC("RemoteLoadUrl", RpcTarget.Others, url);
            }
        });
        webView.Navigated += OnNavigated;

        if (_webView.Page != null)
        {
            URLField.text = _webView.Page.AbsoluteUri;
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

    // hand ray �Է� ó�� (��: ��Ʈ�ѷ� ����)
    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        Debug.Log("OnSelectEntered (hand ray)");
        base.OnSelectEntered(args);
        UnityEngine.XR.Interaction.Toolkit.Interactors.XRRayInteractor rayInteractor = args.interactorObject as UnityEngine.XR.Interaction.Toolkit.Interactors.XRRayInteractor;
        if (rayInteractor != null)
        {
            Vector3 hitPoint = GetRayHitPoint(rayInteractor);
            if (hitPoint != Vector3.zero)
            {
                isHandRayInteraction = true;
                TranslateToWebViewMouseEvent(hitPoint, WebViewMouseEventData.EventType.MouseDown);
                if (rpcWrapper != null)
                {
                    rpcWrapper.photonView.RPC("RemoteMouseEvent", RpcTarget.Others, hitPoint, (int)WebViewMouseEventData.EventType.MouseDown);
                    rpcWrapper.photonView.RPC("RemoteMouseEvent", RpcTarget.Others, hitPoint, (int)WebViewMouseEventData.EventType.MouseUp);
                }
            }
        }
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        Debug.Log("OnSelectExited (hand ray)");
        base.OnSelectExited(args);
        UnityEngine.XR.Interaction.Toolkit.Interactors.XRRayInteractor rayInteractor = args.interactorObject as UnityEngine.XR.Interaction.Toolkit.Interactors.XRRayInteractor;
        if (rayInteractor != null)
        {
            Vector3 hitPoint = GetRayHitPoint(rayInteractor);
            if (hitPoint != Vector3.zero)
            {
                // MouseUp �̺�Ʈ ����
                TranslateToWebViewMouseEvent(hitPoint, WebViewMouseEventData.EventType.MouseUp);
                if (rpcWrapper != null)
                {
                    rpcWrapper.photonView.RPC("RemoteMouseEvent", RpcTarget.Others, hitPoint, (int)WebViewMouseEventData.EventType.MouseUp);
                }
                // hand ray�� ���, Ŭ�� �̺�Ʈ�� ������ �����մϴ�.
                if (isHandRayInteraction)
                {
                    TranslateToWebViewMouseEvent(hitPoint, WebViewMouseEventData.EventType.MouseDown);
                    if (rpcWrapper != null)
                    {
                        rpcWrapper.photonView.RPC("RemoteMouseEvent", RpcTarget.Others, hitPoint, (int)WebViewMouseEventData.EventType.MouseDown);
                        rpcWrapper.photonView.RPC("RemoteMouseEvent", RpcTarget.Others, hitPoint, (int)WebViewMouseEventData.EventType.MouseUp);
                    }
                    isHandRayInteraction = false;
                }
            }
        }
    }

    // ���콺 ������ �Է� ó��
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Mouse pointer down");
        Vector3 hitPoint = eventData.pointerCurrentRaycast.worldPosition;
        TranslateToWebViewMouseEvent(hitPoint, WebViewMouseEventData.EventType.MouseDown);
        if (rpcWrapper != null)
        {
            rpcWrapper.photonView.RPC("RemoteMouseEvent", RpcTarget.Others, hitPoint, (int)WebViewMouseEventData.EventType.MouseDown);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("Mouse pointer up");
        Vector3 hitPoint = eventData.pointerCurrentRaycast.worldPosition;
        TranslateToWebViewMouseEvent(hitPoint, WebViewMouseEventData.EventType.MouseUp);
        if (rpcWrapper != null)
        {
            rpcWrapper.photonView.RPC("RemoteMouseEvent", RpcTarget.Others, hitPoint, (int)WebViewMouseEventData.EventType.MouseUp);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Mouse pointer click");
        Vector3 hitPoint = eventData.pointerCurrentRaycast.worldPosition;
        TranslateToWebViewMouseEvent(hitPoint, WebViewMouseEventData.EventType.MouseDown);
        if (rpcWrapper != null)
        {
            rpcWrapper.photonView.RPC("RemoteMouseEvent", RpcTarget.Others, hitPoint, (int)WebViewMouseEventData.EventType.MouseDown);
            rpcWrapper.photonView.RPC("RemoteMouseEvent", RpcTarget.Others, hitPoint, (int)WebViewMouseEventData.EventType.MouseUp);
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

    // ��ǥ ��ȯ �� WebView�� ���콺 �̺�Ʈ ����
    public void TranslateToWebViewMouseEvent(Vector3 inputPoint, WebViewMouseEventData.EventType eventType, bool isScreenSpace = false)
    {
        if (_webView == null || Collider == null) return;

        Vector2 hitCoord;
        if (isScreenSpace)
        {
            Vector3 worldPoint = Camera.main.ScreenToWorldPoint(new Vector3(inputPoint.x, inputPoint.y, Camera.main.nearClipPlane));
            hitCoord = NormalizeWorldPoint(worldPoint);
        }
        else
        {
            hitCoord = NormalizeWorldPoint(inputPoint);
        }

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
