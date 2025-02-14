using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class UserMatchingManagerSM : MonoBehaviour
{
    [SerializeField] NotificationManager notificationManager;           // 알림을 관리하는 부분
    [SerializeField] InteractionUIManager interactionUIManager;         // HMD UI를 관리하는 부분
    [SerializeField] UserBehaviourManager userBehaviourManager;         // 사용자의 행동(만났기 위해 이동,)을 관리하는 부분

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
    //    // 첫 매칭
    //    switch (currentPhase)
    //    {
    //        // Phase 1. Matching - Accept가 눌렸는지 주기적으로 감시
    //        case Phase.Matching:
    //            if (isUserMatchingReceived)
    //            {
    //                notificationManager.OnMatchRequestReceived(imsiId);
    //                isUserMatchingReceived = false;
    //                currentPhase++;
    //            }
    //            break;

    //        // Phase 2. Result Managing - 매칭의 결과에 따른 UI 팝업 및 결과 전송
    //        case Phase.ResultManaging:
    //            // 매칭 성공 UI 팝업
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
    //            // 매칭 실패 UI 팝업
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

    //        // Phase 3. Route Visualizing - 매칭이 완료되면 경로 시각화
    //        case Phase.RouteVisualizing:
    //            if(leftime <= 0)
    //            {
    //                if (!isUserMet)
    //                {
    //                    interactionUIManager.ShowRoute(myPosition.position, partnerPosition.position);
    //                    userBehaviourManager.CheckMetState(myPosition.position, partnerPosition.position);

    //                    // IMSI MOVER - 사용자가 움직이는 중이라고 임시 가정
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
    //            if (isUserRibbonSelected)       // 1) 만났을 때 리본이 선택되었는지 주기적으로 확인
    //            {
    //                interactionUIManager.OpenBox();
    //            }
    //            //if (isUserFileSended)           // 2) 파일이 왔는지 확인
    //            //{
    //            //    notificationManager.OnFileReceived(imsiId);
    //            //    isUserFileSended = false;
    //            //}
    //            if (isUserMatchingReceived)       // 3) 새로운 매칭 요청이 왔다면
    //            {
    //                notificationManager.OpenNewMatchingUI();
    //            }

    //            break;
    //    }

    //    // 매칭 중 또 다른 매칭
    //    // isUserMatchingSucceed는 현재 매칭이 진행 중이었다면 계속 True
    //    if (isUserMatchingSucceed && isUserMatchingReceived)
    //    {
    //        notificationManager.OpenNewMatchingUI();
    //    }

    //    // 이전에 보낸 매칭 요청 실패
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
