using MoreMountains.Tools;
using UnityEngine;

public class WorldProgressBar : MonoBehaviour
{
    [SerializeField] MMProgressBar bar;
    Transform toFollow;
    [SerializeField] Vector3 followOffset;

    private void Update()
    {
        Follow();
    }

    private void Follow()
    {
        if (toFollow != null)
        {
            transform.position = toFollow.position + followOffset;
        }
    }
    public void Setup(Transform toFollow)
    {
        this.toFollow = toFollow;
    }
    public Vector3 SetFollowOffset(Vector3 offset) => followOffset = offset;
    public void UpdateBar(int current, int max)
    {
        bar.UpdateBar(current, 0, max);
    }

}
