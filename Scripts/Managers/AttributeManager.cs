using System.Collections.Generic;
using UnityEngine;

public class AttributeManager : MonoBehaviour
{
    public static AttributeManager Instance {  get; private set; }
    [field: SerializeField] public List<Attribute> BasePlayerAttributes { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else Instance = this;
    }   
}
