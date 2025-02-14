using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine;
using Microsoft.MixedReality.WebView;
using System;

public class WebViewerSpawner : MonoBehaviourPunCallbacks
{
    // Inspector���� ���� �Ҵ��� WebView prefab (PhotonView�� WebView ������Ʈ�� �پ� �־�� ��)
    [SerializeField]
    private GameObject webviewPrefab;

    // �����ϰ��� �ϴ� URL (�÷��̾�� �ٸ��� ������ �� ����)
    public string targetURL = null;

    // ���������� ������ WebView prefab�� ������ �����մϴ�.
    private GameObject lastInstantiatedWebView;

    // Photon �̺�Ʈ �ڵ�
    private const byte LoadURLEvent = 12;
    private const byte DestroyWebViewEvent = 13; // ��� ����

    private void Start()
    {
        // DefaultPool ��� ��, prefab�� ResourceCache�� ����մϴ�.
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
    /// UI ��ư�� OnClick�� ������ �Լ�.
    /// �ڽ��� ���� WebView �ν��Ͻ��� �ִٸ� ������ �� ���Ӱ� Instantiate�ϰ�, targetURL�� ������ ��,
    /// �� �ν��Ͻ��� PhotonView ID�� URL�� Photon �̺�Ʈ�� ��ε�ĳ��Ʈ�մϴ�.
    /// </summary>
    public void SpawnWebView()
    {
        if (PhotonNetwork.IsConnectedAndReady && PhotonNetwork.InRoom)
        {

            if (lastInstantiatedWebView != null)
            {
                PhotonNetwork.Destroy(lastInstantiatedWebView);
                lastInstantiatedWebView = null;
                Debug.Log("���� WebView prefab�� ���ŵǾ����ϴ�.");
            }

            // ���� ������ ���� WebView �ν��Ͻ��� ���� (�ٸ� �÷��̾��� �ν��Ͻ��� �ǵ帮�� ����)
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

            // ī�޶��� ���� �������� 0.5m ������ ��ġ ���
            Camera mainCam = Camera.main;
            if (mainCam == null)
            {
                Debug.LogError("Main Camera�� ã�� �� �����ϴ�.");
                return;
            }
            Vector3 spawnPos = mainCam.transform.position + mainCam.transform.forward * 0.5f;
            Quaternion spawnRot = Quaternion.LookRotation(mainCam.transform.forward);

            // �� WebView prefab �ν��Ͻ� ���� (PhotonNetwork.Instantiate�� ��� Ŭ���̾�Ʈ�� ������)
            lastInstantiatedWebView = PhotonNetwork.Instantiate(webviewPrefab.name, spawnPos, spawnRot);
            Debug.Log("WebView prefab�� " + spawnPos + " ��ġ�� �����Ǿ����ϴ�.");

            // ������ prefab�� �ڽĿ��� WebView ������Ʈ�� ã�� targetURL�� ����
            WebView webViewComponent = lastInstantiatedWebView.GetComponentInChildren<WebView>();
            if (webViewComponent != null)
            {
                webViewComponent.Load(targetURL);
                Debug.Log("���� WebView�� URL�� �ε��մϴ�: " + targetURL);
            }
            else
            {
                Debug.LogError("WebView ������Ʈ�� ã�� �� �����ϴ�.");
            }

            // �� �ν��Ͻ��� PhotonView ID�� �����ɴϴ�.
            PhotonView pvNew = lastInstantiatedWebView.GetComponent<PhotonView>();
            if (pvNew == null)
            {
                Debug.LogError("PhotonView ������Ʈ�� ã�� �� �����ϴ�.");
                return;
            }
            int newViewID = pvNew.ViewID;

            // �̺�Ʈ ������: { newViewID, targetURL }
            object[] data = new object[] { newViewID, targetURL };
            RaiseEventOptions options = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            PhotonNetwork.RaiseEvent(LoadURLEvent, data, options, SendOptions.SendReliable);
            Debug.Log("LoadURLEvent ���۵�, viewID: " + newViewID + ", URL: " + targetURL);
        }
        else
        {
            Debug.LogWarning("Photon�� ����Ǿ� ���� �ʰų�, �濡 �������� �ʾҽ��ϴ�.");
        }
    }

    // Photon �̺�Ʈ ���� ó��: LoadURLEvent�� ���� �� �ν��Ͻ� ������ �����ϸ�,
    // ���� �����(�̺�Ʈ �߽��� ����)�� ���� �ν��Ͻ����� �����ϰ�, �� �ν��Ͻ��� URL�� �ε��մϴ�.
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

                // �̺�Ʈ �߽��� ��ȣ
                int senderActor = photonEvent.Sender;

                // �� Ŭ���̾�Ʈ����, �ڽ��� ������ �̺�Ʈ�� �߽��ڿ� ������ �������� WebView �ν��Ͻ� ��,
                // ���ο� viewID�� �ٸ� �ν��Ͻ��� �����մϴ�.
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
                        Debug.Log("���� �ν��Ͻ� ���ŵ�, viewID: " + pv.ViewID);
                    }
                }

                // �׸��� �� �ν��Ͻ��� �ִٸ� URL �ε�
                PhotonView targetPV = PhotonView.Find(receivedViewID);
                if (targetPV != null)
                {
                    WebView webViewComponent = targetPV.GetComponentInChildren<WebView>();
                    if (webViewComponent != null)
                    {
                        webViewComponent.Load(url);
                        Debug.Log("���� WebView�� URL�� �ε��մϴ�: " + url);
                    }
                    else
                    {
                        Debug.LogWarning("OnPhotonEvent: �ش� PhotonView�� WebView ������Ʈ�� ã�� �� �����ϴ�.");
                    }
                }
                else
                {
                    Debug.LogWarning("OnPhotonEvent: viewID " + receivedViewID + "�� �ش��ϴ� ������Ʈ�� ã�� �� �����ϴ�.");
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
