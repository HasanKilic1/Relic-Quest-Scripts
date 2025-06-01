using Cinemachine;
using UnityEngine;

public class BlackSmithCamera : MonoBehaviour
{
    private Vector3 startPosition;
    private Quaternion startRotation;
    private CinemachineVirtualCamera virtualCamera;
    private Transform startFollow;
    private Transform startLook;
    private Vector3 startFollowOffset;
    private Vector3 startLookOffset;
    void Awake()
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        startPosition = transform.position;
        startRotation = transform.rotation;
    }
    private void Start()
    {
        startFollow = virtualCamera.Follow;
        startLook = virtualCamera.LookAt;

        startFollowOffset = virtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset;
        startLookOffset = virtualCamera.GetCinemachineComponent<CinemachineComposer>().m_TrackedObjectOffset;
    }
    public void AssignTarget(Transform target , Vector3 followOffset , Vector3 lookOffset)
    {
        virtualCamera.Follow = target;
        virtualCamera.LookAt = target;
        virtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset = followOffset;
        virtualCamera.GetCinemachineComponent<CinemachineComposer>().m_TrackedObjectOffset = lookOffset;
    }

    public void ReturnStartSettings()
    {
        transform.position = startPosition;
        transform.rotation = startRotation;
        virtualCamera.Follow = startFollow;
        virtualCamera.LookAt = startLook;
        virtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset = startFollowOffset;
        virtualCamera.GetCinemachineComponent<CinemachineComposer>().m_TrackedObjectOffset = startLookOffset;
    }
}
