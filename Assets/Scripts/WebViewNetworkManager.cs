using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine;
using Microsoft.MixedReality.WebView;
using System;

public class WebViewNetworkManager : MonoBehaviourPunCallbacks
{
    // Inspector에서 할당할 Canvas prefab (PhotonView가 Canvas에 붙어있고, 자식에 WebView 스크립트가 있음)
    [SerializeField] private GameObject canvasPrefab;

    // Instantiate된 Canvas GameObject의 참조
    private GameObject instantiatedCanvas;

    // Photon 이벤트 코드
    public const byte LoadURLEvent = 8;
    public const byte DestroyWebViewEvent = 9;

    /// <summary>
    /// 버튼을 누르면 호출되는 함수.
    /// Canvas prefab을 Instantiate한 후, 자식 WebView 컴포넌트를 통해 지정 URL을 로드하고, 이를 모든 클라이언트에 동기화합니다.
    /// </summary>
    /// <param name="url">접속하고자 하는 URL</param>
    public void InstantiateWebViewAndLoad()
    {
        string url = "https://www.youtube.com/watch?v=_vtAyHOQRUg";
        if (PhotonNetwork.IsConnectedAndReady && PhotonNetwork.InRoom)
        {
            // 원하는 위치에서 Canvas prefab을 Instantiate
            Vector3 spawnPosition = new Vector3(-0.64f, 1.72f, 0.4f);
            instantiatedCanvas = PhotonNetwork.Instantiate(canvasPrefab.name, spawnPosition, Quaternion.identity);
            Debug.Log("WebView Canvas instantiated.");

            // Instantiate된 Canvas의 자식에서 WebView 컴포넌트를 찾은 후 비동기적으로 초기화 완료 시 URL 로드
            WebView webView = instantiatedCanvas.GetComponentInChildren<WebView>();
            if (webView != null)
            {
                webView.GetWebViewWhenReady((wv) =>
                {
                    wv.Load(new Uri(url));
                    Debug.Log("WebView.Load(url) called in callback.");
                });
            }
            else
            {
                Debug.LogError("WebView 컴포넌트를 찾을 수 없습니다.");
            }

            // 모든 클라이언트에 URL 로드 명령을 동기화하기 위해 Photon 이벤트 전송
            object[] data = new object[] { url };
            RaiseEventOptions options = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            PhotonNetwork.RaiseEvent(LoadURLEvent, data, options, SendOptions.SendReliable);
            Debug.Log($"LoadURLEvent sent with URL: {url}");
        }
        else
        {
            Debug.LogWarning("Photon에 연결되어 있지 않거나, 방에 입장하지 않았습니다.");
        }
    }


    /// <summary>
    /// 버튼을 누르면 호출되는 함수.
    /// Instantiate된 WebView Canvas를 모든 클라이언트에서 제거합니다.
    /// </summary>
    public void DestroyWebViewNetworked()
    {
        if (instantiatedCanvas != null)
        {
            PhotonNetwork.Destroy(instantiatedCanvas);
            // 옵션: 제거 명령을 동기화할 수 있도록 이벤트 전송 (로컬에서 추가 처리가 필요하면)
            PhotonNetwork.RaiseEvent(DestroyWebViewEvent, null, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
            Debug.Log("DestroyWebViewEvent sent; WebView Canvas will be removed on all clients.");
        }
        else
        {
            Debug.LogWarning("제거할 WebView Canvas가 없습니다.");
        }
    }

    #region Photon Event Handling

    public override void OnEnable()
    {
        base.OnEnable();
        PhotonNetwork.NetworkingClient.EventReceived += OnPhotonEvent;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        PhotonNetwork.NetworkingClient.EventReceived -= OnPhotonEvent;
    }

    private void OnPhotonEvent(EventData photonEvent)
    {
        if (photonEvent.Code == LoadURLEvent)
        {
            object[] data = (object[])photonEvent.CustomData;
            if (data != null && data.Length > 0)
            {
                string url = data[0] as string;
                Debug.Log($"[LoadURLEvent] Received URL: {url}");

                // 각 클라이언트에서 이미 Instantiate된 Canvas의 자식 WebView 컴포넌트를 찾아 URL 로드
                if (instantiatedCanvas != null)
                {
                    WebView webView = instantiatedCanvas.GetComponentInChildren<WebView>();
                    if (webView != null)
                    {
                        webView.Load(url);
                        Debug.Log("WebView.Load(url) called on received event.");
                    }
                    else
                    {
                        Debug.LogError("OnPhotonEvent: WebView 컴포넌트를 찾을 수 없습니다.");
                    }
                }
            }
        }
        else if (photonEvent.Code == DestroyWebViewEvent)
        {
            Debug.Log("[DestroyWebViewEvent] Received destroy command.");
            instantiatedCanvas = null;
        }
    }

    #endregion
}
