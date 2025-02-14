using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine;
using Microsoft.MixedReality.WebView;
using System;

public class WebViewNetworkManager : MonoBehaviourPunCallbacks
{
    // Inspector���� �Ҵ��� Canvas prefab (PhotonView�� Canvas�� �پ��ְ�, �ڽĿ� WebView ��ũ��Ʈ�� ����)
    [SerializeField] private GameObject canvasPrefab;

    // Instantiate�� Canvas GameObject�� ����
    private GameObject instantiatedCanvas;

    // Photon �̺�Ʈ �ڵ�
    public const byte LoadURLEvent = 8;
    public const byte DestroyWebViewEvent = 9;

    /// <summary>
    /// ��ư�� ������ ȣ��Ǵ� �Լ�.
    /// Canvas prefab�� Instantiate�� ��, �ڽ� WebView ������Ʈ�� ���� ���� URL�� �ε��ϰ�, �̸� ��� Ŭ���̾�Ʈ�� ����ȭ�մϴ�.
    /// </summary>
    /// <param name="url">�����ϰ��� �ϴ� URL</param>
    public void InstantiateWebViewAndLoad()
    {
        string url = "https://www.youtube.com/watch?v=_vtAyHOQRUg";
        if (PhotonNetwork.IsConnectedAndReady && PhotonNetwork.InRoom)
        {
            // ���ϴ� ��ġ���� Canvas prefab�� Instantiate
            Vector3 spawnPosition = new Vector3(-0.64f, 1.72f, 0.4f);
            instantiatedCanvas = PhotonNetwork.Instantiate(canvasPrefab.name, spawnPosition, Quaternion.identity);
            Debug.Log("WebView Canvas instantiated.");

            // Instantiate�� Canvas�� �ڽĿ��� WebView ������Ʈ�� ã�� �� �񵿱������� �ʱ�ȭ �Ϸ� �� URL �ε�
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
                Debug.LogError("WebView ������Ʈ�� ã�� �� �����ϴ�.");
            }

            // ��� Ŭ���̾�Ʈ�� URL �ε� ����� ����ȭ�ϱ� ���� Photon �̺�Ʈ ����
            object[] data = new object[] { url };
            RaiseEventOptions options = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            PhotonNetwork.RaiseEvent(LoadURLEvent, data, options, SendOptions.SendReliable);
            Debug.Log($"LoadURLEvent sent with URL: {url}");
        }
        else
        {
            Debug.LogWarning("Photon�� ����Ǿ� ���� �ʰų�, �濡 �������� �ʾҽ��ϴ�.");
        }
    }


    /// <summary>
    /// ��ư�� ������ ȣ��Ǵ� �Լ�.
    /// Instantiate�� WebView Canvas�� ��� Ŭ���̾�Ʈ���� �����մϴ�.
    /// </summary>
    public void DestroyWebViewNetworked()
    {
        if (instantiatedCanvas != null)
        {
            PhotonNetwork.Destroy(instantiatedCanvas);
            // �ɼ�: ���� ����� ����ȭ�� �� �ֵ��� �̺�Ʈ ���� (���ÿ��� �߰� ó���� �ʿ��ϸ�)
            PhotonNetwork.RaiseEvent(DestroyWebViewEvent, null, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
            Debug.Log("DestroyWebViewEvent sent; WebView Canvas will be removed on all clients.");
        }
        else
        {
            Debug.LogWarning("������ WebView Canvas�� �����ϴ�.");
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

                // �� Ŭ���̾�Ʈ���� �̹� Instantiate�� Canvas�� �ڽ� WebView ������Ʈ�� ã�� URL �ε�
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
                        Debug.LogError("OnPhotonEvent: WebView ������Ʈ�� ã�� �� �����ϴ�.");
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
