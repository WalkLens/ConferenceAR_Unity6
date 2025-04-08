using CustomLogger;
public class UserConnectionManager : HostOnlyBehaviour
{
    public override void OnBecameHost(){
        FileLogger.Log("UserConnectionManager 초기화 시작", this);

        // TODO: 초기화 작업 구현

        FileLogger.Log("UserConnectionManager 초기화 완료", this);

        base.OnBecameHost();
    }

    public override void OnStoppedBeingHost(){
        FileLogger.Log("UserConnectionManager 잉여 데이터 정리 시작", this);

        // TODO: 잉여 데이터 정리 작업 구현

        FileLogger.Log("UserConnectionManager 잉여 데이터 정리 완료", this);

        base.OnStoppedBeingHost();
    }    
}
