using Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CinemachineVirtualCamera))]
public class FOVAdjuster : MonoBehaviour
{
    CinemachineVirtualCamera cam;
    [SerializeField] Transform follow;
    [SerializeField] float FOVByDistance;
    [SerializeField] float fovMax;
    [SerializeField] float fovMin;

    private void Awake()
    {
        cam = GetComponent<CinemachineVirtualCamera>();
    }
    void Start()
    {
    }

    void LateUpdate()
    {
        float fov = FOVByDistance / Vector3.Distance(follow.transform.position, transform.position);
        
        if(fov > fovMax) fov = fovMax;
        if(fov < fovMin) fov = fovMin;

        cam.m_Lens.FieldOfView = fov;    
    }
}
