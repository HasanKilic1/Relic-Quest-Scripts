using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLooker : MonoBehaviour
{
    public bool isLookingForward;
    private Transform mainCamera;
    
    private void Start()
    {
        mainCamera = Camera.main.transform;
    }
    void LateUpdate()
    {
        // Make the health bar face the camera
        transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward, mainCamera.transform.rotation * Vector3.up);
    }
}
