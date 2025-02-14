using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class UserMatchingManagerSM : MonoBehaviour
{
    [SerializeField] NotificationManager notificationManager;           // �˸��� �����ϴ� �κ�
    [SerializeField] InteractionUIManager interactionUIManager;         // HMD UI�� �����ϴ� �κ�
    [SerializeField] UserBehaviourManager userBehaviourManager;         // ������� �ൿ(������ ���� �̵�,)�� �����ϴ� �κ�

    public static UserMatchingManagerSM Instance { get; private set; }

    // Flags
    public bool isUserMatchingReceived = false;
    public bool isUserMatchingFailReceived = false;
    public bool isUserMatchingSucceed = false;
    public bool isUserMatchingFailed = false;
    public bool isUserMet = false;
    public bool isUserRibbonSelected = false;
    public bool isUserFileSended = false;
    public float uitimer1 = 0;
    public float uitimer1limit = 3;
    public float uitimer2 = 0;
    public float uitimer2limit = 3;
    public float requestTime = 0;
    public float leftime = 0;

    // User Data
    public string imsiId = "241201";
    public Transform myPosition;
    public Transform partnerPosition;

    // Phases
    public enum Phase
    {
        Matching,
        ResultManaging,
        RouteVisualizing,
        AfterMatching,
    }
    private Phase currentPhase;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    ///////////// Corutine just for Test /////////////////
    private IEnumerator SendUserMatchingRoutine()
    {
        if (!isUserMatchingSucceed)
        {
            yield return new WaitForSeconds(5f);
            isUserMatchingReceived = true;
        }
    }

    //private IEnumerator SendUserFileRoutine()
    //{
    //    if (isUserMet)
    //    {
    //        yield return new WaitForSeconds(20f);
    //        isUserFileSended = true;
    //    }
    //}
    //////////////////////////////////////////////////////

    private void Start()
    {
        //StartCoroutine(SendUserMatchingRoutine());
        //StartCoroutine(SendUserFileRoutine());
    }

    //void Update()
    //{
    //    // ù ��Ī
    //    switch (currentPhase)
    //    {
    //        // Phase 1. Matching - Accept�� ���ȴ��� �ֱ������� ����
    //        case Phase.Matching:
    //            if (isUserMatchingReceived)
    //            {
    //                notificationManager.OnMatchRequestReceived(imsiId);
    //                isUserMatchingReceived = false;
    //                currentPhase++;
    //            }
    //            break;

    //        // Phase 2. Result Managing - ��Ī�� ����� ���� UI �˾� �� ��� ����
    //        case Phase.ResultManaging:
    //            // ��Ī ���� UI �˾�
    //            if (isUserMatchingSucceed)
    //            {
    //                if (uitimer1 < uitimer1limit)
    //                {
    //                    notificationManager.OpenReceiveAcceptPopupUI();
    //                    uitimer1 += Time.deltaTime;
    //                }
    //                else    // reset
    //                {
    //                    //notificationManager.CloseAcceptPopupUI();
    //                    uitimer1 = 0;
    //                    currentPhase++;
    //                    leftime = notificationManager.GetTime();
    //                }
    //            }
    //            // ��Ī ���� UI �˾�
    //            else if (isUserMatchingFailed)
    //            {
    //                if (uitimer1 < uitimer1limit)
    //                {
    //                    notificationManager.OpenReceiveAcceptPopupUI();
    //                    uitimer1 += Time.deltaTime;
    //                }
    //                else    // reset
    //                {
    //                    //notificationManager.CloseDeclinePopupUI();
    //                    isUserMatchingFailed = false;
    //                    uitimer1 = 0;
    //                    currentPhase--;
    //                }
    //            }
    //            break;

    //        // Phase 3. Route Visualizing - ��Ī�� �Ϸ�Ǹ� ��� �ð�ȭ
    //        case Phase.RouteVisualizing:
    //            if(leftime <= 0)
    //            {
    //                if (!isUserMet)
    //                {
    //                    interactionUIManager.ShowRoute(myPosition.position, partnerPosition.position);
    //                    userBehaviourManager.CheckMetState(myPosition.position, partnerPosition.position);

    //                    // IMSI MOVER - ����ڰ� �����̴� ���̶�� �ӽ� ����
    //                    myPosition.transform.position = Vector3.MoveTowards(myPosition.position, partnerPosition.position, 1f * Time.deltaTime);
    //                }
    //                else
    //                {
    //                    interactionUIManager.HideRoute();
    //                    interactionUIManager.ShowBox();
    //                    currentPhase++;
    //                }
    //            }
    //            else
    //            {
    //                leftime -= Time.deltaTime;
    //            }
                
    //            break;

    //        // Phase 4. After Matching Service On
    //        case Phase.AfterMatching:
    //            if (isUserRibbonSelected)       // 1) ������ �� ������ ���õǾ����� �ֱ������� Ȯ��
    //            {
    //                interactionUIManager.OpenBox();
    //            }
    //            //if (isUserFileSended)           // 2) ������ �Դ��� Ȯ��
    //            //{
    //            //    notificationManager.OnFileReceived(imsiId);
    //            //    isUserFileSended = false;
    //            //}
    //            if (isUserMatchingReceived)       // 3) ���ο� ��Ī ��û�� �Դٸ�
    //            {
    //                notificationManager.OpenNewMatchingUI();
    //            }

    //            break;
    //    }

    //    // ��Ī �� �� �ٸ� ��Ī
    //    // isUserMatchingSucceed�� ���� ��Ī�� ���� ���̾��ٸ� ��� True
    //    if (isUserMatchingSucceed && isUserMatchingReceived)
    //    {
    //        notificationManager.OpenNewMatchingUI();
    //    }

    //    // ������ ���� ��Ī ��û ����
    //    if (isUserMatchingFailReceived)
    //    {
    //        if (uitimer2 < uitimer2limit)
    //        {
    //            notificationManager.OpenMatchingFailUI();
    //            uitimer2 += Time.deltaTime;
    //        }
    //        else    // reset
    //        {
    //            isUserMatchingFailReceived = false;
    //            notificationManager.CloseMatchingFailUI();
    //            uitimer2 = 0;
    //        }
    //    }
    //}
}
