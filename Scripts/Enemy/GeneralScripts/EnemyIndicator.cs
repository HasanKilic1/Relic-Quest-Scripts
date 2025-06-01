using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIndicator : MonoBehaviour
{
    private bool indicated = false;
    private float rotationSpeed = 120f;
    [SerializeField] private GameObject indicator;
    
    void Update()
    {
        if (indicated)
        {
            indicator.transform.Rotate(rotationSpeed * Time.deltaTime * Vector3.up );
        }
    }
    public void Indicate()
    {   
        indicated = true;
        indicator.SetActive(indicated);
    }
    public void UnIndicate()
    {
        indicated = false;
        indicator.SetActive(indicated);
    }
}
