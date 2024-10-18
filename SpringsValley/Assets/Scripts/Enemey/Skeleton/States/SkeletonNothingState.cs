using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonNothingState : SkeletonBaseState
{

    public override void EnterState(SkeletonStateManager skeleton)
    {
        Debug.Log("hello from nothing state");
    }

    // Update is called once per frame
    public override void UpdateState(SkeletonStateManager skeleton)
    {
        
    }

    public override void OnCollisionEnter(SkeletonStateManager skeleton, Collision collision)
    {
        
    }
}
