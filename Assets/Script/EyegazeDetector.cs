using UnityEngine;
using MixedReality.Toolkit;
using Photon.Pun;
using MRTK.Tutorials.MultiUserCapabilities;
using System.Collections;

public class EyegazeDetector : MonoBehaviour
{
    public Material redMaterial, whiteMaterial;
    private MeshRenderer eyegazedMesh;
    private PhotonUserConferenceAR photonUser;
    private bool isUIActivated = false;
    private float eyegazedTime = 0;
    private float deactivatedTime = 0;

    private void Start()
    {
        photonUser = this.gameObject.GetComponent<PhotonUserConferenceAR>();
        // eyegazedMesh = this.GetComponentInChildren<MeshRenderer>();
    }

    void Update()
    {
        Ray ray = new Ray(photonUser.transform.position, photonUser.transform.transform.forward);
        RaycastHit hit;

        // 30 : Photon User
        int photonUserlayer = 1 << 30;
        int eyegazedUILayer = 1 << 31;

        if (!isUIActivated)
        {
            // if Ray hits Photon User, Instantiate InfoUI
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, photonUserlayer))
            {
                eyegazedMesh = hit.collider.GetComponentInChildren<MeshRenderer>();
                eyegazedMesh.material = redMaterial;
                EyegazeUIManager.main.ActivateEyegazeUI(hit);
                isUIActivated = true;
            }
        }
        else
        {
            // if Ray hits UI
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, eyegazedUILayer))
            {
                if (eyegazedTime >= 3.0f)
                {
                    EyegazeUIManager.main.ActivateAllUIInfos();
                    //StartCoroutine(Activate());
                    eyegazedTime = 0;
                }
                else
                {
                    eyegazedTime += Time.deltaTime;
                }
            }
            else
            {
                eyegazedTime = 0;
            }

            if (!Physics.Raycast(ray, out hit, Mathf.Infinity, photonUserlayer))
            {
                deactivatedTime += Time.deltaTime;

                if (deactivatedTime >= 3.0f)
                {
                    EyegazeUIManager.main.DeactivateEyegazeUI();
                    eyegazedMesh.material = whiteMaterial;
                    isUIActivated = false;
                    deactivatedTime = 0;
                }
            }
        }
    }

    //private IEnumerator Activate()
    //{
    //    EyegazeUIManager.main.ActivateAllUIInfos();
    //    EyegazeUIManager.main.eyegazeUIClone.SetActive(false);
    //    yield return new WaitForSeconds(0.1f);
    //    EyegazeUIManager.main.eyegazeUIClone.SetActive(true);
    //}

    // public void OnEyegazeEnter()
    // {
    //     if (!isUIActivated)
    //     {

    //         EyegazeUIManager.main.ActivateEyegazeUI(this.gameObject);
    //         EyegazeUIManager.main.DeactivateEyegazeUI();
    //         EyegazeUIManager.main.ActivateEyegazeUI(this.gameObject);
    //         eyegazedMesh.material = redMaterial;
    //         isUIActivated = true;
    //     }
    // }

    // public void OnEyegazeExit()
    // {
    //     EyegazeUIManager.main.DeactivateEyegazeUI();
    //     eyegazedMesh.material = whiteMaterial;
    //     isUIActivated = false;
    // }

}