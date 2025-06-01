using MoreMountains.Feedbacks;
using System;
using UnityEngine;

public class DwarfAnimController : MonoBehaviour
{
    [SerializeField] MMF_Player splashFeedbacks;
    [SerializeField] PoolObject splashObject;
    [SerializeField] Transform splashPosition;
    private ObjectPooler<PoolObject> splashPooler;
    private void Start()
    {
        splashPooler = new ObjectPooler<PoolObject>();
        splashPooler.InitializeObjectPooler(splashObject, SceneObjectPooler.Instance.transform, 10);        
    }

    public void PlaySplashFeedbacks()
    {
        splashFeedbacks?.PlayFeedbacks();
        var splash = splashPooler.GetObject();
        splash.transform.position = splashPosition.position;
    }

    public Vector3 GetSplashPosition => splashPosition.position;
}
