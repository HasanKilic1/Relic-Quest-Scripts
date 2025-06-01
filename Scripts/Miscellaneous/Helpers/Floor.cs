using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Floor : MonoBehaviour
{
    Rigidbody rb;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }
}
