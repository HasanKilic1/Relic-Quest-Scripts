using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleMover : MonoBehaviour
{
    [SerializeField] private float speed;
    void Update()
    {
        transform.position += transform.forward * Time.deltaTime * speed;
    }
}
