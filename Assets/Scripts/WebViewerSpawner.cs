using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine;
using Microsoft.MixedReality.WebView;
using System;

public class WebViewerSpawner : MonoBehaviourPunCallbacks
{
    // Inspector에서 직접 할당할 WebView prefab (PhotonView와 WebView 컴포넌트가 붙어 있어야 함)
    [SerializeField]
    private GameObject webviewPrefab;

    // 접속하고자 하는 URL (플레이어별로 다르게 설정할 수 있음)
    public string targetURL = null;

    // 마지막으로 생성된 WebView prefab의 참조를 보관합니다.
    private GameObject lastInstantiatedWebView;

    // Photon 이벤트 코드
    private const byte LoadURLEvent = 12;
    private const byte DestroyWebViewEvent = 13; // 사용 가능

    private void Start()
    {
        // DefaultPool 사용 시, prefab을 ResourceCache에 등록합니다.
        if (PhotonNetwork.PrefabPool is DefaultPool pool)
        {
            if (webviewPrefab != null && !pool.ResourceCache.ContainsKey(webviewPrefab.name))
            {
                pool.ResourceCache.Add(webviewPrefab.name, webviewPrefab);
            }
        }
    }

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

    /// <summary>
    /// UI 버튼의 OnClick에 연결할 함수.
    /// 자신의 기존 WebView 인스턴스가 있다면 제거한 후 새롭게 Instantiate하고, targetURL로 접속한 후,
    /// 새 인스턴스의 PhotonView ID와 URL을 Photon 이벤트로 브로드캐스트합니다.
    /// </summary>
    public void SpawnWebView()
    {
        if (PhotonNetwork.IsConnectedAndReady && PhotonNetwork.InRoom)
        {

            if (lastInstantiatedWebView != null)
            {
                PhotonNetwork.Destroy(lastInstantiatedWebView);
                lastInstantiatedWebView = null;
                Debug.Log("기존 WebView prefab이 제거되었습니다.");
            }

            // 로컬 소유의 기존 WebView 인스턴스만 제거 (다른 플레이어의 인스턴스는 건드리지 않음)
            /*WebView[] existingWebViews = FindObjectsOfType<WebView>();
            foreach (WebView wv in existingWebViews)
            {
                PhotonView pv = wv.GetComponent<PhotonView>();
                if (pv != null && pv.IsMine)
                {
                    PhotonNetwork.Destroy(pv.gameObject);
                }
            }*/
            lastInstantiatedWebView = null;

            // 카메라의 정면 방향으로 0.5m 떨어진 위치 계산
            Camera mainCam = Camera.main;
            if (mainCam == null)
            {
                Debug.LogError("Main Camera를 찾을 수 없습니다.");
                return;
            }
            Vector3 spawnPos = mainCam.transform.position + mainCam.transform.forward * 0.5f;
            Quaternion spawnRot = Quaternion.LookRotation(mainCam.transform.forward);

            // 새 WebView prefab 인스턴스 생성 (PhotonNetwork.Instantiate는 모든 클라이언트에 생성됨)
            lastInstantiatedWebView = PhotonNetwork.Instantiate(webviewPrefab.name, spawnPos, spawnRot);
            Debug.Log("WebView prefab이 " + spawnPos + " 위치에 생성되었습니다.");

            // 생성된 prefab의 자식에서 WebView 컴포넌트를 찾아 targetURL로 접속
            WebView webViewComponent = lastInstantiatedWebView.GetComponentInChildren<WebView>();
            if (webViewComponent != null)
            {
                webViewComponent.Load(targetURL);
                Debug.Log("로컬 WebView가 URL을 로드합니다: " + targetURL);
            }
            else
            {
                Debug.LogError("WebView 컴포넌트를 찾을 수 없습니다.");
            }

            // 새 인스턴스의 PhotonView ID를 가져옵니다.
            PhotonView pvNew = lastInstantiatedWebView.GetComponent<PhotonView>();
            if (pvNew == null)
            {
                Debug.LogError("PhotonView 컴포넌트를 찾을 수 없습니다.");
                return;
            }
            int newViewID = pvNew.ViewID;

            // 이벤트 데이터: { newViewID, targetURL }
            object[] data = new object[] { newViewID, targetURL };
            RaiseEventOptions options = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            PhotonNetwork.RaiseEvent(LoadURLEvent, data, options, SendOptions.SendReliable);
            Debug.Log("LoadURLEvent 전송됨, viewID: " + newViewID + ", URL: " + targetURL);
        }
        else
        {
            Debug.LogWarning("Photon에 연결되어 있지 않거나, 방에 입장하지 않았습니다.");
        }
    }

    // Photon 이벤트 수신 처리: LoadURLEvent를 통해 새 인스턴스 정보를 수신하면,
    // 같은 사용자(이벤트 발신자 소유)의 기존 인스턴스들은 제거하고, 새 인스턴스에 URL을 로드합니다.
    private void OnPhotonEvent(EventData photonEvent)
    {
        if (photonEvent.Code == LoadURLEvent)
        {
            object[] eventData = photonEvent.CustomData as object[];
            if (eventData != null && eventData.Length >= 2)
            {
                int receivedViewID = (int)eventData[0];
                string url = eventData[1] as string;
                Debug.Log("[LoadURLEvent] Received: viewID = " + receivedViewID + ", URL = " + url);

                // 이벤트 발신자 번호
                int senderActor = photonEvent.Sender;

                // 각 클라이언트에서, 자신이 수신한 이벤트의 발신자와 동일한 소유자인 WebView 인스턴스 중,
                // 새로운 viewID와 다른 인스턴스는 제거합니다.
                WebView[] webViews = FindObjectsOfType<WebView>();
                foreach (WebView wv in webViews)
                {
                    PhotonView pv = wv.GetComponent<PhotonView>();
                    if (pv != null && pv.OwnerActorNr == senderActor && pv.ViewID != receivedViewID)
                    {
                        if (pv.IsMine)
                            PhotonNetwork.Destroy(pv.gameObject);
                        else
                            Destroy(pv.gameObject);
                        Debug.Log("기존 인스턴스 제거됨, viewID: " + pv.ViewID);
                    }
                }

                // 그리고 새 인스턴스가 있다면 URL 로드
                PhotonView targetPV = PhotonView.Find(receivedViewID);
                if (targetPV != null)
                {
                    WebView webViewComponent = targetPV.GetComponentInChildren<WebView>();
                    if (webViewComponent != null)
                    {
                        webViewComponent.Load(url);
                        Debug.Log("원격 WebView가 URL을 로드합니다: " + url);
                    }
                    else
                    {
                        Debug.LogWarning("OnPhotonEvent: 해당 PhotonView의 WebView 컴포넌트를 찾을 수 없습니다.");
                    }
                }
                else
                {
                    Debug.LogWarning("OnPhotonEvent: viewID " + receivedViewID + "에 해당하는 오브젝트를 찾을 수 없습니다.");
                }
            }
        }
        else if (photonEvent.Code == DestroyWebViewEvent)
        {
            object[] eventData = photonEvent.CustomData as object[];
            if (eventData != null && eventData.Length >= 1)
            {
                int viewID = (int)eventData[0];
                Debug.Log("[DestroyWebViewEvent] Received for viewID: " + viewID);
                PhotonView targetPV = PhotonView.Find(viewID);
                if (targetPV != null)
                {
                    PhotonNetwork.Destroy(targetPV.gameObject);
                }
            }
        }
    }
}
