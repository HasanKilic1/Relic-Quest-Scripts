using UnityEngine;

public class Rotator : MonoBehaviour , IVisualFeedback
{
    [SerializeField] Transform target;
    [SerializeField] bool rotateX;
    [SerializeField] bool rotateY;
    [SerializeField] bool rotateZ;
    [SerializeField] float rotationSpeed = 180f;
    private Transform targetTransform;
    public bool canRotate = true;
    private Quaternion startRotation;

    private void Awake()
    {
        targetTransform = target != null ? target : this.transform;
    }
    private void Start()
    {
       startRotation = transform.rotation;
    }
    void Update()
    {
        if(!canRotate) return;

        if(rotateX)
        {
            targetTransform.Rotate(rotationSpeed * Time.deltaTime * Vector3.right);
        }
        if (rotateY)
        {
            targetTransform.Rotate(rotationSpeed * Time.deltaTime * Vector3.up);
        }
        if (rotateZ)
        {
            targetTransform.Rotate(rotationSpeed * Time.deltaTime * Vector3.forward);
        }
    }

    public void Perform()
    {
        canRotate = true;
    }

    public void Stop()
    {
        canRotate = false;
        targetTransform.rotation = startRotation;
    }
}
