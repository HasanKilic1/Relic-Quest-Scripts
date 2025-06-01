using MoreMountains.Feedbacks;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class PositionAnimationer : MonoBehaviour
{
    public UnityEvent OnFinish;
    public Vector3 startPoint;
    public Vector3 endPoint;
    [SerializeField] private AnimationCurve curve;
    public float duration;
    [SerializeField] MMF_Player pathFinishFeedbacks;
    [SerializeField] bool drawGizmo;

    public void Animate()
    {
        StartCoroutine(PositionChange(transform));
    }

    public void AnimateExternal(Transform follower)
    {
        StartCoroutine(PositionChange(follower));
    }

    private IEnumerator PositionChange(Transform follower)
    {
        follower.transform.position = startPoint;
        float t = 0f;
        while(t < duration)
        {
            t += Time.deltaTime;
            float evaluated = curve.Evaluate(t / duration);
            Vector3 location = Vector3.Lerp(startPoint, endPoint, evaluated);
            follower.transform.position = location;
            yield return null;
        }
        OnFinish?.Invoke();
        pathFinishFeedbacks?.PlayFeedbacks();
    }

    private void OnDestroy()
    {
        OnFinish?.RemoveAllListeners();
    }
    private void OnDrawGizmosSelected()
    {
        if(!drawGizmo) { return; }
       // Mesh mesh = GetComponent<MeshFilter>().sharedMesh;
        Gizmos.color = Color.cyan;
        //Gizmos.DrawWireMesh(mesh, 0, startPoint , transform.rotation , transform.localScale);
        //Gizmos.DrawWireMesh(mesh, 0, endPoint, transform.rotation, transform.localScale);
        Gizmos.DrawWireSphere(startPoint, 1f);
        Gizmos.DrawWireSphere(endPoint, 1f);
    }
}
