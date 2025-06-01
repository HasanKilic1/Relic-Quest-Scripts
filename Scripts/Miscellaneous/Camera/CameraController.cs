using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;
using MoreMountains.FeedbacksForThirdParty;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance { get; private set; }
    [SerializeField] CinemachineVirtualCamera mainVCam;
    public CinemachineVirtualCamera blackSmithVCam;
    [SerializeField] CinemachineMixingCamera bossCam;
    [SerializeField] MMCinemachineCameraShaker mainCamShaker;
    public CinemachineVirtualCamera PortalCamera;
    private GameObject activeCamera;

    Transform targetLookBefore;
    private void Awake()
    {
        if(Instance == null) Instance = this;                
    }
    private void Start()
    {
        mainVCam.Follow = PlayerHealth.Instance.transform;
        activeCamera = mainVCam.gameObject;
        SceneManager.activeSceneChanged += OpenMainCameraOnSceneChange;
    }

    private void OpenMainCameraOnSceneChange(Scene arg0, Scene arg1)
    {
        mainVCam.gameObject.SetActive(true);
        activeCamera = mainVCam.gameObject;
        mainCamShaker.CameraReset();      
    }
    
    public void OpenMainCamera()
    {
        activeCamera.SetActive(false);
        mainVCam.gameObject.SetActive(true);
        activeCamera = mainVCam.gameObject;
    }

    public void OpenBlacksmithCamera()
    {
        activeCamera.SetActive(false);
        blackSmithVCam.gameObject.SetActive(true);
        activeCamera = blackSmithVCam.gameObject;
    }

    public void OpenBossCamera(Boss boss)
    {
        activeCamera.SetActive(false);
        bossCam.gameObject.SetActive(true);
        activeCamera = bossCam.gameObject;
        bossCam.ChildCameras[1].Follow = boss.transform;
        bossCam.ChildCameras[1].LookAt = boss.transform;
    }

    public void ChangeLookTarget(Transform target)
    {
        if(mainVCam.LookAt != target)
        {
            targetLookBefore = mainVCam.LookAt;
            mainVCam.LookAt = target;
        }    
    }

    public void ReturnNormal()
    {
        mainVCam.LookAt = PlayerController.Instance.transform;
        mainVCam.Follow = PlayerController.Instance.transform;
    }

    
    public void ResetLookTarget()
    {
        mainVCam.LookAt = targetLookBefore;
    }
    public void ChangeFollowTarget(Transform target)
    {
        if (mainVCam.Follow != target)
        {
            mainVCam.Follow = target;
        }
    }

    private void OnDestroy()
    {
        SceneManager.activeSceneChanged -= OpenMainCameraOnSceneChange;
    }
}
