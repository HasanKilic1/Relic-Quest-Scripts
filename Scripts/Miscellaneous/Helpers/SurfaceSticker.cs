using System.Collections;
using UnityEngine;

public class SurfaceSticker : MonoBehaviour
{
    /// <summary>
    /// Responsible to hold a game object below or above a surface
    /// </summary>
    [SerializeField] float distanceToSurface = 0.5f;
    [SerializeField] float checkInterval;
    [SerializeField] LayerMask surfaceLayer;
    [SerializeField] float searchDistance = 1000f;

    private void Start()
    {
        StartCoroutine(HoldOnSurface());
    }
    private IEnumerator HoldOnSurface()
    {
        while (true)
        {
            transform.position = GetHoldPosition();
            yield return new WaitForSecondsRealtime(checkInterval);
        }
    }

    private Vector3 GetHoldPosition()
    {

        Ray downRay = new Ray(transform.position + Vector3.up * 5f, Vector3.down);
        if (Physics.Raycast(downRay, out RaycastHit downHit, searchDistance, surfaceLayer))
        {
            return downHit.point + Vector3.up * distanceToSurface;
        }

        Ray upRay = new Ray(transform.position - Vector3.up * 5f, Vector3.up);
        if(Physics.Raycast(upRay,out RaycastHit upHit, searchDistance, surfaceLayer))
        {
            return upHit.point - Vector3.up * distanceToSurface;
        }

       
        return transform.position;
    }
}
