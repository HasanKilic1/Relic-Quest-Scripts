using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingDemon : Enemy
{
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();        
    }

    protected override void Update()
    {
        base.Update();
        HandleStates();
    }
    protected override void Attack()
    {
        
    }
}
