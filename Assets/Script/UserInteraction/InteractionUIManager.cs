using ExitGames.Client.Photon;
using MixedReality.Toolkit.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Microsoft.MixedReality.GraphicsTools.MeshInstancer;
using static UnityEngine.GraphicsBuffer;

public class InteractionUIManager : MonoBehaviour
{
    private LineRenderer lineRenderer;
    //private MRTKLineVisual mrtkLineVisual;

    //private bool isShowProfile = false;

    //[SerializeField] GameObject interactionUI;
    [SerializeField] GameObject box;

    //////////// Before Matching ////////////
    public void ShowRoute(Vector3 me, Vector3 partner)                 // route visualization ON
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startWidth = 0.05f;
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, me);
        lineRenderer.SetPosition(1, partner);

        Debug.Log("show route to matched user");
    }

    public void HideRoute()                 // route visualization OFF
    {
        Destroy(lineRenderer);
        Debug.Log("User Met!");
    }

    public void SendEmoji()                 // send emoji
    {

    }
    /////////////////////////////////////////

    //////////// After matching ////////////
    public void ShowBox()
    {
        //box.transform.position = new Vector3(0, 0, 0);
        box.SetActive(true);
    }

    public void OpenBox()
    {
        //box.transform.position = new Vector3(0, 0, 0);
        box.SetActive(false);
    }

    public void SelectBox()
    {
        UserMatchingManagerSM.Instance.isUserRibbonSelected = true;
        // Box Animation
    }

    //public void ShowVisualMaterials()       // shared material visualization ON
    //{
    //    profileUI.SetActive(true);
    //}

    //public void HideVisualMaterials()       // shared material visualization OFF
    //{
    //    profileUI.SetActive(false);
    //}

    public void SpawnCharacter()            // interfere on conversation
    {

    }
    ////////////////////////////////////////
}
