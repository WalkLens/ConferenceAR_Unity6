using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserBehaviourManager : MonoBehaviour
{
    public void CheckMetState(Vector3 me, Vector3 partner)
    {
        if ((me - partner).magnitude < 2f) { UserMatchingManagerSM.Instance.isUserMet = true; }
        else { UserMatchingManagerSM.Instance.isUserMet = false; }

        Debug.Log((me - partner).magnitude);
    }
}
