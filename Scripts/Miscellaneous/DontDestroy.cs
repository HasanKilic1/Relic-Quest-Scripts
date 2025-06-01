using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    public static DontDestroy Instance {  get; private set; }
    private void Awake()
    {
        if(Instance == null) 
        { 
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void MakeObjectUndestroyable(Transform obj)
    {
        obj.SetParent(transform);
    }
}
