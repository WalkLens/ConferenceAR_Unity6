using Microsoft.MixedReality.WebView;
using Photon.Pun;
using UnityEngine;

public class PhotonRPCWrapper : MonoBehaviourPun
{
    // 원격지에서 마우스 이벤트를 수신하여 WebViewBrowser에 전달합니다.
    [PunRPC]
    public void RemoteMouseEvent(Vector3 inputPoint, int eventType)
    {
        WebViewBrowser browser = GetComponentInChildren<WebViewBrowser>();
        if (browser != null)
        {
            browser.TranslateToWebViewMouseEvent(inputPoint, (WebViewMouseEventData.EventType)eventType);
        }
    }

    // 원격지에서 URL 변경 요청을 수신하여 WebView에 로드합니다.
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
