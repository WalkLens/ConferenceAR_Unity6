using Microsoft.MixedReality.WebView;
using Photon.Pun;
using UnityEngine;

public class PhotonRPCWrapper : MonoBehaviourPun
{
    // ���������� ���콺 �̺�Ʈ�� �����Ͽ� WebViewBrowser�� �����մϴ�.
    [PunRPC]
    public void RemoteMouseEvent(Vector3 inputPoint, int eventType)
    {
        WebViewBrowser browser = GetComponentInChildren<WebViewBrowser>();
        if (browser != null)
        {
            browser.TranslateToWebViewMouseEvent(inputPoint, (WebViewMouseEventData.EventType)eventType);
        }
    }

    // ���������� URL ���� ��û�� �����Ͽ� WebView�� �ε��մϴ�.
    [PunRPC]
    public void RemoteLoadUrl(string url)
    {
        WebViewBrowser browser = GetComponentInChildren<WebViewBrowser>();
        if (browser != null)
        {
            WebView webView = browser.GetComponent<WebView>();
            if (webView != null)
            {
                webView.Load(url);
            }
        }
    }
}
