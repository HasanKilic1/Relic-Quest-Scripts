using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LineDrawer : MonoBehaviour , IPooledObject
{
    LineRenderer lineRenderer;
    public bool continuousDraw = false;

    public Transform FirstTarget;
    public Transform SecondTarget;
    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        if (continuousDraw)
        {
            if(FirstTarget != null && SecondTarget != null)
            {
                Vector3 firstTargetPos = new Vector3(FirstTarget.position.x, 1.5f, FirstTarget.position.z);
                Vector3 secondTargetPos = new Vector3(SecondTarget.position.x, 1.5f, SecondTarget.position.z);
                Draw(firstTargetPos, secondTargetPos);
            }
        }
    }

    public void Draw(Vector3 from , Vector3 to)
    {
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0 , from);
        lineRenderer.SetPosition(1 , to);        
    }

    public void DrawContinuously(Transform from, Transform to)
    {
        continuousDraw = true;
        FirstTarget = from; SecondTarget = to;  
    }

    public void Initialize()
    {
        Deactivate();
    }

    public void Activate()
    {
        gameObject.SetActive(true);
        Invoke(nameof(Deactivate), 1f);
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
