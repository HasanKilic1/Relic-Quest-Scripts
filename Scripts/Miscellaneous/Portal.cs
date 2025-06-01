using MoreMountains.Feedbacks;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(PathMovement))]
[RequireComponent(typeof(BoxCollider))]
public class Portal : MonoBehaviour
{/*
    public static event Action OnPlayerEnter;
    PathMovement pathMovement;
    [SerializeField] string loadScene;
    [Tooltip("Wait until path movement finish")][SerializeField] float waitUntilCameraLook = 0.5f;
    [SerializeField] float waitBeforeActivationComplete = 2f;
    [SerializeField] MMF_Player activationStartFb;
    [SerializeField] MMF_Player activationCompletedFeedbacks;
    [SerializeField] bool camLooksPortal;
    BoxCollider boxColl;
    private bool isActiveCompletely;
    private void Awake()
    {
        boxColl = GetComponent<BoxCollider>();
        boxColl.enabled = false;
        pathMovement = GetComponent<PathMovement>();
    }

    public void ActivatePortal()
    {
        pathMovement.StartFollow(transform);
        boxColl.enabled = true; 
        activationStartFb.PlayFeedbacks();
        if(camLooksPortal)
        {
            CameraController.Instance.OpenPortalCamera();
        }
        Invoke(nameof(CamPortalLook), waitUntilCameraLook);
        Invoke(nameof(TotallyActivate), waitBeforeActivationComplete);
    }

    public void TotallyActivate()
    {
        boxColl.isTrigger = true;
        activationCompletedFeedbacks.PlayFeedbacks();
    }

    private void CamPortalLook()
    {
        CameraController.Instance.PortalCamera.LookAt = transform;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<PlayerHealth>() != null)
        {
            activationCompletedFeedbacks.StopFeedbacks();
            SceneManager.LoadScene(loadScene);
            CameraController.Instance.PortalCamera.LookAt = PlayerHealth.Instance.transform;
            OnPlayerEnter?.Invoke();
        }
    }*/
}
